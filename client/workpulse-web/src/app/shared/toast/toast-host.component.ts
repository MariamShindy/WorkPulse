import { Component, inject } from '@angular/core';
import { ToastService } from '../../core/services/toast.service';

/**
 * Renders the toast stack. The container is an aria-live region so a screen reader announces
 * saves and failures — the app previously gave assistive tech no feedback at all on a mutation.
 * Errors use assertive; successes are polite so they do not interrupt.
 */
@Component({
  selector: 'app-toast-host',
  standalone: true,
  template: `
    <div class="toast-host">
      <!-- Two regions: a live region's politeness cannot vary per message. -->
      <div class="region" role="status" aria-live="polite" aria-atomic="false">
        @for (toast of politeToasts(); track toast.id) {
          <div class="toast" [class]="'toast-' + toast.kind">
            <span class="msg">{{ toast.message }}</span>
            <button type="button" class="close" (click)="toasts.dismiss(toast.id)" aria-label="Dismiss">
              <svg viewBox="0 0 24 24" width="14" height="14" aria-hidden="true" fill="none"
                   stroke="currentColor" stroke-width="2">
                <path d="M6 6l12 12M18 6L6 18" stroke-linecap="round" />
              </svg>
            </button>
          </div>
        }
      </div>

      <div class="region" role="alert" aria-live="assertive" aria-atomic="false">
        @for (toast of errorToasts(); track toast.id) {
          <div class="toast toast-error">
            <span class="msg">{{ toast.message }}</span>
            <button type="button" class="close" (click)="toasts.dismiss(toast.id)" aria-label="Dismiss">
              <svg viewBox="0 0 24 24" width="14" height="14" aria-hidden="true" fill="none"
                   stroke="currentColor" stroke-width="2">
                <path d="M6 6l12 12M18 6L6 18" stroke-linecap="round" />
              </svg>
            </button>
          </div>
        }
      </div>
    </div>
  `,
  styles: `
    .toast-host {
      position: fixed;
      z-index: 80;
      bottom: var(--sp-5);
      right: var(--sp-5);
      display: flex;
      flex-direction: column;
      gap: var(--sp-2);
      max-width: min(92vw, 24rem);
      pointer-events: none;
    }

    .region {
      display: flex;
      flex-direction: column;
      gap: var(--sp-2);
    }

    .toast {
      display: flex;
      align-items: flex-start;
      gap: var(--sp-3);
      padding: var(--sp-3) var(--sp-3) var(--sp-3) var(--sp-4);
      border-radius: var(--r-md);
      border: 1px solid var(--border-strong);
      background: var(--bg-elevated);
      color: var(--text);
      box-shadow: var(--shadow-md);
      font-size: var(--font-sm);
      pointer-events: auto;
      animation: toast-in var(--transition-spring) both;
      border-left-width: 3px;
    }

    .toast-success { border-left-color: var(--success); }
    .toast-error   { border-left-color: var(--danger); }
    .toast-info    { border-left-color: var(--info); }

    .msg {
      flex: 1;
      line-height: 1.45;
    }

    .close {
      flex-shrink: 0;
      display: inline-flex;
      align-items: center;
      justify-content: center;
      width: 1.375rem;
      height: 1.375rem;
      padding: 0;
      border: 0;
      border-radius: var(--r-xs);
      background: transparent;
      color: var(--text-subtle);
      cursor: pointer;
      transition: color var(--transition-fast), background var(--transition-fast);
    }

    .close:hover {
      color: var(--text);
      background: var(--bg-hover);
    }

    @keyframes toast-in {
      from { opacity: 0; transform: translateY(0.5rem) scale(0.98); }
      to   { opacity: 1; transform: translateY(0) scale(1); }
    }

    @media (prefers-reduced-motion: reduce) {
      .toast { animation: none; }
    }
  `
})
export class ToastHostComponent {
  readonly toasts = inject(ToastService);

  readonly politeToasts = () => this.toasts.toasts().filter((t) => t.kind !== 'error');
  readonly errorToasts = () => this.toasts.toasts().filter((t) => t.kind === 'error');
}
