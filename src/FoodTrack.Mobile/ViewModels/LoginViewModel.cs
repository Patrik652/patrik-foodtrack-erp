using FoodTrack.Mobile.Services;

namespace FoodTrack.Mobile.ViewModels;

/// <summary>
/// Drives the operator login screen.
/// </summary>
public sealed class LoginViewModel(IApiService apiService, AuthSessionService authSessionService) : ViewModelBase
{
    private string _badgeCode = string.Empty;
    private string _pin = string.Empty;

    public string BadgeCode
    {
        get => _badgeCode;
        set => SetProperty(ref _badgeCode, value);
    }

    public string Pin
    {
        get => _pin;
        set => SetProperty(ref _pin, value);
    }

    public string OperatorName => authSessionService.OperatorName;

    public async Task<bool> LoginAsync(CancellationToken cancellationToken = default)
    {
        var success = false;
        await ExecuteBusyAsync(async () =>
        {
            var response = await apiService.LoginAsync(BadgeCode, Pin, cancellationToken);
            BadgeCode = response.BadgeCode;
            OnPropertyChanged(nameof(OperatorName));
            StatusMessage = $"Prihlaseny operator: {response.OperatorName}";
            success = true;
        }, "Prihlasenie sa nepodarilo. Skontrolujte pripojenie alebo prihlasovacie udaje.");

        return success;
    }
}
