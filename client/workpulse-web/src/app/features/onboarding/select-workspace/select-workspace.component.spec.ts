import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router, provideRouter } from '@angular/router';
import { of, throwError } from 'rxjs';
import { SelectWorkspaceComponent } from './select-workspace.component';
import { UsersService } from '../../../core/services/users.service';
import { TenantService } from '../../../core/services/tenant.service';
import { Company, UserCompany } from '../../../core/models';

describe('SelectWorkspaceComponent', () => {
  let fixture: ComponentFixture<SelectWorkspaceComponent>;
  let users: jasmine.SpyObj<UsersService>;
  let tenant: TenantService;
  let router: Router;

  const acme: UserCompany = {
    id: 'c1',
    name: 'Acme Inc',
    slug: 'acme-inc',
    logoUrl: null,
    role: 'Owner'
  };

  const acmeCompany: Company = {
    id: 'c1',
    name: 'Acme Inc',
    slug: 'acme-inc',
    logoUrl: null,
    description: null,
    isActive: true,
    createdAtUtc: new Date().toISOString()
  };

  beforeEach(async () => {
    localStorage.clear();
    users = jasmine.createSpyObj('UsersService', ['getMyCompanies', 'setCurrentCompany']);

    await TestBed.configureTestingModule({
      imports: [SelectWorkspaceComponent],
      providers: [provideRouter([]), TenantService, { provide: UsersService, useValue: users }]
    }).compileComponents();

    tenant = TestBed.inject(TenantService);
    router = TestBed.inject(Router);
    spyOn(router, 'navigate');
  });

  afterEach(() => localStorage.clear());

  function create(): void {
    fixture = TestBed.createComponent(SelectWorkspaceComponent);
    fixture.detectChanges();
  }

  it('lists the companies the user belongs to', () => {
    users.getMyCompanies.and.returnValue(of([acme]));
    create();

    expect(fixture.componentInstance.companies()).toEqual([acme]);
    const cards = fixture.nativeElement.querySelectorAll('.workspace-card');
    expect(cards.length).toBe(1);
    expect(cards[0].textContent).toContain('Acme Inc');
  });

  it('redirects to create-workspace when the user has no companies', () => {
    users.getMyCompanies.and.returnValue(of([]));
    create();

    expect(router.navigate).toHaveBeenCalledWith(['/onboarding/workspace']);
  });

  it('redirects to dashboard when stored tenant matches a company', () => {
    tenant.setTenant('c1', 'Acme Inc');
    users.getMyCompanies.and.returnValue(of([acme]));
    create();

    expect(users.getMyCompanies).toHaveBeenCalled();
    expect(router.navigate).toHaveBeenCalledWith(['/dashboard']);
  });

  it('clears stale tenant and shows picker when stored tenant is not in companies', () => {
    tenant.setTenant('stale-id', 'Old Workspace');
    users.getMyCompanies.and.returnValue(of([acme]));
    create();

    expect(tenant.hasTenant()).toBeFalse();
    expect(fixture.componentInstance.companies()).toEqual([acme]);
    expect(router.navigate).not.toHaveBeenCalled();
  });

  it('sets the tenant and navigates to dashboard on selection', () => {
    users.getMyCompanies.and.returnValue(of([acme]));
    users.setCurrentCompany.and.returnValue(of(acmeCompany));
    create();

    fixture.componentInstance.select(acme);

    expect(users.setCurrentCompany).toHaveBeenCalledWith('c1');
    expect(tenant.tenantId()).toBe('c1');
    expect(tenant.tenantName()).toBe('Acme Inc');
    expect(router.navigate).toHaveBeenCalledWith(['/dashboard']);
  });

  it('shows an error when switching fails', () => {
    users.getMyCompanies.and.returnValue(of([acme]));
    users.setCurrentCompany.and.returnValue(
      throwError(() => ({ error: { description: 'Not a member' } }))
    );
    create();

    fixture.componentInstance.select(acme);

    expect(fixture.componentInstance.error()).toBe('Not a member');
    expect(fixture.componentInstance.switchingId()).toBeNull();
  });
});
