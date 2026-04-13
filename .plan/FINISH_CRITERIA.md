# Finish Criteria

Edit the JSON block below. The runner enforces the machine fields directly and also feeds this whole file to the judge.

```json codex-runner
{
  "version": 1,
  "task": "Dokonci cely FoodTrack ERP podla AGENTS.md vratane .NET MAUI mobilnej appky, Dockeru, testov a README",
  "done_when": [
    "The ASP.NET Core 8 Web API is complete with all CRUD endpoints for products, batches, stock movements, and expiration dashboard.",
    "A .NET MAUI mobile project exists with Shell navigation, product list, batch detail, receive/dispatch/adjust pages, dashboard, and operator login.",
    "The MAUI app has an HttpClient service layer that calls the backend API with bearer token authentication.",
    "At least 25 automated tests pass across domain, application, API integration, and optionally MAUI layers.",
    "A Dockerfile and docker-compose.yml exist for the API project.",
    "README.md contains a Mermaid architecture diagram that includes the MAUI layer and full setup instructions.",
    "All items in `.plan/TODO.md` Backlog are moved to Done.",
    "No active work remains in `.plan/TODO.md` In Progress or Blocked.",
    "The verification commands succeed."
  ],
  "required_paths": [
    "FoodTrack.sln",
    "README.md",
    "src/FoodTrack.API/Program.cs",
    "src/FoodTrack.API/Dockerfile",
    "docker-compose.yml",
    "src/FoodTrack.Mobile/FoodTrack.Mobile.csproj",
    "src/FoodTrack.Mobile/AppShell.xaml",
    "src/FoodTrack.Mobile/Views/ProductListPage.xaml",
    "src/FoodTrack.Mobile/Views/DashboardPage.xaml",
    "src/FoodTrack.Mobile/Views/LoginPage.xaml",
    "src/FoodTrack.Mobile/Services/ApiService.cs"
  ],
  "forbidden_paths": [],
  "verify_commands": [
    "export PATH=\"$HOME/.dotnet:$PATH\" && dotnet build FoodTrack.sln",
    "export PATH=\"$HOME/.dotnet:$PATH\" && dotnet test FoodTrack.sln --filter \"FullyQualifiedName!~FoodTrack.Mobile\"",
    "test $(export PATH=\"$HOME/.dotnet:$PATH\" && dotnet test FoodTrack.sln --filter \"FullyQualifiedName!~FoodTrack.Mobile\" --list-tests 2>/dev/null | grep -c 'test host') -ge 1"
  ],
  "todo": {
    "require_file": true,
    "in_progress_must_be_empty": true,
    "blocked_must_be_empty": true
  },
  "work_lock": {
    "must_be_absent_for_complete": true
  }
}
```

## Additional Judge Guidance

The judge MUST NOT mark this task as complete until:

1. The `.plan/TODO.md` **Backlog section is empty** — all items must be moved to Done.
2. A `.NET MAUI` mobile project is present under `src/FoodTrack.Mobile/` with working XAML pages and Shell navigation.
3. The MAUI project connects to the API via an HttpClient-based service with JWT auth support.
4. Docker support files exist (Dockerfile for API + docker-compose.yml at root).
5. README.md has a full Mermaid diagram showing API + MAUI + Infrastructure layers.
6. All verify_commands pass with zero errors.
7. At least 25 tests pass across the solution.

If the worker has items remaining in Backlog, the task is **NOT complete** regardless of other checks.
