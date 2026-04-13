# Inventory Write Flows Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Add write-side inventory workflows for receiving stock, dispatching stock with FIFO rotation, and adjusting batch quantities.

**Architecture:** The feature will extend the current Clean Architecture backend with a write-oriented application service, minimal domain mutation methods, EF-backed repositories, and thin API endpoints. FIFO dispatch will reuse existing domain policy ordering and produce persisted stock movements for traceability.

**Tech Stack:** .NET 8, ASP.NET Core Web API, EF Core 8, SQLite, xUnit, FluentAssertions.

### Task 1: Red Tests For Inventory Commands

**Files:**
- Create: `tests/FoodTrack.Application.Tests/Inventory/InventoryCommandServiceTests.cs`

**Step 1: Write the failing tests**

Add tests covering:
- receiving a new batch creates one `Receive` movement
- dispatching a product consumes older FIFO batches first and creates `Dispatch` movements
- adjusting a batch quantity updates its state and records an `Adjust` movement

**Step 2: Run test to verify it fails**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet test tests/FoodTrack.Application.Tests/FoodTrack.Application.Tests.csproj --filter FullyQualifiedName~InventoryCommandServiceTests`
Expected: FAIL because the command service, commands, and write repository contracts do not exist.

**Step 3: Write minimal implementation**

Add command DTOs, repository abstractions, and a write service that creates batches and stock movements while applying domain rules.

**Step 4: Run test to verify it passes**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet test tests/FoodTrack.Application.Tests/FoodTrack.Application.Tests.csproj --filter FullyQualifiedName~InventoryCommandServiceTests`
Expected: PASS.

### Task 2: Domain and Infrastructure Support

**Files:**
- Modify: `src/FoodTrack.Domain/Entities/Batch.cs`
- Modify: `src/FoodTrack.Application/Abstractions/Persistence/IProductRepository.cs`
- Modify: `src/FoodTrack.Application/Abstractions/Persistence/IBatchRepository.cs`
- Create: `src/FoodTrack.Application/Abstractions/Persistence/IStockMovementRepository.cs`
- Create: `src/FoodTrack.Application/Inventory/IInventoryCommandService.cs`
- Create: `src/FoodTrack.Application/Inventory/Commands/*.cs`
- Create: `src/FoodTrack.Application/Inventory/Results/*.cs`
- Create: `src/FoodTrack.Application/Inventory/InventoryCommandService.cs`
- Modify: `src/FoodTrack.Infrastructure/Persistence/ProductRepository.cs`
- Modify: `src/FoodTrack.Infrastructure/Persistence/BatchRepository.cs`
- Create: `src/FoodTrack.Infrastructure/Persistence/StockMovementRepository.cs`
- Modify: `src/FoodTrack.Infrastructure/Persistence/FoodTrackDbContext.cs`

**Step 1: Extend domain behavior**

Add minimal batch mutation methods for dispatching and quantity adjustment with proper status refresh.

**Step 2: Implement persistence**

Support product lookup, batch add/update, movement writes, and `SaveChangesAsync`.

**Step 3: Wire the command service**

Implement receive, dispatch, and adjust flows with FIFO dispatch ordering and persisted movement history.

**Step 4: Verify the feature slice**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet build FoodTrack.sln`
Expected: build succeeds cleanly.

### Task 3: API Endpoints And Verification

**Files:**
- Create: `src/FoodTrack.API/Controllers/InventoryController.cs`
- Modify: `src/FoodTrack.API/Program.cs`
- Modify: `README.md`
- Modify: `.plan/TODO.md`

**Step 1: Expose write endpoints**

Add:
- `POST /api/inventory/receive`
- `POST /api/inventory/dispatch`
- `POST /api/inventory/adjustments`

**Step 2: Document the new flows**

Update README endpoint list and describe how these write operations behave.

**Step 3: Run full verification**

Run:

```bash
export PATH="$HOME/.dotnet:$PATH" && dotnet build FoodTrack.sln
export PATH="$HOME/.dotnet:$PATH" && dotnet test FoodTrack.sln
```

Expected: both commands succeed.

**Step 4: Run API smoke tests**

Start the API and verify that receiving stock, dispatching stock, and adjusting stock all return successful responses with persisted changes visible through read endpoints.

**Step 5: Close active TODO items**

Move the write-flow item from `In Progress` to `Done` only when verification and smoke tests are green.
