import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { TenantService } from '../services/tenant.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const tenant = inject(TenantService);

  let headers = req.headers;
  const token = auth.accessToken();
  if (token) {
    headers = headers.set('Authorization', `Bearer ${token}`);
  }

  const tenantId = tenant.tenantId();
  if (tenantId) {
    headers = headers.set('X-Tenant-Id', tenantId);
  }

  return next(req.clone({ headers }));
};
