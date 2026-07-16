import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AnalyticsService } from '../../core/services/analytics.service';
import { TeamsService } from '../../core/services/teams.service';
import { DashboardAnalytics, ProjectProgress, Team, TeamVelocityPoint } from '../../core/models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  private readonly analytics = inject(AnalyticsService);
  private readonly teamsService = inject(TeamsService);

  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly teams = signal<Team[]>([]);
  readonly selectedTeamId = signal<string | null>(null);
  readonly data = signal<DashboardAnalytics | null>(null);
  readonly velocity = signal<TeamVelocityPoint[]>([]);
  readonly projectProgress = signal<ProjectProgress[]>([]);

  readonly maxStatusCount = computed(() =>
    Math.max(1, ...(this.data()?.tasksByStatus.map((s) => s.count) ?? [1])));
  readonly maxPriorityCount = computed(() =>
    Math.max(1, ...(this.data()?.tasksByPriority.map((p) => p.count) ?? [1])));
  readonly maxVelocity = computed(() =>
    Math.max(1, ...this.velocity().flatMap((v) => [v.created, v.completed])));

  ngOnInit(): void {
    this.teamsService.list().subscribe({
      next: (res) => this.teams.set(res.items)
    });
    this.load();
  }

  selectTeam(teamId: string): void {
    this.selectedTeamId.set(teamId || null);
    this.load();
  }

  private load(): void {
    this.loading.set(true);
    this.error.set(null);
    const teamId = this.selectedTeamId() ?? undefined;

    this.analytics.dashboard({ teamId }).subscribe({
      next: (data) => {
        this.data.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.description ?? 'Failed to load analytics.');
        this.loading.set(false);
      }
    });

    this.analytics.velocity(teamId, 8).subscribe({
      next: (points) => this.velocity.set(points)
    });

    this.analytics.projectProgress(teamId).subscribe({
      next: (progress) => this.projectProgress.set(progress)
    });
  }
}
