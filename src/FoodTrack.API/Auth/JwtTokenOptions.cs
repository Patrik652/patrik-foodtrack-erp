namespace FoodTrack.API.Auth;

/// <summary>
/// Configures JWT issuance and validation for warehouse operators.
/// </summary>
public sealed class JwtTokenOptions
{
    /// <summary>
    /// Gets the configuration section name.
    /// </summary>
    public const string SectionName = "Authentication:Jwt";

    /// <summary>
    /// Gets or sets the expected token issuer.
    /// </summary>
    public string Issuer { get; set; } = "FoodTrack.API";

    /// <summary>
    /// Gets or sets the expected token audience.
    /// </summary>
    public string Audience { get; set; } = "FoodTrack.Warehouse";

    /// <summary>
    /// Gets or sets the symmetric signing key.
    /// </summary>
    public string SigningKey { get; set; } = "FoodTrackDemoSigningKey-2026-04-Auth-Slice";

    /// <summary>
    /// Gets or sets the access-token lifetime in minutes.
    /// </summary>
    public int AccessTokenLifetimeMinutes { get; set; } = 240;

    /// <summary>
    /// Validates that the configured token settings are usable.
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(SigningKey) || SigningKey.Length < 32)
        {
            throw new InvalidOperationException("JWT signing key must be at least 32 characters long.");
        }

        if (AccessTokenLifetimeMinutes <= 0)
        {
            throw new InvalidOperationException("JWT access-token lifetime must be positive.");
        }
    }
}
