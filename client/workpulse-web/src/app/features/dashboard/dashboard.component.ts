import { Component, DestroyRef, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Subject, forkJoin, of } from 'rxjs';
import { catchError, finalize, switchMap, tap } from 'rxjs/operators';
import { AnalyticsService } from '../../core/services/analytics.service';
import { TeamsService } from '../../core/services/teams.service';
import { DashboardAnalytics, ProjectProgress, Team, TeamVelocityPoint } from '../../core/models';
import { apiErrorMessage } from '../../core/utils/api-error';
import { EmptyStateComponent } from '../../shared/empty-state/empty-state.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, EmptyStateComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  private readonly analytics = inject(AnalyticsService);
  private readonly teamsService = inject(TeamsService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly reload$ = new Subject<string | undefined>();

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

  constructor() {
    this.reload$
      .pipe(
        tap(() => {
          this.loading.set(true);
          this.error.set(null);
        }),
        switchMap((teamId) =>
          forkJoin({
            dashboard: this.analytics.dashboard({ teamId }),
            velocity: this.analytics.velocity(teamId, 8),
            projectProgress: this.analytics.projectProgress(teamId)
          }).pipe(
            catchError((err) => {
              this.error.set(apiErrorMessage(err, 'Failed to load analytics.'));
              return of(null);
            }),
            finalize(() => this.loading.set(false))
          )
        ),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe((result) => {
        if (!result) return;
        this.data.set(result.dashboard);
        this.velocity.set(result.velocity);
        this.projectProgress.set(result.projectProgress);
      });
  }

  ngOnInit(): void {
    this.teamsService
      .list()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (res) => this.teams.set(res.items)
      });
    this.reload$.next(undefined);
  }

  selectTeam(teamId: string): void {
    this.selectedTeamId.set(teamId || null);
    this.reload$.next(this.selectedTeamId() ?? undefined);
  }
}
