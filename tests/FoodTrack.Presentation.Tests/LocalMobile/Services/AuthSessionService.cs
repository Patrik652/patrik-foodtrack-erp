using System.Net.Http.Headers;

namespace FoodTrack.Mobile.Services;

public sealed class AuthSessionService
{
    public event EventHandler? SessionChanged;

    public string AccessToken { get; private set; } = string.Empty;

    public string BadgeCode { get; private set; } = string.Empty;

    public string OperatorName { get; private set; } = string.Empty;

    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(AccessToken);

    public void SetSession(OperatorLoginResponse response)
    {
        AccessToken = response.AccessToken;
        BadgeCode = response.BadgeCode;
        OperatorName = response.OperatorName;
        SessionChanged?.Invoke(this, EventArgs.Empty);
    }

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
