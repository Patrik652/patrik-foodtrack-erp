using FoodTrack.Mobile.ViewModels;

namespace FoodTrack.Mobile.Views;

/// <summary>
/// Adjustment page for physical stock corrections.
/// </summary>
public partial class AdjustStockPage : ContentPage, IQueryAttributable
{
    private readonly AdjustStockViewModel _viewModel;
    private Guid? _pendingBatchId;
    private Guid? _loadedBatchId;

    public AdjustStockPage()
    {
        InitializeComponent();
        _viewModel = ServiceHelper.GetRequiredService<AdjustStockViewModel>();
        BindingContext = _viewModel;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (NavigationQueryHelper.TryGetGuid(query, "batchId", out var batchId))
        {
            _pendingBatchId = batchId;
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_pendingBatchId.HasValue && _pendingBatchId != _loadedBatchId)
        {
            _loadedBatchId = _pendingBatchId;
            await _viewModel.LoadAsync(_pendingBatchId.Value);
        }
    }

    private async void OnLoadBatchClicked(object? sender, EventArgs e)
    {
        if (!Guid.TryParse(_viewModel.BatchIdText, out var batchId))
        {
            await DisplayAlert("Inventura", "Zadajte platne Batch ID.", "OK");
            return;
        }

        _loadedBatchId = batchId;
        await _viewModel.LoadAsync(batchId);
    }

    private async void OnSubmitClicked(object? sender, EventArgs e)
    {
        var result = await _viewModel.SubmitAsync();
        if (result is not null)
        {
            await DisplayAlert("Inventura", $"Nova mnozstvena hladina: {result.NewQuantity:N2}.", "OK");
        }
    }
}
