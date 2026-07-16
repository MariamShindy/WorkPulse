import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Notification, PagedList, TaskActivity, TaskComment } from '../models';

@Injectable({ providedIn: 'root' })
export class CollaborationService {
  private readonly http = inject(HttpClient);
  private readonly base = environment.apiUrl;

  // ── Comments ───────────────────────────────────────────────────────────────

  listComments(taskId: string, page = 1, pageSize = 25): Observable<PagedList<TaskComment>> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<PagedList<TaskComment>>(`${this.base}/tasks/${taskId}/comments`, { params });
  }

  addComment(taskId: string, body: string, mentionedUserIds?: string[]): Observable<TaskComment> {
    return this.http.post<TaskComment>(`${this.base}/tasks/${taskId}/comments`, {
      body,
      mentionedUserIds: mentionedUserIds ?? null
    });
  }

  updateComment(commentId: string, body: string): Observable<TaskComment> {
    return this.http.put<TaskComment>(`${this.base}/comments/${commentId}`, { body });
  }

  // ── Activity ───────────────────────────────────────────────────────────────

  listActivity(taskId: string, page = 1, pageSize = 50): Observable<PagedList<TaskActivity>> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<PagedList<TaskActivity>>(`${this.base}/tasks/${taskId}/activity`, { params });
  }

  // ── Watchers ───────────────────────────────────────────────────────────────

  watch(taskId: string): Observable<void> {
    return this.http.post<void>(`${this.base}/tasks/${taskId}/watch`, {});
  }

  unwatch(taskId: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/tasks/${taskId}/watch`);
  }

  // ── Notifications ──────────────────────────────────────────────────────────

  listNotifications(unreadOnly = false, page = 1, pageSize = 25): Observable<PagedList<Notification>> {
    const params = new HttpParams()
      .set('unreadOnly', unreadOnly)
      .set('page', page)
      .set('pageSize', pageSize);
    return this.http.get<PagedList<Notification>>(`${this.base}/notifications`, { params });
  }

  markNotificationRead(notificationId: string): Observable<void> {
    return this.http.post<void>(`${this.base}/notifications/${notificationId}/read`, {});
  }

  markAllNotificationsRead(): Observable<void> {
    return this.http.post<void>(`${this.base}/notifications/read-all`, {});
  }
}
