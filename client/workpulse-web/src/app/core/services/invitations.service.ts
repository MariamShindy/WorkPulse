import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthResponse, COMPANY_ROLES, Invitation, enumValue } from '../models';

@Injectable({ providedIn: 'root' })
export class InvitationsService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/invitations`;

  invite(email: string, role: string): Observable<Invitation> {
    return this.http.post<Invitation>(this.base, {
      email,
      role: enumValue(COMPANY_ROLES, role)
    });
  }

  list(): Observable<Invitation[]> {
    return this.http.get<Invitation[]>(this.base);
  }

  accept(payload: {
    token: string;
    password?: string;
    firstName?: string;
    lastName?: string;
  }): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.base}/accept`, {
      token: payload.token,
      password: payload.password ?? null,
      firstName: payload.firstName ?? null,
      lastName: payload.lastName ?? null
    });
  }

  cancel(invitationId: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${invitationId}`);
  }
}
