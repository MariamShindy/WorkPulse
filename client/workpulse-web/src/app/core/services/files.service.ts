import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { FILE_ENTITY_TYPES, PagedList, StoredFile, enumValue } from '../models';

@Injectable({ providedIn: 'root' })
export class FilesService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/files`;

  list(entityType: string, entityId: string, page = 1, pageSize = 25): Observable<PagedList<StoredFile>> {
    const params = new HttpParams()
      .set('entityType', entityType)
      .set('entityId', entityId)
      .set('page', page)
      .set('pageSize', pageSize);
    return this.http.get<PagedList<StoredFile>>(this.base, { params });
  }

  getMetadata(fileId: string): Observable<StoredFile> {
    return this.http.get<StoredFile>(`${this.base}/${fileId}`);
  }

  upload(file: File, entityType: string, entityId: string): Observable<StoredFile> {
    const form = new FormData();
    form.append('file', file);
    form.append('entityType', String(enumValue(FILE_ENTITY_TYPES, entityType)));
    form.append('entityId', entityId);
    return this.http.post<StoredFile>(`${this.base}/upload`, form);
  }

  download(fileId: string): Observable<HttpResponse<Blob>> {
    return this.http.get(`${this.base}/${fileId}/download`, {
      responseType: 'blob',
      observe: 'response'
    });
  }

  delete(fileId: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${fileId}`);
  }

  /** Triggers a browser download for a downloaded file blob. */
  saveBlob(response: HttpResponse<Blob>, fallbackName: string): void {
    const disposition = response.headers.get('Content-Disposition') ?? '';
    const match = /filename\*?=(?:UTF-8'')?"?([^";]+)"?/i.exec(disposition);
    const name = match ? decodeURIComponent(match[1]) : fallbackName;

    const url = URL.createObjectURL(response.body!);
    const link = document.createElement('a');
    link.href = url;
    link.download = name;
    link.click();
    URL.revokeObjectURL(url);
  }
}
