import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { UsersService } from '../../core/services/users.service';
import { CompaniesService } from '../../core/services/companies.service';
import { InvitationsService } from '../../core/services/invitations.service';
import { TenantService } from '../../core/services/tenant.service';
import {
  COMPANY_ROLES,
  Company,
  CompanyMember,
  Invitation,
  PagedList
} from '../../core/models';
import { PaginatorComponent } from '../../shared/paginator/paginator.component';
import { apiErrorMessage } from '../../core/utils/api-error';

type SettingsTab = 'profile' | 'company' | 'members';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, PaginatorComponent],
  templateUrl: './settings.component.html',
  styleUrl: './settings.component.scss'
})
export class SettingsComponent implements OnInit {
  private readonly users = inject(UsersService);
  private readonly companies = inject(CompaniesService);
  private readonly invitations = inject(InvitationsService);
  readonly tenant = inject(TenantService);
  private readonly fb = inject(FormBuilder);

  readonly roles = COMPANY_ROLES;

  readonly tab = signal<SettingsTab>('profile');
  readonly error = signal<string | null>(null);
  readonly success = signal<string | null>(null);
  readonly savingProfile = signal(false);
  readonly savingCompany = signal(false);
  readonly inviting = signal(false);

  readonly company = signal<Company | null>(null);
  readonly members = signal<PagedList<CompanyMember> | null>(null);
  readonly invitationList = signal<Invitation[]>([]);
  readonly invitationsError = signal<string | null>(null);

  readonly profileForm = this.fb.nonNullable.group({
    firstName: ['', [Validators.required, Validators.maxLength(100)]],
    lastName: ['', [Validators.required, Validators.maxLength(100)]],
    avatarUrl: ['']
  });

  readonly companyForm = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(200)]],
    description: [''],
    logoUrl: ['']
  });

  readonly inviteForm = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    role: ['Member', Validators.required]
  });

  ngOnInit(): void {
    this.users.getProfile().subscribe({
      next: (profile) =>
        this.profileForm.patchValue({
          firstName: profile.firstName,
          lastName: profile.lastName,
          avatarUrl: profile.avatarUrl ?? ''
        })
    });

    // The workspace, member and invitation data all feed admin-only panels — and
    // GET /api/invitations is admin-gated, so a member requesting it just logs a 403.
    if (!this.tenant.isAdmin()) {
      return;
    }

    this.companies.getCurrent().subscribe({
      next: (company) => {
        this.company.set(company);
        this.companyForm.patchValue({
          name: company.name,
          description: company.description ?? '',
          logoUrl: company.logoUrl ?? ''
        });
      }
    });

    this.loadMembers();
    this.loadInvitations();
  }

  selectTab(tab: SettingsTab): void {
    // The workspace and member panels are admin-only; ignore a request for them otherwise.
    if (tab !== 'profile' && !this.tenant.isAdmin()) {
      this.tab.set('profile');
      return;
    }

    this.tab.set(tab);
    this.error.set(null);
    this.success.set(null);
  }

  saveProfile(): void {
    if (this.profileForm.invalid) {
      this.profileForm.markAllAsTouched();
      return;
    }
    this.savingProfile.set(true);
    this.clearMessages();
    const { firstName, lastName, avatarUrl } = this.profileForm.getRawValue();
    this.users.updateProfile({ firstName, lastName, avatarUrl: avatarUrl || null }).subscribe({
      next: () => {
        this.success.set('Profile updated.');
        this.savingProfile.set(false);
      },
      error: (err) => {
        this.error.set(apiErrorMessage(err, 'Failed to update profile.'));
        this.savingProfile.set(false);
      }
    });
  }

  saveCompany(): void {
    if (this.companyForm.invalid) {
      this.companyForm.markAllAsTouched();
      return;
    }
    this.savingCompany.set(true);
    this.clearMessages();
    const { name, description, logoUrl } = this.companyForm.getRawValue();
    this.companies
      .updateCurrent({ name, description: description || null, logoUrl: logoUrl || null })
      .subscribe({
        next: (company) => {
          this.company.set(company);
          this.tenant.setTenant(company.id, company.name);
          this.success.set('Workspace updated.');
          this.savingCompany.set(false);
        },
        error: (err) => {
          this.error.set(apiErrorMessage(err, 'Failed to update workspace.'));
          this.savingCompany.set(false);
        }
      });
  }

  loadMembers(page = 1): void {
    this.companies.listMembers(page, 25).subscribe({
      next: (res) => this.members.set(res)
    });
  }

  updateMember(member: CompanyMember, role: string): void {
    this.clearMessages();
    this.companies.updateMember(member.id, role, member.isActive).subscribe({
      next: () =>
        this.members.update((p) =>
          p ? { ...p, items: p.items.map((m) => (m.id === member.id ? { ...m, role } : m)) } : p),
      error: (err) => this.error.set(apiErrorMessage(err, 'Failed to update member.'))
    });
  }

  toggleMemberActive(member: CompanyMember): void {
    this.clearMessages();
    this.companies.updateMember(member.id, member.role, !member.isActive).subscribe({
      next: () =>
        this.members.update((p) =>
          p
            ? {
                ...p,
                items: p.items.map((m) =>
                  m.id === member.id ? { ...m, isActive: !member.isActive } : m)
              }
            : p),
      error: (err) => this.error.set(apiErrorMessage(err, 'Failed to update member.'))
    });
  }

  loadInvitations(): void {
    this.invitations.list().subscribe({
      next: (list) => this.invitationList.set(list),
      error: () =>
        this.invitationsError.set('Invitations are only visible to company admins.')
    });
  }

  sendInvite(): void {
    if (this.inviteForm.invalid) {
      this.inviteForm.markAllAsTouched();
      return;
    }
    this.inviting.set(true);
    this.clearMessages();
    const { email, role } = this.inviteForm.getRawValue();
    this.invitations.invite(email, role).subscribe({
      next: (invitation) => {
        this.invitationList.update((list) => [invitation, ...list]);
        this.inviteForm.reset({ email: '', role: 'Member' });
        this.success.set(`Invitation sent to ${invitation.email}.`);
        this.inviting.set(false);
      },
      error: (err) => {
        this.error.set(apiErrorMessage(err, 'Failed to send invitation.'));
        this.inviting.set(false);
      }
    });
  }

  cancelInvite(invitation: Invitation): void {
    this.invitations.cancel(invitation.id).subscribe({
      next: () =>
        this.invitationList.update((list) => list.filter((i) => i.id !== invitation.id)),
      error: (err) => this.error.set(apiErrorMessage(err, 'Failed to cancel invitation.'))
    });
  }

  private clearMessages(): void {
    this.error.set(null);
    this.success.set(null);
  }
}
