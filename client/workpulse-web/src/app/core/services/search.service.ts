import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { SearchResult } from '../models';

@Injectable({ providedIn: 'root' })
export class SearchService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/search`;

  search(q: string, scope = 'All', limit = 25): Observable<SearchResult[]> {
    const params = new HttpParams().set('q', q).set('scope', scope).set('limit', limit);
    return this.http.get<SearchResult[]>(this.base, { params });
  }
}
