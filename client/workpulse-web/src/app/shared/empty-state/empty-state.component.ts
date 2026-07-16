import { Component, input } from '@angular/core';

@Component({
  selector: 'app-empty-state',
  standalone: true,
  template: `
    <div class="empty-state">
      <span class="glyph">{{ icon() }}</span>
      <h3>{{ heading() }}</h3>
      <p>{{ message() }}</p>
      <ng-content />
    </div>
  `,
  styles: `
    .empty-state {
      display: grid;
      place-items: center;
      gap: 0.35rem;
      padding: 2.5rem 1rem;
      text-align: center;

      h3 {
        margin: 0;
      }

      p {
        margin: 0;
        color: var(--text-muted);
        max-width: 420px;
      }
    }

    .glyph {
      font-size: 1.75rem;
      opacity: 0.8;
    }
  `
})
export class EmptyStateComponent {
  readonly icon = input('◇');
  readonly heading = input('Nothing here yet');
  readonly message = input('');
}
