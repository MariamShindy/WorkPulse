import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { TeamsService } from '../../../core/services/teams.service';
import { Team } from '../../../core/models';
import { EmptyStateComponent } from '../../../shared/empty-state/empty-state.component';

@Component({
  selector: 'app-teams-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, EmptyStateComponent],
  templateUrl: './teams-list.component.html',
  styleUrl: './teams-list.component.scss'
})
export class TeamsListComponent {
  private readonly teamsService = inject(TeamsService);
  private readonly fb = inject(FormBuilder);

  readonly teams = signal<Team[]>([]);
  readonly loading = signal(true);
  readonly creating = signal(false);
  readonly error = signal<string | null>(null);
  readonly includeArchived = signal(false);

  readonly form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    key: [''],
    color: ['#6366f1']
  });

  constructor() {
    this.loadTeams();
  }

  loadTeams(): void {
    this.loading.set(true);
    this.teamsService.list(1, 100, this.includeArchived()).subscribe({
      next: (res) => this.teams.set(res.items),
      complete: () => this.loading.set(false),
      error: () => this.loading.set(false)
    });
  }

  toggleArchived(checked: boolean): void {
    this.includeArchived.set(checked);
    this.loadTeams();
  }

  createTeam(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.creating.set(true);
    this.error.set(null);
    const { name, key, color } = this.form.getRawValue();

    this.teamsService.create({ name, key: key || undefined, color }).subscribe({
      next: (team) => {
        this.teams.update((list) => [team, ...list]);
        this.form.reset({ name: '', key: '', color: '#6366f1' });
      },
      error: (err) => {
        this.error.set(err.error?.description ?? 'Failed to create team.');
        this.creating.set(false);
      },
      complete: () => this.creating.set(false)
    });
  }
}
