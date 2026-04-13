using FoodTrack.Mobile.Services;

namespace FoodTrack.Mobile.ViewModels;

/// <summary>
/// Drives the mobile batch detail screen.
/// </summary>
public sealed class BatchDetailViewModel(IApiService apiService) : ViewModelBase
{
    private BatchDetailModel? _batch;
    private Guid? _loadedBatchId;

    public BatchDetailModel? Batch
    {
        get => _batch;
        private set => SetProperty(ref _batch, value);
    }

    public Guid? LoadedBatchId
    {
        get => _loadedBatchId;
        private set => SetProperty(ref _loadedBatchId, value);
    }

    public async Task LoadAsync(Guid batchId, CancellationToken cancellationToken = default)
    {
        await ExecuteBusyAsync(async () =>
        {
            Batch = await apiService.GetBatchAsync(batchId, cancellationToken);
            LoadedBatchId = batchId;
            StatusMessage = $"Nacitana sarza {Batch.BatchNumber}";
        }, "Detail sarze sa nepodarilo nacitat. Skontrolujte pripojenie.");
    }
}
