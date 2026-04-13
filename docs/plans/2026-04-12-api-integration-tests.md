# API Integration Tests Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Add integration coverage for the ASP.NET Core API and SQLite-backed infrastructure so the main warehouse flows are exercised end-to-end.

**Architecture:** The test slice will add a dedicated API test project that boots the real host through `WebApplicationFactory`, overrides the SQLite database path per test run, and exercises HTTP endpoints over the running ASP.NET Core pipeline. This keeps current application/domain tests intact while extending coverage to controllers, DI wiring, persistence, and seeded data.

**Tech Stack:** .NET 8, ASP.NET Core integration testing, xUnit, FluentAssertions, SQLite.

### Task 1: Red Tests For API Coverage

**Files:**
- Create: `tests/FoodTrack.API.Tests/FoodTrack.API.Tests.csproj`
- Create: `tests/FoodTrack.API.Tests/Infrastructure/FoodTrackApiFactory.cs`
- Create: `tests/FoodTrack.API.Tests/Endpoints/DashboardEndpointTests.cs`
- Create: `tests/FoodTrack.API.Tests/Endpoints/InventoryWriteEndpointTests.cs`

**Step 1: Write the failing tests**

Add integration tests covering:
- `GET /api/dashboard/expiration-overview`
- `POST /api/inventory/receive`
- `POST /api/inventory/dispatch`
- `POST /api/inventory/adjustments`

**Step 2: Run test to verify it fails**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet test tests/FoodTrack.API.Tests/FoodTrack.API.Tests.csproj`
Expected: FAIL because the API test project and test host setup do not exist yet.

**Step 3: Write minimal implementation**

Add the API test project, host factory, and any minimal program/test-host adjustments required for stable isolated database files.

**Step 4: Run test to verify it passes**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet test tests/FoodTrack.API.Tests/FoodTrack.API.Tests.csproj`
Expected: PASS.

### Task 2: Solution Wiring And Clean Isolation

**Files:**
- Modify: `FoodTrack.sln`
- Modify: `README.md`
- Modify: `.plan/TODO.md`

**Step 1: Add the project to the solution**

Include the new API test project in `FoodTrack.sln` so solution-wide verification covers it automatically.

**Step 2: Keep tests isolated**

Ensure each integration test host uses a temp SQLite file and cleans it up after execution.

**Step 3: Update docs**

Note that `dotnet test FoodTrack.sln` now includes HTTP-level integration coverage.

### Task 3: Full Verification

**Files:**
- Modify: `.plan/TODO.md`

**Step 1: Run full verification**

Run:

```bash
export PATH="$HOME/.dotnet:$PATH" && dotnet build FoodTrack.sln
export PATH="$HOME/.dotnet:$PATH" && dotnet test FoodTrack.sln
```

Expected: both commands succeed with the new API tests included.

**Step 2: Close active TODO items**

Move the coverage-expansion item from `In Progress` to `Done` only after the new test project is green and the repo is left clean.
