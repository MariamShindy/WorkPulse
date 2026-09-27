import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { UsersService } from '../../../core/services/users.service';
import { TenantService } from '../../../core/services/tenant.service';
import { UserCompany } from '../../../core/models';
import { apiErrorMessage } from '../../../core/utils/api-error';
import { EmptyStateComponent } from '../../../shared/empty-state/empty-state.component';

@Component({
  selector: 'app-select-workspace',
  standalone: true,
  imports: [CommonModule, RouterLink, EmptyStateComponent],
  templateUrl: './select-workspace.component.html',
  styleUrl: './select-workspace.component.scss'
})
export class SelectWorkspaceComponent implements OnInit {
  private readonly users = inject(UsersService);
  private readonly tenant = inject(TenantService);
  private readonly router = inject(Router);

  readonly loading = signal(true);
  readonly switchingId = signal<string | null>(null);
  readonly error = signal<string | null>(null);
  readonly companies = signal<UserCompany[]>([]);

  ngOnInit(): void {
    this.loadCompanies();
  }

  loadCompanies(): void {
    this.loading.set(true);
    this.error.set(null);

    this.users.getMyCompanies().subscribe({
      next: (companies) => {
        if (companies.length === 0) {
          this.tenant.clear();
          this.router.navigate(['/onboarding/workspace']);
          return;
        }

        const storedTenantId = this.tenant.tenantId();
        if (storedTenantId) {
          const match = companies.find((c) => c.id === storedTenantId);
          if (match) {
            this.router.navigate(['/dashboard']);
            return;
          }
          this.tenant.clear();
        }

        this.companies.set(companies);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(apiErrorMessage(err, 'Could not load your workspaces.'));
        this.loading.set(false);
      }
    });
  }

  select(company: UserCompany): void {
    if (this.switchingId()) return;

    this.switchingId.set(company.id);
    this.error.set(null);

    this.users.setCurrentCompany(company.id).subscribe({
      next: (selected) => {
        this.tenant.setTenant(selected.id, selected.name, company.role);
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.error.set(apiErrorMessage(err, 'Could not open this workspace.'));
        this.switchingId.set(null);
      }
    });
  }

  initials(name: string): string {
    return name
      .split(/\s+/)
      .filter(Boolean)
      .slice(0, 2)
      .map((part) => part[0]!.toUpperCase())
      .join('');
  }
}
