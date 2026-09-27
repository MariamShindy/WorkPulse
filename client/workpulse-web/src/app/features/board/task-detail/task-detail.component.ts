import { Component, DestroyRef, OnInit, computed, inject, input, output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { forkJoin } from 'rxjs';
import { TasksService } from '../../../core/services/tasks.service';
import { CollaborationService } from '../../../core/services/collaboration.service';
import { WorkLogsService } from '../../../core/services/work-logs.service';
import { FilesService } from '../../../core/services/files.service';
import { AuthService } from '../../../core/services/auth.service';
import {
  CompanyMember,
  DEPENDENCY_TYPES,
  Epic,
  Label,
  Project,
  Sprint,
  StoredFile,
  TASK_PRIORITIES,
  TaskActivity,
  TaskAssignee,
  TaskComment,
  TaskDependency,
  TaskItem,
  WorkLog
} from '../../../core/models';
import { ConfirmDialogComponent } from '../../../shared/confirm-dialog/confirm-dialog.component';
import { apiErrorMessage } from '../../../core/utils/api-error';

type DrawerTab = 'details' | 'comments' | 'activity' | 'worklog' | 'files';

@Component({
  selector: 'app-task-detail',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ConfirmDialogComponent],
  templateUrl: './task-detail.component.html',
  styleUrl: './task-detail.component.scss'
})
export class TaskDetailComponent implements OnInit {
  private readonly tasksService = inject(TasksService);
  private readonly collaboration = inject(CollaborationService);
  private readonly workLogsService = inject(WorkLogsService);
  private readonly filesService = inject(FilesService);
  private readonly auth = inject(AuthService);
  private readonly fb = inject(FormBuilder);
  private readonly destroyRef = inject(DestroyRef);

  readonly task = input.required<TaskItem>();
  readonly projects = input<Project[]>([]);
  readonly epics = input<Epic[]>([]);
  readonly sprints = input<Sprint[]>([]);
  readonly allLabels = input<Label[]>([]);
  readonly companyMembers = input<CompanyMember[]>([]);
  readonly teamTasks = input<TaskItem[]>([]);

  readonly updated = output<TaskItem>();
  readonly deleted = output<string>();

  readonly priorities = TASK_PRIORITIES;
  readonly dependencyTypes = DEPENDENCY_TYPES;

  readonly tab = signal<DrawerTab>('details');
  readonly error = signal<string | null>(null);
  readonly saving = signal(false);
  readonly watching = signal(false);
  readonly confirmDelete = signal(false);

  readonly assignees = signal<TaskAssignee[]>([]);
  readonly taskLabels = signal<Label[]>([]);
  readonly dependencies = signal<TaskDependency[]>([]);
  readonly comments = signal<TaskComment[]>([]);
  readonly activity = signal<TaskActivity[]>([]);
  readonly workLogs = signal<WorkLog[]>([]);
  readonly files = signal<StoredFile[]>([]);
  readonly editingCommentId = signal<string | null>(null);

  readonly unassignedMembers = computed(() => {
    const assigned = new Set(this.assignees().map((a) => a.userId));
    return this.companyMembers().filter((m) => !assigned.has(m.userId));
  });

  readonly unusedLabels = computed(() => {
    const used = new Set(this.taskLabels().map((l) => l.id));
    return this.allLabels().filter((l) => !used.has(l.id));
  });

  readonly dependencyCandidates = computed(() => {
    const linked = new Set(this.dependencies().map((d) => d.dependsOnTaskId));
    return this.teamTasks().filter((t) => t.id !== this.task().id && !linked.has(t.id));
  });

