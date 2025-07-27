namespace WeaponApi.Api.Models.Responses;

public sealed record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    UserResponse User);

public sealed record UserResponse(
    Guid Id,
    string Email,
    string Username,
    string Name);
