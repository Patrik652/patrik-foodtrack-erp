using FoodTrack.Domain.Entities;

namespace FoodTrack.Application.Abstractions.Persistence;

/// <summary>
/// Reads operator accounts from persistence.
/// </summary>
public interface IOperatorAccountRepository
{
    /// <summary>
    /// Loads one operator account by badge code.
    /// </summary>
    Task<OperatorAccount?> GetByBadgeCodeAsync(string badgeCode, CancellationToken cancellationToken);
}
