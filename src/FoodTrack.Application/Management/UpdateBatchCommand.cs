using FoodTrack.Domain.Enums;

namespace FoodTrack.Application.Management;

/// <summary>
/// Captures the editable warehouse batch data used during updates.
/// </summary>
public sealed record UpdateBatchCommand(
    string Location,
    decimal Quantity,
    BatchStatus Status,
    DateTime UpdatedAtUtc);
