using FoodTrack.Mobile.Services;
using FoodTrack.Mobile.ViewModels;

namespace FoodTrack.Mobile.Views;

/// <summary>
/// Inventory product list page with search and refresh behavior.
/// </summary>
public partial class ProductListPage : ContentPage
{
    private readonly ProductListViewModel _viewModel;
    private bool _hasLoaded;

    public ProductListPage()
    {
        InitializeComponent();
        _viewModel = ServiceHelper.GetRequiredService<ProductListViewModel>();
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_hasLoaded)
        {
            return;
        }

        _hasLoaded = true;
        await _viewModel.LoadAsync();
    }

    private async void OnRefreshRequested(object? sender, EventArgs e)
    {
        await _viewModel.LoadAsync();
        if (sender is RefreshView refreshView)
        {
            refreshView.IsRefreshing = false;
        }
    }

    private async void OnViewBatchClicked(object? sender, EventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is not ProductSummary product)
        {
            return;
        }

        var batch = await _viewModel.GetNextBatchAsync(product.Id);
        if (batch is null)
        {
            var message = string.IsNullOrWhiteSpace(_viewModel.ErrorMessage)
                ? "Pre vybrany produkt zatial nie je aktivna FIFO sarza."
                : _viewModel.ErrorMessage;
            await DisplayAlert("Sarze", message, "OK");
            return;
        }

        await Shell.Current.GoToAsync($"{MobileRoutes.BatchDetail}?batchId={batch.Id}");
    }

    private async void OnReceiveClicked(object? sender, EventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is not ProductSummary product)
        {
            return;
        }

        var route = $"{MobileRoutes.ReceiveStock}?productId={product.Id}&productName={Uri.EscapeDataString(product.Name)}";
        await Shell.Current.GoToAsync(route);
    }

    private async void OnDispatchClicked(object? sender, EventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is not ProductSummary product)
        {
            return;
        }

        var route = $"{MobileRoutes.DispatchStock}?productId={product.Id}&productName={Uri.EscapeDataString(product.Name)}";
        await Shell.Current.GoToAsync(route);
    }
}
