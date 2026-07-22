import { Component, OnDestroy, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { CdkDragDrop, DragDropModule } from '@angular/cdk/drag-drop';
import { Subscription, forkJoin, of } from 'rxjs';
import { TasksService } from '../../../core/services/tasks.service';
import { TeamsService } from '../../../core/services/teams.service';
import { WorkflowsService } from '../../../core/services/workflows.service';
import { ProjectsService } from '../../../core/services/projects.service';
import { EpicsService } from '../../../core/services/epics.service';
import { SprintsService } from '../../../core/services/sprints.service';
import { LabelsService } from '../../../core/services/labels.service';
import { SavedViewsService } from '../../../core/services/saved-views.service';
import { CompaniesService } from '../../../core/services/companies.service';
import { RealtimeService } from '../../../core/services/realtime.service';
import {
  CompanyMember,
  Epic,
  Label,
  Project,
  SavedView,
  Sprint,
  TASK_PRIORITIES,
  TaskItem,
  Team,
  WorkflowState
} from '../../../core/models';
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
  imports: [CommonModule, ReactiveFormsModule, DrawerComponent, EmptyStateComponent, TaskDetailComponent, DragDropModule],
  templateUrl: './kanban-board.component.html',
  styleUrl: './kanban-board.component.scss'
})
export class KanbanBoardComponent implements OnInit, OnDestroy {
  private readonly tasksService = inject(TasksService);
  private readonly teamsService = inject(TeamsService);
  private readonly workflows = inject(WorkflowsService);
  private readonly projectsService = inject(ProjectsService);
  private readonly epicsService = inject(EpicsService);
  private readonly sprintsService = inject(SprintsService);
  private readonly labelsService = inject(LabelsService);
  private readonly savedViewsService = inject(SavedViewsService);
  private readonly companies = inject(CompaniesService);
  private readonly realtime = inject(RealtimeService);
  private readonly route = inject(ActivatedRoute);
  private readonly fb = inject(FormBuilder);

  private realtimeSub?: Subscription;
  private joinedTeamId: string | null = null;

  readonly priorities = TASK_PRIORITIES;

  readonly teams = signal<Team[]>([]);
  readonly selectedTeamId = signal<string | null>(null);
  readonly states = signal<WorkflowState[]>([]);
  readonly tasks = signal<TaskItem[]>([]);
  readonly projects = signal<Project[]>([]);
  readonly epics = signal<Epic[]>([]);
  readonly sprints = signal<Sprint[]>([]);
  readonly labels = signal<Label[]>([]);
  readonly savedViews = signal<SavedView[]>([]);
  readonly companyMembers = signal<CompanyMember[]>([]);
  readonly taskLabelMap = signal<Map<string, string[]>>(new Map());

