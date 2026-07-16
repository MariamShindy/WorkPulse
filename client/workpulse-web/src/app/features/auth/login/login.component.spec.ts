import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { LoginComponent } from './login.component';
import { AuthService } from '../../../core/services/auth.service';
import { of, throwError } from 'rxjs';

describe('LoginComponent', () => {
  let fixture: ComponentFixture<LoginComponent>;
  let auth: jasmine.SpyObj<AuthService>;

  beforeEach(async () => {
    auth = jasmine.createSpyObj('AuthService', ['login']);

    await TestBed.configureTestingModule({
      imports: [LoginComponent],
      providers: [
        provideRouter([]),
        { provide: AuthService, useValue: auth }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('shows error when login fails', () => {
    auth.login.and.returnValue(throwError(() => ({ error: { description: 'Invalid credentials' } })));

    fixture.componentInstance.form.setValue({
      email: 'user@example.com',
      password: 'wrong'
    });
    fixture.componentInstance.submit();

    expect(fixture.componentInstance.error()).toBe('Invalid credentials');
  });

  it('calls auth service when form is valid', () => {
    auth.login.and.returnValue(of({
      accessToken: 'a',
      refreshToken: 'r',
      accessTokenExpiresAtUtc: new Date().toISOString(),
      user: {
        id: '1',
        email: 'user@example.com',
        firstName: 'Jane',
        lastName: 'Doe',
        createdAtUtc: new Date().toISOString()
      }
    }));

    fixture.componentInstance.form.setValue({
      email: 'user@example.com',
      password: 'password123'
    });
    fixture.componentInstance.submit();

    expect(auth.login).toHaveBeenCalledWith('user@example.com', 'password123');
  });
});
