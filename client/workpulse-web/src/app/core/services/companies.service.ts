import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { COMPANY_ROLES, Company, CompanyMember, PagedList, enumValue } from '../models';

@Injectable({ providedIn: 'root' })
export class CompaniesService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/companies`;

  create(name: string, description?: string, logoUrl?: string): Observable<Company> {
    return this.http.post<Company>(this.base, {
      name,
      description: description ?? null,
      logoUrl: logoUrl ?? null
    });
  }

  getCurrent(): Observable<Company> {
    return this.http.get<Company>(`${this.base}/current`);
  }

  updateCurrent(payload: {
    name: string;
    description?: string | null;
    logoUrl?: string | null;
  }): Observable<Company> {
    return this.http.put<Company>(`${this.base}/current`, {
      name: payload.name,
      description: payload.description ?? null,
      logoUrl: payload.logoUrl ?? null
    });
  }

  listMembers(page = 1, pageSize = 25): Observable<PagedList<CompanyMember>> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<PagedList<CompanyMember>>(`${this.base}/current/members`, { params });
  }

  addMember(userId: string, role: string): Observable<CompanyMember> {
    return this.http.post<CompanyMember>(`${this.base}/current/members`, {
      userId,
      role: enumValue(COMPANY_ROLES, role)
    });
  }

  updateMember(memberId: string, role: string, isActive: boolean): Observable<void> {
    return this.http.put<void>(`${this.base}/current/members/${memberId}`, {
      role: enumValue(COMPANY_ROLES, role),
      isActive
    });
  }
}
