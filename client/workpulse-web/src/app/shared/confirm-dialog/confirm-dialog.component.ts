import { Component, input, output } from '@angular/core';
import { ModalComponent } from '../modal/modal.component';

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [ModalComponent],
  template: `
    <app-modal [open]="open()" [title]="title()" (closed)="cancelled.emit()">
      <p class="message">{{ message() }}</p>
      <div class="actions">
        <button type="button" class="btn btn-ghost" (click)="cancelled.emit()">Cancel</button>
        <button type="button" class="btn btn-danger" (click)="confirmed.emit()">
          {{ confirmLabel() }}
        </button>
      </div>
    </app-modal>
  `,
  styles: `
    .message {
      margin: 0 0 1.25rem;
      color: var(--text-muted);
    }

    .actions {
      display: flex;
      justify-content: flex-end;
      gap: 0.6rem;
    }
  `
})
export class ConfirmDialogComponent {
  readonly open = input.required<boolean>();
  readonly title = input('Are you sure?');
  readonly message = input('This action cannot be undone.');
  readonly confirmLabel = input('Delete');
  readonly confirmed = output<void>();
  readonly cancelled = output<void>();
}
