import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { catchError, finalize, shareReplay, tap } from 'rxjs/operators';
import { Observable, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthResponse, UserCompany, UserProfile } from '../models';
import { isJwtExpired } from '../utils/jwt';
import { TenantService } from './tenant.service';

const ACCESS_TOKEN_KEY = 'workpulse.accessToken';
const USER_KEY = 'workpulse.user';

/** Left behind by pre-cookie builds; cleared on the next sign-out. */
const LEGACY_REFRESH_TOKEN_KEY = 'workpulse.refreshToken';

/**
 * Marks that the browser is holding a refresh cookie. The cookie itself is HttpOnly and
 * therefore invisible to this code, so this flag is the only way to know whether attempting a
 * silent refresh is worthwhile. It is a boolean, not a credential — leaking it grants nothing.
 */
const HAS_SESSION_KEY = 'workpulse.hasSession';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly tenant = inject(TenantService);

  private readonly _accessToken = signal<string | null>(localStorage.getItem(ACCESS_TOKEN_KEY));
  private readonly _hasSession = signal<boolean>(localStorage.getItem(HAS_SESSION_KEY) === '1');
  private readonly _user = signal<UserProfile | null>(this.readUser());

  /** In-flight refresh so concurrent 401s share one /auth/refresh call. */
  private refreshInFlight: Observable<AuthResponse> | null = null;

  readonly accessToken = computed(() => this._accessToken());
  readonly user = computed(() => this._user());

  /**
   * A stored access token is not enough — an expired one used to pass the guard and drop the
   * user on the dashboard, where every request then failed with 401. A session is still live
   * when the access token has lapsed but a refresh cookie remains, since it can be renewed.
   */
  readonly isAuthenticated = computed(
    () => !isJwtExpired(this._accessToken()) || this._hasSession()
  );

  /** True when the access token itself is spent, regardless of refresh availability. */
  readonly isAccessTokenExpired = computed(() => isJwtExpired(this._accessToken()));

  /** Whether a silent refresh is worth attempting. */
  readonly hasSession = computed(() => this._hasSession());

  readonly displayName = computed(() => {
    const u = this._user();
    return u ? `${u.firstName} ${u.lastName}`.trim() : '';
  });

  register(payload: {
    email: string;
    password: string;
    firstName: string;
    lastName: string;
  }): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${environment.apiUrl}/auth/register`, payload, { withCredentials: true })
      .pipe(tap((res) => this.persistSession(res)));
  }

  login(email: string, password: string): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(
        `${environment.apiUrl}/auth/login`,
        { email, password },
        { withCredentials: true }
      )
      .pipe(tap((res) => this.persistSession(res)));
  }

  /** Stores a session obtained outside the normal login flow (e.g. invite acceptance). */
  applyExternalSession(response: AuthResponse): void {
    this.persistSession(response);
  }

  logout(): void {
    // withCredentials so the API receives — and can revoke — the refresh cookie.
    this.http.post(`${environment.apiUrl}/auth/logout`, {}, { withCredentials: true }).subscribe({
      next: () => this.clearSession(),
      error: () => this.clearSession()
    });
  }

  /**
   * Exchanges the refresh cookie for a new access token. The token is never sent in the body —
   * the browser attaches the HttpOnly cookie automatically.
   * On failure the session is cleared and the user is sent to login.
   */
  refreshSession(): Observable<AuthResponse> {
    if (this.refreshInFlight) {
      return this.refreshInFlight;
    }

    if (!this._hasSession()) {
      this.clearSession();
      return throwError(() => new Error('No refresh session'));
    }

    this.refreshInFlight = this.http
      .post<AuthResponse>(`${environment.apiUrl}/auth/refresh`, {}, { withCredentials: true })
      .pipe(
        tap((res) => this.persistSession(res)),
        catchError((err) => {
          this.clearSession();
          return throwError(() => err);
        }),
        finalize(() => {
          this.refreshInFlight = null;
        }),
        shareReplay(1)
      );

    return this.refreshInFlight;
  }

  private persistSession(response: AuthResponse): void {
    localStorage.setItem(ACCESS_TOKEN_KEY, response.accessToken);
    localStorage.setItem(USER_KEY, JSON.stringify(response.user));
    localStorage.setItem(HAS_SESSION_KEY, '1');

    this._accessToken.set(response.accessToken);
    this._hasSession.set(true);
    this._user.set(response.user);

    if (response.user.currentTenantId) {
      this.tenant.setTenant(response.user.currentTenantId);
      this.resolveTenantName(response.user.currentTenantId);
    } else {
      this.tenant.clear();
    }
  }

  /**
   * Fetches the workspace name and the user's role in it. The role drives which admin-only
   * navigation the shell renders, so this runs even when the name is already cached.
   */
  private resolveTenantName(tenantId: string): void {
    if (this.tenant.tenantName() && this.tenant.role()) return;

    this.http.get<UserCompany[]>(`${environment.apiUrl}/users/me/companies`).subscribe({
      next: (companies) => {
        const match = companies.find((c) => c.id === tenantId);
        if (match) {
          this.tenant.setTenant(match.id, match.name, match.role);
        }
      },
      error: () => {
        // Cosmetic; the API still enforces access on every admin endpoint.
      }
    });
  }

  private clearSession(): void {
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    localStorage.removeItem(HAS_SESSION_KEY);
    localStorage.removeItem(LEGACY_REFRESH_TOKEN_KEY);

    this._accessToken.set(null);
    this._hasSession.set(false);
    this._user.set(null);
    this.tenant.clear();
    this.router.navigate(['/auth/login']);
  }

  private readUser(): UserProfile | null {
    const raw = localStorage.getItem(USER_KEY);
    if (!raw) return null;
    try {
      return JSON.parse(raw) as UserProfile;
    } catch {
      return null;
    }
  }
}
