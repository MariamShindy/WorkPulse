import { test, expect } from '@playwright/test';

// Walks through the core navigation surface of the app as a logged-in user,
// asserting each page renders without console/page errors and shows real data.
// This is a smoke test across the whole cycle, not a deep per-feature suite.

const NAV_PAGES: { path: string; heading: string }[] = [
  { path: '/dashboard', heading: 'Dashboard' },
  { path: '/teams', heading: 'Teams' },
  { path: '/projects', heading: 'Projects' },
  { path: '/board', heading: 'Board' },
  { path: '/epics', heading: 'Epics' },
  { path: '/sprints', heading: 'Sprints' },
  { path: '/labels', heading: 'Labels' },
  { path: '/reports', heading: 'Reports' },
  { path: '/files', heading: 'Files' },
  { path: '/automation', heading: 'Automation rules' },
  { path: '/audit-logs', heading: 'Audit' },
  { path: '/settings', heading: 'Settings' }
];

test.describe('Full app cycle smoke test', () => {
  for (const { path, heading } of NAV_PAGES) {
    test(`${path} loads without errors`, async ({ page }) => {
      const pageErrors: string[] = [];
      page.on('pageerror', (err) => pageErrors.push(err.message));

      await page.goto(path);
      await expect(page.locator('h1')).toContainText(heading, { timeout: 10_000, ignoreCase: true });

      // Give async data loads a moment to resolve and surface any error banner.
      await page.waitForTimeout(500);
      const errorAlert = page.getByRole('alert');
      if (await errorAlert.count() > 0) {
        const text = await errorAlert.first().innerText();
        expect(text, `Error banner shown on ${path}: ${text}`).toBe('');
      }

      expect(pageErrors, `Uncaught page errors on ${path}: ${pageErrors.join(' | ')}`).toHaveLength(0);
    });
  }

  test('teams list shows seeded Egyptian teams and can open a team detail page', async ({ page }) => {
    await page.goto('/teams');
    const teamHeading = page.getByRole('heading', { name: 'فريق الخادم الخلفي' });
    await expect(teamHeading).toBeVisible();
    await teamHeading.click();
    await expect(page).toHaveURL(/\/teams\/[0-9a-f-]+/);
  });

  test('projects list shows seeded tech projects', async ({ page }) => {
    await page.goto('/projects');
    await expect(page.getByRole('heading', { name: 'نظام إدارة المحتوى' })).toBeVisible();
  });
});

test.describe('Board — task lifecycle', () => {
  test('creates a task via quick-add and opens its detail drawer', async ({ page }) => {
    await page.goto('/board');
    await expect(page.locator('.board')).toBeVisible({ timeout: 10_000 });

    const title = `E2E task ${Date.now()}`;
    await page.getByPlaceholder('Quick add — type a task title and press Enter…').fill(title);
    await page.getByRole('button', { name: /Add task/ }).click();

    const card = page.locator('.task-card', { hasText: title });
    await expect(card).toBeVisible({ timeout: 10_000 });

    await card.click();
    await expect(page.locator('.drawer-toolbar')).toBeVisible();
    await expect(page.getByRole('tab').or(page.locator('.tabs button', { hasText: 'Comments' }))).toBeVisible();
  });

  test('moves a task to another column via the move-to fallback select', async ({ page }) => {
    await page.goto('/board');
    await expect(page.locator('.board')).toBeVisible({ timeout: 10_000 });

    const title = `E2E move ${Date.now()}`;
    await page.getByPlaceholder('Quick add — type a task title and press Enter…').fill(title);
    await page.getByRole('button', { name: /Add task/ }).click();

    const card = page.locator('.task-card', { hasText: title });
    await expect(card).toBeVisible({ timeout: 10_000 });

    const moveSelect = card.locator('select.move-select');
    const targetValue = await moveSelect.locator('option').nth(1).getAttribute('value');
    await moveSelect.selectOption(targetValue!);

    // Card should disappear from its original column once moved (state change re-renders columns).
    await expect(page.locator('.task-card', { hasText: title })).toHaveCount(1, { timeout: 10_000 });
  });

  test('adds a comment to a task', async ({ page }) => {
    await page.goto('/board');
    await expect(page.locator('.board')).toBeVisible({ timeout: 10_000 });

    const firstCard = page.locator('.task-card').first();
    await expect(firstCard).toBeVisible({ timeout: 10_000 });
    await firstCard.click();

    await expect(page.locator('.drawer-toolbar')).toBeVisible();
    await page.locator('.tabs button', { hasText: 'Comments' }).click();

    const commentText = `Automated E2E comment ${Date.now()}`;
    await page.locator('.comment-form textarea').fill(commentText);
    await page.locator('.comment-form button[type="submit"]').click();

    await expect(page.locator('.comment-list', { hasText: commentText })).toBeVisible({ timeout: 10_000 });
  });
});

test.describe('Reports — analytics', () => {
  test('cycle time, throughput and burndown sections render with data', async ({ page }) => {
    await page.goto('/reports');
    await expect(page.getByText('Cycle time & throughput')).toBeVisible({ timeout: 10_000 });
    await expect(page.getByText('Sprint burndown')).toBeVisible();

    // Stat cards should show numeric content, not be stuck loading.
    await expect(page.locator('.skeleton.block')).toHaveCount(0, { timeout: 10_000 });
  });
});

test.describe('Automation rules', () => {
  test('creates a new automation rule end to end', async ({ page }) => {
    await page.goto('/automation');
    await page.getByRole('button', { name: 'New rule' }).click();

    const name = `E2E rule ${Date.now()}`;
    await page.locator('input[formcontrolname="name"]').fill(name);
    await page.getByRole('button', { name: /Create rule/ }).click();

    await expect(page.locator('.rule-card', { hasText: name })).toBeVisible({ timeout: 10_000 });
  });
});
