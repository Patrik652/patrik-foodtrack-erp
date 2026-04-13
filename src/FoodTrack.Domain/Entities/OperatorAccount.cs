using System.Security.Cryptography;

namespace FoodTrack.Domain.Entities;

/// <summary>
/// Represents a warehouse operator who can authenticate into write-side workflows.
/// </summary>
public sealed class OperatorAccount
{
    private const int PinIterations = 100_000;
    private const int PinSaltSize = 16;
    private const int PinHashSize = 32;

    private OperatorAccount()
    {
    }

    private OperatorAccount(
        Guid id,
        string badgeCode,
        string displayName,
        string pinSalt,
        string pinHash,
        bool isActive)
    {
        Id = id;
        BadgeCode = badgeCode;
        DisplayName = displayName;
        PinSalt = pinSalt;
        PinHash = pinHash;
        IsActive = isActive;
    }

    /// <summary>
    /// Gets the unique operator identifier.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the badge code used during warehouse login.
    /// </summary>
    public string BadgeCode { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the operator name shown in stock movement history.
    /// </summary>
    public string DisplayName { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the persisted base64 salt for the hashed PIN.
    /// </summary>
    public string PinSalt { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the persisted base64 hash for the PIN.
    /// </summary>
    public string PinHash { get; private set; } = string.Empty;

    /// <summary>
    /// Gets whether the operator account is currently active.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Creates a validated operator account with a securely hashed PIN.
    /// </summary>
    public static OperatorAccount Create(string badgeCode, string displayName, string pin, bool isActive = true)
    {
        var normalizedPin = NormalizeRequired(pin, nameof(pin));
        var salt = RandomNumberGenerator.GetBytes(PinSaltSize);
        var hash = HashPin(normalizedPin, salt);

        return new OperatorAccount(
            Guid.NewGuid(),
            NormalizeRequired(badgeCode, nameof(badgeCode)).ToUpperInvariant(),
            NormalizeRequired(displayName, nameof(displayName)),
            Convert.ToBase64String(salt),
            Convert.ToBase64String(hash),
            isActive);
    }

    /// <summary>
    /// Verifies a PIN against the stored salted hash.
    /// </summary>
    public bool VerifyPin(string pin)
    {
        if (!IsActive || string.IsNullOrWhiteSpace(pin))
        {
            return false;
        }

        var salt = Convert.FromBase64String(PinSalt);
        var expectedHash = Convert.FromBase64String(PinHash);
        var candidateHash = HashPin(pin.Trim(), salt);

        return CryptographicOperations.FixedTimeEquals(candidateHash, expectedHash);
    }

    private static byte[] HashPin(string pin, byte[] salt)
    {
        return Rfc2898DeriveBytes.Pbkdf2(pin, salt, PinIterations, HashAlgorithmName.SHA256, PinHashSize);
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
