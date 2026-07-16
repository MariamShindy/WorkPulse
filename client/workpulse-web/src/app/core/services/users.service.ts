import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Company, UserCompany, UserProfile } from '../models';

@Injectable({ providedIn: 'root' })
export class UsersService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/users`;

  getProfile(): Observable<UserProfile> {
    return this.http.get<UserProfile>(`${this.base}/profile`);
  }

  updateProfile(payload: {
    firstName: string;
    lastName: string;
    avatarUrl?: string | null;
  }): Observable<UserProfile> {
    return this.http.put<UserProfile>(`${this.base}/profile`, {
      firstName: payload.firstName,
      lastName: payload.lastName,
      avatarUrl: payload.avatarUrl ?? null
    });
  }

  /** Companies the current user is a member of (works without a tenant header). */
  getMyCompanies(): Observable<UserCompany[]> {
    return this.http.get<UserCompany[]>(`${this.base}/me/companies`);
  }

  /** Switches the user's current workspace and returns the selected company. */
  setCurrentCompany(companyId: string): Observable<Company> {
    return this.http.post<Company>(`${this.base}/me/current-company`, { companyId });
  }
}
