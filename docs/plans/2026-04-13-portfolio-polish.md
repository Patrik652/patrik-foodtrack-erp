# FoodTrack Portfolio Polish Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Add GitHub portfolio presentation assets around the existing FoodTrack ERP solution.

**Architecture:** Keep the production code unchanged and add presentation-layer artifacts around it: a GitHub Actions workflow for restore/build/test and Docker validation, repository-owned API demo assets, and recruiter-facing README sections that point to those assets. Postman collateral should mirror the real API flow and preserve the JWT-driven warehouse scenario already implemented in the backend.

**Tech Stack:** GitHub Actions, Markdown, Postman Collection v2.1 JSON, Swagger UI screenshot assets, ffmpeg/Pillow for lightweight presentation media.

### Task 1: CI Workflow

**Files:**
- Create: `.github/workflows/ci.yml`

**Step 1: Write the workflow file**

Create a workflow that runs on `push`, `pull_request`, and `workflow_dispatch`, installs `.NET 8`, restores `FoodTrack.sln`, builds it, runs filtered tests, validates `docker compose -f docker-compose.yml config`, builds the API container, and uploads TRX results.

**Step 2: Verify the workflow content locally**

Run: `sed -n '1,240p' .github/workflows/ci.yml`
Expected: YAML contains restore/build/test/docker steps and artifact upload.

### Task 2: Presentation Assets

**Files:**
- Create: `docs/postman/FoodTrackERP.postman_collection.json`
- Create: `docs/postman/FoodTrackERP.local.postman_environment.json`
- Create: `docs/presentation/github-demo-script.md`
- Create: `docs/assets/swagger-ui.png`
- Create: `docs/assets/foodtrack-demo.mp4`

**Step 1: Add Postman artifacts**

Encode login, products, dashboard, receive, dispatch, recall, and batch-filtered stock movement requests with collection variables for `baseUrl`, `jwtToken`, `productId`, and `batchId`.

**Step 2: Capture the Swagger screenshot**

Run the local API, open `/swagger`, and capture a repository-owned screenshot into `docs/assets/swagger-ui.png`.

**Step 3: Generate a lightweight demo video**

Create a short MP4 slideshow that can be attached to the GitHub repository or release as a presentation asset.

### Task 3: README Integration

**Files:**
- Modify: `README.md`

**Step 1: Add portfolio-facing sections**

Add a CI badge, Swagger preview image, Postman collection references, and a GitHub demo video section that points to the repository assets.

**Step 2: Verify docs references**

Run: `rg -n "Swagger Preview|Postman Collection|Demo Video|actions/workflows/ci.yml" README.md`
Expected: all new sections and the workflow badge appear.

### Task 4: Verification and Commit

**Files:**
- Modify: `README.md`
- Create: `.github/workflows/ci.yml`
- Create: `docs/postman/FoodTrackERP.postman_collection.json`
- Create: `docs/postman/FoodTrackERP.local.postman_environment.json`
- Create: `docs/presentation/github-demo-script.md`
- Create: `docs/assets/swagger-ui.png`
- Create: `docs/assets/foodtrack-demo.mp4`

**Step 1: Run verification**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet build FoodTrack.sln && dotnet test FoodTrack.sln --filter "FullyQualifiedName!~FoodTrack.Mobile" && docker compose -f docker-compose.yml config`
Expected: exit code `0`.

**Step 2: Commit and push**

Run:

```bash
git add .github/workflows/ci.yml README.md docs/postman docs/presentation docs/assets
git commit -m "chore: add portfolio CI and presentation assets"
git push origin HEAD
```

Expected: the repository contains the full GitHub-facing polish bundle.
