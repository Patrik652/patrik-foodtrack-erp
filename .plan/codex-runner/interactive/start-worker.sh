#!/usr/bin/env bash
cd /home/pato/projects/work/Foodtrack_erp
exec codex-strejc-bin --dangerously-bypass-approvals-and-sandbox -s danger-full-access -C /home/pato/projects/work/Foodtrack_erp --no-alt-screen -c 'notify=["/home/pato/.local/share/pipx/venvs/codex-runner/bin/python", "/home/pato/projects/personal/codex-runner/codex_runner/notify_hook.py", "worker", "/home/pato/projects/work/Foodtrack_erp/.plan/codex-runner/interactive/worker-turn-events.jsonl"]' 'You are the interactive worker.

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

'
