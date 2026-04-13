# Stock Alerts, Recall, and Audit Filtering Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Extend FoodTrack ERP with low-stock dashboard alerts, a batch recall write flow, and explicit audit-log filtering coverage.

**Architecture:** Keep stock alerts in the read-side `InventoryQueryService`, because the feature is a projection over products and batches. Implement batch recall in the management/write side so the state transition and audit movement are created atomically through the existing repositories and unit of work. Keep stock-movement filtering in the existing management/API surface, but add explicit coverage to lock the contract down.

**Tech Stack:** ASP.NET Core 8, EF Core 8, xUnit, FluentAssertions.

### Task 1: Stock alerts

**Files:**
- Modify: `tests/FoodTrack.Application.Tests/Inventory/InventoryQueryServiceTests.cs`
- Modify: `src/FoodTrack.Application/Inventory/IInventoryQueryService.cs`
- Create: `src/FoodTrack.Application/Inventory/LowStockAlertDto.cs`
- Modify: `src/FoodTrack.Application/Inventory/InventoryQueryService.cs`
- Modify: `src/FoodTrack.API/Controllers/DashboardController.cs`
- Modify: `tests/FoodTrack.API.Tests/Endpoints/DashboardEndpointTests.cs`

**Step 1: Write the failing application test**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet test tests/FoodTrack.Application.Tests/FoodTrack.Application.Tests.csproj --filter LowStock`
Expected: FAIL because no stock-alert API exists in the query service.

**Step 2: Implement the DTO and query method**

Return products where the sum of effective active batch quantities is below `MinStockLevel`.

**Step 3: Add the API endpoint and integration test**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet test tests/FoodTrack.API.Tests/FoodTrack.API.Tests.csproj --filter StockAlerts`
Expected: PASS.

### Task 2: Batch recall flow

**Files:**
- Modify: `tests/FoodTrack.API.Tests/Endpoints/BatchCrudEndpointTests.cs`
- Modify: `src/FoodTrack.Domain/Entities/Batch.cs`
- Modify: `src/FoodTrack.Domain/Enums/StockMovementType.cs`
- Modify: `src/FoodTrack.Application/Management/IWarehouseManagementService.cs`
- Modify: `src/FoodTrack.Application/Management/WarehouseManagementService.cs`
- Modify: `src/FoodTrack.API/Controllers/WarehouseBatchesController.cs`

**Step 1: Write the failing recall endpoint test**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet test tests/FoodTrack.API.Tests/FoodTrack.API.Tests.csproj --filter Recall`
Expected: FAIL because the endpoint does not exist.

**Step 2: Implement batch recall**

Add a write-side method that marks the batch as recalled and writes a `Recall` stock movement with operator identity.

**Step 3: Re-run the recall test**

Expected: PASS.

### Task 3: Audit filtering coverage

**Files:**
- Modify: `tests/FoodTrack.API.Tests/Endpoints/StockMovementEndpointTests.cs`

**Step 1: Add explicit batch filter coverage**

Create or use multiple batches and verify `GET /api/stock-movements?batchId={id}` only returns movements for the requested batch.

**Step 2: Run the focused stock-movement test**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet test tests/FoodTrack.API.Tests/FoodTrack.API.Tests.csproj --filter StockMovement`
Expected: PASS.

### Task 4: Final verification and commit

**Files:**
- Commit all touched source and test files

**Step 1: Run full verification**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet build && dotnet test`
Expected: PASS.

**Step 2: Commit**

Run: `git commit -m "feat: stock alerts, batch recall, movement audit filtering"`
Expected: PASS.
