using FoodTrack.Application.Abstractions.Auth;
using FoodTrack.Application.Abstractions.Persistence;
using FoodTrack.Application.Auth;
using FoodTrack.Application.Inventory;
using FoodTrack.Application.Management;
using FoodTrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FoodTrack.Infrastructure;

/// <summary>
/// Registers infrastructure services and bootstraps the SQLite database.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers EF Core persistence and repository implementations.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<FoodTrackDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IBatchRepository, BatchRepository>();
        services.AddScoped<IStockMovementRepository, StockMovementRepository>();
        services.AddScoped<IOperatorAccountRepository, OperatorAccountRepository>();
        services.AddScoped<IInventoryUnitOfWork, InventoryUnitOfWork>();
        services.AddScoped<IInventoryCommandService, InventoryCommandService>();
        services.AddScoped<IOperatorAuthService, OperatorAuthService>();
        services.AddScoped<IWarehouseManagementService, WarehouseManagementService>();

        return services;
    }

    /// <summary>
    /// Ensures the database exists and contains demo seed data.
    /// </summary>
    public static async Task InitializeInfrastructureAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FoodTrackDbContext>();
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        await EnsureOperatorAccountSchemaAsync(dbContext, cancellationToken);
        await FoodTrackDbSeeder.SeedAsync(dbContext, cancellationToken);
    }

    private static async Task EnsureOperatorAccountSchemaAsync(FoodTrackDbContext dbContext, CancellationToken cancellationToken)
    {
        await dbContext.Database.ExecuteSqlRawAsync(
            """
            CREATE TABLE IF NOT EXISTS "OperatorAccounts" (
                "Id" TEXT NOT NULL CONSTRAINT "PK_OperatorAccounts" PRIMARY KEY,
                "BadgeCode" TEXT NOT NULL,
                "DisplayName" TEXT NOT NULL,
                "PinSalt" TEXT NOT NULL,
                "PinHash" TEXT NOT NULL,
                "IsActive" INTEGER NOT NULL
            );
            """,
            cancellationToken);

        await dbContext.Database.ExecuteSqlRawAsync(
            """
            CREATE UNIQUE INDEX IF NOT EXISTS "IX_OperatorAccounts_BadgeCode"
            ON "OperatorAccounts" ("BadgeCode");
            """,
            cancellationToken);
    }
}
