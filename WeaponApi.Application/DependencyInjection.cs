using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace WeaponApi.Application;

/// <summary>
/// Application layer dependency injection configuration.
/// Registers MediatR and application services for CQRS pattern implementation.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register MediatR with command and query handlers from this assembly
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        return services;
    }
}
