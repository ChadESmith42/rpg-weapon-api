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
        // Table configuration
        builder.ToTable("users");

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
        builder.Property(u => u.Roles)
            .HasConversion(
                roles => string.Join(',', roles.Select(r => r.Value.ToString())),
                value => value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(v => Role.Create(Enum.Parse<RoleEnum>(v)))
                            .ToList())
            .HasColumnName("roles")
            .HasMaxLength(500)
            .IsRequired();

        // Ignore domain events (not persisted)
        builder.Ignore(u => u.DomainEvents);

        // Indexes for performance
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("ix_users_email_unique");

        builder.HasIndex(u => u.Profile.Username)
            .IsUnique()
            .HasDatabaseName("ix_users_username_unique");

        builder.HasIndex(u => u.CreatedAt)
            .HasDatabaseName("ix_users_created_at");

        // Check constraints for data integrity
        builder.HasCheckConstraint("ck_users_email_format", "email ~ '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,}$'");
        builder.HasCheckConstraint("ck_users_username_length", "LENGTH(username) >= 3");
        builder.HasCheckConstraint("ck_users_name_length", "LENGTH(name) >= 1");
    }
}
