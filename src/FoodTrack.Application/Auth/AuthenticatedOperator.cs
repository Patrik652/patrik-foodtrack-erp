namespace FoodTrack.Application.Auth;

/// <summary>
/// Describes a successfully authenticated warehouse operator.
/// </summary>
public sealed record AuthenticatedOperator(Guid OperatorId, string BadgeCode, string DisplayName);
