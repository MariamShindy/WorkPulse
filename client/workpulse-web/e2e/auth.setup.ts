import { test as setup, expect } from '@playwright/test';

const DEMO_EMAIL = 'demo@workpulse.local';
const DEMO_PASSWORD = 'Demo1234!';
const AUTH_FILE = 'playwright/.auth/user.json';

setup('authenticate once and reuse the session for the main suite', async ({ page }) => {
  await page.goto('/auth/login');
  await page.getByPlaceholder('you@company.com').fill(DEMO_EMAIL);
  await page.getByPlaceholder('••••••••').fill(DEMO_PASSWORD);
  await page.getByRole('button', { name: 'Sign in' }).click();
  await expect(page).toHaveURL(/\/dashboard/, { timeout: 15_000 });
  await page.context().storageState({ path: AUTH_FILE });
});
