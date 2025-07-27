using MediatR;
using WeaponApi.Application.User;
using WeaponApi.Application.User.Queries;

namespace WeaponApi.Application.User.Queries;

/// <summary>
/// Handler for GetUserProfileQuery.
/// Retrieves user profile information from the repository.
/// </summary>
public sealed class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, GetUserProfileQueryResult>
{
    private readonly IUserRepository userRepository;

    public GetUserProfileQueryHandler(IUserRepository userRepository)
    {
        this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<GetUserProfileQueryResult> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userRepository.FindByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                return GetUserProfileQueryResult.NotFound(request.UserId);
            }

            return GetUserProfileQueryResult.Success(user);
        }
        catch (Exception ex)
        {
            return GetUserProfileQueryResult.Failure($"Failed to retrieve user profile: {ex.Message}");
        }
    }
}
