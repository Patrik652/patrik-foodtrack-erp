using FoodTrack.Domain.Entities;
using FoodTrack.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FoodTrack.Infrastructure.Persistence;

/// <summary>
/// Seeds demo warehouse data for the portfolio application.
/// </summary>
public static class FoodTrackDbSeeder
{
    /// <summary>
    /// Populates the database when no seed data exists yet.
    /// </summary>
    public static async Task SeedAsync(FoodTrackDbContext dbContext, CancellationToken cancellationToken = default)
    {
        var hasChanges = false;

        if (!await dbContext.Products.AnyAsync(cancellationToken))
        {
            var products = new[]
            {
                Product.Create("Mlieko Tatranske 1L", "MLK-001", ProductCategory.Dairy, UnitOfMeasure.Liter, 25),
                Product.Create("Kuracie Prsia Chladene 1kg", "MAS-002", ProductCategory.Meat, UnitOfMeasure.Kilogram, 18),
                Product.Create("Rozok Bily 60g", "PEC-003", ProductCategory.Bakery, UnitOfMeasure.Piece, 120),
                Product.Create("Mineralna Voda Jemne Sycena 1.5L", "NAP-004", ProductCategory.Beverages, UnitOfMeasure.Liter, 30)
            };

            var now = new DateTime(2026, 4, 12, 8, 0, 0, DateTimeKind.Utc);
            var batches = new[]
            {
                Batch.Create(products[0].Id, "LOT-2026-04-001", now.AddDays(-8), now.AddDays(5), 128m, "A-01"),
                Batch.Create(products[0].Id, "LOT-2026-04-002", now.AddDays(-3), now.AddDays(16), 96m, "A-02"),
                Batch.Create(products[1].Id, "LOT-2026-04-101", now.AddDays(-4), now.AddDays(7), 42m, "B-01"),
                Batch.Create(products[2].Id, "LOT-2026-04-201", now.AddDays(-1), now.AddDays(2), 220m, "C-03"),
                Batch.Create(products[3].Id, "LOT-2026-04-301", now.AddDays(-12), now.AddDays(28), 180m, "D-02")
            };

            var movements = new[]
            {
                StockMovement.Create(batches[0].Id, StockMovementType.Receive, 128m, now.AddDays(-8), "Prijem od dodavatela Tatramilk", "Roman Skladnik"),
                StockMovement.Create(batches[1].Id, StockMovementType.Receive, 96m, now.AddDays(-3), "Doplnujuci prijem", "Roman Skladnik"),
                StockMovement.Create(batches[2].Id, StockMovementType.Receive, 42m, now.AddDays(-4), "Chladene maso", "Jana Expedicia"),
                StockMovement.Create(batches[3].Id, StockMovementType.Receive, 220m, now.AddDays(-1), "Pekarensky dovoz", "Marek Pekar"),
                StockMovement.Create(batches[4].Id, StockMovementType.Receive, 180m, now.AddDays(-12), "Napojovy sklad", "Iveta Smena")
            };

            await dbContext.Products.AddRangeAsync(products, cancellationToken);
            await dbContext.Batches.AddRangeAsync(batches, cancellationToken);
            await dbContext.StockMovements.AddRangeAsync(movements, cancellationToken);
            hasChanges = true;
        }

        if (!await dbContext.OperatorAccounts.AnyAsync(cancellationToken))
        {
            var operators = new[]
            {
                OperatorAccount.Create("OP-1001", "Roman Skladnik", "1234"),
                OperatorAccount.Create("OP-1002", "Jana Expedicia", "2345"),
                OperatorAccount.Create("OP-1003", "Marek Pekar", "3456")
            };

            await dbContext.OperatorAccounts.AddRangeAsync(operators, cancellationToken);
            hasChanges = true;
        }

        if (hasChanges)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
