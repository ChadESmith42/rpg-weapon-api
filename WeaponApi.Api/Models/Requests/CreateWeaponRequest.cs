namespace WeaponApi.Api.Models.Requests;

public sealed class CreateWeaponRequest
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int HitPoints { get; set; }
    public int Damage { get; set; }
    public bool IsRepairable { get; set; }
    public decimal Value { get; set; }
}
