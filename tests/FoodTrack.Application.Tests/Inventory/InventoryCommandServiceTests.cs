using FluentAssertions;
using FoodTrack.Application.Abstractions.Persistence;
using FoodTrack.Application.Inventory;
using FoodTrack.Application.Inventory.Commands;
using FoodTrack.Domain.Entities;
using FoodTrack.Domain.Enums;

namespace FoodTrack.Application.Tests.Inventory;

public sealed class InventoryCommandServiceTests
{
    [Fact]
    public async Task ReceiveAsync_ShouldCreateBatchAndReceiveMovement()
    {
        var receivedAtUtc = new DateTime(2026, 4, 12, 8, 0, 0, DateTimeKind.Utc);
        var product = Product.Create("Mlieko Tatranske 1L", "MLK-001", ProductCategory.Dairy, UnitOfMeasure.Liter, 25);
        var productRepository = new FakeProductRepository(product);
        var batchRepository = new FakeBatchRepository();
        var movementRepository = new FakeStockMovementRepository();
        var unitOfWork = new FakeInventoryUnitOfWork();
        var service = new InventoryCommandService(productRepository, batchRepository, movementRepository, unitOfWork);

        var result = await service.ReceiveAsync(
            new ReceiveStockCommand(
                product.Id,
                "lot-2026-04-555",
                receivedAtUtc.AddDays(-1),
                receivedAtUtc.AddDays(12),
                48m,
                "a-09",
                "Prijem z ranneho rozvozu",
                "Roman Skladnik",
                receivedAtUtc),
            CancellationToken.None);

        batchRepository.Items.Should().ContainSingle();
        batchRepository.Items[0].BatchNumber.Should().Be("LOT-2026-04-555");
        batchRepository.Items[0].Quantity.Should().Be(48m);
        batchRepository.Items[0].Location.Should().Be("A-09");
        movementRepository.Items.Should().ContainSingle(movement =>
            movement.Type == StockMovementType.Receive
            && movement.Quantity == 48m
            && movement.PerformedBy == "Roman Skladnik");
        unitOfWork.SaveChangesCalls.Should().Be(1);
        result.BatchNumber.Should().Be("LOT-2026-04-555");
    }

    [Fact]
    public async Task DispatchAsync_ShouldConsumeBatchesUsingFifoOrderAndCreateDispatchMovements()
    {
        var dispatchedAtUtc = new DateTime(2026, 4, 12, 8, 0, 0, DateTimeKind.Utc);
        var product = Product.Create("Maslo Cerstve 250g", "MLK-004", ProductCategory.Dairy, UnitOfMeasure.Piece, 15);
        var oldestBatch = Batch.Create(product.Id, "LOT-001", dispatchedAtUtc.AddDays(-9), dispatchedAtUtc.AddDays(5), 4m, "A-01");
        var nextBatch = Batch.Create(product.Id, "LOT-002", dispatchedAtUtc.AddDays(-4), dispatchedAtUtc.AddDays(8), 6m, "A-02");
        var productRepository = new FakeProductRepository(product);
        var batchRepository = new FakeBatchRepository(oldestBatch, nextBatch);
        var movementRepository = new FakeStockMovementRepository();
        var unitOfWork = new FakeInventoryUnitOfWork();
        var service = new InventoryCommandService(productRepository, batchRepository, movementRepository, unitOfWork);

        var result = await service.DispatchAsync(
            new DispatchStockCommand(
                product.Id,
                7m,
                dispatchedAtUtc,
                "Expedicia pre predajnu BA-101",
                "Jana Expedicia"),
            CancellationToken.None);

        oldestBatch.Quantity.Should().Be(0m);
        oldestBatch.Status.Should().Be(BatchStatus.Depleted);
        nextBatch.Quantity.Should().Be(3m);
        movementRepository.Items.Should().HaveCount(2);
        movementRepository.Items.Select(movement => movement.Quantity).Should().Equal(4m, 3m);
        result.RequestedQuantity.Should().Be(7m);
        result.AllocatedQuantity.Should().Be(7m);
        result.Lines.Select(line => line.BatchNumber).Should().Equal("LOT-001", "LOT-002");
    }

