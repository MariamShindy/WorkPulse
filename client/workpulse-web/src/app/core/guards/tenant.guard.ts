import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { TenantService } from '../services/tenant.service';

export const tenantGuard: CanActivateFn = () => {
  const tenant = inject(TenantService);
  const router = inject(Router);

  if (tenant.hasTenant()) {
    return true;
  }

  // A logged-in user without a resolved tenant may still belong to companies;
  // the select-workspace page resolves that (picker, or create-workspace when none).
  return router.createUrlTree(['/onboarding/select-workspace']);
};
