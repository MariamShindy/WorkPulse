import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  AUTOMATION_ACTIONS,
  AUTOMATION_TRIGGERS,
  AutomationRule,
  PagedList,
  enumValue
} from '../models';

export interface AutomationRulePayload {
  name: string;
  triggerType: string;
  triggerConfigJson: string;
  actionType: string;
  actionConfigJson: string;
  isEnabled: boolean;
}

@Injectable({ providedIn: 'root' })
export class AutomationService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/automation-rules`;

  list(page = 1, pageSize = 50): Observable<PagedList<AutomationRule>> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<PagedList<AutomationRule>>(this.base, { params });
  }

  get(ruleId: string): Observable<AutomationRule> {
    return this.http.get<AutomationRule>(`${this.base}/${ruleId}`);
  }

  create(payload: AutomationRulePayload): Observable<AutomationRule> {
    return this.http.post<AutomationRule>(this.base, this.toBody(payload));
  }

  update(ruleId: string, payload: AutomationRulePayload): Observable<AutomationRule> {
    return this.http.put<AutomationRule>(`${this.base}/${ruleId}`, this.toBody(payload));
  }

  delete(ruleId: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${ruleId}`);
  }

  private toBody(payload: AutomationRulePayload) {
    return {
      name: payload.name,
      triggerType: enumValue(AUTOMATION_TRIGGERS, payload.triggerType),
      triggerConfigJson: payload.triggerConfigJson,
      actionType: enumValue(AUTOMATION_ACTIONS, payload.actionType),
      actionConfigJson: payload.actionConfigJson,
      isEnabled: payload.isEnabled
    };
  }
}
