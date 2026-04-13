using System.Globalization;
using FoodTrack.Mobile.Services;

namespace FoodTrack.Mobile.ViewModels;

/// <summary>
/// Drives the FIFO dispatch screen.
/// </summary>
public sealed class DispatchStockViewModel(IApiService apiService) : ViewModelBase
{
    private IReadOnlyList<BatchSummary> _availableBatches = [];
    private string _productIdText = string.Empty;
    private string _productName = "Vyber produktu pre expediciu";
    private string _quantityText = string.Empty;
    private string _note = string.Empty;

    public string ProductIdText
    {
        get => _productIdText;
        set => SetProperty(ref _productIdText, value);
    }

    public string ProductName
    {
        get => _productName;
        private set => SetProperty(ref _productName, value);
    }

    public string QuantityText
    {
        get => _quantityText;
        set => SetProperty(ref _quantityText, value);
    }

    public string Note
    {
        get => _note;
        set => SetProperty(ref _note, value);
    }

    public IReadOnlyList<BatchSummary> AvailableBatches
    {
        get => _availableBatches;
        private set => SetProperty(ref _availableBatches, value);
    }

    public async Task LoadFifoAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        ProductIdText = productId.ToString();
        await ExecuteBusyAsync(async () =>
        {
            AvailableBatches = await apiService.GetFifoBatchesAsync(productId, cancellationToken);
            StatusMessage = $"Nacitaných FIFO sarzi: {AvailableBatches.Count}";
        }, "FIFO poradie sarzi sa nepodarilo nacitat.");
    }

    public void SetProduct(Guid productId, string? productName)
    {
        ProductIdText = productId.ToString();
        ProductName = string.IsNullOrWhiteSpace(productName)
            ? "Vybrany produkt"
            : Uri.UnescapeDataString(productName);
    }

    public async Task<DispatchStockResultModel?> SubmitAsync(CancellationToken cancellationToken = default)
    {
        DispatchStockResultModel? result = null;
        await ExecuteBusyAsync(async () =>
        {
            var productId = ParseGuid(ProductIdText);
            var quantity = ParseDecimal(QuantityText, "Zadajte platne mnozstvo pre vyskladnenie.");

            result = await apiService.DispatchStockAsync(new DispatchStockRequestModel
            {
                ProductId = productId,
                Quantity = quantity,
                Note = Note,
                DispatchedAtUtc = DateTime.UtcNow
            }, cancellationToken);

            StatusMessage = $"Vydanych kusov: {result.AllocatedQuantity:N2}";
            QuantityText = string.Empty;
        }, "Vyskladnenie sa nepodarilo odoslat.");

        return result;
    }

    private static Guid ParseGuid(string value)
    {
        if (Guid.TryParse(value, out var guid))
        {
            return guid;
        }

        throw new InvalidOperationException("Zadajte platne Product ID.");
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
