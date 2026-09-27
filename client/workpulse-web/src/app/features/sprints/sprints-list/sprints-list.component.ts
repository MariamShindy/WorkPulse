import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { SprintsService } from '../../../core/services/sprints.service';
import { TeamsService } from '../../../core/services/teams.service';
import { TasksService } from '../../../core/services/tasks.service';
import { PagedList, SPRINT_STATUSES, Sprint, TaskItem, Team } from '../../../core/models';
import { ModalComponent } from '../../../shared/modal/modal.component';
import { ConfirmDialogComponent } from '../../../shared/confirm-dialog/confirm-dialog.component';
import { PaginatorComponent } from '../../../shared/paginator/paginator.component';
import { EmptyStateComponent } from '../../../shared/empty-state/empty-state.component';
import { DrawerComponent } from '../../../shared/drawer/drawer.component';
import { apiErrorMessage } from '../../../core/utils/api-error';

@Component({
  selector: 'app-sprints-list',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ModalComponent,
    ConfirmDialogComponent,
    PaginatorComponent,
    EmptyStateComponent,
    DrawerComponent
  ],
  templateUrl: './sprints-list.component.html',
  styleUrl: './sprints-list.component.scss'
})
export class SprintsListComponent implements OnInit {
  private readonly sprintsService = inject(SprintsService);
  private readonly teamsService = inject(TeamsService);
  private readonly tasksService = inject(TasksService);
  private readonly fb = inject(FormBuilder);

  readonly statuses = SPRINT_STATUSES;

  readonly paged = signal<PagedList<Sprint> | null>(null);
  readonly teams = signal<Team[]>([]);
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);
  readonly modalOpen = signal(false);
  readonly editingSprint = signal<Sprint | null>(null);
  readonly deleteTarget = signal<Sprint | null>(null);
  readonly filterTeamId = signal('');
  readonly filterStatus = signal('');

  // Backlog / planning drawer
  readonly planningSprint = signal<Sprint | null>(null);
  readonly backlogTasks = signal<TaskItem[]>([]);
  readonly sprintTasks = signal<TaskItem[]>([]);
  readonly planningLoading = signal(false);

  readonly form = this.fb.nonNullable.group({
    teamId: ['', Validators.required],
    name: ['', [Validators.required, Validators.maxLength(100)]],
    goal: [''],
    startDate: ['', Validators.required],
    endDate: ['', Validators.required],
    status: ['Planned', Validators.required]
  });

  ngOnInit(): void {
    this.teamsService.list().subscribe({ next: (res) => this.teams.set(res.items) });
    this.load();
  }

  load(page = 1): void {
    this.loading.set(true);
    this.sprintsService
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
          this.error.set(apiErrorMessage(err, 'Failed to load sprints.'));
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
    this.editingSprint.set(null);
    const today = new Date();
    const inTwoWeeks = new Date(today.getTime() + 14 * 24 * 3600 * 1000);
    this.form.reset({
      teamId: this.filterTeamId() || this.teams()[0]?.id || '',
      name: '',
      goal: '',
      startDate: today.toISOString().slice(0, 10),
      endDate: inTwoWeeks.toISOString().slice(0, 10),
      status: 'Planned'
    });
    this.modalOpen.set(true);
  }

  openEdit(sprint: Sprint): void {
    this.editingSprint.set(sprint);
    this.form.reset({
      teamId: sprint.teamId,
      name: sprint.name,
      goal: sprint.goal ?? '',
      startDate: sprint.startDate,
      endDate: sprint.endDate,
      status: sprint.status
    });
    this.modalOpen.set(true);
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    const value = this.form.getRawValue();
    const payload = {
      name: value.name,
      goal: value.goal || null,
      startDate: value.startDate,
      endDate: value.endDate,
      status: value.status
    };
    const editing = this.editingSprint();
    const request$ = editing
      ? this.sprintsService.update(editing.id, payload)
      : this.sprintsService.create(value.teamId, payload);

    request$.subscribe({
      next: () => {
        this.saving.set(false);
        this.modalOpen.set(false);
        this.load(this.paged()?.page ?? 1);
      },
      error: (err) => {
        this.error.set(apiErrorMessage(err, 'Failed to save sprint.'));
        this.saving.set(false);
      }
    });
  }

  deleteSprint(): void {
    const sprint = this.deleteTarget();
    if (!sprint) return;
    this.deleteTarget.set(null);
    this.sprintsService.delete(sprint.id).subscribe({
      next: () => this.load(this.paged()?.page ?? 1),
      error: (err) => this.error.set(apiErrorMessage(err, 'Failed to delete sprint.'))
    });
  }

  // ── Sprint planning ────────────────────────────────────────────────────────

  openPlanning(sprint: Sprint): void {
    this.planningSprint.set(sprint);
    this.refreshPlanning(sprint);
  }

  refreshPlanning(sprint: Sprint): void {
    this.planningLoading.set(true);
    this.sprintsService.backlog(sprint.teamId, undefined, 1, 100).subscribe({
      next: (res) => {
        this.backlogTasks.set(res.items.filter((t) => !t.sprintId));
        this.planningLoading.set(false);
      },
      error: () => this.planningLoading.set(false)
    });
    this.tasksService.list({ teamId: sprint.teamId, pageSize: 200 }).subscribe({
      next: (res) => this.sprintTasks.set(res.items.filter((t) => t.sprintId === sprint.id))
    });
  }

  assignToSprint(task: TaskItem): void {
    const sprint = this.planningSprint();
    if (!sprint) return;
    this.updateTaskSprint(task, sprint.id, () => {
      this.backlogTasks.update((list) => list.filter((t) => t.id !== task.id));
      this.sprintTasks.update((list) => [...list, { ...task, sprintId: sprint.id }]);
    });
  }

  removeFromSprint(task: TaskItem): void {
    this.updateTaskSprint(task, null, () => {
      this.sprintTasks.update((list) => list.filter((t) => t.id !== task.id));
      this.backlogTasks.update((list) => [...list, { ...task, sprintId: null }]);
    });
  }

  teamName(teamId: string): string {
    return this.teams().find((t) => t.id === teamId)?.name ?? '';
  }

  statusBadgeClass(status: string): string {
    switch (status) {
      case 'Active':
        return 'badge-success';
      case 'Completed':
        return 'badge-accent';
      case 'Cancelled':
        return 'badge-danger';
      default:
        return '';
    }
  }

  private updateTaskSprint(task: TaskItem, sprintId: string | null, onSuccess: () => void): void {
    this.tasksService
      .update(task.id, {
        title: task.title,
        description: task.description,
        priority: task.priority,
        projectId: task.projectId,
        assigneeId: task.assigneeId,
        assigneeIds: task.assigneeIds,
        dueDate: task.dueDate,
        storyPoints: task.storyPoints,
        estimatedHours: task.estimatedHours,
        isBlocked: task.isBlocked,
        blockedReason: task.blockedReason,
        epicId: task.epicId,
        sprintId,
        assignedTeamId: task.assignedTeamId
      })
      .subscribe({
        next: onSuccess,
        error: (err) => this.error.set(apiErrorMessage(err, 'Failed to move task.'))
      });
  }
}
