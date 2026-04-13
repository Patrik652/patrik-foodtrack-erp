using FoodTrack.API.Auth;
using FoodTrack.Application.Abstractions.Auth;
using FoodTrack.Application.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodTrack.API.Controllers;

/// <summary>
/// Exposes warehouse operator authentication endpoints.
/// </summary>
[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public sealed class AuthController(IOperatorAuthService operatorAuthService, IJwtTokenIssuer jwtTokenIssuer) : ControllerBase
{
    /// <summary>
    /// Logs a warehouse operator in with badge code and PIN.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(OperatorLoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<OperatorLoginResponse>> Login(
        [FromBody] OperatorLoginRequest request,
        CancellationToken cancellationToken)
    {
        var authenticatedOperator = await operatorAuthService.AuthenticateAsync(
            new AuthenticateOperatorCommand(request.BadgeCode, request.Pin),
            cancellationToken);

        if (authenticatedOperator is null)
        {
            return Unauthorized();
        }

        var token = jwtTokenIssuer.Issue(authenticatedOperator);
        return Ok(new OperatorLoginResponse
        {
            AccessToken = token.AccessToken,
            ExpiresAtUtc = token.ExpiresAtUtc,
            OperatorId = authenticatedOperator.OperatorId,
            OperatorName = authenticatedOperator.DisplayName,
            BadgeCode = authenticatedOperator.BadgeCode
        });
    }
}

/// <summary>
/// Captures the badge-code login request sent from the warehouse UI.
/// </summary>
public sealed record OperatorLoginRequest(string BadgeCode, string Pin);

/// <summary>
/// Returns a bearer token for a successfully authenticated warehouse operator.
/// </summary>
public sealed class OperatorLoginResponse
{
    /// <summary>
    /// Gets the signed JWT bearer token.
    /// </summary>
    public string AccessToken { get; init; } = string.Empty;

    /// <summary>
    /// Gets the token expiration timestamp in UTC.
    /// </summary>
    public DateTime ExpiresAtUtc { get; init; }

    /// <summary>
    /// Gets the operator identifier.
    /// </summary>
    public Guid OperatorId { get; init; }

    /// <summary>
    /// Gets the operator badge code.
    /// </summary>
    public string BadgeCode { get; init; } = string.Empty;

    /// <summary>
    /// Gets the display name bound into stock movement audit trails.
    /// </summary>
    public string OperatorName { get; init; } = string.Empty;
}
