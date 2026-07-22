import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AiChatMessage, AiChatResponse } from '../models';

@Injectable({ providedIn: 'root' })
export class AiService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/ai`;

  chat(messages: AiChatMessage[]): Observable<AiChatResponse> {
    return this.http.post<AiChatResponse>(`${this.base}/chat`, { messages });
  }
}
