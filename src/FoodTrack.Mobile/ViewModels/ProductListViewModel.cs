using FoodTrack.Mobile.Services;

namespace FoodTrack.Mobile.ViewModels;

/// <summary>
/// Drives the mobile inventory product list.
/// </summary>
public sealed class ProductListViewModel(IApiService apiService) : ViewModelBase
{
    private string _searchText = string.Empty;
    private IReadOnlyList<ProductSummary> _allProducts = [];
    private IReadOnlyList<ProductSummary> _products = [];

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                ApplyFilter();
            }
        }
    }

    public IReadOnlyList<ProductSummary> Products
    {
        get => _products;
        private set => SetProperty(ref _products, value);
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        await ExecuteBusyAsync(async () =>
        {
            _allProducts = await apiService.GetProductsAsync(cancellationToken);
            ApplyFilter();
            StatusMessage = $"Synchronizovanych produktov: {_allProducts.Count}";
        }, "Produkty sa nepodarilo nacitat. Skuste obnovit synchronizaciu.");
    }

    public async Task<BatchSummary?> GetNextBatchAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        ErrorMessage = string.Empty;

        try
        {
            var batches = await apiService.GetFifoBatchesAsync(productId, cancellationToken);
            return batches.FirstOrDefault();
        }
        catch (HttpRequestException)
        {
            ErrorMessage = "FIFO poradie sarzi sa nepodarilo nacitat.";
            return null;
        }
        catch (Exception exception)
        {
            ErrorMessage = exception.Message;
            return null;
        }
    }

    private void ApplyFilter()
    {
        var filtered = string.IsNullOrWhiteSpace(SearchText)
            ? _allProducts
            : _allProducts
                .Where(product =>
                    product.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                    || product.Sku.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                    || product.Category.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                .ToArray();

        Products = filtered;
    }
}
