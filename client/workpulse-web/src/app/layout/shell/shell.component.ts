import { Component, OnDestroy, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { TenantService } from '../../core/services/tenant.service';
import { ThemeService } from '../../core/services/theme.service';
import { RealtimeService } from '../../core/services/realtime.service';
import { TopbarSearchComponent } from '../topbar-search/topbar-search.component';
import { NotificationsComponent } from '../notifications/notifications.component';

interface NavItem {
  label: string;
  path: string;
  icon: NavIcon;
}

type NavIcon =
  | 'dashboard'
  | 'teams'
  | 'projects'
  | 'board'
  | 'epics'
  | 'sprints'
  | 'labels'
  | 'assistant'
  | 'reports'
  | 'files'
  | 'automation'
  | 'audit'
  | 'settings';

interface NavSection {
  title: string;
  items: NavItem[];
  /** Rendered only for Admin/Owner members. The API enforces this independently. */
  adminOnly?: boolean;
}

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive, TopbarSearchComponent, NotificationsComponent],
  templateUrl: './shell.component.html',
  styleUrl: './shell.component.scss'
})
export class ShellComponent implements OnInit, OnDestroy {
  readonly auth = inject(AuthService);
  readonly tenant = inject(TenantService);
  readonly theme = inject(ThemeService);
  private readonly realtime = inject(RealtimeService);

  readonly navOpen = signal(false);

  private readonly allNavSections: NavSection[] = [
    {
      title: 'Workspace',
      items: [
        { label: 'Dashboard', path: '/dashboard', icon: 'dashboard' },
        { label: 'Teams', path: '/teams', icon: 'teams' },
        { label: 'Projects', path: '/projects', icon: 'projects' },
        { label: 'Board', path: '/board', icon: 'board' }
      ]
    },
    {
      title: 'Planning',
      items: [
        { label: 'Epics', path: '/epics', icon: 'epics' },
        { label: 'Sprints', path: '/sprints', icon: 'sprints' },
        { label: 'Labels', path: '/labels', icon: 'labels' }
      ]
    },
    {
      title: 'Insights',
      items: [
        { label: 'AI Assistant', path: '/assistant', icon: 'assistant' },
        { label: 'Reports', path: '/reports', icon: 'reports' },
        { label: 'Files', path: '/files', icon: 'files' }
      ]
    },
    {
      title: 'Admin',
      adminOnly: true,
      items: [
        { label: 'Automation', path: '/automation', icon: 'automation' },
        { label: 'Audit logs', path: '/audit-logs', icon: 'audit' }
      ]
    },
    {
      // Settings stays open to everyone: it holds the user's own profile. The
      // workspace and member panels inside it are gated on the role instead.
      title: 'Account',
      items: [{ label: 'Settings', path: '/settings', icon: 'settings' }]
    }
  ];

  /**
   * Hides admin-only sections from members and guests. Previously every user saw Automation,
   * Audit logs and Settings and got a 403 on click.
   */
  readonly navSections = computed(() =>
    this.allNavSections.filter((section) => !section.adminOnly || this.tenant.isAdmin())
  );

  ngOnInit(): void {
    this.realtime.connect();
  }

  ngOnDestroy(): void {
    this.realtime.disconnect();
  }

  toggleNav(): void {
    this.navOpen.update((v) => !v);
  }

  closeNav(): void {
    this.navOpen.set(false);
  }

  toggleTheme(): void {
    this.theme.toggle();
  }

  logout(): void {
    this.auth.logout();
  }
}
