using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FoodTrack.Application.Auth;
using Microsoft.IdentityModel.Tokens;

namespace FoodTrack.API.Auth;

/// <summary>
/// Issues JWT bearer tokens for authenticated warehouse operators.
/// </summary>
internal sealed class JwtTokenIssuer(JwtTokenOptions options) : IJwtTokenIssuer
{
    /// <inheritdoc />
    public IssuedJwtToken Issue(AuthenticatedOperator authenticatedOperator)
    {
        var issuedAtUtc = DateTime.UtcNow;
        var expiresAtUtc = issuedAtUtc.AddMinutes(options.AccessTokenLifetimeMinutes);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, authenticatedOperator.OperatorId.ToString()),
            new Claim(ClaimTypes.NameIdentifier, authenticatedOperator.OperatorId.ToString()),
            new Claim(ClaimTypes.Name, authenticatedOperator.DisplayName),
            new Claim("badge_code", authenticatedOperator.BadgeCode)
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            options.Issuer,
            options.Audience,
            claims,
            issuedAtUtc,
            expiresAtUtc,
            credentials);

        return new IssuedJwtToken(new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc);
    }
}

/// <summary>
/// Exposes JWT token issuance for authenticated operators.
/// </summary>
public interface IJwtTokenIssuer
{
    /// <summary>
    /// Creates a signed bearer token for the supplied operator.
    /// </summary>
    IssuedJwtToken Issue(AuthenticatedOperator authenticatedOperator);
}

/// <summary>
/// Describes a freshly issued bearer token.
/// </summary>
public sealed record IssuedJwtToken(string AccessToken, DateTime ExpiresAtUtc);
