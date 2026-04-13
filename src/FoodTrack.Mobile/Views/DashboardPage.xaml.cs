using FoodTrack.Mobile.Services;
using FoodTrack.Mobile.ViewModels;

namespace FoodTrack.Mobile.Views;

/// <summary>
/// Dashboard page for expiration risk visibility.
/// </summary>
public partial class DashboardPage : ContentPage
{
    private readonly DashboardViewModel _viewModel;

    public DashboardPage()
    {
        InitializeComponent();
        _viewModel = ServiceHelper.GetRequiredService<DashboardViewModel>();
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_viewModel.Overview is null)
        {
            await _viewModel.LoadAsync();
        }
    }

    private async void OnRefreshClicked(object? sender, EventArgs e)
    {
        await _viewModel.LoadAsync();
    }

    private async void OnOpenAlertBatchClicked(object? sender, EventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is not ExpirationAlertModel alert)
        {
            return;
        }

        await Shell.Current.GoToAsync($"{MobileRoutes.BatchDetail}?batchId={alert.BatchId}");
    }
}
