using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

var secretKey = "test-secret-key-that-is-long-enough-for-jwt-tokens-in-integration-tests-12345";
var issuer = "test-issuer";
var audience = "test-audience";

// Create the same key
var keyBytes = Encoding.UTF8.GetBytes(secretKey);
var signingKey = new SymmetricSecurityKey(keyBytes) { KeyId = "WeaponApiKey" };

// Create a simple JWT token
var handler = new JwtSecurityTokenHandler();
var tokenDescriptor = new SecurityTokenDescriptor
{
    Subject = new System.Security.Claims.ClaimsIdentity(new[]
    {
        new System.Security.Claims.Claim("sub", "test-user-id")
    }),
    Expires = DateTime.UtcNow.AddHours(1),
    Issuer = issuer,
    Audience = audience,
    SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature)
};

var token = handler.CreateToken(tokenDescriptor);
var tokenString = handler.WriteToken(token);

Console.WriteLine($"Token: {tokenString}");

// Try to validate the same token
var validationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = issuer,
    ValidAudience = audience,
    IssuerSigningKey = signingKey,
    ClockSkew = TimeSpan.FromMinutes(5)
};

try
{
    var principal = handler.ValidateToken(tokenString, validationParameters, out _);
    Console.WriteLine("Token validation successful!");
}
catch (Exception ex)
{
    Console.WriteLine($"Token validation failed: {ex.Message}");
}

