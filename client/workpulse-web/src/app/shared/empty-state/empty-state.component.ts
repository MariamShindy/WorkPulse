import { Component, input } from '@angular/core';

@Component({
  selector: 'app-empty-state',
  standalone: true,
  template: `
    <div class="empty-state">
      <div class="glyph-wrap" aria-hidden="true">
        <span class="glyph">{{ icon() }}</span>
      </div>
      <h3>{{ heading() }}</h3>
      @if (message()) {
        <p>{{ message() }}</p>
      }
      <div class="actions">
        <ng-content />
      </div>
    </div>
  `,
  styles: `
    .empty-state {
      display: grid;
      place-items: center;
      gap: var(--sp-2);
      padding: var(--sp-8) var(--sp-4);
      text-align: center;
    }

    .glyph-wrap {
      display: grid;
      place-items: center;
      width: 3.25rem;
      height: 3.25rem;
      margin-bottom: var(--sp-2);
      border-radius: var(--r-lg);
      background: var(--accent-soft);
      border: 1px solid color-mix(in srgb, var(--accent) 22%, transparent);
      color: var(--accent);
    }

    .glyph {
      font-size: 1.25rem;
      line-height: 1;
      opacity: 0.95;
    }

    h3 {
      margin: 0;
      font-size: var(--font-md);
      font-weight: 650;
      letter-spacing: -0.015em;
    }

    p {
      margin: 0;
      color: var(--text-muted);
      max-width: 28rem;
      font-size: var(--font-sm);
      line-height: 1.5;
    }

    .actions {
      display: flex;
      flex-wrap: wrap;
      justify-content: center;
      gap: var(--sp-2);
      margin-top: var(--sp-3);
    }

    .actions:empty {
      display: none;
    }
  `
})
export class EmptyStateComponent {
  readonly icon = input('◇');
  readonly heading = input('Nothing here yet');
  readonly message = input('');
}
