# FoodTrack Initial Scaffold Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Bootstrap a real FoodTrack ERP solution with Clean Architecture, SQLite-backed API foundations, tests, and project documentation.

**Architecture:** The solution will use separate Domain, Application, Infrastructure, API, and Tests projects under `src/` and `tests/`. Domain owns entities and enums, Application owns contracts and use cases, Infrastructure implements EF Core SQLite persistence, and the API wires dependency injection plus Swagger.

**Tech Stack:** .NET 8, ASP.NET Core Web API, Entity Framework Core 8, SQLite, xUnit, FluentAssertions, Swagger/Swashbuckle.

### Task 1: Toolchain and Solution Bootstrap

**Files:**
- Create: `FoodTrack.sln`
- Create: `global.json`
- Create: `Directory.Build.props`
- Create: `src/FoodTrack.Domain/FoodTrack.Domain.csproj`
- Create: `src/FoodTrack.Application/FoodTrack.Application.csproj`
- Create: `src/FoodTrack.Infrastructure/FoodTrack.Infrastructure.csproj`
- Create: `src/FoodTrack.API/FoodTrack.API.csproj`
- Create: `tests/FoodTrack.Domain.Tests/FoodTrack.Domain.Tests.csproj`

**Step 1: Install the local SDK**

Run: `curl -fsSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh && bash /tmp/dotnet-install.sh --channel 8.0 --install-dir "$HOME/.dotnet"`
Expected: local `~/.dotnet/dotnet` becomes available and reports an 8.x SDK.

**Step 2: Scaffold the empty projects**

Run: `PATH="$HOME/.dotnet:$PATH" dotnet new sln -n FoodTrack`
Expected: solution file created.

**Step 3: Add project references and shared build defaults**

Create `Directory.Build.props` with nullable, implicit usings, and C# language version enabled. Add project references that enforce Clean Architecture dependency direction.

**Step 4: Verify bootstrap**

Run: `PATH="$HOME/.dotnet:$PATH" dotnet sln FoodTrack.sln list`
Expected: all scaffolded projects are listed.

### Task 2: Domain Tests and Entities

**Files:**
- Create: `src/FoodTrack.Domain/Enums/ProductCategory.cs`
- Create: `src/FoodTrack.Domain/Enums/UnitOfMeasure.cs`
- Create: `src/FoodTrack.Domain/Enums/BatchStatus.cs`
- Create: `src/FoodTrack.Domain/Enums/StockMovementType.cs`
- Create: `src/FoodTrack.Domain/Entities/Product.cs`
- Create: `src/FoodTrack.Domain/Entities/Batch.cs`
- Create: `src/FoodTrack.Domain/Entities/StockMovement.cs`
- Test: `tests/FoodTrack.Domain.Tests/Entities/ProductTests.cs`
- Test: `tests/FoodTrack.Domain.Tests/Entities/BatchTests.cs`

**Step 1: Write the failing tests**

```csharp
[Fact]
public void Create_ShouldRejectNonPositiveMinStockLevel()
{
    var action = () => Product.Create("Mlieko Tatranske 1L", "MLK-001", ProductCategory.Dairy, UnitOfMeasure.Liter, 0);
    action.Should().Throw<ArgumentOutOfRangeException>();
}
```

```csharp
[Fact]
public void RefreshStatus_ShouldMarkBatchExpired_WhenExpirationDateIsInPast()
{
    var batch = Batch.Create(Guid.NewGuid(), "LOT-2026-04-001", DateTime.UtcNow.AddDays(-10), DateTime.UtcNow.AddDays(-1), 15m, "A-01");
    batch.RefreshStatus(DateTime.UtcNow);
    batch.Status.Should().Be(BatchStatus.Expired);
}
```

**Step 2: Run tests to verify RED**

Run: `PATH="$HOME/.dotnet:$PATH" dotnet test tests/FoodTrack.Domain.Tests/FoodTrack.Domain.Tests.csproj --filter FullyQualifiedName~ProductTests`
Expected: FAIL because domain types do not exist yet.

**Step 3: Implement minimal domain model**

Add factory methods and invariants for product and batch creation, plus stock movement entity structure.

**Step 4: Run tests to verify GREEN**

Run: `PATH="$HOME/.dotnet:$PATH" dotnet test tests/FoodTrack.Domain.Tests/FoodTrack.Domain.Tests.csproj`
Expected: domain tests pass.

### Task 3: Application, Infrastructure, and API Baseline

**Files:**
- Create: `src/FoodTrack.Application/Abstractions/Persistence/IAppDbContext.cs`
- Create: `src/FoodTrack.Application/Products/ProductDto.cs`
- Create: `src/FoodTrack.Application/Products/IProductService.cs`
- Create: `src/FoodTrack.Application/Products/ProductService.cs`
- Create: `src/FoodTrack.Infrastructure/Persistence/FoodTrackDbContext.cs`
- Create: `src/FoodTrack.Infrastructure/DependencyInjection.cs`
- Create: `src/FoodTrack.API/Program.cs`
- Create: `src/FoodTrack.API/Controllers/ProductsController.cs`
- Create: `src/FoodTrack.API/appsettings.json`

**Step 1: Write the failing application/API tests**

Start with a service-level test proving products can be listed from persistence and mapped to DTOs.

**Step 2: Run tests to verify RED**

Run: `PATH="$HOME/.dotnet:$PATH" dotnet test`
Expected: FAIL because application services and infrastructure wiring are missing.

**Step 3: Implement the minimal vertical slice**

Add EF Core SQLite persistence, DI registration, Swagger, and a thin products controller returning seeded product data.

**Step 4: Run tests to verify GREEN**

Run: `PATH="$HOME/.dotnet:$PATH" dotnet test`
Expected: all current tests pass.

### Task 4: Documentation and Verification

**Files:**
- Create: `README.md`
- Modify: `.plan/FINISH_CRITERIA.md`
- Modify: `.plan/TODO.md`

**Step 1: Document architecture**

Write the project overview, setup instructions, and a Mermaid diagram describing the layer flow.

**Step 2: Define verification**

Populate `.plan/FINISH_CRITERIA.md` verification commands with the actual SDK-qualified `dotnet` build and test commands.

**Step 3: Run verification**

Run:

```bash
PATH="$HOME/.dotnet:$PATH" dotnet build FoodTrack.sln
PATH="$HOME/.dotnet:$PATH" dotnet test FoodTrack.sln
```

Expected: build and test succeed cleanly.

**Step 4: Close active TODO items**

Move completed bootstrap work into `Done`, leave future feature work in `Backlog`, and remove `.plan/WORK_LOCK` only after verification is clean.
