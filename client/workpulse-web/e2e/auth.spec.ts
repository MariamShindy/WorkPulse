import { test, expect } from '@playwright/test';

const DEMO_EMAIL = 'demo@workpulse.local';
const DEMO_PASSWORD = 'Demo1234!';

test.describe('Authentication', () => {
  test('shows an error on invalid credentials', async ({ page }) => {
    await page.goto('/auth/login');
    await page.getByPlaceholder('you@company.com').fill('nobody@workpulse.local');
    await page.getByPlaceholder('••••••••').fill('WrongPassword1!');
    await page.getByRole('button', { name: 'Sign in' }).click();
    await expect(page.locator('.alert.alert-error')).toBeVisible({ timeout: 10_000 });
  });

  test('logs in with valid demo credentials and reaches the dashboard', async ({ page }) => {
    await page.goto('/auth/login');
    await page.getByPlaceholder('you@company.com').fill(DEMO_EMAIL);
    await page.getByPlaceholder('••••••••').fill(DEMO_PASSWORD);
    await page.getByRole('button', { name: 'Sign in' }).click();
    await expect(page).toHaveURL(/\/dashboard/, { timeout: 15_000 });
    await expect(page.locator('h1')).toContainText('Dashboard');
  });

  test('logs out back to the login screen', async ({ page }) => {
    await page.goto('/auth/login');
    await page.getByPlaceholder('you@company.com').fill(DEMO_EMAIL);
    await page.getByPlaceholder('••••••••').fill(DEMO_PASSWORD);
    await page.getByRole('button', { name: 'Sign in' }).click();
    await expect(page).toHaveURL(/\/dashboard/, { timeout: 15_000 });

    await page.getByRole('button', { name: 'Sign out' }).click();
    await expect(page).toHaveURL(/\/auth\/login/, { timeout: 10_000 });
  });
});
