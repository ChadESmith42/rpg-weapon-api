namespace WeaponApi.Api.Models.Requests;

public sealed class RefreshTokenRequest
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
