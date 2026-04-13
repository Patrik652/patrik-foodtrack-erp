using FoodTrack.Mobile.ViewModels;

namespace FoodTrack.Mobile.Views;

/// <summary>
/// Receive stock page with batch/barcode input flow.
/// </summary>
public partial class ReceiveStockPage : ContentPage, IQueryAttributable
{
    private readonly ReceiveStockViewModel _viewModel;

    public ReceiveStockPage()
    {
        InitializeComponent();
        _viewModel = ServiceHelper.GetRequiredService<ReceiveStockViewModel>();
        BindingContext = _viewModel;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (NavigationQueryHelper.TryGetGuid(query, "productId", out var productId))
        {
            _viewModel.SetProduct(productId, NavigationQueryHelper.GetString(query, "productName"));
        }
    }

    private async void OnSubmitClicked(object? sender, EventArgs e)
    {
        var result = await _viewModel.SubmitAsync();
        if (result is not null)
        {
            await DisplayAlert("Prijem ulozeny", $"Sarza {result.BatchNumber} bola prijata do skladu.", "OK");
        }
    }
}
