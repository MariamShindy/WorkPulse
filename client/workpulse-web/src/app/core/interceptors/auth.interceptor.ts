import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { TenantService } from '../services/tenant.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const tenant = inject(TenantService);

  const withAuth = (request = req) => {
    let headers = request.headers;
    const token = auth.accessToken();
    if (token) {
      headers = headers.set('Authorization', `Bearer ${token}`);
    }

    const tenantId = tenant.tenantId();
    if (tenantId) {
      headers = headers.set('X-Tenant-Id', tenantId);
    }

    return request.clone({ headers });
  };

  // Avoid recursive refresh loops on auth endpoints.
  const isAuthEndpoint =
    req.url.includes('/auth/login') ||
    req.url.includes('/auth/register') ||
    req.url.includes('/auth/refresh') ||
    req.url.includes('/auth/logout');

  return next(withAuth()).pipe(
    catchError((err: unknown) => {
      if (!(err instanceof HttpErrorResponse) || err.status !== 401 || isAuthEndpoint) {
        return throwError(() => err);
      }

      if (!auth.hasSession()) {
        return throwError(() => err);
      }

      return auth.refreshSession().pipe(
        switchMap(() => next(withAuth())),
        catchError(() => throwError(() => err))
      );
    })
  );
};
