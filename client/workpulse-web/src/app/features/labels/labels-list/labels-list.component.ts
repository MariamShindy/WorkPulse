import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { LabelsService } from '../../../core/services/labels.service';
import { Label } from '../../../core/models';
import { ConfirmDialogComponent } from '../../../shared/confirm-dialog/confirm-dialog.component';
import { EmptyStateComponent } from '../../../shared/empty-state/empty-state.component';
import { apiErrorMessage } from '../../../core/utils/api-error';

@Component({
  selector: 'app-labels-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ConfirmDialogComponent, EmptyStateComponent],
  templateUrl: './labels-list.component.html',
  styleUrl: './labels-list.component.scss'
})
export class LabelsListComponent implements OnInit {
  private readonly labelsService = inject(LabelsService);
  private readonly fb = inject(FormBuilder);

  readonly labels = signal<Label[]>([]);
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);
  readonly editingId = signal<string | null>(null);
  readonly deleteTarget = signal<Label | null>(null);

  readonly createForm = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(60)]],
    color: ['#6366f1', Validators.required]
  });

  readonly editForm = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(60)]],
    color: ['#6366f1', Validators.required]
  });

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.labelsService.list().subscribe({
      next: (res) => {
        this.labels.set(res.items);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(apiErrorMessage(err, 'Failed to load labels.'));
        this.loading.set(false);
      }
    });
  }

  create(): void {
    if (this.createForm.invalid) {
      this.createForm.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    const { name, color } = this.createForm.getRawValue();
    this.labelsService.create(name, color).subscribe({
      next: (label) => {
        this.labels.update((list) => [label, ...list]);
        this.createForm.reset({ name: '', color: '#6366f1' });
        this.saving.set(false);
      },
      error: (err) => {
        this.error.set(apiErrorMessage(err, 'Failed to create label.'));
        this.saving.set(false);
      }
    });
  }

  startEdit(label: Label): void {
    this.editingId.set(label.id);
    this.editForm.patchValue({ name: label.name, color: label.color });
  }

  saveEdit(): void {
    const id = this.editingId();
    if (!id || this.editForm.invalid) return;
    const { name, color } = this.editForm.getRawValue();
    this.labelsService.update(id, name, color).subscribe({
      next: (updated) => {
        this.labels.update((list) => list.map((l) => (l.id === id ? updated : l)));
        this.editingId.set(null);
      },
      error: (err) => this.error.set(apiErrorMessage(err, 'Failed to update label.'))
    });
  }

  deleteLabel(): void {
    const label = this.deleteTarget();
    if (!label) return;
    this.deleteTarget.set(null);
    this.labelsService.delete(label.id).subscribe({
      next: () => this.labels.update((list) => list.filter((l) => l.id !== label.id)),
      error: (err) => this.error.set(apiErrorMessage(err, 'Failed to delete label.'))
    });
  }
}
