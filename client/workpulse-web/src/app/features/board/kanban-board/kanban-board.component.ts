import { Component, DestroyRef, OnDestroy, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CdkDragDrop, DragDropModule } from '@angular/cdk/drag-drop';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Subject, Subscription, forkJoin, of } from 'rxjs';
import { catchError, finalize, switchMap, tap } from 'rxjs/operators';
import { TasksService } from '../../../core/services/tasks.service';
import { TeamsService } from '../../../core/services/teams.service';
import { WorkflowsService } from '../../../core/services/workflows.service';
import { ProjectsService } from '../../../core/services/projects.service';
import { EpicsService } from '../../../core/services/epics.service';
import { SprintsService } from '../../../core/services/sprints.service';
import { LabelsService } from '../../../core/services/labels.service';
import { CompaniesService } from '../../../core/services/companies.service';
import { RealtimeService } from '../../../core/services/realtime.service';
import {
  CompanyMember,
  Epic,
  Label,
  Project,
  Sprint,
  TASK_PRIORITIES,
  TaskItem,
  Team,
  WorkflowState
} from '../../../core/models';
import { apiErrorMessage } from '../../../core/utils/api-error';
import { ToastService } from '../../../core/services/toast.service';
import { DrawerComponent } from '../../../shared/drawer/drawer.component';
import { EmptyStateComponent } from '../../../shared/empty-state/empty-state.component';
import { TaskDetailComponent } from '../task-detail/task-detail.component';

interface BoardFilters {
  projectId: string;
  epicId: string;
  sprintId: string;
  labelId: string;
  priority: string;
}

const EMPTY_FILTERS: BoardFilters = { projectId: '', epicId: '', sprintId: '', labelId: '', priority: '' };

@Component({
  selector: 'app-kanban-board',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, DrawerComponent, EmptyStateComponent, TaskDetailComponent, DragDropModule],
  templateUrl: './kanban-board.component.html',
  styleUrl: './kanban-board.component.scss'
})
export class KanbanBoardComponent implements OnInit, OnDestroy {
  private readonly tasksService = inject(TasksService);
  private readonly toasts = inject(ToastService);
  private readonly teamsService = inject(TeamsService);
  private readonly workflows = inject(WorkflowsService);
  private readonly projectsService = inject(ProjectsService);
  private readonly epicsService = inject(EpicsService);
  private readonly sprintsService = inject(SprintsService);
  private readonly labelsService = inject(LabelsService);
  private readonly companies = inject(CompaniesService);
  private readonly realtime = inject(RealtimeService);
  private readonly route = inject(ActivatedRoute);
  private readonly fb = inject(FormBuilder);

  private realtimeSub?: Subscription;
  private joinedTeamId: string | null = null;
  private readonly teamLoad$ = new Subject<string>();
  private readonly tasksReload$ = new Subject<{ teamId: string; after?: () => void }>();
  private readonly destroyRef = inject(DestroyRef);

  readonly priorities = TASK_PRIORITIES;

  readonly teams = signal<Team[]>([]);
  readonly selectedTeamId = signal<string | null>(null);
  readonly states = signal<WorkflowState[]>([]);
  readonly tasks = signal<TaskItem[]>([]);
  readonly projects = signal<Project[]>([]);
  readonly epics = signal<Epic[]>([]);
  readonly sprints = signal<Sprint[]>([]);
  readonly labels = signal<Label[]>([]);
  readonly companyMembers = signal<CompanyMember[]>([]);
  readonly taskLabelMap = signal<Map<string, string[]>>(new Map());

  readonly loading = signal(true);
  readonly creating = signal(false);
  readonly creatingFull = signal(false);
  readonly error = signal<string | null>(null);
  readonly filters = signal<BoardFilters>({ ...EMPTY_FILTERS });
  readonly selectedTask = signal<TaskItem | null>(null);
  readonly fullAddOpen = signal(false);
  readonly draggingTaskId = signal<string | null>(null);

  /** Suppresses card click after a drag so the drawer does not open on drop. */
  private suppressNextClick = false;

  /** Task ids currently being moved — skip redundant realtime reloads for these. */
  private readonly pendingMoveIds = new Set<string>();

