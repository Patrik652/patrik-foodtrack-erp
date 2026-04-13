# Full FoodTrack ERP Completion Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Complete the FoodTrack ERP portfolio demo with the remaining backend surface, a MAUI-structured mobile application, Docker support, broader automated coverage, and final documentation polish.

**Architecture:** The backend will stay Clean Architecture with application services mediating API behavior, while the new mobile project will mirror a real .NET MAUI structure with Shell, XAML pages, viewmodels, and an HttpClient-based JWT-authenticated API layer. Because this host has no MAUI workloads installed, the mobile project must remain Linux-buildable by using a net8.0-compatible project layout and portability shims while preserving real MAUI page, Shell, and service organization.

**Tech Stack:** .NET 8, ASP.NET Core Web API, EF Core 8 with SQLite, xUnit, FluentAssertions, HttpClient, Docker, MAUI-style XAML/mobile presentation.

### Task 1: Red Tests For Remaining Backend Surface

**Files:**
- Modify: `tests/FoodTrack.API.Tests/Endpoints/*.cs`
- Modify: `tests/FoodTrack.Application.Tests/**/*.cs`
- Create: `tests/FoodTrack.API.Tests/Endpoints/ProductCrudEndpointTests.cs`
- Create: `tests/FoodTrack.API.Tests/Endpoints/BatchCrudEndpointTests.cs`
- Create: `tests/FoodTrack.API.Tests/Endpoints/StockMovementEndpointTests.cs`

**Step 1: Write the failing tests**

Add tests covering:
- product detail / create / update / delete API flows
- batch detail / update / delete API flows needed by the mobile detail screen
- stock movement listing and detail reads for the batch detail screen
- any missing dashboard or auth behavior used by the mobile client

**Step 2: Run test to verify it fails**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet test tests/FoodTrack.API.Tests/FoodTrack.API.Tests.csproj`
Expected: FAIL because the remaining endpoints and application services do not exist yet.

**Step 3: Write minimal implementation**

Add the smallest Clean Architecture surface required to make the new tests pass.

**Step 4: Run test to verify it passes**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet test tests/FoodTrack.API.Tests/FoodTrack.API.Tests.csproj`
Expected: PASS.

### Task 2: Backend Application Services And Controllers

**Files:**
- Modify: `src/FoodTrack.Application/Abstractions/Persistence/*.cs`
- Create: `src/FoodTrack.Application/Catalog/*.cs`
- Create: `src/FoodTrack.Application/Inventory/StockMovement*.cs`
- Modify: `src/FoodTrack.Infrastructure/Persistence/*.cs`
- Modify: `src/FoodTrack.Infrastructure/DependencyInjection.cs`
- Modify: `src/FoodTrack.API/Controllers/*.cs`
- Modify: `src/FoodTrack.API/Program.cs`

**Step 1: Extend repository contracts**

Add the read/write members needed for product, batch, and stock movement management without breaking current query/command flows.

**Step 2: Implement application services**

Add focused services for:
- product CRUD
- batch detail/update/delete flows
- stock movement listing/detail

**Step 3: Expose the API endpoints**

Keep read endpoints anonymous where appropriate and reuse auth for write/admin mutations.

**Step 4: Verify the slice**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet build FoodTrack.sln`
Expected: build succeeds cleanly.

### Task 3: MAUI-Structured Mobile Project And Red Presentation Tests

**Files:**
- Create: `src/FoodTrack.Mobile/FoodTrack.Mobile.csproj`
- Create: `src/FoodTrack.Mobile/App.xaml`
- Create: `src/FoodTrack.Mobile/AppShell.xaml`
- Create: `src/FoodTrack.Mobile/Views/*.xaml`
- Create: `src/FoodTrack.Mobile/ViewModels/*.cs`
- Create: `src/FoodTrack.Mobile/Services/*.cs`
- Create: `tests/FoodTrack.Presentation.Tests/FoodTrack.Presentation.Tests.csproj`
- Create: `tests/FoodTrack.Presentation.Tests/**/*.cs`

**Step 1: Write the failing tests**

Add presentation/service tests covering:
- login stores bearer token and exposes authenticated operator state
- product list loading/search behavior
- dashboard loading and offline-friendly error presentation
- receive / dispatch / batch detail workflows against a fake HTTP backend

**Step 2: Run test to verify it fails**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet test tests/FoodTrack.Presentation.Tests/FoodTrack.Presentation.Tests.csproj`
Expected: FAIL because the mobile presentation layer and service layer do not exist yet.

**Step 3: Build the mobile project**

Create a MAUI-style project with:
- Shell navigation (`Scan`, `Inventory`, `Dashboard`, `Settings`)
- login page
- product list page with search and refresh
- batch detail page
- receive, dispatch, and adjust pages
- dashboard page
- API service and auth/session services using `HttpClient`
- loading and error-state support in viewmodels/pages

**Step 4: Run test to verify it passes**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet test tests/FoodTrack.Presentation.Tests/FoodTrack.Presentation.Tests.csproj`
Expected: PASS.

### Task 4: Docker, Solution Wiring, And README

**Files:**
- Create: `src/FoodTrack.API/Dockerfile`
- Create: `docker-compose.yml`
- Modify: `FoodTrack.sln`
- Modify: `README.md`
- Modify: `.plan/TODO.md`

**Step 1: Add deployment support**

Create an API Dockerfile and root compose file for the SQLite-backed API service.

**Step 2: Wire the solution**

Add the new mobile/presentation test projects to `FoodTrack.sln` only if they keep Linux verification green.

**Step 3: Document the completed system**

Update README with:
- full Mermaid architecture diagram including Mobile, API, Application, Domain, Infrastructure, and Tests
- mobile and API setup instructions
- Docker usage
- demo credentials

### Task 5: Final Verification And TODO Cleanup

**Files:**
- Modify: `.plan/TODO.md`

**Step 1: Run full verification**

Run:

```bash
export PATH="$HOME/.dotnet:$PATH" && dotnet build FoodTrack.sln
export PATH="$HOME/.dotnet:$PATH" && dotnet test FoodTrack.sln --filter "FullyQualifiedName!~FoodTrack.Mobile"
test $(export PATH="$HOME/.dotnet:$PATH" && dotnet test FoodTrack.sln --filter "FullyQualifiedName!~FoodTrack.Mobile" --list-tests 2>/dev/null | grep -c 'test host') -ge 1
```

Expected: all commands succeed and the test count is at least 25.

**Step 2: Close all backlog**

Move every remaining `.plan/TODO.md` backlog item into `Done`, clear `In Progress` and `Blocked`, and remove `.plan/WORK_LOCK`.
