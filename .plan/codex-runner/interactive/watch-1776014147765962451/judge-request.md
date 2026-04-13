Judge evaluation request.

Your only job here is to decide whether the worker is REALLY finished for now.

Rules:
- do not act like a second worker
- do not expand scope
- prefer `continue` over `complete` when uncertain
- `blocked` is only for real human-required blockers
- if deterministic checks fail, completion should normally be rejected

Task:
prejdi si súbori v tomto priečinku v ktorom si a začni pracovať na tomto projekte

Finish criteria:
# Finish Criteria

Edit the JSON block below. The runner enforces the machine fields directly and also feeds this whole file to the judge.

```json codex-runner
{
  "version": 1,
  "task": "prejdi si s\u00fabori v tomto prie\u010dinku v ktorom si a za\u010dni pracova\u0165 na tomto projekte",
  "done_when": [
    "The requested task is implemented end-to-end.",
    "No active work remains in `.plan/TODO.md`.",
    "The verification commands succeed."
  ],
  "required_paths": [],
  "forbidden_paths": [],
  "verify_commands": [],
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


Deterministic report:
{
  "passed": true,
  "checks": [
    {
      "name": "todo:file",
      "passed": true,
      "detail": "present",
      "required": true
    },
    {
      "name": "todo:in-progress-empty",
      "passed": true,
      "detail": "empty",
      "required": true
    },
    {
      "name": "todo:blocked-empty",
      "passed": true,
      "detail": "empty",
      "required": true
    },
    {
      "name": "work-lock:absent",
      "passed": true,
      "detail": "absent",
      "required": true
    }
  ]
}

Worker turn summary:
Worker turn id: 019d829e-e055-7f62-a128-9c2f11fb6236

Worker input messages:
- You are the interactive worker.

Task:
prejdi si súbori v tomto priečinku v ktorom si a začni pracovať na tomto projekte

Rules:
- Work directly in this repository.
- Read and obey `.plan/TODO.md`, `.plan/FINISH_CRITERIA.md`, and `.plan/WORK_LOCK` if present.
- Ignore `.plan/JUDGE_TODO.md`; that file belongs to the judge.
- The user may talk to you normally in this session.
- Your completed turns are forwarded to the judge automatically. Do not rely on the human to type `continue` if work remains.
- If you think the task is done, make sure the finish criteria and verification commands really pass first.
- Prefer continuing work over asking whether you should proceed.
- Never ask `would you like me to continue`, `should I proceed`, `if you want, I can`, or `let me know if you want` when the next local step is discoverable from repo state.
- Treat human messages like `continue`, `gogogo`, `yes continue`, `go ahead`, and `proceed` as evidence that you previously stopped too early. Do not force the human to repeat those nudges.
- Do not use `done` or `complete` language unless the whole task is actually finished, not just one sub-slice.
- When you finish a turn, write a concise, factual assistant message because the judge will review that message.

Finish criteria:
# Finish Criteria

Edit the JSON block below. The runner enforces the machine fields directly and also feeds this whole file to the judge.

```json codex-runner
{
  "version": 1,
  "task": "prejdi si s\u00fabori v tomto prie\u010dinku v ktorom si a za\u010dni pracova\u0165 na tomto projekte",
  "done_when": [
    "The requested task is implemented end-to-end.",
    "No active work remains in `.plan/TODO.md`.",
    "The verification commands succeed."
  ],
  "required_paths": [],
  "forbidden_paths": [],
  "verify_commands": [],
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


Current TODO:
# TODO

## Backlog

## In Progress
- [ ] prejdi si súbori v tomto priečinku v ktorom si a začni pracovať na tomto projekte (owner: main)

## Blocked

## Done


- pokračuj
- pokračuj
- pokračuj
- pokračuj
- Precitaj si aktualizovany .plan/TODO.md a .plan/FINISH_CRITERIA.md. Maju novy scope a prisne finish kriteria. Tvoj task je teraz: Dokonci cely         
  FoodTrack ERP podla AGENTS.md. Konkretne musis: (1) Vytvorit .NET MAUI mobilnu appku pod src/FoodTrack.Mobile/ s Shell navigaciou, login page, product 
  list, batch detail, receive/dispatch strankami, dashboard. (2) Wire MAUI HttpClient service layer na API s JWT auth. (3) Pridat Dockerfile a
  docker-compose.yml. (4) Rozirit testy na minimalne 25. (5) Aktualizovat README s plnym Mermaid diagramom. Vsetky polozky v TODO Backlog musia byt      
  hotove a presunutie do Done. Nepytaj sa ci mas pokracovat - rob az kym nebude vsetko hotove. Zacni hned.
- Precitaj si aktualizovany .plan/TODO.md a .plan/FINISH_CRITERIA.md. Maju novy scope a prisne finish kriteria. Tvoj task je teraz: Dokonci cely         
  FoodTrack ERP podla AGENTS.md. Konkretne musis: (1) Vytvorit .NET MAUI mobilnu appku pod src/FoodTrack.Mobile/ s Shell navigaciou, login page, product 
  list, batch detail, receive/dispatch strankami, dashboard. (2) Wire MAUI HttpClient service layer na API s JWT auth. (3) Pridat Dockerfile a
  docker-compose.yml. (4) Rozirit testy na minimalne 25. (5) Aktualizovat README s plnym Mermaid diagramom. Vsetky polozky v TODO Backlog musia byt      
  hotove a presunutie do Done. Nepytaj sa ci mas pokracovat - rob az kym nebude vsetko hotove. Zacni hned.

Worker last assistant message:
Rozšíril som backend aj mobilnú vrstvu na nový scope. Backend má teraz doplnený management surface pre product detail/CRUD, batch detail/update/delete a stock movement history v [ProductsController.cs](/home/pato/projects/work/Foodtrack_erp/src/FoodTrack.API/Controllers/ProductsController.cs), [WarehouseBatchesController.cs](/home/pato/projects/work/Foodtrack_erp/src/FoodTrack.API/Controllers/WarehouseBatchesController.cs) a [StockMovementsController.cs](/home/pato/projects/work/Foodtrack_erp/src/FoodTrack.API/Controllers/StockMovementsController.cs). Mobilný projekt je pod [src/FoodTrack.Mobile/FoodTrack.Mobile.csproj](/home/pato/projects/work/Foodtrack_erp/src/FoodTrack.Mobile/FoodTrack.Mobile.csproj) s [AppShell.xaml](/home/pato/projects/work/Foodtrack_erp/src/FoodTrack.Mobile/AppShell.xaml), loginom, inventory listom, batch detailom, receive/dispatch/adjust stránkami, dashboardom a JWT `HttpClient` service layerom v [ApiService.cs](/home/pato/projects/work/Foodtrack_erp/src/FoodTrack.Mobile/Services/ApiService.cs). Na tomto Linux hoste bez MAUI workloadov som to udržal buildovateľné ako MAUI-structured shell s XAML, viewmodelmi a DI.

Pridal som aj [Dockerfile](/home/pato/projects/work/Foodtrack_erp/src/FoodTrack.API/Dockerfile), [docker-compose.yml](/home/pato/projects/work/Foodtrack_erp/docker-compose.yml) a kompletne prepísal [README.md](/home/pato/projects/work/Foodtrack_erp/README.md) vrátane plného Mermaid diagramu a setup inštrukcií. `.plan/TODO.md` je vyčistené, backlog je prázdny a `.plan/WORK_LOCK` je preč.

Verifikácia:
- `export PATH="$HOME/.dotnet:$PATH" && dotnet build FoodTrack.sln` prešlo, `0` warningov, `0` errorov.
- `export PATH="$HOME/.dotnet:$PATH" && dotnet test FoodTrack.sln --filter "FullyQualifiedName!~FoodTrack.Mobile"` prešlo.
- Test count po filtre je `31`:
  - `FoodTrack.Domain.Tests`: `5`
  - `FoodTrack.Application.Tests`: `9`
  - `FoodTrack.API.Tests`: `11`
  - `FoodTrack.Presentation.Tests`: `6`
- `docker compose config` prešlo.


Git status:
Not a git repository.

Git diff --stat:
(no diff stat)

Git diff --name-only:
(no changed files)

Required action:
1. Optionally update `.plan/JUDGE_TODO.md` with tiny scratchpad notes.
2. Write EXACT JSON to `/home/pato/projects/work/Foodtrack_erp/.plan/codex-runner/interactive/watch-1776014147765962451/decision.json`.
3. The JSON must be a single object with keys:
   - `decision`: `continue` | `complete` | `blocked`
   - `summary`: short string
   - `reasons`: string array
   - `instructions_for_worker`: short direct instruction
   - `missing_checks`: string array

Use a shell command to write the JSON file. Do not ask the human anything.
