import { Injectable, computed, effect, signal } from '@angular/core';

export type Theme = 'dark' | 'light';

const THEME_KEY = 'workpulse.theme';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly _theme = signal<Theme>(this.initialTheme());

  readonly theme = computed(() => this._theme());
  readonly isDark = computed(() => this._theme() === 'dark');

  constructor() {
    effect(() => {
      document.documentElement.setAttribute('data-theme', this._theme());
    });
  }

  toggle(): void {
    this.setTheme(this._theme() === 'dark' ? 'light' : 'dark');
  }

  setTheme(theme: Theme): void {
    localStorage.setItem(THEME_KEY, theme);
    this._theme.set(theme);
  }

  private initialTheme(): Theme {
    const stored = localStorage.getItem(THEME_KEY);
    if (stored === 'dark' || stored === 'light') {
      return stored;
    }
    return window.matchMedia?.('(prefers-color-scheme: light)').matches ? 'light' : 'dark';
  }
}
