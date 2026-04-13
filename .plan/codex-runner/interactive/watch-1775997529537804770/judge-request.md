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
Worker turn id: 019d81ad-beca-77b1-a804-13fd57339acf

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

Worker last assistant message:
Pridal som reálne HTTP-level integration testy nad API a SQLite hostom. Nový projekt je [FoodTrack.API.Tests.csproj](/home/pato/projects/work/Foodtrack_erp/tests/FoodTrack.API.Tests/FoodTrack.API.Tests.csproj), test host je v [FoodTrackApiFactory.cs](/home/pato/projects/work/Foodtrack_erp/tests/FoodTrack.API.Tests/Infrastructure/FoodTrackApiFactory.cs) a endpoint coverage je v [DashboardEndpointTests.cs](/home/pato/projects/work/Foodtrack_erp/tests/FoodTrack.API.Tests/Endpoints/DashboardEndpointTests.cs) a [InventoryWriteEndpointTests.cs](/home/pato/projects/work/Foodtrack_erp/tests/FoodTrack.API.Tests/Endpoints/InventoryWriteEndpointTests.cs). Pri tom sa ukázal reálny paralelný problém pri štarte dvoch API hostov nad SQLite bootstrapom; oprava je v [ApiTestCollection.cs](/home/pato/projects/work/Foodtrack_erp/tests/FoodTrack.API.Tests/Infrastructure/ApiTestCollection.cs), kde sú API integračné testy zámerne neparalelné.

README som doplnil o API integration coverage a `.plan` je znovu čistý: [README.md](/home/pato/projects/work/Foodtrack_erp/README.md), [.plan/TODO.md](/home/pato/projects/work/Foodtrack_erp/.plan/TODO.md). `.plan/WORK_LOCK` je odstránený.

Verifikácia:
- `export PATH="$HOME/.dotnet:$PATH" && dotnet build FoodTrack.sln` prešlo bez warningov a errorov.
- `export PATH="$HOME/.dotnet:$PATH" && dotnet test FoodTrack.sln` prešlo.
- Aktuálny stav testov po solution run:
  - `FoodTrack.Domain.Tests`: `5/5`
  - `FoodTrack.Application.Tests`: `6/6`
  - `FoodTrack.API.Tests`: `2/2`

Spolu je zelených `13/13` testov.


Git status:
Not a git repository.

Git diff --stat:
(no diff stat)

Git diff --name-only:
(no changed files)

Required action:
1. Optionally update `.plan/JUDGE_TODO.md` with tiny scratchpad notes.
2. Write EXACT JSON to `/home/pato/projects/work/Foodtrack_erp/.plan/codex-runner/interactive/watch-1775997529537804770/decision.json`.
3. The JSON must be a single object with keys:
   - `decision`: `continue` | `complete` | `blocked`
   - `summary`: short string
   - `reasons`: string array
   - `instructions_for_worker`: short direct instruction
   - `missing_checks`: string array

Use a shell command to write the JSON file. Do not ask the human anything.
