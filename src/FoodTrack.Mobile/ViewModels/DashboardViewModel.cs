using FoodTrack.Mobile.Services;

namespace FoodTrack.Mobile.ViewModels;

/// <summary>
/// Drives the mobile expiration dashboard.
/// </summary>
public sealed class DashboardViewModel(IApiService apiService) : ViewModelBase
{
    private DashboardOverview? _overview;

    public DashboardOverview? Overview
    {
        get => _overview;
        private set => SetProperty(ref _overview, value);
    }

    public async Task LoadAsync(DateTime? asOfUtc = null, CancellationToken cancellationToken = default)
    {
        await ExecuteBusyAsync(async () =>
        {
            Overview = await apiService.GetDashboardAsync(asOfUtc, cancellationToken);
            StatusMessage = $"Dashboard k {Overview.AsOfUtc:dd.MM.yyyy HH:mm}";
        }, "Dashboard expiracii nie je dostupny. Skuste obnovit data neskor.");
    }
}
