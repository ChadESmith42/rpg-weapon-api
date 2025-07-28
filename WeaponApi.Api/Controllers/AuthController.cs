using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WeaponApi.Api.Models.Requests;
using WeaponApi.Api.Models.Responses;
using WeaponApi.Application.Authentication;
using WeaponApi.Application.User.Commands;
using WeaponApi.Application.User.Queries;
using WeaponApi.Domain.User;

namespace WeaponApi.Api.Controllers;

/// <summary>
/// Authentication API controller providing user registration, login, and profile operations.
/// Implements JWT-based authentication following Clean Architecture patterns.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator mediator;
    private readonly IJwtTokenService jwtTokenService;

    public AuthController(IMediator mediator, IJwtTokenService jwtTokenService)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        this.jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
    }

    /// <summary>
    /// Registers a new user in the system.
    /// </summary>
    /// <param name="request">The user registration request.</param>
    /// <returns>Returns authentication response with JWT token on success.</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        var command = new RegisterUserCommand(
            request.Username,
            request.Name,
            request.Email,
            request.Password,
            request.DateOfBirth);

        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            if (result.ValidationErrors.Any())
            {
                return BadRequest(new { errors = result.ValidationErrors });
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        // Get the created user to generate tokens
        var userQuery = new GetUserProfileQuery(result.UserId!);
        var userResult = await mediator.Send(userQuery);

        if (!userResult.IsSuccess)
        {
            return StatusCode(500, new { message = "User created but failed to retrieve profile" });
        }

        // Generate JWT tokens
        var accessToken = jwtTokenService.GenerateAccessToken(userResult.User!, userResult.User!.Roles);
        var refreshToken = jwtTokenService.GenerateRefreshToken(userResult.User!.Id);
        var expiresAt = DateTime.UtcNow.AddMinutes(180); // Standard 3-hour expiration

        var response = new AuthResponse(
            accessToken,
            refreshToken,
            expiresAt,
            new UserResponse(
                userResult.User.Id.Value,
                userResult.User.Email.Value,
                userResult.User.Profile.Username,
                userResult.User.Profile.Name));

        return Created($"/api/auth/profile", response);
    }

    /// <summary>
    /// Authenticates a user and returns JWT tokens.
    /// </summary>
    /// <param name="request">The user login request.</param>
    /// <returns>Returns authentication response with JWT token on success.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
    {
        var command = new LoginUserCommand(request.EmailOrUsername, request.Password);
        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            return Unauthorized(new { message = result.ErrorMessage });
        }

        // Generate JWT tokens
        var accessToken = jwtTokenService.GenerateAccessToken(result.User!, result.Roles);
        var refreshToken = jwtTokenService.GenerateRefreshToken(result.User!.Id);
        var expiresAt = DateTime.UtcNow.AddMinutes(180); // Standard 3-hour expiration

        var response = new AuthResponse(
            accessToken,
            refreshToken,
            expiresAt,
            new UserResponse(
                result.User!.Id.Value,
                result.User.Email.Value,
                result.User.Profile.Username,
                result.User.Profile.Name));

        return Ok(response);
    }

    /// <summary>
    /// Refreshes JWT tokens using a valid refresh token.
    /// </summary>
    /// <param name="request">The refresh token request.</param>
    /// <returns>Returns new authentication response with refreshed JWT tokens.</returns>
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            // Validate the refresh token
            var tokenValidation = jwtTokenService.ValidateToken(request.RefreshToken);
            if (!tokenValidation.IsSuccess)
            {
                return Unauthorized(new { message = "Invalid or expired refresh token" });
            }

            // Get user ID from token
            var userIdResult = jwtTokenService.GetUserIdFromToken(request.RefreshToken);
            if (!userIdResult.IsSuccess || userIdResult.Value == null)
            {
                return Unauthorized(new { message = "Invalid user ID in refresh token" });
            }

            var query = new GetUserProfileQuery(userIdResult.Value);
            var result = await mediator.Send(query);

            if (!result.IsSuccess)
            {
                return Unauthorized(new { message = "User not found" });
            }

            // Generate new tokens
            var accessToken = jwtTokenService.GenerateAccessToken(result.User!, result.User!.Roles);
            var refreshToken = jwtTokenService.GenerateRefreshToken(result.User!.Id);
            var expiresAt = DateTime.UtcNow.AddMinutes(180);

            var response = new AuthResponse(
                accessToken,
                refreshToken,
                expiresAt,
                new UserResponse(
                    result.User!.Id.Value,
                    result.User.Email.Value,
                    result.User.Profile.Username,
                    result.User.Profile.Name));

            return Ok(response);
        }
        catch (Exception)
        {
            return Unauthorized(new { message = "Invalid refresh token" });
        }
    }

    /// <summary>
    /// Logs out the current user by invalidating their tokens.
    /// </summary>
    /// <returns>Returns success confirmation.</returns>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        // Extract user ID from JWT claims - ASP.NET Core maps 'sub' to nameidentifier
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub") ?? User.FindFirst("userId");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userGuid))
        {
            return Unauthorized(new { message = "Invalid or missing user ID in token" });
        }

        // In a real implementation, you would typically:
        // 1. Add the token to a blacklist/revocation list
        // 2. Store revoked tokens in a cache or database
        // 3. Check this list during token validation
        // For now, we'll just return success as the client should discard the tokens

        return Ok(new { message = "Successfully logged out" });
    }

    /// <summary>
    /// Retrieves the current user's profile information.
    /// Users can only access their own profile unless they have admin role.
    /// </summary>
    /// <returns>Returns the authenticated user's profile data.</returns>
    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        // Extract user ID from JWT claims - ASP.NET Core maps 'sub' to nameidentifier
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub") ?? User.FindFirst("userId");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var authenticatedUserGuid))
        {
            Console.WriteLine($"Failed to find user ID claim. userIdClaim: {userIdClaim?.Value}");
            return Unauthorized(new { message = "Invalid or missing user ID in token" });
        }

        var authenticatedUserId = UserId.Create(authenticatedUserGuid);
        var query = new GetUserProfileQuery(authenticatedUserId);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(new { message = result.ErrorMessage });
        }

        var response = new UserResponse(
            result.User!.Id.Value,
            result.User.Email.Value,
            result.User.Profile.Username,
            result.User.Profile.Name);

        return Ok(response);
    }

    /// <summary>
    /// Retrieves a user's profile information by user ID.
    /// Only accessible by admins or the user requesting their own profile.
    /// </summary>
    /// <param name="userId">The user ID to retrieve profile for.</param>
    /// <returns>Returns the requested user's profile data.</returns>
    [HttpGet("profile/{userId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetUserProfile(Guid userId)
    {
        // Extract authenticated user ID from JWT claims - ASP.NET Core maps 'sub' to nameidentifier
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub") ?? User.FindFirst("userId");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var authenticatedUserGuid))
        {
            return Unauthorized(new { message = "Invalid or missing user ID in token" });
        }

        var authenticatedUserId = UserId.Create(authenticatedUserGuid);
        var requestedUserId = UserId.Create(userId);

        // Check if user is requesting their own profile
        var isOwnProfile = authenticatedUserId.Value == requestedUserId.Value;

        // Check if user has admin role
        var isAdmin = User.IsInRole("Admin") || User.HasClaim("role", "Admin");

        // Allow access only if it's the user's own profile or they're an admin
        if (!isOwnProfile && !isAdmin)
        {
            return Forbid("You can only access your own profile unless you have admin privileges");
        }

        var query = new GetUserProfileQuery(requestedUserId);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(new { message = result.ErrorMessage });
        }

        var response = new UserResponse(
            result.User!.Id.Value,
            result.User.Email.Value,
            result.User.Profile.Username,
            result.User.Profile.Name);

        return Ok(response);
    }
}
