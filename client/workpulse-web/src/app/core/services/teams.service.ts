import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PagedList, TEAM_ROLES, Team, TeamMember, enumValue } from '../models';

@Injectable({ providedIn: 'root' })
export class TeamsService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/teams`;

  list(page = 1, pageSize = 50, includeArchived = false): Observable<PagedList<Team>> {
    const params = new HttpParams()
      .set('page', page)
      .set('pageSize', pageSize)
      .set('includeArchived', includeArchived);
    return this.http.get<PagedList<Team>>(this.base, { params });
  }

  get(teamId: string): Observable<Team> {
    return this.http.get<Team>(`${this.base}/${teamId}`);
  }

  create(payload: {
    name: string;
    key?: string;
    description?: string;
    icon?: string;
    color?: string;
  }): Observable<Team> {
    return this.http.post<Team>(this.base, {
      name: payload.name,
      key: payload.key ?? null,
      description: payload.description ?? null,
      icon: payload.icon ?? null,
      color: payload.color ?? null
    });
  }

  update(
    teamId: string,
    payload: { name: string; description?: string | null; icon?: string | null; color?: string | null }
  ): Observable<Team> {
    return this.http.put<Team>(`${this.base}/${teamId}`, {
      name: payload.name,
      description: payload.description ?? null,
      icon: payload.icon ?? null,
      color: payload.color ?? null
    });
  }

  archive(teamId: string): Observable<void> {
    return this.http.post<void>(`${this.base}/${teamId}/archive`, {});
  }

  listMembers(teamId: string, page = 1, pageSize = 50): Observable<PagedList<TeamMember>> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<PagedList<TeamMember>>(`${this.base}/${teamId}/members`, { params });
  }

  addMember(teamId: string, userId: string, role: string): Observable<TeamMember> {
    return this.http.post<TeamMember>(`${this.base}/${teamId}/members`, {
      userId,
      role: enumValue(TEAM_ROLES, role)
    });
  }

  updateMember(teamId: string, memberId: string, role: string): Observable<void> {
    return this.http.put<void>(`${this.base}/${teamId}/members/${memberId}`, {
      role: enumValue(TEAM_ROLES, role)
    });
  }

  removeMember(teamId: string, memberId: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${teamId}/members/${memberId}`);
  }
}
