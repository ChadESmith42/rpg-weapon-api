using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeaponApi.Application.Authentication;
using WeaponApi.Application.User;
using WeaponApi.Application.Weapon;
using WeaponApi.Infrastructure.Authentication;
using WeaponApi.Infrastructure.Persistence;
using WeaponApi.Infrastructure.Persistence.Repositories;

namespace WeaponApi.Infrastructure;

/// <summary>
/// Infrastructure layer dependency injection configuration.
/// Registers all infrastructure services including database context, repositories, and authentication services.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.AddRepositories();
        services.AddAuthenticationServices();

        return services;
    }

    private static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection string is not configured");

        services.AddDbContext<WeaponApiDbContext>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IWeaponRepository, WeaponRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }

    private static IServiceCollection AddAuthenticationServices(this IServiceCollection services)
    {
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordHashService, PasswordHashService>();

        // Register the domain password hasher adapter
        services.AddScoped<WeaponApi.Domain.User.IPasswordHasher, PasswordHasherAdapter>();

        return services;
    }
}
