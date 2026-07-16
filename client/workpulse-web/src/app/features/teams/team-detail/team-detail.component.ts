import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TeamsService } from '../../../core/services/teams.service';
import { WorkflowsService } from '../../../core/services/workflows.service';
import { CompaniesService } from '../../../core/services/companies.service';
import {
  CompanyMember,
  TEAM_ROLES,
  Team,
  TeamMember,
  WORKFLOW_STATE_TYPES,
  Workflow
} from '../../../core/models';
import { ConfirmDialogComponent } from '../../../shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-team-detail',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, ConfirmDialogComponent],
  templateUrl: './team-detail.component.html',
  styleUrl: './team-detail.component.scss'
})
export class TeamDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly teamsService = inject(TeamsService);
  private readonly workflows = inject(WorkflowsService);
  private readonly companies = inject(CompaniesService);
  private readonly fb = inject(FormBuilder);

  readonly teamRoles = TEAM_ROLES;
  readonly stateTypes = WORKFLOW_STATE_TYPES;

  readonly teamId = signal('');
  readonly team = signal<Team | null>(null);
  readonly members = signal<TeamMember[]>([]);
  readonly companyMembers = signal<CompanyMember[]>([]);
  readonly workflow = signal<Workflow | null>(null);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly saving = signal(false);
  readonly confirmArchive = signal(false);
  readonly editingStateId = signal<string | null>(null);

  readonly orderedStates = computed(() =>
    [...(this.workflow()?.states ?? [])].sort((a, b) => a.position - b.position));

  readonly availableCompanyMembers = computed(() => {
    const memberUserIds = new Set(this.members().map((m) => m.userId));
    return this.companyMembers().filter((cm) => !memberUserIds.has(cm.userId));
  });

  readonly teamForm = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    description: [''],
    color: ['#6366f1']
  });

  readonly memberForm = this.fb.nonNullable.group({
    userId: ['', Validators.required],
    role: ['Member', Validators.required]
  });

  readonly stateForm = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(60)]],
    type: ['Unstarted', Validators.required],
    color: ['#818cf8']
  });

  readonly editStateForm = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(60)]],
    type: ['Unstarted', Validators.required],
    color: ['#818cf8'],
    isDefault: [false]
  });

  ngOnInit(): void {
    this.teamId.set(this.route.snapshot.paramMap.get('id') ?? '');
    this.load();
    this.companies.listMembers(1, 100).subscribe({
      next: (res) => this.companyMembers.set(res.items)
    });
  }

  load(): void {
    const id = this.teamId();
    this.loading.set(true);

    this.teamsService.get(id).subscribe({
      next: (team) => {
        this.team.set(team);
        this.teamForm.patchValue({
          name: team.name,
          description: team.description ?? '',
          color: team.color ?? '#6366f1'
        });
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.description ?? 'Failed to load team.');
        this.loading.set(false);
      }
    });

    this.teamsService.listMembers(id).subscribe({
      next: (res) => this.members.set(res.items)
    });

    this.workflows.get(id).subscribe({
      next: (workflow) => this.workflow.set(workflow)
    });
  }

  saveTeam(): void {
    if (this.teamForm.invalid) {
      this.teamForm.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    const { name, description, color } = this.teamForm.getRawValue();
    this.teamsService.update(this.teamId(), { name, description: description || null, color }).subscribe({
      next: (team) => {
        this.team.set(team);
        this.saving.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.description ?? 'Failed to update team.');
        this.saving.set(false);
      }
    });
  }

  archiveTeam(): void {
    this.confirmArchive.set(false);
    this.teamsService.archive(this.teamId()).subscribe({
      next: () => this.router.navigate(['/teams']),
      error: (err) => this.error.set(err.error?.description ?? 'Failed to archive team.')
    });
  }

  // ── Members ────────────────────────────────────────────────────────────────

  addMember(): void {
    if (this.memberForm.invalid) return;
    const { userId, role } = this.memberForm.getRawValue();
    this.teamsService.addMember(this.teamId(), userId, role).subscribe({
      next: (member) => {
        this.members.update((list) => [...list, member]);
        this.memberForm.reset({ userId: '', role: 'Member' });
      },
      error: (err) => this.error.set(err.error?.description ?? 'Failed to add member.')
    });
  }

  changeMemberRole(member: TeamMember, role: string): void {
    this.teamsService.updateMember(this.teamId(), member.id, role).subscribe({
      next: () =>
        this.members.update((list) =>
          list.map((m) => (m.id === member.id ? { ...m, role } : m))),
      error: (err) => this.error.set(err.error?.description ?? 'Failed to update member.')
    });
  }

  removeMember(member: TeamMember): void {
    this.teamsService.removeMember(this.teamId(), member.id).subscribe({
      next: () => this.members.update((list) => list.filter((m) => m.id !== member.id)),
      error: (err) => this.error.set(err.error?.description ?? 'Failed to remove member.')
    });
  }

  // ── Workflow states ────────────────────────────────────────────────────────

  addState(): void {
    if (this.stateForm.invalid) return;
    const { name, type, color } = this.stateForm.getRawValue();
    const position = this.orderedStates().length;
    this.workflows.createState(this.teamId(), { name, type, color, position }).subscribe({
      next: () => {
        this.stateForm.reset({ name: '', type: 'Unstarted', color: '#818cf8' });
        this.reloadWorkflow();
      },
      error: (err) => this.error.set(err.error?.description ?? 'Failed to create state.')
    });
  }

  startEditState(stateId: string): void {
    const state = this.orderedStates().find((s) => s.id === stateId);
    if (!state) return;
    this.editingStateId.set(stateId);
    this.editStateForm.patchValue({
      name: state.name,
      type: state.type,
      color: state.color,
      isDefault: state.isDefault
    });
  }

  saveState(): void {
    const stateId = this.editingStateId();
    const state = this.orderedStates().find((s) => s.id === stateId);
    if (!stateId || !state || this.editStateForm.invalid) return;

    const { name, type, color, isDefault } = this.editStateForm.getRawValue();
    this.workflows
      .updateState(this.teamId(), stateId, { name, type, color, position: state.position, isDefault })
      .subscribe({
        next: () => {
          this.editingStateId.set(null);
          this.reloadWorkflow();
        },
        error: (err) => this.error.set(err.error?.description ?? 'Failed to update state.')
      });
  }

  deleteState(stateId: string): void {
    this.workflows.deleteState(this.teamId(), stateId).subscribe({
      next: () => this.reloadWorkflow(),
      error: (err) => this.error.set(err.error?.description ?? 'Failed to delete state.')
    });
  }

  moveState(stateId: string, direction: -1 | 1): void {
    const states = this.orderedStates();
    const index = states.findIndex((s) => s.id === stateId);
    const target = index + direction;
    if (index < 0 || target < 0 || target >= states.length) return;

    const ids = states.map((s) => s.id);
    [ids[index], ids[target]] = [ids[target], ids[index]];

    this.workflows.reorder(this.teamId(), ids).subscribe({
      next: () => this.reloadWorkflow(),
      error: (err) => this.error.set(err.error?.description ?? 'Failed to reorder states.')
    });
  }

  private reloadWorkflow(): void {
    this.workflows.get(this.teamId()).subscribe({
      next: (workflow) => this.workflow.set(workflow)
    });
  }
}
