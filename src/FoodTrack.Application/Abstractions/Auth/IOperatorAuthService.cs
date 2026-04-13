using FoodTrack.Application.Auth;

namespace FoodTrack.Application.Abstractions.Auth;

/// <summary>
/// Authenticates warehouse operators against the current credential store.
/// </summary>
public interface IOperatorAuthService
{
    /// <summary>
    /// Validates the supplied operator credentials.
    /// </summary>
    Task<AuthenticatedOperator?> AuthenticateAsync(AuthenticateOperatorCommand command, CancellationToken cancellationToken);
}
