import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { signal } from '@angular/core';
import { ShellComponent } from './shell.component';
import { AuthService } from '../../core/services/auth.service';
import { TenantService } from '../../core/services/tenant.service';
import { RealtimeService } from '../../core/services/realtime.service';

describe('ShellComponent', () => {
  let fixture: ComponentFixture<ShellComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ShellComponent],
      providers: [
        provideRouter([]),
        provideHttpClient(),
        provideHttpClientTesting(),
        {
          provide: AuthService,
          useValue: {
            displayName: signal('Jane Doe'),
            user: signal({ email: 'jane@example.com' }),
            isAuthenticated: signal(true),
            accessToken: signal('token'),
            logout: jasmine.createSpy('logout')
          }
        },
        {
          provide: TenantService,
          useValue: {
            tenantName: signal('Acme'),
            tenantId: signal('t1'),
            hasTenant: signal(true),
            role: signal('Admin'),
            isAdmin: signal(true)
          }
        },
        {
          provide: RealtimeService,
          useValue: {
            connect: jasmine.createSpy('connect'),
            disconnect: jasmine.createSpy('disconnect')
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ShellComponent);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('toggles mobile navigation', () => {
    expect(fixture.componentInstance.navOpen()).toBeFalse();
    fixture.componentInstance.toggleNav();
    expect(fixture.componentInstance.navOpen()).toBeTrue();
  });

  it('connects realtime hub on init', () => {
    const realtime = TestBed.inject(RealtimeService);
    expect(realtime.connect).toHaveBeenCalled();
  });

  it('renders grouped navigation sections', () => {
    const el: HTMLElement = fixture.nativeElement;
    const sectionTitles = Array.from(el.querySelectorAll('.nav-section-title')).map((n) =>
      n.textContent?.trim()
    );
    expect(sectionTitles).toEqual(['Workspace', 'Planning', 'Insights', 'Admin', 'Account']);
  });

  it('offers a skip link to the main region', () => {
    const el: HTMLElement = fixture.nativeElement;
    const skip = el.querySelector<HTMLAnchorElement>('a.skip-link');

    expect(skip).not.toBeNull();
    expect(skip?.getAttribute('href')).toBe('#main-content');
    expect(el.querySelector('#main-content')).not.toBeNull();
  });
});

describe('ShellComponent navigation for a non-admin member', () => {
  let fixture: ComponentFixture<ShellComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ShellComponent],
      providers: [
        provideRouter([]),
        provideHttpClient(),
        provideHttpClientTesting(),
        {
          provide: AuthService,
          useValue: {
            displayName: () => 'Jane Doe',
            user: signal({ email: 'jane@example.com' }),
            logout: jasmine.createSpy('logout')
          }
        },
        {
          provide: TenantService,
          useValue: {
            tenantName: signal('Acme'),
            tenantId: signal('t1'),
            hasTenant: signal(true),
            role: signal('Member'),
            isAdmin: signal(false)
          }
        },
        {
          provide: RealtimeService,
          useValue: {
            connect: jasmine.createSpy('connect'),
            disconnect: jasmine.createSpy('disconnect')
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ShellComponent);
    fixture.detectChanges();
  });

  it('hides the Admin section', () => {
    const el: HTMLElement = fixture.nativeElement;
    const sectionTitles = Array.from(el.querySelectorAll('.nav-section-title')).map((n) =>
      n.textContent?.trim()
    );

    expect(sectionTitles).not.toContain('Admin');
  });

  it('does not link to admin-only routes', () => {
    const el: HTMLElement = fixture.nativeElement;
    const hrefs = Array.from(el.querySelectorAll('a[href]')).map((a) => a.getAttribute('href'));

    // These 403 on the API, so a member should never be offered them.
    expect(hrefs).not.toContain('/automation');
    expect(hrefs).not.toContain('/audit-logs');
  });

  it('still offers Settings, which holds the user own profile', () => {
    const el: HTMLElement = fixture.nativeElement;
    const hrefs = Array.from(el.querySelectorAll('a[href]')).map((a) => a.getAttribute('href'));

    expect(hrefs).toContain('/settings');
  });
});
