import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthResponse, UserCompany, UserProfile } from '../models';
import { TenantService } from './tenant.service';

const ACCESS_TOKEN_KEY = 'workpulse.accessToken';
const REFRESH_TOKEN_KEY = 'workpulse.refreshToken';
const USER_KEY = 'workpulse.user';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly tenant = inject(TenantService);

  private readonly _accessToken = signal<string | null>(localStorage.getItem(ACCESS_TOKEN_KEY));
  private readonly _refreshToken = signal<string | null>(localStorage.getItem(REFRESH_TOKEN_KEY));
  private readonly _user = signal<UserProfile | null>(this.readUser());

  readonly accessToken = computed(() => this._accessToken());
  readonly user = computed(() => this._user());
  readonly isAuthenticated = computed(() => !!this._accessToken());
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
      .post<AuthResponse>(`${environment.apiUrl}/auth/register`, payload)
      .pipe(tap((res) => this.persistSession(res)));
  }

  login(email: string, password: string): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${environment.apiUrl}/auth/login`, { email, password })
      .pipe(tap((res) => this.persistSession(res)));
  }

  /** Stores a session obtained outside the normal login flow (e.g. invite acceptance). */
  applyExternalSession(response: AuthResponse): void {
    this.persistSession(response);
  }

  logout(): void {
    const refresh = this._refreshToken();
    if (refresh) {
      this.http.post(`${environment.apiUrl}/auth/logout`, { refreshToken: refresh }).subscribe({
        complete: () => this.clearSession()
      });
      return;
    }
    this.clearSession();
  }

  private persistSession(response: AuthResponse): void {
    localStorage.setItem(ACCESS_TOKEN_KEY, response.accessToken);
    localStorage.setItem(REFRESH_TOKEN_KEY, response.refreshToken);
    localStorage.setItem(USER_KEY, JSON.stringify(response.user));
    this._accessToken.set(response.accessToken);
    this._refreshToken.set(response.refreshToken);
    this._user.set(response.user);

    if (response.user.currentTenantId) {
      this.tenant.setTenant(response.user.currentTenantId);
      this.resolveTenantName(response.user.currentTenantId);
    } else {
      this.tenant.clear();
    }
  }

  /** Fetches the workspace name so the shell can display it after a plain login. */
  private resolveTenantName(tenantId: string): void {
    if (this.tenant.tenantName()) return;

    this.http.get<UserCompany[]>(`${environment.apiUrl}/users/me/companies`).subscribe({
      next: (companies) => {
        const match = companies.find((c) => c.id === tenantId);
        if (match) {
          this.tenant.setTenant(match.id, match.name);
        }
      },
      error: () => {
        // Name resolution is cosmetic; ignore failures.
      }
    });
  }

  private clearSession(): void {
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    this._accessToken.set(null);
    this._refreshToken.set(null);
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
