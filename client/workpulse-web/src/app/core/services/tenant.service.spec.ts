import { TenantService } from './tenant.service';

describe('TenantService', () => {
  let service: TenantService;

  beforeEach(() => {
    localStorage.clear();
    service = new TenantService();
  });

  afterEach(() => localStorage.clear());

  it('has no tenant before one is set', () => {
    expect(service.hasTenant()).toBeFalse();
    expect(service.role()).toBeNull();
    expect(service.isAdmin()).toBeFalse();
  });

  it('stores the tenant id, name and role', () => {
    service.setTenant('t1', 'Acme', 'Admin');

    expect(service.tenantId()).toBe('t1');
    expect(service.tenantName()).toBe('Acme');
    expect(service.role()).toBe('Admin');
  });

  it('treats Admin and Owner as admin', () => {
    service.setTenant('t1', 'Acme', 'Admin');
    expect(service.isAdmin()).toBeTrue();

    service.setTenant('t1', 'Acme', 'Owner');
    expect(service.isAdmin()).toBeTrue();
  });

  it('treats Member and Guest as non-admin', () => {
    service.setTenant('t1', 'Acme', 'Member');
    expect(service.isAdmin()).toBeFalse();

    service.setTenant('t1', 'Acme', 'Guest');
    expect(service.isAdmin()).toBeFalse();
  });

  it('ignores an unrecognised role rather than granting admin', () => {
    service.setTenant('t1', 'Acme', 'SuperUser');

    expect(service.role()).toBeNull();
    expect(service.isAdmin()).toBeFalse();
  });

  it('keeps the cached role when a later call omits it', () => {
    service.setTenant('t1', 'Acme', 'Owner');
    // Renaming the workspace in Settings must not silently demote the user.
    service.setTenant('t1', 'Acme Renamed');

    expect(service.tenantName()).toBe('Acme Renamed');
    expect(service.isAdmin()).toBeTrue();
  });

  it('rehydrates the role from storage on construction', () => {
    service.setTenant('t1', 'Acme', 'Admin');

    const revived = new TenantService();

    expect(revived.role()).toBe('Admin');
    expect(revived.isAdmin()).toBeTrue();
  });

  it('clear removes the role along with the tenant', () => {
    service.setTenant('t1', 'Acme', 'Owner');

    service.clear();

    expect(service.hasTenant()).toBeFalse();
    expect(service.role()).toBeNull();
    expect(service.isAdmin()).toBeFalse();
    expect(localStorage.getItem('workpulse.tenantRole')).toBeNull();
  });
});
