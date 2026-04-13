using FoodTrack.Application.Abstractions.Persistence;
using FoodTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodTrack.Infrastructure.Persistence;

/// <summary>
/// Reads operator accounts through EF Core.
/// </summary>
public sealed class OperatorAccountRepository(FoodTrackDbContext dbContext) : IOperatorAccountRepository
{
    /// <inheritdoc />
    public async Task<OperatorAccount?> GetByBadgeCodeAsync(string badgeCode, CancellationToken cancellationToken)
    {
        var normalizedBadgeCode = string.IsNullOrWhiteSpace(badgeCode)
            ? string.Empty
            : badgeCode.Trim().ToUpperInvariant();

        if (string.IsNullOrEmpty(normalizedBadgeCode))
        {
            return null;
        }

        return await dbContext.OperatorAccounts
            .SingleOrDefaultAsync(account => account.BadgeCode == normalizedBadgeCode, cancellationToken);
    }
}