    [Fact]
    public async Task AdjustAsync_ShouldUpdateBatchQuantityAndCreateAdjustmentMovement()
    {
        var adjustedAtUtc = new DateTime(2026, 4, 12, 8, 0, 0, DateTimeKind.Utc);
        var product = Product.Create("Rozok Bily 60g", "PEC-003", ProductCategory.Bakery, UnitOfMeasure.Piece, 120);
        var batch = Batch.Create(product.Id, "LOT-201", adjustedAtUtc.AddDays(-1), adjustedAtUtc.AddDays(2), 25m, "C-03");
        var movementRepository = new FakeStockMovementRepository();
        var service = new InventoryCommandService(
            new FakeProductRepository(product),
            new FakeBatchRepository(batch),
            movementRepository,
            new FakeInventoryUnitOfWork());

        var result = await service.AdjustAsync(
            new AdjustStockCommand(
                batch.Id,
                0m,
                adjustedAtUtc,
                "Inventura potvrdila nulovy stav",
                "Marek Pekar"),
            CancellationToken.None);

        batch.Quantity.Should().Be(0m);
        batch.Status.Should().Be(BatchStatus.Depleted);
        movementRepository.Items.Should().ContainSingle(movement =>
            movement.Type == StockMovementType.Adjust
            && movement.Quantity == 25m);
        result.PreviousQuantity.Should().Be(25m);
        result.NewQuantity.Should().Be(0m);
        result.Status.Should().Be(nameof(BatchStatus.Depleted));
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
        public List<Batch> Items { get; } = [.. batches];

        public Task AddAsync(Batch batch, CancellationToken cancellationToken)
        {
            Items.Add(batch);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Batch batch, CancellationToken cancellationToken)
        {
            Items.Remove(batch);
            return Task.CompletedTask;
        }

        public Task<Batch?> GetByIdAsync(Guid batchId, CancellationToken cancellationToken)
        {
            return Task.FromResult(Items.SingleOrDefault(batch => batch.Id == batchId));
        }

        public Task<IReadOnlyList<Batch>> ListAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<Batch>>(Items);
        }

        public Task<IReadOnlyList<Batch>> ListByProductAsync(Guid productId, CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<Batch>>(Items.Where(batch => batch.ProductId == productId).ToArray());
        }
    }

    private sealed class FakeStockMovementRepository : IStockMovementRepository
    {
        public List<StockMovement> Items { get; } = [];

        public Task AddAsync(StockMovement movement, CancellationToken cancellationToken)
        {
            Items.Add(movement);
            return Task.CompletedTask;
        }

        public Task AddRangeAsync(IEnumerable<StockMovement> movements, CancellationToken cancellationToken)
        {
            Items.AddRange(movements);
            return Task.CompletedTask;
        }

        public Task DeleteByBatchIdAsync(Guid batchId, CancellationToken cancellationToken)
        {
            Items.RemoveAll(movement => movement.BatchId == batchId);
            return Task.CompletedTask;
        }

        public Task<StockMovement?> GetByIdAsync(Guid movementId, CancellationToken cancellationToken)
        {
            return Task.FromResult(Items.SingleOrDefault(movement => movement.Id == movementId));
        }

        public Task<IReadOnlyList<StockMovement>> ListAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<StockMovement>>(Items);
        }

        public Task<IReadOnlyList<StockMovement>> ListByBatchAsync(Guid batchId, CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<StockMovement>>(Items.Where(movement => movement.BatchId == batchId).ToArray());
        }
    }

    private sealed class FakeInventoryUnitOfWork : IInventoryUnitOfWork
    {
        public int SaveChangesCalls { get; private set; }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            SaveChangesCalls++;
            return Task.CompletedTask;
        }
    }
}
