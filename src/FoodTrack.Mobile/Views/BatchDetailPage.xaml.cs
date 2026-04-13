using FoodTrack.Mobile.ViewModels;

namespace FoodTrack.Mobile.Views;

/// <summary>
/// Batch detail page showing quantity, location, expiration state, and history.
/// </summary>
public partial class BatchDetailPage : ContentPage, IQueryAttributable
{
    private readonly BatchDetailViewModel _viewModel;
    private Guid? _pendingBatchId;

    public BatchDetailPage()
    {
        InitializeComponent();
        _viewModel = ServiceHelper.GetRequiredService<BatchDetailViewModel>();
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
        if (_pendingBatchId.HasValue && _pendingBatchId != _viewModel.LoadedBatchId)
        {
            await _viewModel.LoadAsync(_pendingBatchId.Value);
        }
    }

    private async void OnAdjustClicked(object? sender, EventArgs e)
    {
        if (_viewModel.Batch is null)
        {
            return;
        }

        await Shell.Current.GoToAsync($"{MobileRoutes.AdjustStock}?batchId={_viewModel.Batch.Id}");
    }

    private async void OnDispatchClicked(object? sender, EventArgs e)
    {
        if (_viewModel.Batch is null)
        {
            return;
        }

        var route = $"{MobileRoutes.DispatchStock}?productId={_viewModel.Batch.ProductId}&productName={Uri.EscapeDataString(_viewModel.Batch.ProductName)}";
        await Shell.Current.GoToAsync(route);
    }
}
