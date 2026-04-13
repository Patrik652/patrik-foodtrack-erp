using FoodTrack.Domain.Entities;
using FoodTrack.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FoodTrack.Infrastructure.Persistence;

/// <summary>
/// EF Core database context for FoodTrack ERP.
/// </summary>
public sealed class FoodTrackDbContext(DbContextOptions<FoodTrackDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets the products table.
    /// </summary>
    public DbSet<Product> Products => Set<Product>();

    /// <summary>
    /// Gets the batches table.
    /// </summary>
    public DbSet<Batch> Batches => Set<Batch>();

    /// <summary>
    /// Gets the stock movements table.
    /// </summary>
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();

    /// <summary>
    /// Gets the operator accounts table.
    /// </summary>
    public DbSet<OperatorAccount> OperatorAccounts => Set<OperatorAccount>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(builder =>
        {
            builder.ToTable("Products");
            builder.HasKey(product => product.Id);
            builder.Property(product => product.Name).HasMaxLength(200).IsRequired();
            builder.Property(product => product.Sku).HasMaxLength(64).IsRequired();
            builder.Property(product => product.Category).HasConversion<string>().HasMaxLength(32);
            builder.Property(product => product.Unit).HasConversion<string>().HasMaxLength(32);
        });

        modelBuilder.Entity<Batch>(builder =>
        {
            builder.ToTable("Batches");
            builder.HasKey(batch => batch.Id);
            builder.Property(batch => batch.BatchNumber).HasMaxLength(64).IsRequired();
            builder.Property(batch => batch.Location).HasMaxLength(32).IsRequired();
            builder.Property(batch => batch.Quantity).HasColumnType("decimal(18,2)");
            builder.Property(batch => batch.Status).HasConversion<string>().HasMaxLength(32);
            builder.HasIndex(batch => new { batch.ProductId, batch.ExpirationDate });
        });

        modelBuilder.Entity<StockMovement>(builder =>
        {
            builder.ToTable("StockMovements");
            builder.HasKey(movement => movement.Id);
            builder.Property(movement => movement.Type).HasConversion<string>().HasMaxLength(32);
            builder.Property(movement => movement.Quantity).HasColumnType("decimal(18,2)");
            builder.Property(movement => movement.Note).HasMaxLength(256);
            builder.Property(movement => movement.PerformedBy).HasMaxLength(128).IsRequired();
        });

        modelBuilder.Entity<OperatorAccount>(builder =>
        {
            builder.ToTable("OperatorAccounts");
            builder.HasKey(account => account.Id);
            builder.Property(account => account.BadgeCode).HasMaxLength(32).IsRequired();
            builder.Property(account => account.DisplayName).HasMaxLength(128).IsRequired();
            builder.Property(account => account.PinSalt).HasMaxLength(128).IsRequired();
            builder.Property(account => account.PinHash).HasMaxLength(128).IsRequired();
            builder.HasIndex(account => account.BadgeCode).IsUnique();
        });
    }
}
