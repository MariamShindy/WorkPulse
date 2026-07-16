import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-drawer',
  standalone: true,
  template: `
    @if (open()) {
      <div class="drawer-backdrop" (click)="closed.emit()" aria-hidden="true"></div>
      <aside class="drawer" role="dialog" [attr.aria-label]="title()">
        <header class="drawer-header">
          <h2>{{ title() }}</h2>
          <button type="button" class="close-btn" (click)="closed.emit()" aria-label="Close">
            &#10005;
          </button>
        </header>
        <div class="drawer-body">
          <ng-content />
        </div>
      </aside>
    }
  `,
  styles: `
    .drawer-backdrop {
      position: fixed;
      inset: 0;
      background: var(--backdrop);
      z-index: 60;
    }

    .drawer {
      position: fixed;
      z-index: 61;
      top: 0;
      right: 0;
      bottom: 0;
      width: min(100vw, 540px);
      display: flex;
      flex-direction: column;
      background: var(--bg-card);
      border-left: 1px solid var(--border);
      box-shadow: var(--shadow-lg);
      animation: slide-in 0.18s ease;
    }

    @keyframes slide-in {
      from {
        transform: translateX(24px);
        opacity: 0;
      }
      to {
        transform: translateX(0);
        opacity: 1;
      }
    }

    .drawer-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      gap: 1rem;
      padding: 1rem 1.25rem;
      border-bottom: 1px solid var(--border);

      h2 {
        margin: 0;
        font-size: 1.05rem;
        overflow: hidden;
        text-overflow: ellipsis;
        white-space: nowrap;
      }
    }

    .close-btn {
      border: none;
      background: transparent;
      color: var(--text-muted);
      font-size: 1rem;
      cursor: pointer;
      padding: 0.25rem;
      flex-shrink: 0;

      &:hover {
        color: var(--text);
      }
    }

    .drawer-body {
      padding: 1.25rem;
      overflow-y: auto;
      flex: 1;
    }
  `
})
export class DrawerComponent {
  readonly open = input.required<boolean>();
  readonly title = input('');
  readonly closed = output<void>();
}
