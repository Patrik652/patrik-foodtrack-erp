namespace FoodTrack.Mobile.Views;

/// <summary>
/// Quick-actions page for future scan and barcode workflows.
/// </summary>
public partial class ScanPage : ContentPage
{
    public ScanPage()
    {
        InitializeComponent();
    }

    private async void OnReceiveClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(MobileRoutes.ReceiveStock);
    }

    private async void OnDispatchClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(MobileRoutes.DispatchStock);
    }

    private async void OnAdjustClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(MobileRoutes.AdjustStock);
    }

    private async void OnInventoryClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//inventory");
    }

    private async void OnDashboardClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//dashboard");
    }
}
