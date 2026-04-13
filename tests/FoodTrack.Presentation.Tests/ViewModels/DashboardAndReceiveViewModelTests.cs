using FluentAssertions;
using FoodTrack.Mobile.Services;
using FoodTrack.Mobile.ViewModels;

namespace FoodTrack.Presentation.Tests.ViewModels;

public sealed class DashboardAndReceiveViewModelTests
{
    [Fact]
    public async Task DashboardLoadAsync_ShouldPopulateOverview()
    {
        var service = new FakeApiService
        {
            Dashboard = new DashboardOverview
            {
                Critical7DaysCount = 2,
                Warning14DaysCount = 1,
                Notice30DaysCount = 3
            }
        };
        var viewModel = new DashboardViewModel(service);

        await viewModel.LoadAsync();

        viewModel.Overview.Should().NotBeNull();
        viewModel.Overview!.Critical7DaysCount.Should().Be(2);
    }

    [Fact]
    public async Task ReceiveSubmitAsync_ShouldReturnSuccessMessage()
    {
        var service = new FakeApiService
        {
            ReceiveResult = new ReceiveStockResultModel
            {
                BatchId = Guid.NewGuid(),
                MovementId = Guid.NewGuid(),
                BatchNumber = "LOT-MOBILE-001",
                Quantity = 12m,
                Location = "A-15"
            }
        };
        var viewModel = new ReceiveStockViewModel(service)
        {
            ProductId = Guid.NewGuid(),
            BatchNumber = "LOT-MOBILE-001",
            Location = "A-15",
            Quantity = 12m
        };

        var result = await viewModel.SubmitAsync();

        result.Should().NotBeNull();
        viewModel.StatusMessage.Should().Contain("LOT-MOBILE-001");
    }
}
