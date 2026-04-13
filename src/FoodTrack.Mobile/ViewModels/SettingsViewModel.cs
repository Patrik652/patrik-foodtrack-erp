using FoodTrack.Mobile.Services;

namespace FoodTrack.Mobile.ViewModels;

/// <summary>
/// Drives the worker settings and session page.
/// </summary>
public sealed class SettingsViewModel(ApiClientOptions apiClientOptions, AuthSessionService authSessionService) : ViewModelBase
{
    public string ApiBaseAddress => apiClientOptions.BaseAddress.ToString();

    public string OperatorName => authSessionService.OperatorName;

    public string BadgeCode => authSessionService.BadgeCode;

    public void Refresh()
    {
        OnPropertyChanged(nameof(ApiBaseAddress));
        OnPropertyChanged(nameof(OperatorName));
        OnPropertyChanged(nameof(BadgeCode));
    }

    public void Logout()
    {
        authSessionService.Clear();
        StatusMessage = "Operator bol odhlaseny.";
        Refresh();
    }
}
