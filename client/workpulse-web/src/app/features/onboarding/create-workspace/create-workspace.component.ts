import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CompaniesService } from '../../../core/services/companies.service';
import { TenantService } from '../../../core/services/tenant.service';
import { apiErrorMessage } from '../../../core/utils/api-error';

@Component({
  selector: 'app-create-workspace',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './create-workspace.component.html',
  styleUrl: './create-workspace.component.scss'
})
export class CreateWorkspaceComponent {
  private readonly fb = inject(FormBuilder);
  private readonly companies = inject(CompaniesService);
  private readonly tenant = inject(TenantService);
  private readonly router = inject(Router);

  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(200)]],
    description: ['']
  });

  submit(): void {
    if (this.form.invalid) return;

    this.loading.set(true);
    this.error.set(null);
    const { name, description } = this.form.getRawValue();

    this.companies.create(name, description || undefined).subscribe({
      next: (company) => {
        // The creator of a workspace is its Owner.
        this.tenant.setTenant(company.id, company.name, 'Owner');
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.error.set(apiErrorMessage(err, 'Could not create workspace.'));
        this.loading.set(false);
      },
      complete: () => this.loading.set(false)
    });
  }
}
