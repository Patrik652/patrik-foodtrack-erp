using FoodTrack.Domain.Enums;

namespace FoodTrack.Domain.Entities;

/// <summary>
/// Represents a physical batch of product units inside the warehouse.
/// </summary>
public sealed class Batch
{
    private Batch()
    {
    }

    private Batch(
        Guid id,
        Guid productId,
        string batchNumber,
        DateTime manufactureDate,
        DateTime expirationDate,
        decimal quantity,
        string location)
    {
        Id = id;
        ProductId = productId;
        BatchNumber = batchNumber;
        ManufactureDate = manufactureDate;
        ExpirationDate = expirationDate;
        Quantity = quantity;
        Location = location;
        Status = BatchStatus.Active;
    }

    /// <summary>
    /// Gets the unique identifier of the batch.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the product linked to the batch.
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// Gets the supplier or warehouse lot number.
    /// </summary>
    public string BatchNumber { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the manufacture date stored in UTC.
    /// </summary>
    public DateTime ManufactureDate { get; private set; }

    /// <summary>
    /// Gets the expiration date stored in UTC.
    /// </summary>
    public DateTime ExpirationDate { get; private set; }

    /// <summary>
    /// Gets the currently available quantity.
    /// </summary>
    public decimal Quantity { get; private set; }

    /// <summary>
    /// Gets the warehouse location code.
    /// </summary>
    public string Location { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the current batch status.
    /// </summary>
    public BatchStatus Status { get; private set; }

    /// <summary>
    /// Creates a validated batch instance.
    /// </summary>
    public static Batch Create(
        Guid productId,
        string batchNumber,
        DateTime manufactureDate,
        DateTime expirationDate,
        decimal quantity,
        string location)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException("Product identifier is required.", nameof(productId));
        }

        if (quantity <= 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Batch quantity must be positive.");
        }

        var normalizedManufactureDate = NormalizeUtc(manufactureDate);
        var normalizedExpirationDate = NormalizeUtc(expirationDate);
        if (normalizedExpirationDate < normalizedManufactureDate)
        {
            throw new ArgumentOutOfRangeException(nameof(expirationDate), "Expiration date cannot be earlier than manufacture date.");
        }

        return new Batch(
            Guid.NewGuid(),
            productId,
            NormalizeRequired(batchNumber, nameof(batchNumber)).ToUpperInvariant(),
            normalizedManufactureDate,
            normalizedExpirationDate,
            quantity,
            NormalizeRequired(location, nameof(location)).ToUpperInvariant());
    }

    /// <summary>
    /// Recomputes the batch status for the supplied point in time.
    /// </summary>
    public void RefreshStatus(DateTime asOfUtc)
    {
        if (Status == BatchStatus.Recalled)
        {
            return;
        }

        var referenceDate = NormalizeUtc(asOfUtc).Date;
        if (Quantity <= 0m)
        {
            Status = BatchStatus.Depleted;
            return;
        }

        Status = ExpirationDate.Date < referenceDate ? BatchStatus.Expired : BatchStatus.Active;
    }

    /// <summary>
    /// Calculates the expiration warning severity for the batch.
    /// </summary>
    public ExpirationAlertLevel GetExpirationAlert(DateTime asOfUtc)
    {
        if (Status == BatchStatus.Recalled)
        {
            return ExpirationAlertLevel.None;
        }

        var referenceDate = NormalizeUtc(asOfUtc).Date;
        if (ExpirationDate.Date < referenceDate || Status == BatchStatus.Expired)
        {
            return ExpirationAlertLevel.Expired;
        }

        var remainingDays = (ExpirationDate.Date - referenceDate).TotalDays;
        if (remainingDays <= 7d)
        {
            return ExpirationAlertLevel.Critical7Days;
        }

        if (remainingDays <= 14d)
        {
            return ExpirationAlertLevel.Warning14Days;
        }

        if (remainingDays <= 30d)
        {
            return ExpirationAlertLevel.Notice30Days;
        }

        return ExpirationAlertLevel.None;
    }

    /// <summary>
    /// Determines whether the batch can be dispatched on the supplied date.
    /// </summary>
    public bool IsDispatchable(DateTime asOfUtc)
    {
        var referenceDate = NormalizeUtc(asOfUtc).Date;
        return Quantity > 0m
            && Status != BatchStatus.Recalled
            && ExpirationDate.Date >= referenceDate;
    }

    /// <summary>
    /// Dispatches quantity from the batch while enforcing availability rules.
    /// </summary>
    public void Dispatch(decimal quantity, DateTime asOfUtc)
    {
        if (quantity <= 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Dispatch quantity must be positive.");
        }

        RefreshStatus(asOfUtc);
        if (!IsDispatchable(asOfUtc))
        {
            throw new InvalidOperationException("Batch cannot be dispatched in its current state.");
        }

        if (quantity > Quantity)
        {
            throw new InvalidOperationException("Dispatch quantity exceeds available stock.");
        }

        Quantity -= quantity;
        RefreshStatus(asOfUtc);
    }

    /// <summary>
    /// Sets the physical quantity of the batch after a warehouse adjustment.
    /// </summary>
    public void SetQuantity(decimal quantity, DateTime asOfUtc)
    {
        if (quantity < 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Adjusted quantity cannot be negative.");
        }

        Quantity = quantity;
        RefreshStatus(asOfUtc);
    }

    /// <summary>
    /// Applies an administrative warehouse update for location, quantity, and status.
    /// </summary>
    public void ApplyWarehouseUpdate(string location, decimal quantity, BatchStatus status, DateTime asOfUtc)
    {
        if (quantity < 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Batch quantity cannot be negative.");
        }

        Location = NormalizeRequired(location, nameof(location)).ToUpperInvariant();
        Quantity = status == BatchStatus.Depleted ? 0m : quantity;

        if (status == BatchStatus.Recalled)
        {
            Status = BatchStatus.Recalled;
            return;
        }

        if (status == BatchStatus.Depleted)
        {
            Status = BatchStatus.Depleted;
            return;
        }

        RefreshStatus(asOfUtc);
        if (status == BatchStatus.Active && Status == BatchStatus.Expired)
        {
            Status = BatchStatus.Active;
        }
    }

    private static DateTime NormalizeUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Unspecified => DateTime.SpecifyKind(value, DateTimeKind.Utc),
            _ => value.ToUniversalTime()
        };
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
