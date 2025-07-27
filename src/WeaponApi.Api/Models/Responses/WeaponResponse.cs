namespace WeaponApi.Api.Models.Responses;

public sealed record WeaponResponse(
    Guid Id,
    string Name,
    string Type,
    string Description,
    int HitPoints,
    int Damage,
    bool IsRepairable,
    decimal Value);
