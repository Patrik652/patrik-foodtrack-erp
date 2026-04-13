namespace FoodTrack.Application.Auth;

/// <summary>
/// Captures the credentials supplied by a warehouse operator during login.
/// </summary>
public sealed record AuthenticateOperatorCommand(string BadgeCode, string Pin);
