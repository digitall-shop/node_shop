using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Helpers;

/// <summary>
/// Provides functionality to generate JSON Web Tokens (JWT) for authenticated users,
/// using issuer, audience, signing key and expiration values loaded from configuration.
/// </summary>
public static class JwtHelpers
{
    /// <summary>
    /// Creates and signs a JWT containing the specified user's identifier, username and role claims.
    /// Token parameters (issuer, audience, secret key and expiration) are read from <see cref="configuration"/>.
    /// </summary>
    /// <param name="user">The user entity for whom the token is being generated.</param>
    /// <param name="configuration"></param>
    /// <returns>A signed JWT as a string.</returns>
    public static string GenerateJwtToken(User user, IConfiguration configuration)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName ?? user.Id.ToString()),
            new Claim("isSuperAdmin", user.IsSuperAdmin.ToString().ToLower())
        };

        var keyBytes = Convert.FromBase64String(configuration.GetValue<string>("JwtSettings:Secret")!);
        var key = new SymmetricSecurityKey(keyBytes);
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration.GetValue<string>("JwtSettings:Issuer"),
            audience: configuration.GetValue<string>("JwtSettings:Audience"),
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(configuration.GetValue<long>("JwtSettings:ExpiryMinutes")),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}