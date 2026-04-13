import { mkdir, cp } from "node:fs/promises";
import { promisify } from "node:util";
import { execFile } from "node:child_process";
import path from "node:path";
import process from "node:process";
import { chromium } from "@playwright/test";

const baseURL = process.env.DEMO_URL ?? "http://localhost:5099";
const rootDir = process.cwd();
const recordDir = path.join(rootDir, "tmp", "demo-video");
const rawOutputPath = path.join(recordDir, "foodtrack-demo-raw.webm");
const outputPath = path.join(rootDir, "docs", "assets", "foodtrack-demo.mp4");
const runExecFile = promisify(execFile);

async function transcodeToMp4(sourcePath, destinationPath) {
  await runExecFile("ffmpeg", [
    "-y",
    "-i", sourcePath,
    "-vf", "fps=24",
    "-c:v", "libx264",
    "-preset", "veryfast",
    "-crf", "26",
    "-an",
    destinationPath,
  ]);
}

async function run() {
  await mkdir(recordDir, { recursive: true });
  await mkdir(path.dirname(outputPath), { recursive: true });

  const browser = await chromium.launch({ headless: true });
  const context = await browser.newContext({
    viewport: { width: 1440, height: 900 },
    recordVideo: {
      dir: recordDir,
      size: { width: 1440, height: 900 },
    },
  });

  const page = await context.newPage();

  // --- Scene 1: Swagger UI overview ---
  await page.goto(`${baseURL}/swagger`, { waitUntil: "networkidle" });
  await page.waitForTimeout(2500);

  // --- Scene 2: Expand Auth endpoint and try it ---
  const authSection = page.locator("#operations-Auth-post_api_auth_login");
  await authSection.click();
  await page.waitForTimeout(1200);

  // Click "Try it out"
  const tryItBtn = authSection.locator("button.btn.try-out__btn");
  await tryItBtn.click();
  await page.waitForTimeout(800);

  // Fill in the request body
  const textarea = authSection.locator("textarea.body-param__text");
  await textarea.fill(JSON.stringify({ badgeCode: "OP-1001", pin: "1234" }, null, 2));
  await page.waitForTimeout(600);

  // Click Execute
  const executeBtn = authSection.locator("button.btn.execute.opblock-control__btn");
  await executeBtn.click();
  await page.waitForTimeout(2000);

  // Scroll to see response
  await authSection.locator(".responses-wrapper").scrollIntoViewIfNeeded();
  await page.waitForTimeout(1500);

  // --- Scene 3: Show Products endpoint ---
  // Collapse auth
  await authSection.locator(".opblock-summary").click();
  await page.waitForTimeout(500);

  // Scroll to Products section
  const productsGet = page.locator("#operations-Products-get_api_products");
  await productsGet.scrollIntoViewIfNeeded();
  await page.waitForTimeout(800);
  await productsGet.click();
  await page.waitForTimeout(1000);

  // Try it out
  const prodTryBtn = productsGet.locator("button.btn.try-out__btn");
  await prodTryBtn.click();
  await page.waitForTimeout(500);

  const prodExecBtn = productsGet.locator("button.btn.execute.opblock-control__btn");
  await prodExecBtn.click();
  await page.waitForTimeout(2000);

  // Scroll to response
  await productsGet.locator(".responses-wrapper").scrollIntoViewIfNeeded();
  await page.waitForTimeout(2000);

  // --- Scene 4: Show Dashboard ---
  await productsGet.locator(".opblock-summary").click();
  await page.waitForTimeout(500);

  const dashboardGet = page.locator("#operations-Dashboard-get_api_dashboard_expiration_overview");
  await dashboardGet.scrollIntoViewIfNeeded();
  await page.waitForTimeout(800);
  await dashboardGet.click();
  await page.waitForTimeout(1000);

  const dashTryBtn = dashboardGet.locator("button.btn.try-out__btn");
  await dashTryBtn.click();
  await page.waitForTimeout(500);

  const dashExecBtn = dashboardGet.locator("button.btn.execute.opblock-control__btn");
  await dashExecBtn.click();
  await page.waitForTimeout(2000);

  await dashboardGet.locator(".responses-wrapper").scrollIntoViewIfNeeded();
  await page.waitForTimeout(2000);

  // --- Scene 5: Show Stock Alerts ---
  await dashboardGet.locator(".opblock-summary").click();
  await page.waitForTimeout(500);

  const stockAlerts = page.locator("#operations-Dashboard-get_api_dashboard_stock_alerts");
  await stockAlerts.scrollIntoViewIfNeeded();
  await page.waitForTimeout(800);
  await stockAlerts.click();
  await page.waitForTimeout(1000);

  const alertsTryBtn = stockAlerts.locator("button.btn.try-out__btn");
  await alertsTryBtn.click();
  await page.waitForTimeout(500);

  const alertsExecBtn = stockAlerts.locator("button.btn.execute.opblock-control__btn");
  await alertsExecBtn.click();
  await page.waitForTimeout(2000);

  await stockAlerts.locator(".responses-wrapper").scrollIntoViewIfNeeded();
  await page.waitForTimeout(2000);

  // --- Scene 6: Show Inventory section ---
  await stockAlerts.locator(".opblock-summary").click();
  await page.waitForTimeout(500);

  const inventorySection = page.locator("#operations-Inventory-post_api_inventory_receive");
  await inventorySection.scrollIntoViewIfNeeded();
  await page.waitForTimeout(1500);

  // --- Scene 7: Scroll back to top for final overview ---
  await page.evaluate(() => window.scrollTo({ top: 0, behavior: "smooth" }));
  await page.waitForTimeout(2500);

  // Finalize video
  const video = page.video();
  if (!video) {
    throw new Error("Playwright did not attach video recording.");
  }

  await context.close();
  await browser.close();

  const sourceVideoPath = await video.path();
  await cp(sourceVideoPath, rawOutputPath, { force: true });
  await transcodeToMp4(rawOutputPath, outputPath);
  process.stdout.write(`Demo video created: ${outputPath}\n`);
}

run().catch((error) => {
  process.stderr.write(`${error instanceof Error ? error.message : String(error)}\n`);
  process.exit(1);
});
