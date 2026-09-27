import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { InvitationsService } from '../../../core/services/invitations.service';
import { AuthService } from '../../../core/services/auth.service';
import { TenantService } from '../../../core/services/tenant.service';
import { AuthResponse } from '../../../core/models';
import { apiErrorMessage } from '../../../core/utils/api-error';

@Component({
  selector: 'app-accept-invite',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './accept-invite.component.html',
  styleUrl: './accept-invite.component.scss'
})
export class AcceptInviteComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly invitations = inject(InvitationsService);
  private readonly auth = inject(AuthService);
  private readonly tenant = inject(TenantService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  readonly token = signal('');
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    firstName: ['', [Validators.required, Validators.maxLength(100)]],
    lastName: ['', [Validators.required, Validators.maxLength(100)]],
    password: ['', [Validators.required, Validators.minLength(8)]]
  });

  ngOnInit(): void {
    this.token.set(this.route.snapshot.queryParamMap.get('token') ?? '');
    if (!this.token()) {
      this.error.set('This invitation link is missing its token. Ask for a new invite.');
    }

    // Existing signed-in users can accept directly without new credentials.
    if (this.auth.isAuthenticated() && this.token()) {
      this.acceptAsExistingUser();
    }
  }

  submit(): void {
    if (this.form.invalid || !this.token()) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.error.set(null);
    const { firstName, lastName, password } = this.form.getRawValue();

    this.invitations
      .accept({ token: this.token(), password, firstName, lastName })
      .subscribe({
        next: (response) => this.finishAccept(response),
        error: (err) => {
          this.error.set(apiErrorMessage(err, 'Could not accept the invitation.'));
          this.loading.set(false);
        }
      });
  }

  private acceptAsExistingUser(): void {
    this.loading.set(true);
    this.invitations.accept({ token: this.token() }).subscribe({
      next: (response) => this.finishAccept(response),
      error: (err) => {
        this.error.set(apiErrorMessage(err, 'Could not accept the invitation.'));
        this.loading.set(false);
      }
    });
  }

  private finishAccept(response: AuthResponse): void {
    this.auth.applyExternalSession(response);
    if (response.user.currentTenantId) {
      this.tenant.setTenant(response.user.currentTenantId);
    }
    this.router.navigate(['/dashboard']);
  }
}
