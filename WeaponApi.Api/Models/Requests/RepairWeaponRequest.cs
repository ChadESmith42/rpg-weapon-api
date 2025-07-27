namespace WeaponApi.Api.Models.Requests;

public sealed class RepairWeaponRequest
{
    public Guid WeaponId { get; set; }
    public int RepairAmount { get; set; }
}
