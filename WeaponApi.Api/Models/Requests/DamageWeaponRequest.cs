namespace WeaponApi.Api.Models.Requests;

public sealed class DamageWeaponRequest
{
    public Guid WeaponId { get; set; }
    public int DamageAmount { get; set; }
}
