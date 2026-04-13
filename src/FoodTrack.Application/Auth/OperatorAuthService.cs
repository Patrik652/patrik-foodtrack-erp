using FoodTrack.Application.Abstractions.Auth;
using FoodTrack.Application.Abstractions.Persistence;

namespace FoodTrack.Application.Auth;

/// <summary>
/// Verifies operator credentials against the warehouse credential store.
/// </summary>
public sealed class OperatorAuthService(IOperatorAccountRepository operatorAccountRepository) : IOperatorAuthService
{
    /// <inheritdoc />
    public async Task<AuthenticatedOperator?> AuthenticateAsync(AuthenticateOperatorCommand command, CancellationToken cancellationToken)
    {
        var normalizedBadgeCode = NormalizeRequired(command.BadgeCode, nameof(command.BadgeCode)).ToUpperInvariant();
        var operatorAccount = await operatorAccountRepository.GetByBadgeCodeAsync(normalizedBadgeCode, cancellationToken);

        if (operatorAccount is null || !operatorAccount.VerifyPin(command.Pin))
        {
            return null;
        }

        return new AuthenticatedOperator(operatorAccount.Id, operatorAccount.BadgeCode, operatorAccount.DisplayName);
    }

    private static string NormalizeRequired(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", paramName);
        }

        return value.Trim();
    }
}
