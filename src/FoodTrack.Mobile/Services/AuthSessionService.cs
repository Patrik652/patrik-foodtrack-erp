using System.Net.Http.Headers;

namespace FoodTrack.Mobile.Services;

/// <summary>
/// Stores the authenticated operator session for outbound API calls.
/// </summary>
public sealed class AuthSessionService
{
    /// <summary>
    /// Raised when the session has changed and the UI should refresh its root state.
    /// </summary>
    public event EventHandler? SessionChanged;

    /// <summary>
    /// Gets the active bearer token.
    /// </summary>
    public string AccessToken { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the authenticated operator badge code.
    /// </summary>
    public string BadgeCode { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the authenticated operator display name.
    /// </summary>
    public string OperatorName { get; private set; } = string.Empty;

    /// <summary>
    /// Gets whether the session currently contains a bearer token.
    /// </summary>
    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(AccessToken);

    /// <summary>
    /// Stores a successful login response inside the current session.
    /// </summary>
    public void SetSession(OperatorLoginResponse response)
    {
        AccessToken = response.AccessToken;
        BadgeCode = response.BadgeCode;
        OperatorName = response.OperatorName;
        SessionChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Clears the current session.
    /// </summary>
    public void Clear()
    {
        AccessToken = string.Empty;
        BadgeCode = string.Empty;
        OperatorName = string.Empty;
        SessionChanged?.Invoke(this, EventArgs.Empty);
    }

    internal void Apply(HttpRequestHeaders headers)
    {
        headers.Authorization = IsAuthenticated
            ? new AuthenticationHeaderValue("Bearer", AccessToken)
            : null;
    }
}
