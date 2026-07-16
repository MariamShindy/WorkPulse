import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FilesService } from '../../core/services/files.service';
import { TeamsService } from '../../core/services/teams.service';
import { ProjectsService } from '../../core/services/projects.service';
import { TasksService } from '../../core/services/tasks.service';
import { PagedList, Project, StoredFile, TaskItem, Team } from '../../core/models';
import { PaginatorComponent } from '../../shared/paginator/paginator.component';
import { EmptyStateComponent } from '../../shared/empty-state/empty-state.component';
import { ConfirmDialogComponent } from '../../shared/confirm-dialog/confirm-dialog.component';

type FileEntityKind = 'Task' | 'Project' | 'Team';

@Component({
  selector: 'app-files',
  standalone: true,
  imports: [CommonModule, PaginatorComponent, EmptyStateComponent, ConfirmDialogComponent],
  templateUrl: './files.component.html',
  styleUrl: './files.component.scss'
})
export class FilesComponent implements OnInit {
  private readonly filesService = inject(FilesService);
  private readonly teamsService = inject(TeamsService);
  private readonly projectsService = inject(ProjectsService);
  private readonly tasksService = inject(TasksService);

  readonly entityTypes: FileEntityKind[] = ['Task', 'Project', 'Team'];

  readonly teams = signal<Team[]>([]);
  readonly projects = signal<Project[]>([]);
  readonly tasks = signal<TaskItem[]>([]);

  readonly entityType = signal<FileEntityKind>('Task');
  readonly entityId = signal('');
  readonly paged = signal<PagedList<StoredFile> | null>(null);
  readonly loading = signal(false);
  readonly uploading = signal(false);
  readonly error = signal<string | null>(null);
  readonly deleteTarget = signal<StoredFile | null>(null);

  readonly entityOptions = computed<{ id: string; label: string }[]>(() => {
    switch (this.entityType()) {
      case 'Task':
        return this.tasks().map((t) => ({ id: t.id, label: `${t.identifier} — ${t.title}` }));
      case 'Project':
        return this.projects().map((p) => ({ id: p.id, label: `${p.key} — ${p.name}` }));
      case 'Team':
        return this.teams().map((t) => ({ id: t.id, label: `${t.key} — ${t.name}` }));
    }
  });

  ngOnInit(): void {
    this.teamsService.list().subscribe({ next: (res) => this.teams.set(res.items) });
    this.projectsService.list({ pageSize: 100 }).subscribe({
      next: (res) => this.projects.set(res.items)
    });
    this.tasksService.list({ pageSize: 100 }).subscribe({
      next: (res) => this.tasks.set(res.items)
    });
  }

  setEntityType(type: string): void {
    this.entityType.set(type as FileEntityKind);
    this.entityId.set('');
    this.paged.set(null);
  }

  setEntity(id: string): void {
    this.entityId.set(id);
    if (id) {
      this.load();
    } else {
      this.paged.set(null);
    }
  }

  load(page = 1): void {
    if (!this.entityId()) return;
    this.loading.set(true);
    this.error.set(null);
    this.filesService.list(this.entityType(), this.entityId(), page, 25).subscribe({
      next: (res) => {
        this.paged.set(res);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.description ?? 'Failed to load files.');
        this.loading.set(false);
      }
    });
  }

  upload(fileList: FileList | null): void {
    const file = fileList?.item(0);
    if (!file || !this.entityId()) return;
    this.uploading.set(true);
    this.filesService.upload(file, this.entityType(), this.entityId()).subscribe({
      next: () => {
        this.uploading.set(false);
        this.load(this.paged()?.page ?? 1);
      },
      error: (err) => {
        this.error.set(err.error?.description ?? 'Upload failed (10 MB limit).');
        this.uploading.set(false);
      }
    });
  }

  download(file: StoredFile): void {
    this.filesService.download(file.id).subscribe({
      next: (res) => this.filesService.saveBlob(res, file.fileName),
      error: () => this.error.set('Failed to download file.')
    });
  }

  deleteFile(): void {
    const file = this.deleteTarget();
    if (!file) return;
    this.deleteTarget.set(null);
    this.filesService.delete(file.id).subscribe({
      next: () => this.load(this.paged()?.page ?? 1),
      error: (err) => this.error.set(err.error?.description ?? 'Failed to delete file.')
    });
  }

  fileSize(bytes: number): string {
    if (bytes < 1024) return `${bytes} B`;
    if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
    return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
  }
}
