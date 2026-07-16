import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { Router } from '@angular/router';
import { AuthService } from './auth.service';
import { TenantService } from './tenant.service';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    localStorage.clear();

    TestBed.configureTestingModule({
      providers: [
        AuthService,
        TenantService,
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: Router, useValue: { navigate: jasmine.createSpy('navigate') } }
      ]
    });

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('stores tokens after login', () => {
    service.login('user@example.com', 'password123').subscribe();

    const req = httpMock.expectOne('/api/auth/login');
    expect(req.request.method).toBe('POST');
    req.flush({
      accessToken: 'token',
      refreshToken: 'refresh',
      accessTokenExpiresAtUtc: new Date().toISOString(),
      user: {
        id: '1',
        email: 'user@example.com',
        firstName: 'Jane',
        lastName: 'Doe',
        createdAtUtc: new Date().toISOString()
      }
    });

    expect(service.isAuthenticated()).toBeTrue();
    expect(service.accessToken()).toBe('token');
  });
});
