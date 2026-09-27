import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { TenantService } from '../services/tenant.service';

/**
 * Keeps non-admin members out of admin-only routes reached by typing the URL directly.
 * Hiding the navigation is not enough on its own; the API's RequireCompanyAdmin policy is
 * still the real boundary, this just avoids rendering a page that can only 403.
 */
export const adminGuard: CanActivateFn = () => {
  const tenant = inject(TenantService);
  const router = inject(Router);

  if (tenant.isAdmin()) {
    return true;
  }

  return router.createUrlTree(['/dashboard']);
};