  readonly form = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(200)]]
  });

  readonly fullForm = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    description: [''],
    priority: ['Medium'],
    workflowStateId: [''],
    projectId: [''],
    dueDate: [''],
    epicId: [''],
    sprintId: [''],
    storyPoints: [null as number | null],
    estimatedHours: [null as number | null],
    isBlocked: [false],
    blockedReason: [''],
    assigneeId: ['']
  });

  readonly filteredTasks = computed(() => {
    const f = this.filters();
    const labelMap = this.taskLabelMap();
    return this.tasks().filter((t) => {
      if (f.projectId && t.projectId !== f.projectId) return false;
      if (f.epicId && t.epicId !== f.epicId) return false;
      if (f.sprintId && t.sprintId !== f.sprintId) return false;
      if (f.priority && t.priority !== f.priority) return false;
      if (f.labelId) {
        const taskLabels = labelMap.get(t.id);
        if (!taskLabels || !taskLabels.includes(f.labelId)) return false;
      }
      return true;
    });
  });

  readonly columns = computed(() => {
    const orderedStates = [...this.states()].sort((a, b) => a.position - b.position);
    const visible = this.filteredTasks();
    return orderedStates.map((state) => ({
      state,
      tasks: visible
        .filter((t) => t.workflowStateId === state.id)
        .sort((a, b) => a.sortOrder - b.sortOrder)
    }));
  });

  ngOnInit(): void {
    this.teamLoad$
      .pipe(
        tap((teamId) => {
          if (this.joinedTeamId && this.joinedTeamId !== teamId) {
            this.realtime.leaveTeam(this.joinedTeamId);
          }
          this.realtime.joinTeam(teamId);
          this.joinedTeamId = teamId;
          this.selectedTeamId.set(teamId);
          this.filters.set({ ...EMPTY_FILTERS });
          this.taskLabelMap.set(new Map());
          this.loading.set(true);
          this.error.set(null);
        }),
        switchMap((teamId) =>
          forkJoin({
            workflow: this.workflows.get(teamId),
            projects: this.projectsService.list({ teamId, pageSize: 100 }),
            epics: this.epicsService.list({ teamId, pageSize: 100 }),
            sprints: this.sprintsService.list({ teamId, pageSize: 100 }),
            tasks: this.tasksService.list({ teamId, pageSize: 200 })
          }).pipe(
            catchError((err) => {
              this.error.set(apiErrorMessage(err, 'Failed to load board.'));
              return of(null);
            }),
            finalize(() => this.loading.set(false))
          )
        ),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe((result) => {
        if (!result) return;
        this.states.set(result.workflow.states);
        this.projects.set(result.projects.items);
        this.epics.set(result.epics.items);
        this.sprints.set(result.sprints.items);
        this.tasks.set(result.tasks.items);
        this.openDeepLinkedTask();
      });

    this.tasksReload$
      .pipe(
        switchMap(({ teamId, after }) =>
          this.tasksService.list({ teamId, pageSize: 200 }).pipe(
            tap((res) => {
              this.tasks.set(res.items);
              this.loading.set(false);
              after?.();
            }),
            catchError((err) => {
              this.error.set(apiErrorMessage(err, 'Failed to reload tasks.'));
              this.loading.set(false);
              return of(null);
            })
          )
        ),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe();

    this.companies.listMembers(1, 100).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (res) => this.companyMembers.set(res.items)
    });
    this.labelsService.list().pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (res) => this.labels.set(res.items)
    });

    this.teamsService.list().pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (res) => {
        this.teams.set(res.items);
        const requestedTeamId = this.route.snapshot.queryParamMap.get('teamId');
        const team = res.items.find((t) => t.id === requestedTeamId) ?? res.items[0];
        if (team) {
          this.selectTeam(team.id);
        } else {
          this.loading.set(false);
        }
      },
      error: () => this.loading.set(false)
    });

    this.realtimeSub = this.realtime.taskEvents$.subscribe((event) => this.applyRealtimeEvent(event));
  }

  ngOnDestroy(): void {
    this.realtimeSub?.unsubscribe();
    if (this.joinedTeamId) {
      this.realtime.leaveTeam(this.joinedTeamId);
    }
  }

  selectTeam(teamId: string): void {
    this.teamLoad$.next(teamId);
  }

  reloadTasks(after?: () => void): void {
    const teamId = this.selectedTeamId();
    if (!teamId) return;
    this.tasksReload$.next({ teamId, after });
  }

  setFilter(key: keyof BoardFilters, value: string): void {
    this.filters.update((f) => ({ ...f, [key]: value }));
    if (key === 'labelId' && value) {
      this.loadTaskLabels();
    }
  }

  clearFilters(): void {
    this.filters.set({ ...EMPTY_FILTERS });
  }

  hasActiveFilters(): boolean {
    const f = this.filters();
    return !!(f.projectId || f.epicId || f.sprintId || f.labelId || f.priority);
  }

  // ── Tasks ──────────────────────────────────────────────────────────────────

  createTask(): void {
    const teamId = this.selectedTeamId();
    const defaultState = this.states().find((s) => s.isDefault) ?? this.states()[0];
    if (!teamId || !defaultState || this.form.invalid) return;

    this.creating.set(true);
    this.tasksService
      .create(teamId, {
        title: this.form.controls.title.value,
        priority: 'Medium',
        workflowStateId: defaultState.id,
        projectId: this.filters().projectId || null,
        epicId: this.filters().epicId || null,
        sprintId: this.filters().sprintId || null
      })
      .subscribe({
        next: (task) => {
          this.tasks.update((list) => [task, ...list]);
          this.form.reset();
          this.creating.set(false);
        },
        error: (err) => {
          this.error.set(apiErrorMessage(err, 'Failed to create task.'));
          this.creating.set(false);
        }
      });
  }

  openFullAdd(): void {
    const defaultState = this.states().find((s) => s.isDefault) ?? this.states()[0];
    const f = this.filters();
    this.selectedTask.set(null);
    this.fullForm.reset({
      title: this.form.controls.title.value.trim(),
      description: '',
      priority: 'Medium',
      workflowStateId: defaultState?.id ?? '',
      projectId: f.projectId,
      dueDate: '',
      epicId: f.epicId,
      sprintId: f.sprintId,
      storyPoints: null,
      estimatedHours: null,
      isBlocked: false,
      blockedReason: '',
      assigneeId: ''
    });
    this.fullAddOpen.set(true);
  }

  closeFullAdd(): void {
    this.fullAddOpen.set(false);
  }

  createFullTask(): void {
    const teamId = this.selectedTeamId();
    const defaultState = this.states().find((s) => s.isDefault) ?? this.states()[0];
    if (!teamId || this.fullForm.invalid) return;

    const v = this.fullForm.getRawValue();
    const workflowStateId = v.workflowStateId || defaultState?.id;
    if (!workflowStateId) return;

    this.creatingFull.set(true);
    this.tasksService
      .create(teamId, {
        title: v.title.trim(),
        description: v.description.trim() || null,
        priority: v.priority,
        workflowStateId,
        projectId: v.projectId || null,
        epicId: v.epicId || null,
        sprintId: v.sprintId || null,
        dueDate: v.dueDate || null,
        storyPoints: v.storyPoints,
        estimatedHours: v.estimatedHours,
        isBlocked: v.isBlocked,
        blockedReason: v.isBlocked ? v.blockedReason.trim() || null : null,
        assigneeId: v.assigneeId || null,
        assigneeIds: v.assigneeId ? [v.assigneeId] : null
      })
      .subscribe({
        next: (task) => {
          this.tasks.update((list) => [task, ...list]);
          this.form.reset();
          this.creatingFull.set(false);
          this.fullAddOpen.set(false);
          this.selectedTask.set(task);
        },
        error: (err) => {
          this.error.set(apiErrorMessage(err, 'Failed to create task.'));
          this.creatingFull.set(false);
        }
      });
  }

  moveTask(task: TaskItem, targetStateId: string, sortOrder?: number): void {
    if (task.workflowStateId === targetStateId && sortOrder === undefined) return;

    const previous = this.tasks();
    const targetState = this.states().find((s) => s.id === targetStateId);
    this.pendingMoveIds.add(task.id);

    // Optimistic update first so the card snaps to the new column with no wait.
    this.tasks.update((list) =>
      list.map((t) =>
        t.id === task.id
          ? {
              ...t,
              workflowStateId: targetStateId,
              workflowStateName: targetState?.name ?? t.workflowStateName,
              workflowStateType: targetState?.type ?? t.workflowStateType,
              sortOrder: sortOrder ?? t.sortOrder
            }
          : t
      ));

    this.tasksService.move(task.id, targetStateId, sortOrder).subscribe({
      next: (updatedTask) => {
        this.pendingMoveIds.delete(task.id);
        // Patch only if server returned different fields — avoid a full board redraw.
        this.tasks.update((list) =>
          list.map((t) => (t.id === updatedTask.id ? { ...t, ...updatedTask } : t))
        );
      },
      error: (err) => {
        this.pendingMoveIds.delete(task.id);
        this.tasks.set(previous);

        // The card snaps back on its own, which is easy to miss — and the inline alert sits at
        // the top of a board the user may have scrolled past. Toast it so the revert is explained.
        const message = apiErrorMessage(err, 'Failed to move task.');
        this.error.set(message);
        this.toasts.error(message);
      }
    });
  }

  dropped(event: CdkDragDrop<TaskItem[]>): void {
    const task = event.item.data as TaskItem;
    if (!task) return;

    const targetStateId = event.container.id.replace(/^col-/, '');
    if (!targetStateId) return;
    if (event.previousContainer === event.container) return;

    // Clear drag chrome immediately so the optimistic column snap isn't masked.
    this.draggingTaskId.set(null);
    this.moveTask(task, targetStateId, event.currentIndex);
  }

  onDragStarted(taskId: string): void {
    this.draggingTaskId.set(taskId);
    this.suppressNextClick = true;
  }

  onDragEnded(): void {
    this.draggingTaskId.set(null);
    window.setTimeout(() => {
      this.suppressNextClick = false;
    }, 80);
  }

  openTask(task: TaskItem): void {
    if (this.suppressNextClick || this.draggingTaskId()) return;
    this.fullAddOpen.set(false);
    this.selectedTask.set(task);
  }

  onMoveSelect(task: TaskItem, event: Event): void {
    event.stopPropagation();
    const select = event.target as HTMLSelectElement;
    const targetStateId = select.value;
    select.value = '';
    if (targetStateId) {
      this.moveTask(task, targetStateId);
    }
  }

  /** Apply SignalR payloads in-place instead of refetching the whole board. */
  private applyRealtimeEvent(event: { type: string; payload: unknown }): void {
    const payload = event.payload as Partial<TaskItem> | null;
    const taskId = payload && typeof payload === 'object' ? (payload as TaskItem).id : undefined;
    if (!taskId) {
      // Unknown payload shape — soft refresh only when not mid-drag.
      if (!this.draggingTaskId() && this.pendingMoveIds.size === 0) {
        this.reloadTasks();
      }
      return;
    }

    if (this.pendingMoveIds.has(taskId)) {
      return;
    }

    const existing = this.tasks().find((t) => t.id === taskId);
    if (!existing) {
      // New task from another client — fetch list once.
      this.reloadTasks();
      return;
    }

    this.tasks.update((list) =>
      list.map((t) => (t.id === taskId ? { ...t, ...(payload as TaskItem) } : t))
    );
  }

  onTaskUpdated(task: TaskItem): void {
    this.tasks.update((list) => list.map((t) => (t.id === task.id ? task : t)));
    this.selectedTask.set(task);
  }

  onTaskDeleted(taskId: string): void {
    this.tasks.update((list) => list.filter((t) => t.id !== taskId));
    this.selectedTask.set(null);
  }

  priorityClass(priority: string): string {
    return priority.toLowerCase();
  }

  private openDeepLinkedTask(): void {
    const taskId = this.route.snapshot.queryParamMap.get('taskId');
    if (!taskId) return;
    const local = this.tasks().find((t) => t.id === taskId);
    if (local) {
      this.selectedTask.set(local);
      return;
    }
    this.tasksService.get(taskId).subscribe({
      next: (task) => this.selectedTask.set(task)
    });
  }

  /** Task labels aren't part of the task DTO, so fetch them per visible task
   *  when a label filter is applied. */
  private loadTaskLabels(): void {
    const pending = this.tasks().filter((t) => !this.taskLabelMap().has(t.id));
    if (pending.length === 0) return;

    forkJoin(
      pending.map((t) =>
        this.tasksService.listLabels(t.id).pipe()
      )
    ).subscribe({
      next: (results) => {
        this.taskLabelMap.update((map) => {
          const next = new Map(map);
          pending.forEach((t, i) => next.set(t.id, results[i].map((l) => l.id)));
          return next;
        });
      },
      error: () => of(null)
    });
  }
}
