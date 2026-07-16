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
            hasTenant: signal(true)
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
    expect(sectionTitles).toEqual(['Workspace', 'Planning', 'Insights', 'Admin']);
  });
});
