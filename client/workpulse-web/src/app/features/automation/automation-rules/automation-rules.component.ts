import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AutomationService } from '../../../core/services/automation.service';
import {
  AUTOMATION_ACTIONS,
  AUTOMATION_TRIGGERS,
  AutomationRule,
  PagedList
} from '../../../core/models';
import { ModalComponent } from '../../../shared/modal/modal.component';
import { ConfirmDialogComponent } from '../../../shared/confirm-dialog/confirm-dialog.component';
import { PaginatorComponent } from '../../../shared/paginator/paginator.component';
import { EmptyStateComponent } from '../../../shared/empty-state/empty-state.component';
import { apiErrorMessage } from '../../../core/utils/api-error';

function validJson(value: string): boolean {
  try {
    JSON.parse(value);
    return true;
  } catch {
    return false;
  }
}

@Component({
  selector: 'app-automation-rules',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ModalComponent,
    ConfirmDialogComponent,
    PaginatorComponent,
    EmptyStateComponent
  ],
  templateUrl: './automation-rules.component.html',
  styleUrl: './automation-rules.component.scss'
})
export class AutomationRulesComponent implements OnInit {
  private readonly automation = inject(AutomationService);
  private readonly fb = inject(FormBuilder);

  readonly triggers = AUTOMATION_TRIGGERS;
  readonly actions = AUTOMATION_ACTIONS;

  readonly paged = signal<PagedList<AutomationRule> | null>(null);
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);
  readonly modalOpen = signal(false);
  readonly editingRule = signal<AutomationRule | null>(null);
  readonly deleteTarget = signal<AutomationRule | null>(null);

  readonly form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    triggerType: ['TaskStatusChanged', Validators.required],
    triggerConfigJson: ['{}', Validators.required],
    actionType: ['SendNotification', Validators.required],
    actionConfigJson: ['{}', Validators.required],
    isEnabled: [true]
  });

  ngOnInit(): void {
    this.load();
  }

  load(page = 1): void {
    this.loading.set(true);
    this.automation.list(page, 25).subscribe({
      next: (res) => {
        this.paged.set(res);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(apiErrorMessage(err, 'Failed to load automation rules.'));
        this.loading.set(false);
      }
    });
  }

  openCreate(): void {
    this.editingRule.set(null);
    this.form.reset({
      name: '',
      triggerType: 'TaskStatusChanged',
      triggerConfigJson: '{}',
      actionType: 'SendNotification',
      actionConfigJson: '{}',
      isEnabled: true
    });
    this.modalOpen.set(true);
  }

  openEdit(rule: AutomationRule): void {
    this.editingRule.set(rule);
    this.form.reset({
      name: rule.name,
      triggerType: rule.triggerType,
      triggerConfigJson: rule.triggerConfigJson,
      actionType: rule.actionType,
      actionConfigJson: rule.actionConfigJson,
      isEnabled: rule.isEnabled
    });
    this.modalOpen.set(true);
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const value = this.form.getRawValue();
    if (!validJson(value.triggerConfigJson) || !validJson(value.actionConfigJson)) {
      this.error.set('Trigger and action config must be valid JSON.');
      return;
    }

    this.saving.set(true);
    this.error.set(null);
    const editing = this.editingRule();
    const request$ = editing
      ? this.automation.update(editing.id, value)
      : this.automation.create(value);

    request$.subscribe({
      next: () => {
        this.saving.set(false);
        this.modalOpen.set(false);
        this.load(this.paged()?.page ?? 1);
      },
      error: (err) => {
        this.error.set(apiErrorMessage(err, 'Failed to save rule.'));
        this.saving.set(false);
      }
    });
  }

  toggleEnabled(rule: AutomationRule): void {
    this.automation
      .update(rule.id, {
        name: rule.name,
        triggerType: rule.triggerType,
        triggerConfigJson: rule.triggerConfigJson,
        actionType: rule.actionType,
        actionConfigJson: rule.actionConfigJson,
        isEnabled: !rule.isEnabled
      })
      .subscribe({
        next: (updated) =>
          this.paged.update((p) =>
            p ? { ...p, items: p.items.map((r) => (r.id === rule.id ? updated : r)) } : p),
        error: (err) => this.error.set(apiErrorMessage(err, 'Failed to toggle rule.'))
      });
  }

  deleteRule(): void {
    const rule = this.deleteTarget();
    if (!rule) return;
    this.deleteTarget.set(null);
    this.automation.delete(rule.id).subscribe({
      next: () => this.load(this.paged()?.page ?? 1),
      error: (err) => this.error.set(apiErrorMessage(err, 'Failed to delete rule.'))
    });
  }
}
