using FluentAssertions;
using FoodTrack.Mobile.Services;
using FoodTrack.Mobile.ViewModels;

namespace FoodTrack.Presentation.Tests.ViewModels;

public sealed class ProductListViewModelTests
{
    [Fact]
    public async Task LoadAsync_ShouldPopulateAndFilterProducts()
    {
        var service = new FakeApiService
        {
            Products =
            [
                new ProductSummary { Id = Guid.NewGuid(), Name = "Mlieko Tatranske 1L", Sku = "MLK-001", Category = "Dairy" },
                new ProductSummary { Id = Guid.NewGuid(), Name = "Rozok Bily 60g", Sku = "PEC-003", Category = "Bakery" }
            ]
        };
        var viewModel = new ProductListViewModel(service);

        await viewModel.LoadAsync();
        viewModel.SearchText = "MLK";

        viewModel.Products.Should().ContainSingle();
        viewModel.Products[0].Name.Should().Be("Mlieko Tatranske 1L");
    }

    [Fact]
    public async Task LoadAsync_ShouldExposeFriendlyErrorWhenApiFails()
    {
        var service = new FakeApiService { ThrowOnProducts = true };
        var viewModel = new ProductListViewModel(service);

        await viewModel.LoadAsync();

        viewModel.ErrorMessage.Should().Contain("Produkty sa nepodarilo nacitat");
    }
}
