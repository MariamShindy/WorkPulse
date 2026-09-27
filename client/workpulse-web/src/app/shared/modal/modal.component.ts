import { CdkTrapFocus } from '@angular/cdk/a11y';
import {
  Component,
  DestroyRef,
  HostListener,
  effect,
  inject,
  input,
  output
} from '@angular/core';

@Component({
  selector: 'app-modal',
  standalone: true,
  imports: [CdkTrapFocus],
  template: `
    @if (open()) {
      <div class="modal-backdrop" (click)="closed.emit()" aria-hidden="true"></div>
      <div
        class="modal"
        role="dialog"
        aria-modal="true"
        [attr.aria-label]="title()"
        cdkTrapFocus
        [cdkTrapFocusAutoCapture]="true">
        <header class="modal-header">
          <h2>{{ title() }}</h2>
          <button type="button" class="btn btn-ghost btn-icon close-btn" (click)="closed.emit()" aria-label="Close">
            <svg viewBox="0 0 24 24" width="16" height="16" aria-hidden="true" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M6 6l12 12M18 6L6 18" stroke-linecap="round"/>
            </svg>
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
      animation: fade-in var(--transition-base) ease;
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
      border-radius: var(--r-xl);
      background: var(--bg-card);
      border: 1px solid var(--border);
      box-shadow: var(--shadow-lg);
      animation: modal-in var(--transition-spring) both;
    }

    .modal-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      gap: var(--sp-4);
      padding: var(--sp-4) var(--sp-5);
      border-bottom: 1px solid var(--border);

      h2 {
        margin: 0;
        font-size: var(--font-lg);
        font-weight: 650;
        letter-spacing: -0.02em;
      }
    }

    .close-btn {
      color: var(--text-muted);
      flex-shrink: 0;

      &:hover {
        color: var(--text);
      }
    }

    .modal-body {
      padding: var(--sp-5);
      overflow-y: auto;
    }

    @keyframes fade-in {
      from { opacity: 0; }
      to { opacity: 1; }
    }

    @keyframes modal-in {
      from { opacity: 0; transform: translate(-50%, -48%) scale(0.98); }
      to { opacity: 1; transform: translate(-50%, -50%) scale(1); }
    }
  `
})
export class ModalComponent {
  private readonly destroyRef = inject(DestroyRef);

  readonly open = input.required<boolean>();
  readonly title = input('');
  readonly closed = output<void>();

  /** Element focused before the modal opened, so focus can be handed back on close. */
  private previouslyFocused: HTMLElement | null = null;

  constructor() {
    effect(() => {
      const isOpen = this.open();
      document.body.style.overflow = isOpen ? 'hidden' : '';

      if (isOpen) {
        this.previouslyFocused = document.activeElement as HTMLElement | null;
      } else if (this.previouslyFocused?.isConnected) {
        // cdkTrapFocus captures focus on open; it is on us to restore it afterwards.
        this.previouslyFocused.focus();
        this.previouslyFocused = null;
      }
    });

    this.destroyRef.onDestroy(() => {
      document.body.style.overflow = '';
    });
  }

  @HostListener('document:keydown.escape')
  onEscape(): void {
    if (this.open()) {
      this.closed.emit();
    }
  }
}
