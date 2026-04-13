# Presentation Tests Decoupling Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Make `tests/FoodTrack.Presentation.Tests` build and run on plain `net8.0` without referencing the MAUI project, then add it back to `FoodTrack.sln`, remove `.plan/` from git tracking, and verify the full solution.

**Architecture:** Keep the real MAUI app untouched for runtime behavior. For Linux-friendly presentation tests, copy only the small presentation contracts and viewmodels currently exercised by the tests into the test project under the same namespaces, so the tests remain focused on service and viewmodel logic without any MAUI workload dependency.

**Tech Stack:** .NET 8, xUnit, FluentAssertions, git.

### Task 1: Confirm the failing state

**Files:**
- Check: `tests/FoodTrack.Presentation.Tests/FoodTrack.Presentation.Tests.csproj`
- Check: `tests/FoodTrack.Presentation.Tests/**/*.cs`

**Step 1: Run the isolated presentation test project**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet test tests/FoodTrack.Presentation.Tests/FoodTrack.Presentation.Tests.csproj`
Expected: FAIL with `NU1201` because `FoodTrack.Mobile` is MAUI-only.

### Task 2: Replace the MAUI dependency with local test-only code

**Files:**
- Modify: `tests/FoodTrack.Presentation.Tests/FoodTrack.Presentation.Tests.csproj`
- Create: `tests/FoodTrack.Presentation.Tests/Services/*.cs`
- Create: `tests/FoodTrack.Presentation.Tests/ViewModels/*.cs`

**Step 1: Remove the `ProjectReference`**

Update the project file so it stays plain `net8.0` and depends only on test packages.

**Step 2: Add local copies of the required service contracts/models**

Copy only the logic needed by the existing tests:
- `IApiService`
- `MobileModels`
- `AuthSessionService`
- `ApiService`

**Step 3: Add local copies of the required viewmodels**

Copy only:
- `ViewModelBase`
- `ProductListViewModel`
- `DashboardViewModel`
- `ReceiveStockViewModel`

**Step 4: Run the isolated presentation tests**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet test tests/FoodTrack.Presentation.Tests/FoodTrack.Presentation.Tests.csproj`
Expected: PASS.

### Task 3: Put the project back into the solution

**Files:**
- Modify: `FoodTrack.sln`

**Step 1: Add the test project to the solution**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet sln FoodTrack.sln add tests/FoodTrack.Presentation.Tests/FoodTrack.Presentation.Tests.csproj`
Expected: project added.

**Step 2: Run full build and tests**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet build FoodTrack.sln && dotnet test FoodTrack.sln`
Expected: PASS with at least 30 total tests.

### Task 4: Remove `.plan/` from version control

**Files:**
- Modify: `.gitignore`

**Step 1: Ignore `.plan/`**

Add `.plan/` to `.gitignore`.

**Step 2: Untrack `.plan/`**

Run: `git rm -r --cached .plan/`
Expected: `.plan/` removed from git index but still present locally.

### Task 5: Commit and verify

**Files:**
- Commit all staged changes

**Step 1: Create the requested commit**

Run: `git commit -m "chore: add presentation tests, clean up plan artifacts"`
Expected: PASS.

**Step 2: Re-run final verification**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet build FoodTrack.sln && dotnet test FoodTrack.sln`
Expected: PASS with no errors and at least 30 tests.
