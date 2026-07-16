import { Injectable, signal, computed } from '@angular/core';

const TENANT_KEY = 'workpulse.tenantId';
const TENANT_NAME_KEY = 'workpulse.tenantName';

@Injectable({ providedIn: 'root' })
export class TenantService {
  private readonly _tenantId = signal<string | null>(this.read(TENANT_KEY));
  private readonly _tenantName = signal<string | null>(this.read(TENANT_NAME_KEY));

  readonly tenantId = computed(() => this._tenantId());
  readonly tenantName = computed(() => this._tenantName());
  readonly hasTenant = computed(() => !!this._tenantId());

  setTenant(id: string, name?: string): void {
    localStorage.setItem(TENANT_KEY, id);
    this._tenantId.set(id);
    if (name) {
      localStorage.setItem(TENANT_NAME_KEY, name);
      this._tenantName.set(name);
    }
  }

  clear(): void {
    localStorage.removeItem(TENANT_KEY);
    localStorage.removeItem(TENANT_NAME_KEY);
    this._tenantId.set(null);
    this._tenantName.set(null);
  }

  private read(key: string): string | null {
    return localStorage.getItem(key);
  }
}
