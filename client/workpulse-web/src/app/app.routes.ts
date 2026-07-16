import { Routes } from '@angular/router';
import { authGuard, guestGuard } from './core/guards/auth.guard';
import { tenantGuard } from './core/guards/tenant.guard';
import { AuthLayoutComponent } from './layout/auth-layout/auth-layout.component';
import { ShellComponent } from './layout/shell/shell.component';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
  {
    path: '',
    component: AuthLayoutComponent,
    children: [
      {
        path: 'auth',
        canActivate: [guestGuard],
        children: [
          {
            path: 'login',
            loadComponent: () =>
              import('./features/auth/login/login.component').then((m) => m.LoginComponent)
          },
          {
            path: 'register',
            loadComponent: () =>
              import('./features/auth/register/register.component').then((m) => m.RegisterComponent)
          },
          { path: '', pathMatch: 'full', redirectTo: 'login' }
        ]
      },
      {
        path: 'invite/accept',
        loadComponent: () =>
          import('./features/auth/accept-invite/accept-invite.component').then(
            (m) => m.AcceptInviteComponent
          )
      },
      {
        path: 'onboarding/workspace',
        canActivate: [authGuard],
        loadComponent: () =>
          import('./features/onboarding/create-workspace/create-workspace.component').then(
            (m) => m.CreateWorkspaceComponent
          )
      },
      {
        path: 'onboarding/select-workspace',
        canActivate: [authGuard],
        loadComponent: () =>
          import('./features/onboarding/select-workspace/select-workspace.component').then(
            (m) => m.SelectWorkspaceComponent
          )
      }
    ]
  },
  {
    path: '',
    component: ShellComponent,
    canActivate: [authGuard, tenantGuard],
    children: [
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/dashboard/dashboard.component').then((m) => m.DashboardComponent)
      },
      {
        path: 'teams',
        loadComponent: () =>
          import('./features/teams/teams-list/teams-list.component').then((m) => m.TeamsListComponent)
      },
      {
        path: 'teams/:id',
        loadComponent: () =>
          import('./features/teams/team-detail/team-detail.component').then(
            (m) => m.TeamDetailComponent
          )
      },
      {
        path: 'projects',
        loadComponent: () =>
          import('./features/projects/projects-list/projects-list.component').then(
            (m) => m.ProjectsListComponent
          )
      },
      {
        path: 'board',
        loadComponent: () =>
          import('./features/board/kanban-board/kanban-board.component').then(
            (m) => m.KanbanBoardComponent
          )
      },
      {
        path: 'epics',
        loadComponent: () =>
          import('./features/epics/epics-list/epics-list.component').then((m) => m.EpicsListComponent)
      },
      {
        path: 'sprints',
        loadComponent: () =>
          import('./features/sprints/sprints-list/sprints-list.component').then(
            (m) => m.SprintsListComponent
          )
      },
      {
        path: 'labels',
        loadComponent: () =>
          import('./features/labels/labels-list/labels-list.component').then(
            (m) => m.LabelsListComponent
          )
      },
      {
        path: 'reports',
        loadComponent: () =>
          import('./features/reports/reports.component').then((m) => m.ReportsComponent)
      },
      {
        path: 'files',
        loadComponent: () =>
          import('./features/files/files.component').then((m) => m.FilesComponent)
      },
      {
        path: 'automation',
        loadComponent: () =>
          import('./features/automation/automation-rules/automation-rules.component').then(
            (m) => m.AutomationRulesComponent
          )
      },
      {
        path: 'audit-logs',
        loadComponent: () =>
          import('./features/audit/audit-logs/audit-logs.component').then(
            (m) => m.AuditLogsComponent
          )
      },
      {
        path: 'settings',
        loadComponent: () =>
          import('./features/settings/settings.component').then((m) => m.SettingsComponent)
      }
    ]
  },
  { path: '**', redirectTo: 'dashboard' }
];
