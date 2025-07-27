using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeaponApi.Api.Models.Requests;
using WeaponApi.Api.Models.Responses;
using WeaponApi.Application.Weapon.Commands;
using WeaponApi.Application.Weapon.Queries;
using WeaponApi.Domain.Weapon;

namespace WeaponApi.Api.Controllers;

/// <summary>
/// Weapons API controller providing CRUD operations for weapon management.
/// Implements RESTful endpoints following Clean Architecture patterns.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class WeaponsController : ControllerBase
{
    private readonly IMediator mediator;

    public WeaponsController(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Creates a new weapon in the system.
    /// </summary>
    /// <param name="request">The weapon creation request.</param>
    /// <returns>Returns the created weapon ID on success.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateWeapon([FromBody] CreateWeaponRequest request)
    {
        var command = new CreateWeaponCommand(
            request.Name,
            request.Type,
            request.Description,
            request.HitPoints,
            request.Damage,
            request.IsRepairable,
            request.Value);

        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            if (result.ValidationErrors.Any())
            {
                return BadRequest(new { errors = result.ValidationErrors });
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        return CreatedAtAction(
            nameof(GetWeapon),
            new { id = result.WeaponId!.Value },
            new { id = result.WeaponId.Value });
    }

    /// <summary>
    /// Retrieves a weapon by its unique identifier.
    /// </summary>
    /// <param name="id">The weapon's unique identifier.</param>
    /// <returns>Returns the weapon data if found.</returns>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetWeapon(Guid id)
    {
        var weaponId = WeaponId.Create(id);
        var query = new GetWeaponQuery(weaponId);

        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(new { message = result.ErrorMessage });
        }

        var response = new WeaponResponse(
            result.Weapon!.Id.Value,
            result.Weapon.Name.Value,
            result.Weapon.Type.Value,
            result.Weapon.Description,
            result.Weapon.HitPoints,
            result.Weapon.Damage,
            result.Weapon.IsRepairable,
            result.Weapon.Value);

        return Ok(response);
    }

    /// <summary>
    /// Generates a random weapon with default characteristics.
    /// </summary>
    /// <returns>Returns the created random weapon.</returns>
    [HttpGet("create-random")]
    public async Task<IActionResult> CreateRandomWeapon()
    {
        var command = new CreateRandomWeaponCommand();
        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        // Retrieve the created weapon to return it
        var weaponQuery = new GetWeaponQuery(result.WeaponId!);
        var weaponResult = await mediator.Send(weaponQuery);

        if (!weaponResult.IsSuccess)
        {
            return StatusCode(500, new { message = "Weapon created but failed to retrieve" });
        }

        var response = new WeaponResponse(
            weaponResult.Weapon!.Id.Value,
            weaponResult.Weapon.Name.Value,
            weaponResult.Weapon.Type.Value,
            weaponResult.Weapon.Description,
            weaponResult.Weapon.HitPoints,
            weaponResult.Weapon.Damage,
            weaponResult.Weapon.IsRepairable,
            weaponResult.Weapon.Value);

        return Ok(response);
    }

    /// <summary>
    /// Applies damage to a weapon.
    /// </summary>
    /// <param name="request">The damage weapon request.</param>
    /// <returns>Returns the updated weapon after damage.</returns>
    [HttpPost("damage")]
    public async Task<IActionResult> DamageWeapon([FromBody] DamageWeaponRequest request)
    {
        var weaponId = WeaponId.Create(request.WeaponId);
        var command = new DamageWeaponCommand(weaponId, request.DamageAmount);
        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            if (result.ErrorMessage?.Contains("not found") == true)
            {
                return NotFound(new { message = result.ErrorMessage });
            }
            return BadRequest(new { message = result.ErrorMessage });
        }

        // Retrieve the updated weapon to return it
        var weaponQuery = new GetWeaponQuery(weaponId);
        var weaponResult = await mediator.Send(weaponQuery);

        if (!weaponResult.IsSuccess)
        {
            return StatusCode(500, new { message = "Weapon damaged but failed to retrieve updated state" });
        }

        var response = new WeaponResponse(
            weaponResult.Weapon!.Id.Value,
            weaponResult.Weapon.Name.Value,
            weaponResult.Weapon.Type.Value,
            weaponResult.Weapon.Description,
            weaponResult.Weapon.HitPoints,
            weaponResult.Weapon.Damage,
            weaponResult.Weapon.IsRepairable,
            weaponResult.Weapon.Value);

        return Ok(response);
    }

    /// <summary>
    /// Repairs a weapon by reducing its damage.
    /// </summary>
    /// <param name="request">The repair weapon request.</param>
    /// <returns>Returns the updated weapon after repair.</returns>
    [HttpPost("repair")]
    public async Task<IActionResult> RepairWeapon([FromBody] RepairWeaponRequest request)
    {
        var weaponId = WeaponId.Create(request.WeaponId);
        var command = new RepairWeaponCommand(weaponId, request.RepairAmount);
        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            if (result.ErrorMessage?.Contains("not found") == true)
            {
                return NotFound(new { message = result.ErrorMessage });
            }
            return BadRequest(new { message = result.ErrorMessage });
        }

        // Retrieve the updated weapon to return it
        var weaponQuery = new GetWeaponQuery(weaponId);
        var weaponResult = await mediator.Send(weaponQuery);

        if (!weaponResult.IsSuccess)
        {
            return StatusCode(500, new { message = "Weapon repaired but failed to retrieve updated state" });
        }

        var response = new WeaponResponse(
            weaponResult.Weapon!.Id.Value,
            weaponResult.Weapon.Name.Value,
            weaponResult.Weapon.Type.Value,
            weaponResult.Weapon.Description,
            weaponResult.Weapon.HitPoints,
            weaponResult.Weapon.Damage,
            weaponResult.Weapon.IsRepairable,
            weaponResult.Weapon.Value);

        return Ok(response);
    }

    /// <summary>
    /// Estimates the cost and benefits of repairing a weapon.
    /// </summary>
    /// <param name="request">The estimate repair request.</param>
    /// <returns>Returns the repair estimate.</returns>
    [HttpPost("estimate-repair")]
    public async Task<IActionResult> EstimateRepair([FromBody] EstimateRepairRequest request)
    {
        // First, get the weapon
        var weaponId = WeaponId.Create(request.WeaponId);
        var weaponQuery = new GetWeaponQuery(weaponId);
        var weaponResult = await mediator.Send(weaponQuery);

        if (!weaponResult.IsSuccess)
        {
            return NotFound(new { message = weaponResult.ErrorMessage });
        }

        // Convert WeaponData to Domain Weapon (this is a workaround - in real scenario we'd get the domain object directly)
        var weaponData = weaponResult.Weapon!;
        var domainWeapon = Domain.Weapon.Weapon.Create(
            WeaponName.Generate(weaponData.Type, weaponData.Name.Value.Split(' ').LastOrDefault() ?? ""),
            weaponData.Description,
            weaponData.MaxHitPoints,
            weaponData.Damage,
            weaponData.IsRepairable,
            weaponData.Value);

        // Simulate current state by applying damage
        if (weaponData.HitPoints < weaponData.MaxHitPoints)
        {
            var currentDamage = weaponData.MaxHitPoints - weaponData.HitPoints;
            domainWeapon.DamageWeapon(currentDamage);
        }

        var estimateQuery = new EstimateRepairQuery(domainWeapon, request.RepairAmount);
        var estimateResult = await mediator.Send(estimateQuery);

        if (!estimateResult.IsSuccess)
        {
            return BadRequest(new { message = estimateResult.ErrorMessage });
        }

        var response = new RepairEstimateResponse(
            estimateResult.Estimate!.RepairCost,
            estimateResult.Estimate.GainedHitPoints,
            estimateResult.Estimate.GainedValue);

        return Ok(response);
    }
}

/// <summary>
/// Request model for creating a new weapon.
/// </summary>
public sealed record CreateWeaponRequest(
    string Name,
    WeaponType.WeaponTypeEnum Type,
    string Description,
    int HitPoints,
    int Damage,
    bool IsRepairable,
    decimal Value);

/// <summary>
/// Response model for weapon data.
/// </summary>
public sealed record WeaponResponse(
    Guid Id,
    string Name,
    WeaponType.WeaponTypeEnum Type,
    string Description,
    int HitPoints,
    int Damage,
    bool IsRepairable,
    decimal Value);

/// <summary>
/// Request model for damaging a weapon.
/// </summary>
public sealed record DamageWeaponRequest(
    Guid WeaponId,
    int DamageAmount);

/// <summary>
/// Request model for repairing a weapon.
/// </summary>
public sealed record RepairWeaponRequest(
    Guid WeaponId,
    int RepairAmount);

/// <summary>
/// Request model for estimating weapon repair.
/// </summary>
public sealed record EstimateRepairRequest(
    Guid WeaponId,
    int RepairAmount);

/// <summary>
/// Response model for repair estimate data.
/// </summary>
public sealed record RepairEstimateResponse(
    int RepairCost,
    int GainedHitPoints,
    decimal GainedValue);
