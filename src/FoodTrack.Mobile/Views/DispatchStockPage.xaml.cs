using FoodTrack.Mobile.ViewModels;

namespace FoodTrack.Mobile.Views;

/// <summary>
/// Dispatch page showing FIFO batch suggestions.
/// </summary>
public partial class DispatchStockPage : ContentPage, IQueryAttributable
{
    private readonly DispatchStockViewModel _viewModel;
    private Guid? _pendingProductId;
    private Guid? _loadedProductId;

    public DispatchStockPage()
    {
        InitializeComponent();
        _viewModel = ServiceHelper.GetRequiredService<DispatchStockViewModel>();
        BindingContext = _viewModel;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (NavigationQueryHelper.TryGetGuid(query, "productId", out var productId))
        {
            _pendingProductId = productId;
            _viewModel.SetProduct(productId, NavigationQueryHelper.GetString(query, "productName"));
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_pendingProductId.HasValue && _pendingProductId != _loadedProductId)
        {
            _loadedProductId = _pendingProductId;
            await _viewModel.LoadFifoAsync(_pendingProductId.Value);
        }
    }

    private async void OnRefreshFifoClicked(object? sender, EventArgs e)
    {
        if (!Guid.TryParse(_viewModel.ProductIdText, out var productId))
        {
            await DisplayAlert("FIFO", "Zadajte platne Product ID.", "OK");
            return;
        }

        _loadedProductId = productId;
        await _viewModel.LoadFifoAsync(productId);
    }

    private async void OnSubmitClicked(object? sender, EventArgs e)
    {
        var result = await _viewModel.SubmitAsync();
        if (result is not null)
        {
            await DisplayAlert("Expedicia", $"Alokovane mnozstvo: {result.AllocatedQuantity:N2}.", "OK");
        }
    }
}
