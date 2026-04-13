using System.Globalization;
using FoodTrack.Mobile.Services;

namespace FoodTrack.Mobile.ViewModels;

public sealed class ReceiveStockViewModel(IApiService apiService) : ViewModelBase
{
    private string _productIdText = string.Empty;
    private string _productName = "Manualny vyber produktu";
    private string _batchNumber = string.Empty;
    private DateTime _manufactureDate = DateTime.UtcNow.Date;
    private DateTime _expirationDate = DateTime.UtcNow.Date.AddDays(7);
    private string _quantityText = string.Empty;
    private string _location = string.Empty;
    private string _note = string.Empty;

    public Guid ProductId
    {
        get => Guid.TryParse(ProductIdText, out var productId) ? productId : Guid.Empty;
        set => ProductIdText = value.ToString();
    }

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

    public string BatchNumber
    {
        get => _batchNumber;
        set => SetProperty(ref _batchNumber, value);
    }

    public DateTime ManufactureDate
    {
        get => _manufactureDate;
        set => SetProperty(ref _manufactureDate, value);
    }

    public DateTime ExpirationDate
    {
        get => _expirationDate;
        set => SetProperty(ref _expirationDate, value);
    }

    public decimal Quantity
    {
        get => decimal.TryParse(QuantityText, NumberStyles.Number, CultureInfo.InvariantCulture, out var quantity)
            ? quantity
            : 0m;
        set => QuantityText = value.ToString(CultureInfo.InvariantCulture);
    }

    public string QuantityText
    {
        get => _quantityText;
        set => SetProperty(ref _quantityText, value);
    }

    public string Location
    {
        get => _location;
        set => SetProperty(ref _location, value);
    }

    public string Note
    {
        get => _note;
        set => SetProperty(ref _note, value);
    }

    public void SetProduct(Guid productId, string? productName)
    {
        ProductIdText = productId.ToString();
        ProductName = string.IsNullOrWhiteSpace(productName)
            ? "Vybrany produkt"
            : Uri.UnescapeDataString(productName);
    }

    public async Task<ReceiveStockResultModel?> SubmitAsync(CancellationToken cancellationToken = default)
    {
        ReceiveStockResultModel? result = null;
        await ExecuteBusyAsync(async () =>
        {
            var productId = ParseGuid(ProductIdText, "Product ID");
            var quantity = ParseDecimal(QuantityText, "Zadajte platne mnozstvo pre prijem.");

            result = await apiService.ReceiveStockAsync(new ReceiveStockRequestModel
            {
                ProductId = productId,
                BatchNumber = BatchNumber,
                ManufactureDate = ManufactureDate,
                ExpirationDate = ExpirationDate,
                Quantity = quantity,
                Location = Location,
                Note = Note,
                ReceivedAtUtc = DateTime.UtcNow
            }, cancellationToken);

            StatusMessage = $"Prijem ulozeny: {result.BatchNumber}";
            QuantityText = string.Empty;
            BatchNumber = string.Empty;
            Note = string.Empty;
        }, "Prijem sarze sa nepodarilo odoslat. Skuste to znovu po obnove siete.");

        return result;
    }

    private static Guid ParseGuid(string value, string fieldName)
    {
        if (Guid.TryParse(value, out var guid))
        {
            return guid;
        }

        throw new InvalidOperationException($"Zadajte platne {fieldName}.");
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
