import { Component, OnDestroy, OnInit, inject, signal } from '@angular/core';
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
  icon: string;
}

interface NavSection {
  title: string;
  items: NavItem[];
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

  readonly navSections: NavSection[] = [
    {
      title: 'Workspace',
      items: [
        { label: 'Dashboard', path: '/dashboard', icon: '▤' },
        { label: 'Teams', path: '/teams', icon: '◎' },
        { label: 'Projects', path: '/projects', icon: '▣' },
        { label: 'Board', path: '/board', icon: '▦' }
      ]
    },
    {
      title: 'Planning',
      items: [
        { label: 'Epics', path: '/epics', icon: '◆' },
        { label: 'Sprints', path: '/sprints', icon: '↻' },
        { label: 'Labels', path: '/labels', icon: '#' }
      ]
    },
    {
      title: 'Insights',
      items: [
        { label: 'Reports', path: '/reports', icon: '▥' },
        { label: 'Files', path: '/files', icon: '⎘' }
      ]
    },
    {
      title: 'Admin',
      items: [
        { label: 'Automation', path: '/automation', icon: '⚙' },
        { label: 'Audit logs', path: '/audit-logs', icon: '≡' },
        { label: 'Settings', path: '/settings', icon: '✎' }
      ]
    }
  ];

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
