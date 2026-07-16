import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { WORKFLOW_STATE_TYPES, Workflow, WorkflowState, enumValue } from '../models';

@Injectable({ providedIn: 'root' })
export class WorkflowsService {
  private readonly http = inject(HttpClient);

  private base(teamId: string): string {
    return `${environment.apiUrl}/teams/${teamId}/workflow`;
  }

  get(teamId: string): Observable<Workflow> {
    return this.http.get<Workflow>(this.base(teamId));
  }

  createState(
    teamId: string,
    payload: { name: string; type: string; color: string; position: number }
  ): Observable<WorkflowState> {
    return this.http.post<WorkflowState>(`${this.base(teamId)}/states`, {
      name: payload.name,
      type: enumValue(WORKFLOW_STATE_TYPES, payload.type),
      color: payload.color,
      position: payload.position
    });
  }

  updateState(
    teamId: string,
    stateId: string,
    payload: { name: string; type: string; color: string; position: number; isDefault: boolean }
  ): Observable<WorkflowState> {
    return this.http.put<WorkflowState>(`${this.base(teamId)}/states/${stateId}`, {
      name: payload.name,
      type: enumValue(WORKFLOW_STATE_TYPES, payload.type),
      color: payload.color,
      position: payload.position,
      isDefault: payload.isDefault
    });
  }

  deleteState(teamId: string, stateId: string): Observable<void> {
    return this.http.delete<void>(`${this.base(teamId)}/states/${stateId}`);
  }

  reorder(teamId: string, stateIdsInOrder: string[]): Observable<void> {
    return this.http.put<void>(`${this.base(teamId)}/states/reorder`, { stateIdsInOrder });
  }
}
