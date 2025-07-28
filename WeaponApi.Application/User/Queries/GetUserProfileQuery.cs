using MediatR;
using WeaponApi.Domain.User;

namespace WeaponApi.Application.User.Queries;

/// <summary>
/// Query to retrieve a user's profile information by their unique identifier.
/// Follows MediatR CQRS pattern for user profile retrieval.
/// </summary>
public sealed record GetUserProfileQuery(UserId UserId) : IRequest<GetUserProfileQueryResult>;

/// <summary>
/// Result type for GetUserProfileQuery execution.
/// Provides structured response with user profile data or error information.
/// </summary>
public sealed class GetUserProfileQueryResult
{
    public bool IsSuccess { get; }
    public Domain.User.User? User { get; }
    public string? ErrorMessage { get; }

    private GetUserProfileQueryResult(bool isSuccess, Domain.User.User? user = null, string? errorMessage = null)
    {
        IsSuccess = isSuccess;
        User = user;
        ErrorMessage = errorMessage;
    }

    public static GetUserProfileQueryResult Success(Domain.User.User user) =>
        new(true, user);

    public static GetUserProfileQueryResult NotFound(UserId userId) =>
        new(false, errorMessage: $"User with ID '{userId}' was not found");

    public static GetUserProfileQueryResult Failure(string errorMessage) =>
        new(false, errorMessage: errorMessage);
}
