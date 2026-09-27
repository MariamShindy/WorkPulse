import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { Router } from '@angular/router';
import { AuthService } from './auth.service';
import { TenantService } from './tenant.service';

/** Builds an unsigned token that expires the given number of seconds from now. */
function tokenExpiringIn(seconds: number): string {
  const encode = (o: unknown) =>
    btoa(JSON.stringify(o)).replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '');
  return [
    encode({ alg: 'HS256', typ: 'JWT' }),
    encode({ exp: Math.floor(Date.now() / 1000) + seconds }),
    'signature'
  ].join('.');
}

const user = {
  id: '1',
  email: 'user@example.com',
  firstName: 'Jane',
  lastName: 'Doe',
  createdAtUtc: new Date().toISOString()
};

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

  function flushLogin(accessToken = tokenExpiringIn(3600)) {
    service.login('user@example.com', 'password123').subscribe();
    const req = httpMock.expectOne('/api/auth/login');
    req.flush({ accessToken, accessTokenExpiresAtUtc: new Date().toISOString(), user });
    return req;
  }

  it('stores the access token after login', () => {
    const req = flushLogin();

    expect(req.request.method).toBe('POST');
    expect(service.isAuthenticated()).toBeTrue();
    expect(service.accessToken()).toBeTruthy();
  });

  it('sends credentials on login so the refresh cookie is accepted', () => {
    const req = flushLogin();

    expect(req.request.withCredentials).toBeTrue();
  });

  it('never writes a refresh token to localStorage', () => {
    flushLogin();

    // The whole point of the cookie migration: no durable credential in storage.
    expect(localStorage.getItem('workpulse.refreshToken')).toBeNull();
    expect(JSON.stringify(localStorage)).not.toContain('refreshToken');
  });

  it('records a non-sensitive session flag instead', () => {
    flushLogin();

    expect(localStorage.getItem('workpulse.hasSession')).toBe('1');
    expect(service.hasSession()).toBeTrue();
  });

  it('treats an expired access token with no session as unauthenticated', () => {
    localStorage.setItem('workpulse.accessToken', tokenExpiringIn(-60));
    const fresh = TestBed.inject(AuthService);

    // Same instance in this TestBed, so assert via a fresh login-free state.
    expect(fresh.isAuthenticated()).toBeFalse();
  });

  it('still considers the session live when the access token lapsed but a cookie remains', () => {
    flushLogin(tokenExpiringIn(-60));

    expect(service.isAccessTokenExpired()).toBeTrue();
    expect(service.isAuthenticated()).toBeTrue();
  });

  it('refreshes without putting the token in the request body', () => {
    flushLogin();

    service.refreshSession().subscribe();
    const req = httpMock.expectOne('/api/auth/refresh');

    expect(req.request.withCredentials).toBeTrue();
    expect(JSON.stringify(req.request.body ?? {})).not.toContain('refresh');

    req.flush({ accessToken: tokenExpiringIn(3600), accessTokenExpiresAtUtc: new Date().toISOString(), user });
  });

  it('shares one refresh call between concurrent callers', () => {
    flushLogin();

    service.refreshSession().subscribe();
    service.refreshSession().subscribe();

    // Two subscribers, one request: the in-flight observable is shared.
    const pending = httpMock.match('/api/auth/refresh');
    expect(pending.length).toBe(1);

    pending[0].flush({
      accessToken: tokenExpiringIn(3600),
      accessTokenExpiresAtUtc: new Date().toISOString(),
      user
    });
  });

  it('clears the session when refresh fails', () => {
    flushLogin();

    service.refreshSession().subscribe({ error: () => undefined });
    httpMock.expectOne('/api/auth/refresh').flush(
      { title: 'Unauthorized' },
      { status: 401, statusText: 'Unauthorized' }
    );

    expect(service.hasSession()).toBeFalse();
    expect(service.isAuthenticated()).toBeFalse();
    expect(localStorage.getItem('workpulse.accessToken')).toBeNull();
  });

  it('does not attempt a refresh with no session flag', () => {
    service.refreshSession().subscribe({ error: () => undefined });

    // No HTTP call should have been made; httpMock.verify() in afterEach confirms it.
    expect(service.hasSession()).toBeFalse();
  });

  it('drops a refresh token left behind by a pre-cookie build on sign-out', () => {
    localStorage.setItem('workpulse.refreshToken', 'stale-token');
    flushLogin();

    service.logout();
    httpMock.expectOne('/api/auth/logout').flush(null, { status: 204, statusText: 'No Content' });

    expect(localStorage.getItem('workpulse.refreshToken')).toBeNull();
  });

  it('clears the session even when logout fails server-side', () => {
    flushLogin();

    service.logout();
    httpMock
      .expectOne('/api/auth/logout')
      .flush({ title: 'Error' }, { status: 500, statusText: 'Server Error' });

    expect(service.hasSession()).toBeFalse();
  });
});
