using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Xunit;
using WeaponApi.Infrastructure.Persistence;

namespace WeaponApi.IntegrationTests;

/// <summary>
/// Integration tests for WeaponsController testing full HTTP request/response cycles
/// with a real PostgreSQL database using test containers.
/// </summary>
public sealed class WeaponsControllerTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private HttpClient _client = null!;
    private readonly PostgreSqlContainer _postgreSqlContainer;

    public WeaponsControllerTests(WebApplicationFactory<Program> factory)
    {
        _postgreSqlContainer = new PostgreSqlBuilder()
            .WithDatabase("weaponapi_test")
            .WithUsername("test_user")
            .WithPassword("test_password")
            .WithCleanUp(true)
            .Build();

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(WeaponApiDbContext));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add test database connection
                services.AddNpgsql<WeaponApiDbContext>(
                    _postgreSqlContainer.GetConnectionString());
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();

        // Create client after container is started
        _client = _factory.CreateClient();

        // Apply migrations
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<WeaponApiDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _postgreSqlContainer.StopAsync();
        _client.Dispose();
    }

    [Fact]
    public async Task GetWeapons_WithNoWeapons_ShouldReturnEmptyList()
    {
        // Act
        var response = await _client.GetAsync("/api/weapons");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var weapons = JsonSerializer.Deserialize<WeaponResponse[]>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        weapons.Should().NotBeNull();
        weapons.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateWeapon_WithValidRequest_ShouldReturnCreated()
    {
        // Arrange
        var request = new CreateWeaponRequest(
            "Test Sword",
            "A mighty test sword",
            100,
            25,
            true,
            150.0m
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/weapons", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var locationHeader = response.Headers.Location;
        locationHeader.Should().NotBeNull();
        locationHeader!.PathAndQuery.Should().StartWith("/api/weapons/");

        var content = await response.Content.ReadAsStringAsync();
        var weaponResponse = JsonSerializer.Deserialize<WeaponResponse>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        weaponResponse.Should().NotBeNull();
        weaponResponse!.Name.Should().Be("Test Sword");
        weaponResponse.Description.Should().Be("A mighty test sword");
        weaponResponse.HitPoints.Should().Be(100);
        weaponResponse.Damage.Should().Be(25);
        weaponResponse.IsRepairable.Should().BeTrue();
        weaponResponse.Value.Should().Be(150.0m);
    }

    [Fact]
    public async Task CreateWeapon_WithInvalidRequest_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateWeaponRequest(
            "", // Invalid empty name
            "A mighty test sword",
            100,
            25,
            true,
            150.0m
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/weapons", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetWeapon_WithExistingId_ShouldReturnWeapon()
    {
        // Arrange - Create a weapon first
        var createRequest = new CreateWeaponRequest(
            "Test Sword",
            "A mighty test sword",
            100,
            25,
            true,
            150.0m
        );

        var createResponse = await _client.PostAsJsonAsync("/api/weapons", createRequest);
        var weaponId = ExtractIdFromLocationHeader(createResponse.Headers.Location!);

        // Act
        var response = await _client.GetAsync($"/api/weapons/{weaponId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var weapon = JsonSerializer.Deserialize<WeaponResponse>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        weapon.Should().NotBeNull();
        weapon!.Id.Should().Be(weaponId);
        weapon.Name.Should().Be("Test Sword");
    }

    [Fact]
    public async Task GetWeapon_WithNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/weapons/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteWeapon_WithExistingId_ShouldReturnNoContent()
    {
        // Arrange - Create a weapon first
        var createRequest = new CreateWeaponRequest(
            "Test Sword",
            "A mighty test sword",
            100,
            25,
            true,
            150.0m
        );

        var createResponse = await _client.PostAsJsonAsync("/api/weapons", createRequest);
        var weaponId = ExtractIdFromLocationHeader(createResponse.Headers.Location!);

        // Act
        var response = await _client.DeleteAsync($"/api/weapons/{weaponId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify weapon is deleted
        var getResponse = await _client.GetAsync($"/api/weapons/{weaponId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteWeapon_WithNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/weapons/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DamageWeapon_WithExistingId_ShouldReturnOk()
    {
        // Arrange - Create a weapon first
        var createRequest = new CreateWeaponRequest(
            "Test Sword",
            "A mighty test sword",
            100,
            25,
            true,
            150.0m
        );

        var createResponse = await _client.PostAsJsonAsync("/api/weapons", createRequest);
        var weaponId = ExtractIdFromLocationHeader(createResponse.Headers.Location!);

        var damageRequest = new { DamageAmount = 20 };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/weapons/{weaponId}/damage", damageRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify weapon hit points are reduced
        var getResponse = await _client.GetAsync($"/api/weapons/{weaponId}");
        var content = await getResponse.Content.ReadAsStringAsync();
        var weapon = JsonSerializer.Deserialize<WeaponResponse>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        weapon!.HitPoints.Should().Be(80); // 100 - 20
    }

    [Fact]
    public async Task RepairWeapon_WithExistingRepairableWeapon_ShouldReturnOk()
    {
        // Arrange - Create and damage a weapon first
        var createRequest = new CreateWeaponRequest(
            "Test Sword",
            "A mighty test sword",
            100,
            25,
            true, // Repairable
            150.0m
        );

        var createResponse = await _client.PostAsJsonAsync("/api/weapons", createRequest);
        var weaponId = ExtractIdFromLocationHeader(createResponse.Headers.Location!);

        // Damage the weapon
        await _client.PostAsJsonAsync($"/api/weapons/{weaponId}/damage", new { DamageAmount = 50 });

        // Act
        var response = await _client.PostAsync($"/api/weapons/{weaponId}/repair", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify weapon is fully repaired
        var getResponse = await _client.GetAsync($"/api/weapons/{weaponId}");
        var content = await getResponse.Content.ReadAsStringAsync();
        var weapon = JsonSerializer.Deserialize<WeaponResponse>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        weapon!.HitPoints.Should().Be(100); // Fully repaired
    }

    private static string ExtractIdFromLocationHeader(Uri locationHeader)
    {
        var segments = locationHeader.ToString().Split('/');
        return segments[^1]; // Last segment is the ID
    }

    // Request/Response DTOs for testing
    private sealed record CreateWeaponRequest(
        string Name,
        string Description,
        int HitPoints,
        int Damage,
        bool IsRepairable,
        decimal Value);

    private sealed record WeaponResponse(
        Guid Id,
        string Name,
        string Description,
        int HitPoints,
        int MaxHitPoints,
        int Damage,
        bool IsRepairable,
        decimal Value,
        bool IsDestroyed);
}
