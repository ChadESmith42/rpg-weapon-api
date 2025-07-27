using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeaponApi.Domain.Weapon;

namespace WeaponApi.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for the Weapon aggregate root.
/// Maps domain value objects to database columns with proper constraints.
/// </summary>
public sealed class WeaponConfiguration : IEntityTypeConfiguration<Weapon>
{
    public void Configure(EntityTypeBuilder<Weapon> builder)
    {
        // Table configuration
        builder.ToTable("weapons");

        // Primary key configuration
        builder.HasKey(w => w.Id);
        builder.Property(w => w.Id)
            .HasConversion(
                weaponId => weaponId.Value,
                value => WeaponId.Create(value))
            .HasColumnName("id")
            .IsRequired();

        // WeaponName value object configuration
        builder.Property(w => w.Name)
            .HasConversion(
                weaponName => weaponName.Value,
                value => WeaponName.Create(value))
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        // WeaponType value object configuration
        builder.Property(w => w.Type)
            .HasConversion(
                weaponType => weaponType.Value,
                value => WeaponType.Create(value))
            .HasColumnName("type")
            .IsRequired();

        // Description configuration
        builder.Property(w => w.Description)
            .HasColumnName("description")
            .HasMaxLength(500)
            .IsRequired();

        // HitPoints configuration
        builder.Property(w => w.HitPoints)
            .HasColumnName("hit_points")
            .IsRequired();

        // MaxHitPoints configuration
        builder.Property(w => w.MaxHitPoints)
            .HasColumnName("max_hit_points")
            .IsRequired();

        // Damage configuration
        builder.Property(w => w.Damage)
            .HasColumnName("damage")
            .IsRequired();

        // IsRepairable configuration
        builder.Property(w => w.IsRepairable)
            .HasColumnName("is_repairable")
            .IsRequired();

        // Value configuration with precision for monetary values
        builder.Property(w => w.Value)
            .HasColumnName("value")
            .HasPrecision(18, 2)
            .IsRequired();

        // Indexes for performance
        builder.HasIndex(w => w.Name)
            .HasDatabaseName("ix_weapons_name");

        builder.HasIndex(w => w.Type)
            .HasDatabaseName("ix_weapons_type");

        // Check constraints for data integrity
        builder.HasCheckConstraint("ck_weapons_hit_points_positive", "hit_points > 0");
        builder.HasCheckConstraint("ck_weapons_max_hit_points_positive", "max_hit_points > 0");
        builder.HasCheckConstraint("ck_weapons_damage_positive", "damage > 0");
        builder.HasCheckConstraint("ck_weapons_value_non_negative", "value >= 0");
        builder.HasCheckConstraint("ck_weapons_hit_points_max", "hit_points <= max_hit_points");
    }
}
