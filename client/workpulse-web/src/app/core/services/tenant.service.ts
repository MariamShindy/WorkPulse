import { Injectable, signal, computed } from '@angular/core';

const TENANT_KEY = 'workpulse.tenantId';
const TENANT_NAME_KEY = 'workpulse.tenantName';
const TENANT_ROLE_KEY = 'workpulse.tenantRole';

/** Company roles, ordered least to most privileged. */
export type TenantRole = 'Guest' | 'Member' | 'Admin' | 'Owner';

@Injectable({ providedIn: 'root' })
export class TenantService {
  private readonly _tenantId = signal<string | null>(this.read(TENANT_KEY));
  private readonly _tenantName = signal<string | null>(this.read(TENANT_NAME_KEY));
  private readonly _role = signal<TenantRole | null>(this.readRole());

  readonly tenantId = computed(() => this._tenantId());
  readonly tenantName = computed(() => this._tenantName());
  readonly hasTenant = computed(() => !!this._tenantId());

  /** The current user's role in the active workspace, once resolved. */
  readonly role = computed(() => this._role());

  /**
   * Whether the user may see admin-only areas (Automation, Audit logs, Settings).
   * This only hides UI the API would reject anyway — the server-side
   * RequireCompanyAdmin policy remains the actual boundary.
   */
  readonly isAdmin = computed(() => {
    const role = this._role();
    return role === 'Admin' || role === 'Owner';
  });

  setTenant(id: string, name?: string, role?: TenantRole | string | null): void {
    localStorage.setItem(TENANT_KEY, id);
    this._tenantId.set(id);

    if (name) {
      localStorage.setItem(TENANT_NAME_KEY, name);
      this._tenantName.set(name);
    }

    if (role) {
      const normalized = this.normalizeRole(role);
      if (normalized) {
        localStorage.setItem(TENANT_ROLE_KEY, normalized);
        this._role.set(normalized);
      }
    }
  }

  clear(): void {
    localStorage.removeItem(TENANT_KEY);
    localStorage.removeItem(TENANT_NAME_KEY);
    localStorage.removeItem(TENANT_ROLE_KEY);
    this._tenantId.set(null);
    this._tenantName.set(null);
    this._role.set(null);
  }

  private read(key: string): string | null {
    return localStorage.getItem(key);
  }

  private readRole(): TenantRole | null {
    return this.normalizeRole(localStorage.getItem(TENANT_ROLE_KEY));
  }

  /** The API sends the role as a name; anything unrecognised is treated as unknown. */
  private normalizeRole(value: string | null): TenantRole | null {
    switch (value) {
      case 'Guest':
      case 'Member':
      case 'Admin':
      case 'Owner':
        return value;
      default:
        return null;
    }
  }
}
