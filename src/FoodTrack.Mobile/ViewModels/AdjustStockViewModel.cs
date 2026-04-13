using System.Globalization;
using FoodTrack.Mobile.Services;

namespace FoodTrack.Mobile.ViewModels;

/// <summary>
/// Drives the physical stock adjustment form.
/// </summary>
public sealed class AdjustStockViewModel(IApiService apiService) : ViewModelBase
{
    private string _batchIdText = string.Empty;
    private string _batchNumber = "Manualny batch";
    private string _productName = string.Empty;
    private string _currentQuantity = string.Empty;
    private string _newQuantityText = string.Empty;
    private string _note = string.Empty;

    public string BatchIdText
    {
        get => _batchIdText;
        set => SetProperty(ref _batchIdText, value);
    }

    public string BatchNumber
    {
        get => _batchNumber;
        private set => SetProperty(ref _batchNumber, value);
    }

    public string ProductName
    {
        get => _productName;
        private set => SetProperty(ref _productName, value);
    }

    public string CurrentQuantity
    {
        get => _currentQuantity;
        private set => SetProperty(ref _currentQuantity, value);
    }

    public string NewQuantityText
    {
        get => _newQuantityText;
        set => SetProperty(ref _newQuantityText, value);
    }

    public string Note
    {
        get => _note;
        set => SetProperty(ref _note, value);
    }

    public async Task LoadAsync(Guid batchId, CancellationToken cancellationToken = default)
    {
        await ExecuteBusyAsync(async () =>
        {
            var batch = await apiService.GetBatchAsync(batchId, cancellationToken);
            BatchIdText = batchId.ToString();
            BatchNumber = batch.BatchNumber;
            ProductName = batch.ProductName;
            CurrentQuantity = batch.Quantity.ToString("N2", CultureInfo.CurrentCulture);
            NewQuantityText = batch.Quantity.ToString("N2", CultureInfo.CurrentCulture);
            StatusMessage = $"Nacitana sarza {batch.BatchNumber}";
        }, "Detail sarze sa nepodarilo nacitat pre inventurnu upravu.");
    }

    public async Task<AdjustStockResultModel?> SubmitAsync(CancellationToken cancellationToken = default)
    {
        AdjustStockResultModel? result = null;
        await ExecuteBusyAsync(async () =>
        {
            var batchId = ParseGuid(BatchIdText);
            var newQuantity = ParseDecimal(NewQuantityText, "Zadajte platne nove mnozstvo.");

            result = await apiService.AdjustStockAsync(new AdjustStockRequestModel
            {
                BatchId = batchId,
                NewQuantity = newQuantity,
                Note = Note,
                AdjustedAtUtc = DateTime.UtcNow
            }, cancellationToken);

            StatusMessage = $"Nova mnozstvena hladina: {result.NewQuantity:N2}";
            CurrentQuantity = result.NewQuantity.ToString("N2", CultureInfo.CurrentCulture);
        }, "Inventurna uprava sa nepodarila odoslat.");

        return result;
    }

    private static Guid ParseGuid(string value)
    {
        if (Guid.TryParse(value, out var guid))
        {
            return guid;
        }

        throw new InvalidOperationException("Zadajte platne Batch ID.");
    }

    private static decimal ParseDecimal(string value, string errorMessage)
    {
        if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.CurrentCulture, out var currentCultureValue))
        {
            return currentCultureValue;
        }

        if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var invariantValue))
        {
            return invariantValue;
        }

        throw new InvalidOperationException(errorMessage);
    }
}
