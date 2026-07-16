import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProjectsService } from '../../../core/services/projects.service';
import { TeamsService } from '../../../core/services/teams.service';
import { PROJECT_STATUSES, PagedList, Project, Team } from '../../../core/models';
import { ModalComponent } from '../../../shared/modal/modal.component';
import { ConfirmDialogComponent } from '../../../shared/confirm-dialog/confirm-dialog.component';
import { PaginatorComponent } from '../../../shared/paginator/paginator.component';
import { EmptyStateComponent } from '../../../shared/empty-state/empty-state.component';

@Component({
  selector: 'app-projects-list',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ModalComponent,
    ConfirmDialogComponent,
    PaginatorComponent,
    EmptyStateComponent
  ],
  templateUrl: './projects-list.component.html',
  styleUrl: './projects-list.component.scss'
})
export class ProjectsListComponent implements OnInit {
  private readonly projectsService = inject(ProjectsService);
  private readonly teamsService = inject(TeamsService);
  private readonly fb = inject(FormBuilder);

  readonly statuses = PROJECT_STATUSES;

  readonly paged = signal<PagedList<Project> | null>(null);
  readonly teams = signal<Team[]>([]);
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);
  readonly modalOpen = signal(false);
  readonly editingProject = signal<Project | null>(null);
  readonly archiveTarget = signal<Project | null>(null);

  readonly filterTeamId = signal('');
  readonly filterStatus = signal('');
  readonly includeArchived = signal(false);

  readonly form = this.fb.nonNullable.group({
    teamId: ['', Validators.required],
    name: ['', [Validators.required, Validators.maxLength(200)]],
    key: [''],
    description: [''],
    status: ['Planned', Validators.required],
    startDate: [''],
    targetDate: ['']
  });

  ngOnInit(): void {
    this.teamsService.list().subscribe({
      next: (res) => this.teams.set(res.items)
    });
    this.load();
  }

  load(page = 1): void {
    this.loading.set(true);
    this.projectsService
      .list({
        teamId: this.filterTeamId() || undefined,
        status: this.filterStatus() || undefined,
        includeArchived: this.includeArchived(),
        page,
        pageSize: 24
      })
      .subscribe({
        next: (res) => {
          this.paged.set(res);
          this.loading.set(false);
        },
        error: (err) => {
          this.error.set(err.error?.description ?? 'Failed to load projects.');
          this.loading.set(false);
        }
      });
  }

  applyFilter(kind: 'team' | 'status' | 'archived', value: string | boolean): void {
    if (kind === 'team') this.filterTeamId.set(value as string);
    if (kind === 'status') this.filterStatus.set(value as string);
    if (kind === 'archived') this.includeArchived.set(value as boolean);
    this.load();
  }

  openCreate(): void {
    this.editingProject.set(null);
    this.form.reset({
      teamId: this.filterTeamId() || this.teams()[0]?.id || '',
      name: '',
      key: '',
      description: '',
      status: 'Planned',
      startDate: '',
      targetDate: ''
    });
    this.modalOpen.set(true);
  }

  openEdit(project: Project): void {
    this.editingProject.set(project);
    this.form.reset({
      teamId: project.teamId,
      name: project.name,
      key: project.key,
      description: project.description ?? '',
      status: project.status,
      startDate: project.startDate ?? '',
      targetDate: project.targetDate ?? ''
    });
    this.modalOpen.set(true);
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving.set(true);
    this.error.set(null);
    const value = this.form.getRawValue();
    const payload = {
      name: value.name,
      key: value.key || null,
      description: value.description || null,
      status: value.status,
      startDate: value.startDate || null,
      targetDate: value.targetDate || null
    };

    const editing = this.editingProject();
    const request$ = editing
      ? this.projectsService.update(editing.id, payload)
      : this.projectsService.create(value.teamId, payload);

    request$.subscribe({
      next: () => {
        this.saving.set(false);
        this.modalOpen.set(false);
        this.load(this.paged()?.page ?? 1);
      },
      error: (err) => {
        this.error.set(err.error?.description ?? 'Failed to save project.');
        this.saving.set(false);
      }
    });
  }

  archive(): void {
    const project = this.archiveTarget();
    if (!project) return;
    this.archiveTarget.set(null);
    this.projectsService.archive(project.id).subscribe({
      next: () => this.load(this.paged()?.page ?? 1),
      error: (err) => this.error.set(err.error?.description ?? 'Failed to archive project.')
    });
  }

  teamName(teamId: string): string {
    return this.teams().find((t) => t.id === teamId)?.name ?? '';
  }

  statusBadgeClass(status: string): string {
    switch (status) {
      case 'Active':
        return 'badge-success';
      case 'Paused':
        return 'badge-warning';
      case 'Cancelled':
        return 'badge-danger';
      case 'Completed':
        return 'badge-accent';
      default:
        return '';
    }
  }
}
