using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeaponApi.Domain.User;

namespace WeaponApi.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for the User aggregate root.
/// Maps complex domain value objects to database columns with proper relationships.
/// </summary>
public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Table configuration with check constraints for data integrity
        builder.ToTable("users", t =>
        {
            t.HasCheckConstraint("ck_users_email_format", "email ~ '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,}$'");
            t.HasCheckConstraint("ck_users_username_length", "LENGTH(username) >= 3");
            t.HasCheckConstraint("ck_users_name_length", "LENGTH(name) >= 1");
        });

        // Primary key configuration
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .HasConversion(
                userId => userId.Value,
                value => UserId.Create(value))
            .HasColumnName("id")
            .IsRequired();

        // Email value object configuration
        builder.Property(u => u.Email)
            .HasConversion(
                email => email.Value,
                value => Email.Create(value))
            .HasColumnName("email")
            .HasMaxLength(255)
            .IsRequired();

        // CreatedAt configuration
        builder.Property(u => u.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        // Configure UserProfile as owned entity
        builder.OwnsOne(u => u.Profile, profile =>
        {
            profile.Property(p => p.Username)
                .HasColumnName("username")
                .HasMaxLength(50)
                .IsRequired();

            profile.Property(p => p.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();

            profile.Property(p => p.DateOfBirth)
                .HasColumnName("date_of_birth")
                .IsRequired();

            // Index on username within the owned entity configuration
            profile.HasIndex(p => p.Username)
                .IsUnique()
                .HasDatabaseName("ix_users_username_unique");
        });

        // Configure UserSecurity as owned entity
        builder.OwnsOne(u => u.Security, security =>
        {
            security.Property(s => s.PasswordHash)
                .HasColumnName("password_hash")
                .HasMaxLength(255)
                .IsRequired();

            security.Property(s => s.LastLoginAt)
                .HasColumnName("last_login_at")
                .IsRequired(false);
        });

        // Configure Roles collection
        var rolesProperty = builder.Property(u => u.Roles)
            .HasConversion(
                roles => string.Join(',', roles.Select(r => r.Value.ToString())),
                value => value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(v => Role.Create(v.Trim()))
                            .ToList())
            .HasColumnName("roles")
            .HasMaxLength(500)
            .IsRequired();

        rolesProperty.Metadata.SetValueComparer(new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<IReadOnlyList<Role>>(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList()));

        // Ignore domain events (not persisted)
        builder.Ignore(u => u.DomainEvents);

        // Indexes for performance
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("ix_users_email_unique");

        builder.HasIndex(u => u.CreatedAt)
            .HasDatabaseName("ix_users_created_at");
    }
}
