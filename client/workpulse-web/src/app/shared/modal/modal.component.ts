import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-modal',
  standalone: true,
  template: `
    @if (open()) {
      <div class="modal-backdrop" (click)="closed.emit()" aria-hidden="true"></div>
      <div class="modal" role="dialog" [attr.aria-label]="title()">
        <header class="modal-header">
          <h2>{{ title() }}</h2>
          <button type="button" class="close-btn" (click)="closed.emit()" aria-label="Close">
            &#10005;
          </button>
        </header>
        <div class="modal-body">
          <ng-content />
        </div>
      </div>
    }
  `,
  styles: `
    .modal-backdrop {
      position: fixed;
      inset: 0;
      background: var(--backdrop);
      z-index: 60;
    }

    .modal {
      position: fixed;
      z-index: 61;
      top: 50%;
      left: 50%;
      transform: translate(-50%, -50%);
      width: min(94vw, 560px);
      max-height: 88vh;
      display: flex;
      flex-direction: column;
      border-radius: 1rem;
      background: var(--bg-card);
      border: 1px solid var(--border);
      box-shadow: var(--shadow-lg);
    }

    .modal-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      gap: 1rem;
      padding: 1rem 1.25rem;
      border-bottom: 1px solid var(--border);

      h2 {
        margin: 0;
        font-size: 1.1rem;
      }
    }

    .close-btn {
      border: none;
      background: transparent;
      color: var(--text-muted);
      font-size: 1rem;
      cursor: pointer;
      padding: 0.25rem;

      &:hover {
        color: var(--text);
      }
    }

    .modal-body {
      padding: 1.25rem;
      overflow-y: auto;
    }
  `
})
export class ModalComponent {
  readonly open = input.required<boolean>();
  readonly title = input('');
  readonly closed = output<void>();
}
