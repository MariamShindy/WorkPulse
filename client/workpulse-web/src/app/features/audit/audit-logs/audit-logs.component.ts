import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuditLogsService } from '../../../core/services/audit-logs.service';
import { CompaniesService } from '../../../core/services/companies.service';
import { AuditLogEntry, CompanyMember, PagedList } from '../../../core/models';
import { PaginatorComponent } from '../../../shared/paginator/paginator.component';
import { EmptyStateComponent } from '../../../shared/empty-state/empty-state.component';
import { apiErrorMessage } from '../../../core/utils/api-error';

@Component({
  selector: 'app-audit-logs',
  standalone: true,
  imports: [CommonModule, PaginatorComponent, EmptyStateComponent],
  templateUrl: './audit-logs.component.html',
  styleUrl: './audit-logs.component.scss'
})
export class AuditLogsComponent implements OnInit {
  private readonly auditLogs = inject(AuditLogsService);
  private readonly companies = inject(CompaniesService);

  readonly paged = signal<PagedList<AuditLogEntry> | null>(null);
  readonly members = signal<CompanyMember[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly filterEntityType = signal('');
  readonly filterUserId = signal('');
  readonly expandedId = signal<string | null>(null);

  readonly entityTypes = ['Task', 'Project', 'Team', 'Company', 'Sprint', 'Epic', 'Label', 'Workflow'];

  ngOnInit(): void {
    this.companies.listMembers(1, 100).subscribe({
      next: (res) => this.members.set(res.items)
    });
    this.load();
  }

  setFilter(kind: 'entityType' | 'userId', value: string): void {
    if (kind === 'entityType') this.filterEntityType.set(value);
    if (kind === 'userId') this.filterUserId.set(value);
    this.load();
  }

  load(page = 1): void {
    this.loading.set(true);
    this.auditLogs
      .list({
        entityType: this.filterEntityType() || undefined,
        userId: this.filterUserId() || undefined,
        page,
        pageSize: 25
      })
      .subscribe({
        next: (res) => {
          this.paged.set(res);
          this.loading.set(false);
        },
        error: (err) => {
          this.error.set(apiErrorMessage(err, 'Failed to load audit logs.'));
          this.loading.set(false);
        }
      });
  }

  toggleChanges(id: string): void {
    this.expandedId.update((current) => (current === id ? null : id));
  }

  userName(userId?: string | null): string {
    if (!userId) return 'System';
    return this.members().find((m) => m.userId === userId)?.fullName ?? userId.slice(0, 8);
  }

  prettyJson(json?: string | null): string {
    if (!json) return '';
    try {
      return JSON.stringify(JSON.parse(json), null, 2);
    } catch {
      return json;
    }
  }
}
