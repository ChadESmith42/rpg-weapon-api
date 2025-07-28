using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Xunit;
using WeaponApi.Infrastructure.Persistence;

namespace WeaponApi.IntegrationTests;

/// <summary>
/// Integration tests for AuthController testing authentication and authorization flows
/// with a real PostgreSQL database using test containers.
/// </summary>
public sealed class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private HttpClient _client = null!;
    private readonly PostgreSqlContainer _postgreSqlContainer;

    public AuthControllerTests(WebApplicationFactory<Program> factory)
    {
        _postgreSqlContainer = new PostgreSqlBuilder()
            .WithDatabase("weaponapi_auth_test")
            .WithUsername("test_user")
            .WithPassword("test_password")
            .WithCleanUp(true)
            .Build();

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                // Add test configuration for JWT with higher priority than appsettings
                config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["Jwt:SecretKey"] = "test-secret-key-that-is-long-enough-for-jwt-tokens-in-integration-tests-12345",
                    ["Jwt:Issuer"] = "test-issuer",
                    ["Jwt:Audience"] = "test-audience"
                });
            });

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

                // Remove existing JWT Bearer authentication and reconfigure with test values
                var authDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(Microsoft.AspNetCore.Authentication.IAuthenticationService));
                if (authDescriptor != null)
                {
                    // Remove JWT Bearer authentication
                    var jwtBearerDescriptors = services.Where(s => s.ServiceType.FullName?.Contains("JwtBearer") == true).ToList();
                    foreach (var jwtDescriptor in jwtBearerDescriptors)
                    {
                        services.Remove(jwtDescriptor);
                    }
                }

                // Reconfigure authentication with test values
                services.Configure<Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerOptions>(
                    Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme,
                    options =>
                    {
                        var secretKey = "test-secret-key-that-is-long-enough-for-jwt-tokens-in-integration-tests-12345";
                        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = "test-issuer",
                            ValidAudience = "test-audience",
                            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey))
                            {
                                KeyId = "WeaponApiKey"
                            },
                            ClockSkew = TimeSpan.FromMinutes(5)
                        };
                    });
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
    public async Task Register_WithValidRequest_ShouldReturnCreated()
    {
        // Arrange
        var request = new RegisterRequest(
            "test@example.com",
            "ComplexPassw0rd!@#$",
            "johndoe",
            "John Doe",
            new DateOnly(1990, 1, 1)
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Debug: Check response content if not successful
        if (response.StatusCode != HttpStatusCode.Created)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Expected 201 Created but got {response.StatusCode}. Response: {errorContent}");
        }

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponse>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        authResponse.Should().NotBeNull();
        authResponse!.User.Id.Should().NotBeNullOrEmpty();
        authResponse.User.Email.Should().Be("test@example.com");
        authResponse.AccessToken.Should().NotBeNullOrEmpty();
        authResponse.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Register_WithInvalidEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new RegisterRequest(
            "invalid-email", // Invalid email format
            "ComplexPassw0rd!@#$",
            "johndoe",
            "John Doe",
            new DateOnly(1990, 1, 1)
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithWeakPassword_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new RegisterRequest(
            "test@example.com",
            "weak", // Too weak password
            "johndoe",
            "John Doe",
            new DateOnly(1990, 1, 1)
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new RegisterRequest(
            "duplicate@example.com",
            "ComplexPassw0rd!@#$",
            "johndoe",
            "John Doe",
            new DateOnly(1990, 1, 1)
        );

        // Register first user
        await _client.PostAsJsonAsync("/api/auth/register", request);

        // Act - Try to register with same email
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert - Should return BadRequest to prevent email enumeration attacks
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnOk()
    {
        // Arrange - Register a user first
        var registerRequest = new RegisterRequest(
            "login@example.com",
            "ComplexPassw0rd!@#$",
            "johndoe",
            "John Doe",
            new DateOnly(1990, 1, 1)
        );
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new LoginRequest(
            "login@example.com",
            "ComplexPassw0rd!@#$"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponse>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        authResponse.Should().NotBeNull();
        authResponse!.User.Email.Should().Be("login@example.com");
        authResponse.AccessToken.Should().NotBeNullOrEmpty();
        authResponse.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequest(
            "nonexistent@example.com",
            "WrongPassword123!"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ShouldReturnUnauthorized()
    {
        // Arrange - Register a user first
        var registerRequest = new RegisterRequest(
            "wrongpass@example.com",
            "CorrectPassword123!",
            "johndoe",
            "John Doe",
            new DateOnly(1990, 1, 1)
        );
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new LoginRequest(
            "wrongpass@example.com",
            "WrongPassword123!" // Wrong password
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RefreshToken_WithValidToken_ShouldReturnOk()
    {
        // Arrange - Register and login to get tokens
        var registerRequest = new RegisterRequest(
            "refresh@example.com",
            "ComplexPassw0rd!@#$",
            "johndoe",
            "John Doe",
            new DateOnly(1990, 1, 1)
        );
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new LoginRequest(
            "refresh@example.com",
            "ComplexPassw0rd!@#$"
        );
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponse>(loginContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var refreshRequest = new RefreshTokenRequest(
            authResponse!.AccessToken,
            authResponse.RefreshToken
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/refresh", refreshRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var newAuthResponse = JsonSerializer.Deserialize<AuthResponse>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        newAuthResponse.Should().NotBeNull();
        newAuthResponse!.AccessToken.Should().NotBeNullOrEmpty();
        newAuthResponse.RefreshToken.Should().NotBeNullOrEmpty();
        newAuthResponse.AccessToken.Should().NotBe(authResponse.AccessToken); // Should be a new token
    }

    [Fact]
    public async Task RefreshToken_WithInvalidToken_ShouldReturnUnauthorized()
    {
        // Arrange
        var refreshRequest = new RefreshTokenRequest(
            "invalid.jwt.token",
            "invalid-refresh-token"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/refresh", refreshRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Logout_WithValidToken_ShouldReturnOk()
    {
        // Arrange - Register and login to get tokens
        var registerRequest = new RegisterRequest(
            "logout@example.com",
            "ComplexPassw0rd!@#$",
            "johndoe",
            "John Doe",
            new DateOnly(1990, 1, 1)
        );
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new LoginRequest(
            "logout@example.com",
            "ComplexPassw0rd!@#$"
        );
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponse>(loginContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // Add JWT token to Authorization header
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
            "Bearer", authResponse!.AccessToken);

        // Act
        var response = await _client.PostAsync("/api/auth/logout", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Logout_WithoutToken_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.PostAsync("/api/auth/logout", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetProfile_WithValidToken_ShouldReturnOk()
    {
        // Arrange - Register and login to get tokens
        var registerRequest = new RegisterRequest(
            "profile@example.com",
            "ComplexPassw0rd!@#$",
            "johndoe",
            "John Doe",
            new DateOnly(1990, 1, 1)
        );
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new LoginRequest(
            "profile@example.com",
            "ComplexPassw0rd!@#$"
        );
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponse>(loginContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // Add JWT token to Authorization header
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
            "Bearer", authResponse!.AccessToken);

        // Act
        var response = await _client.GetAsync("/api/auth/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var profileResponse = JsonSerializer.Deserialize<UserResponse>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        profileResponse.Should().NotBeNull();
        profileResponse!.Email.Should().Be("profile@example.com");
        profileResponse.Username.Should().Be("johndoe");
        profileResponse.Name.Should().Be("John Doe");
    }

    [Fact]
    public async Task GetProfile_WithoutToken_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/auth/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // Request/Response DTOs for testing
    private sealed record RegisterRequest(
        string Email,
        string Password,
        string Username,
        string Name,
        DateOnly DateOfBirth);

    private sealed record LoginRequest(
        string EmailOrUsername,
        string Password);

    private sealed record RefreshTokenRequest(
        string Token,
        string RefreshToken);

    private sealed record AuthResponse(
        string AccessToken,
        string RefreshToken,
        DateTime ExpiresAt,
        UserResponse User);

    private sealed record UserResponse(
        string Id,
        string Email,
        string Username,
        string Name);

    private sealed record UserProfileResponse(
        Guid Id,
        string Email,
        string FirstName,
        string LastName,
        bool IsEmailConfirmed,
        string Role,
        DateTime CreatedAt);
}
