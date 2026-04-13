# Delivery Hardening Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Verify and harden the final FoodTrack ERP delivery so Docker, MAUI configuration, git history, and required verification commands all work from the current repository state.

**Architecture:** Keep the existing Clean Architecture code intact and limit work to delivery infrastructure: container build/runtime, MAUI project metadata, solution configuration, repository hygiene, and final verification. Only change application code when a failing Docker or runtime check proves a real defect.

**Tech Stack:** ASP.NET Core 8, EF Core 8 with SQLite, .NET MAUI, Docker Compose, git, xUnit.

### Task 1: Baseline inspection

**Files:**
- Create: `docs/plans/2026-04-13-delivery-hardening.md`
- Modify: `.plan/TODO.md` if new work must be tracked
- Check: `src/FoodTrack.API/Dockerfile`, `docker-compose.yml`, `src/FoodTrack.Mobile/FoodTrack.Mobile.csproj`, `FoodTrack.sln`

**Step 1: Inspect the current delivery files**

Run: `sed -n '1,220p' src/FoodTrack.API/Dockerfile && sed -n '1,220p' docker-compose.yml`
Expected: Docker configuration is readable and points to the API project.

**Step 2: Inspect mobile and solution configuration**

Run: `sed -n '1,220p' src/FoodTrack.Mobile/FoodTrack.Mobile.csproj && rg -n "FoodTrack.Mobile" FoodTrack.sln`
Expected: MAUI target frameworks are visible and solution build behavior can be assessed.

### Task 2: Docker runtime verification

**Files:**
- Modify: `src/FoodTrack.API/Dockerfile`
- Modify: `docker-compose.yml`
- Modify: backend files only if runtime checks prove a defect

**Step 1: Build containers**

Run: `docker compose build`
Expected: PASS. If it fails, capture the exact error and fix the minimal root cause.

**Step 2: Start the API**

Run: `docker compose up -d`
Expected: PASS and the `foodtrack-api` container stays healthy enough to serve HTTP.

**Step 3: Verify Swagger**

Run: `curl -I http://127.0.0.1:8080/swagger/index.html`
Expected: `HTTP/1.1 200 OK`.

### Task 3: MAUI delivery verification

**Files:**
- Modify: `src/FoodTrack.Mobile/FoodTrack.Mobile.csproj`
- Modify: `FoodTrack.sln`
- Modify: `README.md` if verification behavior changes

**Step 1: Validate target frameworks**

Run: `sed -n '1,120p' src/FoodTrack.Mobile/FoodTrack.Mobile.csproj`
Expected: Android-first target frameworks with conditional iOS, MacCatalyst, and Windows targets.

**Step 2: Attempt project build when toolchain exists**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet build src/FoodTrack.Mobile/FoodTrack.Mobile.csproj`
Expected: PASS if MAUI workloads and Android toolchain are available; otherwise document and preserve solution exclusion behavior.

**Step 3: Verify solution does not auto-build unsupported mobile targets**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet build FoodTrack.sln`
Expected: PASS even on the current Linux host.

### Task 4: Repository hygiene and history

**Files:**
- Modify: `.gitignore`
- Create: `.git/` metadata via `git init`

**Step 1: Initialize git**

Run: `git init`
Expected: repository created in the current directory.

**Step 2: Commit the delivery state**

Run: `git add . && git commit -m "feat: initial FoodTrack ERP - ASP.NET Core 8 + .NET MAUI + Docker"`
Expected: PASS with a single initial commit.

### Task 5: Final verification

**Files:**
- Modify: `.plan/TODO.md` only if any work item status must change

**Step 1: Run the exact required verification**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet build && dotnet test && docker compose config`
Expected: PASS from repository root.

**Step 2: Re-check strict finish evidence**

Run: `sed -n '1,220p' .plan/TODO.md && test ! -e .plan/WORK_LOCK`
Expected: no backlog/in-progress/blocked items and no work lock file.
