using Microsoft.EntityFrameworkCore;
using WeaponApi.Application.User;
using WeaponApi.Domain.User;
using User = WeaponApi.Domain.User.User;

namespace WeaponApi.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for User aggregate root operations.
/// Provides business-oriented data access methods using Entity Framework Core.
/// </summary>
public sealed class UserRepository : IUserRepository
{
    private readonly WeaponApiDbContext context;

    public UserRepository(WeaponApiDbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<User?> FindByIdAsync(UserId id, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<User>> FindAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await context.Users.ToListAsync(cancellationToken);
        return users.AsReadOnly();
    }

    public async Task AddAsync(User aggregate, CancellationToken cancellationToken = default)
    {
        await context.Users.AddAsync(aggregate, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(User aggregate, CancellationToken cancellationToken = default)
    {
        context.Users.Update(aggregate);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(User aggregate, CancellationToken cancellationToken = default)
    {
        context.Users.Remove(aggregate);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(UserId id, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .AnyAsync(user => user.Id == id, cancellationToken);
    }

    public async Task<User?> FindByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .FirstOrDefaultAsync(user => user.Email == email, cancellationToken);
    }

    public async Task<User?> FindByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .FirstOrDefaultAsync(user => user.Profile.Username == username, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .AnyAsync(user => user.Email == email, cancellationToken);
    }

    public async Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .AnyAsync(user => user.Profile.Username == username, cancellationToken);
    }
}