  readonly form = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    description: [''],
    priority: ['Medium'],
    projectId: [''],
    dueDate: [''],
    storyPoints: [null as number | null],
    estimatedHours: [null as number | null],
    isBlocked: [false],
    blockedReason: [''],
    epicId: [''],
    sprintId: ['']
  });

  readonly commentForm = this.fb.nonNullable.group({
    body: ['', [Validators.required, Validators.maxLength(2000)]]
  });

  readonly editCommentForm = this.fb.nonNullable.group({
    body: ['', [Validators.required, Validators.maxLength(2000)]]
  });

  readonly workLogForm = this.fb.nonNullable.group({
    hours: [1, [Validators.required, Validators.min(0.1)]],
    loggedDate: [new Date().toISOString().slice(0, 10), Validators.required],
    description: ['']
  });

  readonly dependencyForm = this.fb.nonNullable.group({
    dependsOnTaskId: ['', Validators.required],
    type: ['Blocks', Validators.required]
  });

  ngOnInit(): void {
    const t = this.task();
    this.form.patchValue({
      title: t.title,
      description: t.description ?? '',
      priority: t.priority,
      projectId: t.projectId ?? '',
      dueDate: t.dueDate ?? '',
      storyPoints: t.storyPoints ?? null,
      estimatedHours: t.estimatedHours ?? null,
      isBlocked: t.isBlocked,
      blockedReason: t.blockedReason ?? '',
      epicId: t.epicId ?? '',
      sprintId: t.sprintId ?? ''
    });

    forkJoin({
      assignees: this.tasksService.listAssignees(t.id),
      labels: this.tasksService.listLabels(t.id),
      dependencies: this.tasksService.listDependencies(t.id),
      comments: this.collaboration.listComments(t.id),
      workLogs: this.workLogsService.list(t.id),
      files: this.filesService.list('Task', t.id)
    })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (result) => {
          this.assignees.set(result.assignees);
          this.taskLabels.set(result.labels);
          this.dependencies.set(result.dependencies);
          this.comments.set(result.comments.items);
          this.workLogs.set(result.workLogs.items);
          this.files.set(result.files.items);
        },
        error: (err) => this.error.set(apiErrorMessage(err, 'Failed to load task details.'))
      });
  }

  selectTab(tab: DrawerTab): void {
    this.tab.set(tab);
    if (tab === 'activity' && this.activity().length === 0) {
      this.collaboration.listActivity(this.task().id).subscribe({
        next: (res) => this.activity.set(res.items)
      });
    }
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    this.error.set(null);
    const v = this.form.getRawValue();

    this.tasksService
      .update(this.task().id, {
        title: v.title,
        description: v.description || null,
        priority: v.priority,
        projectId: v.projectId || null,
        assigneeId: this.task().assigneeId ?? null,
        assigneeIds: this.assignees().map((a) => a.userId),
        dueDate: v.dueDate || null,
        storyPoints: v.storyPoints,
        estimatedHours: v.estimatedHours,
        isBlocked: v.isBlocked,
        blockedReason: v.isBlocked ? v.blockedReason || null : null,
        epicId: v.epicId || null,
        sprintId: v.sprintId || null,
        assignedTeamId: this.task().assignedTeamId ?? null
      })
      .subscribe({
        next: (task) => {
          this.saving.set(false);
          this.updated.emit(task);
        },
        error: (err) => {
          this.error.set(apiErrorMessage(err, 'Failed to save task.'));
          this.saving.set(false);
        }
      });
  }

  deleteTask(): void {
    this.confirmDelete.set(false);
    this.tasksService.delete(this.task().id).subscribe({
      next: () => this.deleted.emit(this.task().id),
      error: (err) => this.error.set(apiErrorMessage(err, 'Failed to delete task.'))
    });
  }

  // ── Watchers ───────────────────────────────────────────────────────────────

  toggleWatch(): void {
    const request$ = this.watching()
      ? this.collaboration.unwatch(this.task().id)
      : this.collaboration.watch(this.task().id);
    request$.subscribe({
      next: () => this.watching.update((v) => !v),
      error: (err) => this.error.set(apiErrorMessage(err, 'Failed to update watcher.'))
    });
  }

  // ── Assignees ──────────────────────────────────────────────────────────────

  addAssignee(userId: string): void {
    if (!userId) return;
    this.tasksService.addAssignee(this.task().id, userId).subscribe({
      next: () =>
        this.tasksService.listAssignees(this.task().id).subscribe({
          next: (a) => this.assignees.set(a)
        }),
      error: (err) => this.error.set(apiErrorMessage(err, 'Failed to add assignee.'))
    });
  }

  removeAssignee(userId: string): void {
    this.tasksService.removeAssignee(this.task().id, userId).subscribe({
      next: () => this.assignees.update((list) => list.filter((a) => a.userId !== userId)),
      error: (err) => this.error.set(apiErrorMessage(err, 'Failed to remove assignee.'))
    });
  }

  // ── Labels ─────────────────────────────────────────────────────────────────

  addLabel(labelId: string): void {
    if (!labelId) return;
    this.tasksService.assignLabel(this.task().id, labelId).subscribe({
      next: () => {
        const label = this.allLabels().find((l) => l.id === labelId);
        if (label) this.taskLabels.update((list) => [...list, label]);
      },
      error: (err) => this.error.set(apiErrorMessage(err, 'Failed to add label.'))
    });
  }

  removeLabel(labelId: string): void {
    this.tasksService.removeLabel(this.task().id, labelId).subscribe({
      next: () => this.taskLabels.update((list) => list.filter((l) => l.id !== labelId)),
      error: (err) => this.error.set(apiErrorMessage(err, 'Failed to remove label.'))
    });
  }

  // ── Dependencies ───────────────────────────────────────────────────────────

  addDependency(): void {
    if (this.dependencyForm.invalid) return;
    const { dependsOnTaskId, type } = this.dependencyForm.getRawValue();
    this.tasksService.addDependency(this.task().id, dependsOnTaskId, type).subscribe({
      next: (dep) => {
        this.dependencies.update((list) => [...list, dep]);
        this.dependencyForm.reset({ dependsOnTaskId: '', type: 'Blocks' });
      },
      error: (err) => this.error.set(apiErrorMessage(err, 'Failed to add dependency.'))
    });
  }

  removeDependency(dependencyId: string): void {
    this.tasksService.removeDependency(this.task().id, dependencyId).subscribe({
      next: () => this.dependencies.update((list) => list.filter((d) => d.id !== dependencyId)),
      error: (err) => this.error.set(apiErrorMessage(err, 'Failed to remove dependency.'))
    });
  }

  // ── Comments ───────────────────────────────────────────────────────────────

  loadComments(): void {
    this.collaboration.listComments(this.task().id).subscribe({
      next: (res) => this.comments.set(res.items)
    });
  }

  addComment(): void {
    if (this.commentForm.invalid) return;
    this.collaboration.addComment(this.task().id, this.commentForm.getRawValue().body).subscribe({
      next: (comment) => {
        this.comments.update((list) => [comment, ...list]);
        this.commentForm.reset({ body: '' });
      },
      error: (err) => this.error.set(apiErrorMessage(err, 'Failed to add comment.'))
    });
  }

  startEditComment(comment: TaskComment): void {
    this.editingCommentId.set(comment.id);
    this.editCommentForm.patchValue({ body: comment.body });
  }

  saveComment(): void {
    const commentId = this.editingCommentId();
    if (!commentId || this.editCommentForm.invalid) return;
    this.collaboration.updateComment(commentId, this.editCommentForm.getRawValue().body).subscribe({
      next: (updatedComment) => {
        this.comments.update((list) =>
          list.map((c) => (c.id === commentId ? updatedComment : c)));
        this.editingCommentId.set(null);
      },
      error: (err) => this.error.set(apiErrorMessage(err, 'Failed to update comment.'))
    });
  }

  isOwnComment(comment: TaskComment): boolean {
    return comment.authorId === this.auth.user()?.id;
  }

  // ── Work logs ──────────────────────────────────────────────────────────────

  loadWorkLogs(): void {
    this.workLogsService.list(this.task().id).subscribe({
      next: (res) => this.workLogs.set(res.items)
    });
  }

  addWorkLog(): void {
    if (this.workLogForm.invalid) return;
    const { hours, loggedDate, description } = this.workLogForm.getRawValue();
    this.workLogsService.create(this.task().id, hours, loggedDate, description || undefined).subscribe({
      next: (log) => {
        this.workLogs.update((list) => [log, ...list]);
        this.workLogForm.reset({
          hours: 1,
          loggedDate: new Date().toISOString().slice(0, 10),
          description: ''
        });
      },
      error: (err) => this.error.set(apiErrorMessage(err, 'Failed to log work.'))
    });
  }

  deleteWorkLog(workLogId: string): void {
    this.workLogsService.delete(workLogId).subscribe({
      next: () => this.workLogs.update((list) => list.filter((w) => w.id !== workLogId)),
      error: (err) => this.error.set(apiErrorMessage(err, 'Failed to delete work log.'))
    });
  }

  // ── Files ──────────────────────────────────────────────────────────────────

  loadFiles(): void {
    this.filesService.list('Task', this.task().id).subscribe({
      next: (res) => this.files.set(res.items)
    });
  }

  uploadFile(fileList: FileList | null): void {
    const file = fileList?.item(0);
    if (!file) return;
    this.filesService.upload(file, 'Task', this.task().id).subscribe({
      next: (stored) => this.files.update((list) => [stored, ...list]),
      error: (err) => this.error.set(apiErrorMessage(err, 'Failed to upload file.'))
    });
  }

  downloadFile(file: StoredFile): void {
    this.filesService.download(file.id).subscribe({
      next: (res) => this.filesService.saveBlob(res, file.fileName),
      error: () => this.error.set('Failed to download file.')
    });
  }

  deleteFile(fileId: string): void {
    this.filesService.delete(fileId).subscribe({
      next: () => this.files.update((list) => list.filter((f) => f.id !== fileId)),
      error: (err) => this.error.set(apiErrorMessage(err, 'Failed to delete file.'))
    });
  }

  // ── Helpers ────────────────────────────────────────────────────────────────

  memberName(userId: string): string {
    return this.companyMembers().find((m) => m.userId === userId)?.fullName ?? 'Unknown user';
  }

  taskIdentifier(taskId: string): string {
    const t = this.teamTasks().find((x) => x.id === taskId);
    return t ? `${t.identifier} — ${t.title}` : taskId.slice(0, 8);
  }

  fileSize(bytes: number): string {
    if (bytes < 1024) return `${bytes} B`;
    if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
    return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
  }
}
