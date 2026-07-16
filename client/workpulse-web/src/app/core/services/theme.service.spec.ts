import { TestBed } from '@angular/core/testing';
import { ThemeService } from './theme.service';

describe('ThemeService', () => {
  beforeEach(() => {
    localStorage.clear();
    document.documentElement.removeAttribute('data-theme');
    TestBed.configureTestingModule({});
  });

  afterEach(() => {
    localStorage.clear();
    document.documentElement.removeAttribute('data-theme');
  });

  it('applies the theme as a data attribute on <html>', () => {
    const service = TestBed.inject(ThemeService);
    TestBed.flushEffects();
    expect(document.documentElement.getAttribute('data-theme')).toBe(service.theme());
  });

  it('toggles between dark and light and persists the choice', () => {
    localStorage.setItem('workpulse.theme', 'dark');
    const service = TestBed.inject(ThemeService);
    expect(service.theme()).toBe('dark');

    service.toggle();
    TestBed.flushEffects();

    expect(service.theme()).toBe('light');
    expect(localStorage.getItem('workpulse.theme')).toBe('light');
    expect(document.documentElement.getAttribute('data-theme')).toBe('light');
  });

  it('honors a stored light preference', () => {
    localStorage.setItem('workpulse.theme', 'light');
    const service = TestBed.inject(ThemeService);
    expect(service.isDark()).toBeFalse();
  });
});
