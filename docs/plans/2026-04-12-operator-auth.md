# Operator Authentication Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Add operator authentication for warehouse write actions and derive warehouse movement identity from the authenticated operator instead of client-supplied input.

**Architecture:** The slice will keep authentication concerns at the API/application boundary while preserving Clean Architecture. The application layer will define operator-auth contracts and login workflows, infrastructure will persist seeded operator accounts in SQLite, and the API will issue JWT bearer tokens, secure write endpoints, and map authenticated claims into inventory commands.

**Tech Stack:** .NET 8, ASP.NET Core Web API, JWT bearer authentication, EF Core 8, SQLite, xUnit, FluentAssertions.

### Task 1: Red API Tests For Authenticated Warehouse Writes

**Files:**
- Modify: `tests/FoodTrack.API.Tests/Endpoints/InventoryWriteEndpointTests.cs`
- Create: `tests/FoodTrack.API.Tests/Endpoints/AuthEndpointTests.cs`

**Step 1: Write the failing tests**

Add integration tests covering:
- `POST /api/auth/login` returns a bearer token for seeded operator credentials
- `POST /api/inventory/receive` rejects anonymous requests with `401`
- authenticated inventory writes succeed without client-provided `performedBy`
- the stored write identity comes from the authenticated operator

**Step 2: Run test to verify it fails**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet test tests/FoodTrack.API.Tests/FoodTrack.API.Tests.csproj --filter FullyQualifiedName~AuthEndpointTests|FullyQualifiedName~InventoryWriteEndpointTests`
Expected: FAIL because there is no auth endpoint, no bearer authentication, and write DTOs still require caller-provided identity.

**Step 3: Write minimal implementation**

Introduce the login endpoint, JWT wiring, secured write endpoints, and movement identity propagation from claims.

**Step 4: Run test to verify it passes**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet test tests/FoodTrack.API.Tests/FoodTrack.API.Tests.csproj --filter FullyQualifiedName~AuthEndpointTests|FullyQualifiedName~InventoryWriteEndpointTests`
Expected: PASS.

### Task 2: Operator Auth Application And Infrastructure

**Files:**
- Create: `src/FoodTrack.Domain/Entities/OperatorAccount.cs`
- Create: `src/FoodTrack.Application/Abstractions/Auth/IOperatorAuthService.cs`
- Create: `src/FoodTrack.Application/Abstractions/Persistence/IOperatorAccountRepository.cs`
- Create: `src/FoodTrack.Application/Auth/*.cs`
- Modify: `src/FoodTrack.Application/Inventory/Commands/*.cs`
- Modify: `src/FoodTrack.Application/Inventory/InventoryCommandService.cs`
- Modify: `src/FoodTrack.Infrastructure/Persistence/FoodTrackDbContext.cs`
- Modify: `src/FoodTrack.Infrastructure/Persistence/FoodTrackDbSeeder.cs`
- Create: `src/FoodTrack.Infrastructure/Persistence/OperatorAccountRepository.cs`
- Modify: `src/FoodTrack.Infrastructure/DependencyInjection.cs`

**Step 1: Extend the domain and persistence model**

Add an operator account entity and EF mapping for badge-code login data, with realistic seeded operators for demo scenarios.

**Step 2: Implement application auth flow**

Add login request/result models and an auth service that validates operator credentials through repository abstractions and returns operator identity data.

**Step 3: Refit inventory commands**

Remove caller-owned `PerformedBy` input from write command contracts and make the API/application flow set the operator identity server-side.

**Step 4: Verify the slice**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet build FoodTrack.sln`
Expected: build succeeds cleanly.

### Task 3: API Auth Surface, Docs, And Verification

**Files:**
- Modify: `src/FoodTrack.API/Program.cs`
- Create: `src/FoodTrack.API/Controllers/AuthController.cs`
- Modify: `src/FoodTrack.API/Controllers/InventoryController.cs`
- Modify: `src/FoodTrack.API/FoodTrack.API.csproj`
- Modify: `README.md`
- Modify: `.plan/TODO.md`

**Step 1: Add bearer auth**

Configure JWT bearer authentication, register token options, add an auth controller, and protect inventory write endpoints with authorization.

**Step 2: Keep read-side endpoints open**

Leave product, batch, and dashboard reads anonymous so the demo remains easy to browse in Swagger.

**Step 3: Document login flow**

Add README guidance for the seeded operator accounts and how to call authenticated write endpoints from Swagger or HTTP clients.

**Step 4: Run full verification**

Run:

```bash
export PATH="$HOME/.dotnet:$PATH" && dotnet build FoodTrack.sln
export PATH="$HOME/.dotnet:$PATH" && dotnet test FoodTrack.sln
```

Expected: both commands succeed.

**Step 5: Close active TODO items**

Move the auth item from `In Progress` to `Done` and remove `.plan/WORK_LOCK` only after integration tests and solution-wide verification are green.
