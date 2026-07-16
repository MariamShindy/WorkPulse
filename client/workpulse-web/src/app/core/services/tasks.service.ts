import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  DEPENDENCY_TYPES,
  Label,
  PagedList,
  TASK_PRIORITIES,
  TaskAssignee,
  TaskDependency,
  TaskItem,
  enumValue
} from '../models';

export interface TaskFilters {
  teamId?: string;
  projectId?: string;
  assigneeId?: string;
  workflowStateId?: string;
  priority?: string;
  page?: number;
  pageSize?: number;
}

export interface TaskPayload {
  title: string;
  priority: string;
  description?: string | null;
  projectId?: string | null;
  assigneeId?: string | null;
  assigneeIds?: string[] | null;
  dueDate?: string | null;
  storyPoints?: number | null;
  estimatedHours?: number | null;
  isBlocked?: boolean;
  blockedReason?: string | null;
  epicId?: string | null;
  sprintId?: string | null;
  assignedTeamId?: string | null;
}

@Injectable({ providedIn: 'root' })
export class TasksService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/tasks`;

  list(filters: TaskFilters = {}): Observable<PagedList<TaskItem>> {
    let params = new HttpParams()
      .set('page', filters.page ?? 1)
      .set('pageSize', filters.pageSize ?? 50);
    if (filters.teamId) params = params.set('teamId', filters.teamId);
    if (filters.projectId) params = params.set('projectId', filters.projectId);
    if (filters.assigneeId) params = params.set('assigneeId', filters.assigneeId);
    if (filters.workflowStateId) params = params.set('workflowStateId', filters.workflowStateId);
    if (filters.priority) params = params.set('priority', filters.priority);
    return this.http.get<PagedList<TaskItem>>(this.base, { params });
  }

  get(taskId: string): Observable<TaskItem> {
    return this.http.get<TaskItem>(`${this.base}/${taskId}`);
  }

  create(
    teamId: string,
    payload: TaskPayload & { workflowStateId?: string | null; parentTaskId?: string | null }
  ): Observable<TaskItem> {
    return this.http.post<TaskItem>(this.base, {
      teamId,
      title: payload.title,
      description: payload.description ?? null,
      priority: enumValue(TASK_PRIORITIES, payload.priority),
      projectId: payload.projectId ?? null,
      workflowStateId: payload.workflowStateId ?? null,
      assigneeId: payload.assigneeId ?? null,
      assigneeIds: payload.assigneeIds ?? null,
      dueDate: payload.dueDate ?? null,
      parentTaskId: payload.parentTaskId ?? null,
      storyPoints: payload.storyPoints ?? null,
      estimatedHours: payload.estimatedHours ?? null,
      isBlocked: payload.isBlocked ?? false,
      blockedReason: payload.blockedReason ?? null,
      epicId: payload.epicId ?? null,
      sprintId: payload.sprintId ?? null,
      assignedTeamId: payload.assignedTeamId ?? null
    });
  }

  update(taskId: string, payload: TaskPayload): Observable<TaskItem> {
    return this.http.put<TaskItem>(`${this.base}/${taskId}`, {
      title: payload.title,
      description: payload.description ?? null,
      priority: enumValue(TASK_PRIORITIES, payload.priority),
      projectId: payload.projectId ?? null,
      assigneeId: payload.assigneeId ?? null,
      assigneeIds: payload.assigneeIds ?? null,
      dueDate: payload.dueDate ?? null,
      storyPoints: payload.storyPoints ?? null,
      estimatedHours: payload.estimatedHours ?? null,
      isBlocked: payload.isBlocked ?? false,
      blockedReason: payload.blockedReason ?? null,
      epicId: payload.epicId ?? null,
      sprintId: payload.sprintId ?? null,
      assignedTeamId: payload.assignedTeamId ?? null,
      rowVersion: null
    });
  }

  move(taskId: string, workflowStateId: string, sortOrder?: number): Observable<TaskItem> {
    return this.http.post<TaskItem>(`${this.base}/${taskId}/move`, {
      workflowStateId,
      sortOrder: sortOrder ?? null
    });
  }

  delete(taskId: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${taskId}`);
  }

  // ── Assignees ──────────────────────────────────────────────────────────────

  listAssignees(taskId: string): Observable<TaskAssignee[]> {
    return this.http.get<TaskAssignee[]>(`${this.base}/${taskId}/assignees`);
  }

  addAssignee(taskId: string, userId: string): Observable<void> {
    return this.http.post<void>(`${this.base}/${taskId}/assignees`, { userId });
  }

  removeAssignee(taskId: string, userId: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${taskId}/assignees/${userId}`);
  }

  // ── Dependencies ───────────────────────────────────────────────────────────

  listDependencies(taskId: string): Observable<TaskDependency[]> {
    return this.http.get<TaskDependency[]>(`${this.base}/${taskId}/dependencies`);
  }

  addDependency(taskId: string, dependsOnTaskId: string, type: string): Observable<TaskDependency> {
    return this.http.post<TaskDependency>(`${this.base}/${taskId}/dependencies`, {
      dependsOnTaskId,
      type: enumValue(DEPENDENCY_TYPES, type)
    });
  }

  removeDependency(taskId: string, dependencyId: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${taskId}/dependencies/${dependencyId}`);
  }

  // ── Labels on tasks ────────────────────────────────────────────────────────

  listLabels(taskId: string): Observable<Label[]> {
    return this.http.get<Label[]>(`${this.base}/${taskId}/labels`);
  }

  assignLabel(taskId: string, labelId: string): Observable<void> {
    return this.http.post<void>(`${this.base}/${taskId}/labels`, { labelId });
  }

  removeLabel(taskId: string, labelId: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${taskId}/labels/${labelId}`);
  }
}
