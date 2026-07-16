import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { EpicsService } from '../../../core/services/epics.service';
import { TeamsService } from '../../../core/services/teams.service';
import { EPIC_STATUSES, Epic, PagedList, Team } from '../../../core/models';
import { ModalComponent } from '../../../shared/modal/modal.component';
import { ConfirmDialogComponent } from '../../../shared/confirm-dialog/confirm-dialog.component';
import { PaginatorComponent } from '../../../shared/paginator/paginator.component';
import { EmptyStateComponent } from '../../../shared/empty-state/empty-state.component';

@Component({
  selector: 'app-epics-list',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ModalComponent,
    ConfirmDialogComponent,
    PaginatorComponent,
    EmptyStateComponent
  ],
  templateUrl: './epics-list.component.html',
  styleUrl: './epics-list.component.scss'
})
export class EpicsListComponent implements OnInit {
  private readonly epicsService = inject(EpicsService);
  private readonly teamsService = inject(TeamsService);
  private readonly fb = inject(FormBuilder);

  readonly statuses = EPIC_STATUSES;

  readonly paged = signal<PagedList<Epic> | null>(null);
  readonly teams = signal<Team[]>([]);
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);
  readonly modalOpen = signal(false);
  readonly editingEpic = signal<Epic | null>(null);
  readonly deleteTarget = signal<Epic | null>(null);
  readonly filterTeamId = signal('');
  readonly filterStatus = signal('');

  readonly form = this.fb.nonNullable.group({
    teamId: ['', Validators.required],
    title: ['', [Validators.required, Validators.maxLength(200)]],
    description: [''],
    status: ['Open', Validators.required]
  });

  ngOnInit(): void {
    this.teamsService.list().subscribe({ next: (res) => this.teams.set(res.items) });
    this.load();
  }

  load(page = 1): void {
    this.loading.set(true);
    this.epicsService
      .list({
        teamId: this.filterTeamId() || undefined,
        status: this.filterStatus() || undefined,
        page,
        pageSize: 24
      })
      .subscribe({
        next: (res) => {
          this.paged.set(res);
          this.loading.set(false);
        },
        error: (err) => {
          this.error.set(err.error?.description ?? 'Failed to load epics.');
          this.loading.set(false);
        }
      });
  }

  applyFilter(kind: 'team' | 'status', value: string): void {
    if (kind === 'team') this.filterTeamId.set(value);
    if (kind === 'status') this.filterStatus.set(value);
    this.load();
  }

  openCreate(): void {
    this.editingEpic.set(null);
    this.form.reset({
      teamId: this.filterTeamId() || this.teams()[0]?.id || '',
      title: '',
      description: '',
      status: 'Open'
    });
    this.modalOpen.set(true);
  }

  openEdit(epic: Epic): void {
    this.editingEpic.set(epic);
    this.form.reset({
      teamId: epic.teamId,
      title: epic.title,
      description: epic.description ?? '',
      status: epic.status
    });
    this.modalOpen.set(true);
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    const { teamId, title, description, status } = this.form.getRawValue();
    const editing = this.editingEpic();
    const request$ = editing
      ? this.epicsService.update(editing.id, title, description || null, status)
      : this.epicsService.create(teamId, title, description || null, status);

    request$.subscribe({
      next: () => {
        this.saving.set(false);
        this.modalOpen.set(false);
        this.load(this.paged()?.page ?? 1);
      },
      error: (err) => {
        this.error.set(err.error?.description ?? 'Failed to save epic.');
        this.saving.set(false);
      }
    });
  }

  deleteEpic(): void {
    const epic = this.deleteTarget();
    if (!epic) return;
    this.deleteTarget.set(null);
    this.epicsService.delete(epic.id).subscribe({
      next: () => this.load(this.paged()?.page ?? 1),
      error: (err) => this.error.set(err.error?.description ?? 'Failed to delete epic.')
    });
  }

  teamName(teamId: string): string {
    return this.teams().find((t) => t.id === teamId)?.name ?? '';
  }

  statusBadgeClass(status: string): string {
    switch (status) {
      case 'InProgress':
        return 'badge-warning';
      case 'Done':
        return 'badge-success';
      case 'Cancelled':
        return 'badge-danger';
      default:
        return 'badge-accent';
    }
  }
}
