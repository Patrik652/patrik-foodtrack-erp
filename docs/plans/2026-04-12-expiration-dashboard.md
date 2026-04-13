# Expiration Dashboard Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Add a warehouse-friendly expiration dashboard summary and prioritized alert flow on top of the current FoodTrack backend.

**Architecture:** The feature stays inside the existing Clean Architecture split. Application will expose a new inventory dashboard query and DTOs, Infrastructure will add the repository method needed to read batches across products, and API will expose a thin dashboard controller. Domain expiration rules remain the source of truth for alert classification.

**Tech Stack:** .NET 8, ASP.NET Core Web API, EF Core 8, SQLite, xUnit, FluentAssertions.

### Task 1: Red Tests For Expiration Dashboard

**Files:**
- Modify: `tests/FoodTrack.Application.Tests/Inventory/InventoryQueryServiceTests.cs`

**Step 1: Write the failing test**

Add a test proving that the dashboard:
- counts expired, 7-day, 14-day, and 30-day alerts
- orders urgent batches first
- includes product metadata for warehouse operators

**Step 2: Run test to verify it fails**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet test tests/FoodTrack.Application.Tests/FoodTrack.Application.Tests.csproj --filter FullyQualifiedName~GetExpirationDashboardAsync`
Expected: FAIL because the new query method and DTOs do not exist yet.

**Step 3: Write minimal implementation**

Add dashboard DTOs, repository contract support, and the query service method with minimal projection and ordering logic.

**Step 4: Run test to verify it passes**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet test tests/FoodTrack.Application.Tests/FoodTrack.Application.Tests.csproj --filter FullyQualifiedName~GetExpirationDashboardAsync`
Expected: PASS.

### Task 2: Infrastructure and API Vertical Slice

**Files:**
- Modify: `src/FoodTrack.Application/Abstractions/Persistence/IBatchRepository.cs`
- Modify: `src/FoodTrack.Application/Inventory/IInventoryQueryService.cs`
- Modify: `src/FoodTrack.Application/Inventory/InventoryQueryService.cs`
- Create: `src/FoodTrack.Application/Inventory/ExpirationDashboardDto.cs`
- Create: `src/FoodTrack.Application/Inventory/ExpirationAlertItemDto.cs`
- Modify: `src/FoodTrack.Infrastructure/Persistence/BatchRepository.cs`
- Create: `src/FoodTrack.API/Controllers/DashboardController.cs`

**Step 1: Extend the contracts**

Add `ListAsync` to the batch repository and `GetExpirationDashboardAsync` to the inventory query service.

**Step 2: Implement the dashboard projection**

Compute alert buckets using the existing domain rule `GetExpirationAlert(asOfUtc)` and sort items by alert severity, nearest expiration, and product name.

**Step 3: Expose the API endpoint**

Add `GET /api/dashboard/expiration-overview` with optional `asOfUtc` query support.

**Step 4: Verify the vertical slice**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet build FoodTrack.sln`
Expected: build succeeds cleanly.

### Task 3: Documentation and Final Verification

**Files:**
- Modify: `README.md`
- Modify: `.plan/TODO.md`

**Step 1: Document the new endpoint**

Update the README endpoint list and describe what the dashboard returns.

**Step 2: Run full verification**

Run:

```bash
export PATH="$HOME/.dotnet:$PATH" && dotnet build FoodTrack.sln
export PATH="$HOME/.dotnet:$PATH" && dotnet test FoodTrack.sln
```

Expected: both commands succeed.

**Step 3: Run API smoke tests**

Run the API and verify:
- `GET /api/dashboard/expiration-overview`
- `GET /api/products`

**Step 4: Close active TODO items**

Move the expiration dashboard item from `In Progress` to `Done` when verification is green.
