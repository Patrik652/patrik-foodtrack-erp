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
Worker turn id: 019d8186-f660-76c3-a7f9-3ff0c011f6f5

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



Worker last assistant message:
Repo som prešiel a rozbehol som prvý reálny slice projektu. Vznikla solution `FoodTrack.sln` s vrstvami Domain/Application/Infrastructure/API/tests, základným doménovým modelom, FIFO rotáciou, expiration alertmi `30/14/7` dní, SQLite seed dátami a Swagger API. Kľúčové miesta sú [README.md](/home/pato/projects/work/Foodtrack_erp/README.md), [Product.cs](/home/pato/projects/work/Foodtrack_erp/src/FoodTrack.Domain/Entities/Product.cs), [BatchRotationPolicy.cs](/home/pato/projects/work/Foodtrack_erp/src/FoodTrack.Domain/Policies/BatchRotationPolicy.cs), [InventoryQueryService.cs](/home/pato/projects/work/Foodtrack_erp/src/FoodTrack.Application/Inventory/InventoryQueryService.cs) a [Program.cs](/home/pato/projects/work/Foodtrack_erp/src/FoodTrack.API/Program.cs).

**Verifikácia**
- `export PATH="$HOME/.dotnet:$PATH" && dotnet build FoodTrack.sln` prešlo bez warningov a errorov.
- `export PATH="$HOME/.dotnet:$PATH" && dotnet test FoodTrack.sln` prešlo, spolu `7/7` testov.
- Runtime smoke test API prešiel: `GET /api/products` aj `GET /api/products/{productId}/fifo-batches` vracali seeded dáta.

Aktívne položky v [.plan/TODO.md](/home/pato/projects/work/Foodtrack_erp/.plan/TODO.md) už nie sú, `.plan/WORK_LOCK` som odstránil a backlog som nechal pripravený na ďalší rez práce.


Git status:
Not a git repository.

Git diff --stat:
(no diff stat)

Git diff --name-only:
(no changed files)

Required action:
1. Optionally update `.plan/JUDGE_TODO.md` with tiny scratchpad notes.
2. Write EXACT JSON to `/home/pato/projects/work/Foodtrack_erp/.plan/codex-runner/interactive/watch-1775995679255640540/decision.json`.
3. The JSON must be a single object with keys:
   - `decision`: `continue` | `complete` | `blocked`
   - `summary`: short string
   - `reasons`: string array
   - `instructions_for_worker`: short direct instruction
   - `missing_checks`: string array

Use a shell command to write the JSON file. Do not ask the human anything.
