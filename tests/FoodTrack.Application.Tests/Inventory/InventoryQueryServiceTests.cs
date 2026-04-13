using FluentAssertions;
using FoodTrack.Application.Abstractions.Persistence;
using FoodTrack.Application.Inventory;
using FoodTrack.Domain.Entities;
using FoodTrack.Domain.Enums;

namespace FoodTrack.Application.Tests.Inventory;

public sealed class InventoryQueryServiceTests
{
    [Fact]
    public async Task GetProductsAsync_ShouldReturnProductsOrderedByName()
    {
        IProductRepository productRepository = new FakeProductRepository(
            Product.Create("Zelene Jablko 1kg", "OVO-002", ProductCategory.Other, UnitOfMeasure.Kilogram, 10),
            Product.Create("Maslo Cerstve 250g", "MLK-004", ProductCategory.Dairy, UnitOfMeasure.Piece, 15));
        IBatchRepository batchRepository = new FakeBatchRepository();
        var service = new InventoryQueryService(productRepository, batchRepository);

        var products = await service.GetProductsAsync(CancellationToken.None);

        products.Select(product => product.Name).Should().Equal("Maslo Cerstve 250g", "Zelene Jablko 1kg");
        products[0].Category.Should().Be("Dairy");
        products[0].Unit.Should().Be("Piece");
    }

    [Fact]
    public async Task GetFifoBatchesAsync_ShouldReturnDispatchableBatchesInPolicyOrder()
    {
        var asOfUtc = new DateTime(2026, 4, 12, 8, 0, 0, DateTimeKind.Utc);
        var productId = Guid.NewGuid();
        IBatchRepository batchRepository = new FakeBatchRepository(
            Batch.Create(productId, "LOT-003", asOfUtc.AddDays(-2), asOfUtc.AddDays(20), 8m, "B-03"),
            Batch.Create(productId, "LOT-001", asOfUtc.AddDays(-6), asOfUtc.AddDays(5), 4m, "B-01"),
            Batch.Create(productId, "LOT-002", asOfUtc.AddDays(-7), asOfUtc.AddDays(5), 6m, "B-02"));
        var service = new InventoryQueryService(new FakeProductRepository(), batchRepository);

        var batches = await service.GetFifoBatchesAsync(productId, asOfUtc, CancellationToken.None);

        batches.Select(batch => batch.BatchNumber).Should().Equal("LOT-002", "LOT-001", "LOT-003");
        batches[0].ExpirationAlert.Should().Be("Critical7Days");
    }

    [Fact]
    public async Task GetExpirationDashboardAsync_ShouldAggregateAlertBucketsAndOrderPriorityItems()
    {
        var asOfUtc = new DateTime(2026, 4, 12, 8, 0, 0, DateTimeKind.Utc);
        var dairyProduct = Product.Create("Mlieko Tatranske 1L", "MLK-001", ProductCategory.Dairy, UnitOfMeasure.Liter, 25);
        var bakeryProduct = Product.Create("Rozok Bily 60g", "PEC-003", ProductCategory.Bakery, UnitOfMeasure.Piece, 120);
        IProductRepository productRepository = new FakeProductRepository(dairyProduct, bakeryProduct);
        IBatchRepository batchRepository = new FakeBatchRepository(
            Batch.Create(dairyProduct.Id, "LOT-EXPIRED", asOfUtc.AddDays(-9), asOfUtc.AddDays(-1), 12m, "A-01"),
            Batch.Create(dairyProduct.Id, "LOT-7D", asOfUtc.AddDays(-4), asOfUtc.AddDays(3), 18m, "A-02"),
            Batch.Create(bakeryProduct.Id, "LOT-14D", asOfUtc.AddDays(-2), asOfUtc.AddDays(11), 160m, "C-01"),
            Batch.Create(bakeryProduct.Id, "LOT-30D", asOfUtc.AddDays(-1), asOfUtc.AddDays(25), 220m, "C-02"),
            Batch.Create(bakeryProduct.Id, "LOT-LATER", asOfUtc.AddDays(-1), asOfUtc.AddDays(45), 75m, "C-03"));
        var service = new InventoryQueryService(productRepository, batchRepository);

        var dashboard = await service.GetExpirationDashboardAsync(asOfUtc, CancellationToken.None);

        dashboard.AsOfUtc.Should().Be(asOfUtc);
        dashboard.ExpiredCount.Should().Be(1);
        dashboard.Critical7DaysCount.Should().Be(1);
        dashboard.Warning14DaysCount.Should().Be(1);
        dashboard.Notice30DaysCount.Should().Be(1);
        dashboard.Items.Select(item => item.BatchNumber).Should().Equal("LOT-EXPIRED", "LOT-7D", "LOT-14D", "LOT-30D");
        dashboard.Items[0].ProductName.Should().Be("Mlieko Tatranske 1L");
        dashboard.Items[0].Alert.Should().Be("Expired");
        dashboard.Items[1].DaysUntilExpiration.Should().Be(3);
    }

    private sealed class FakeProductRepository(params Product[] products) : IProductRepository
    {
        public Task AddAsync(Product product, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task DeleteAsync(Product product, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken)
        {
            return Task.FromResult(products.SingleOrDefault(product => product.Id == productId));
        }

        public Task<IReadOnlyList<Product>> ListAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<Product>>(products);
        }
    }

    private sealed class FakeBatchRepository(params Batch[] batches) : IBatchRepository
    {
        public Task AddAsync(Batch batch, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task DeleteAsync(Batch batch, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<Batch?> GetByIdAsync(Guid batchId, CancellationToken cancellationToken)
        {
            return Task.FromResult(batches.SingleOrDefault(batch => batch.Id == batchId));
        }

        public Task<IReadOnlyList<Batch>> ListAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<Batch>>(batches);
        }

        public Task<IReadOnlyList<Batch>> ListByProductAsync(Guid productId, CancellationToken cancellationToken)
        {
            var filtered = batches.Where(batch => batch.ProductId == productId).ToArray();
            return Task.FromResult<IReadOnlyList<Batch>>(filtered);
        }
    }
}
