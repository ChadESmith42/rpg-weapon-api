using Microsoft.EntityFrameworkCore;
using WeaponApi.Domain.User;
using WeaponApi.Domain.Weapon;

namespace WeaponApi.Infrastructure.Persistence;

/// <summary>
/// Entity Framework Core database context for the Weapon API.
/// Manages database connections and entity configurations for Clean Architecture.
/// </summary>
public sealed class WeaponApiDbContext : DbContext
{
    public WeaponApiDbContext(DbContextOptions<WeaponApiDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Users aggregate root entities
    /// </summary>
    public DbSet<User> Users { get; set; } = null!;

    /// <summary>
    /// Weapons aggregate root entities
    /// </summary>
    public DbSet<Weapon> Weapons { get; set; } = null!;

    /// <summary>
    /// Configures entity mappings using Fluent API.
    /// Applies all entity configurations from the assembly.
    /// </summary>
    /// <param name="modelBuilder">Model builder for entity configuration</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WeaponApiDbContext).Assembly);
    }

    /// <summary>
    /// Configures database conventions and global settings.
    /// </summary>
    /// <param name="configurationBuilder">Configuration builder</param>
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        // Configure string properties to use VARCHAR instead of NVARCHAR for PostgreSQL optimization
        configurationBuilder.Properties<string>()
            .HaveMaxLength(1000); // Default max length to prevent unlimited strings
    }
}