  readonly loading = signal(true);
  readonly creating = signal(false);
  readonly error = signal<string | null>(null);
  readonly filters = signal<BoardFilters>({ ...EMPTY_FILTERS });
  readonly selectedTask = signal<TaskItem | null>(null);
  readonly saveViewOpen = signal(false);
  readonly draggingTaskId = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(200)]]
  });

  readonly saveViewForm = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    isShared: [false]
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
    this.companies.listMembers(1, 100).subscribe({
      next: (res) => this.companyMembers.set(res.items)
    });
    this.labelsService.list().subscribe({ next: (res) => this.labels.set(res.items) });
    this.loadSavedViews();

    this.teamsService.list().subscribe({
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

    this.realtimeSub = this.realtime.taskEvents$.subscribe(() => this.reloadTasks());
  }

  ngOnDestroy(): void {
    this.realtimeSub?.unsubscribe();
    if (this.joinedTeamId) {
      this.realtime.leaveTeam(this.joinedTeamId);
    }
  }

  selectTeam(teamId: string): void {
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

    this.workflows.get(teamId).subscribe({
      next: (workflow) => this.states.set(workflow.states),
      error: (err) => this.error.set(err.error?.description ?? 'Failed to load workflow.')
    });

    this.projectsService.list({ teamId, pageSize: 100 }).subscribe({
      next: (res) => this.projects.set(res.items)
    });
    this.epicsService.list({ teamId, pageSize: 100 }).subscribe({
      next: (res) => this.epics.set(res.items)
    });
    this.sprintsService.list({ teamId, pageSize: 100 }).subscribe({
      next: (res) => this.sprints.set(res.items)
    });

    this.reloadTasks(() => this.openDeepLinkedTask());
  }

  reloadTasks(after?: () => void): void {
    const teamId = this.selectedTeamId();
    if (!teamId) return;
    this.tasksService.list({ teamId, pageSize: 200 }).subscribe({
      next: (res) => {
        this.tasks.set(res.items);
        this.loading.set(false);
        after?.();
      },
      error: () => this.loading.set(false)
    });
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

  // ── Saved views ────────────────────────────────────────────────────────────

  loadSavedViews(): void {
    this.savedViewsService.list('Tasks').subscribe({
      next: (res) => this.savedViews.set(res.items)
    });
  }

  applySavedView(viewId: string): void {
    if (!viewId) return;
    const view = this.savedViews().find((v) => v.id === viewId);
    if (!view) return;
    try {
      const parsed = JSON.parse(view.filtersJson) as Partial<BoardFilters> & { teamId?: string };
      if (parsed.teamId && parsed.teamId !== this.selectedTeamId()) {
        this.selectTeam(parsed.teamId);
      }
      this.filters.set({
        projectId: parsed.projectId ?? '',
        epicId: parsed.epicId ?? '',
        sprintId: parsed.sprintId ?? '',
        labelId: parsed.labelId ?? '',
        priority: parsed.priority ?? ''
      });
      if (parsed.labelId) {
        this.loadTaskLabels();
      }
    } catch {
      this.error.set('This saved view could not be applied.');
    }
  }

  saveCurrentView(): void {
    if (this.saveViewForm.invalid) return;
    const { name, isShared } = this.saveViewForm.getRawValue();
    this.savedViewsService
      .create({
        name,
        entityType: 'Tasks',
        filtersJson: JSON.stringify({ teamId: this.selectedTeamId(), ...this.filters() }),
        sortJson: '{}',
        isShared
      })
      .subscribe({
        next: (view) => {
          this.savedViews.update((list) => [view, ...list]);
          this.saveViewOpen.set(false);
          this.saveViewForm.reset({ name: '', isShared: false });
        },
        error: (err) => this.error.set(err.error?.description ?? 'Failed to save view.')
      });
  }

  deleteSavedView(viewId: string): void {
    this.savedViewsService.delete(viewId).subscribe({
      next: () => this.savedViews.update((list) => list.filter((v) => v.id !== viewId))
    });
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
          this.error.set(err.error?.description ?? 'Failed to create task.');
          this.creating.set(false);
        }
      });
  }

  moveTask(task: TaskItem, targetStateId: string): void {
    if (task.workflowStateId === targetStateId) return;

    // Optimistic update; revert on failure.
    const previous = this.tasks();
    const targetState = this.states().find((s) => s.id === targetStateId);
    this.tasks.update((list) =>
      list.map((t) =>
        t.id === task.id
          ? {
              ...t,
              workflowStateId: targetStateId,
              workflowStateName: targetState?.name ?? t.workflowStateName,
              workflowStateType: targetState?.type ?? t.workflowStateType
            }
          : t
      ));

    this.tasksService.move(task.id, targetStateId).subscribe({
      next: (updatedTask) =>
        this.tasks.update((list) => list.map((t) => (t.id === updatedTask.id ? updatedTask : t))),
      error: (err) => {
        this.tasks.set(previous);
        this.error.set(err.error?.description ?? 'Failed to move task.');
      }
    });
  }

  dropped(event: CdkDragDrop<TaskItem[]>): void {
    if (event.previousContainer === event.container) return;
    const task = event.item.data as TaskItem;
    const targetStateId = event.container.id.replace(/^col-/, '');
    this.moveTask(task, targetStateId);
  }

  openTask(task: TaskItem): void {
    this.selectedTask.set(task);
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
