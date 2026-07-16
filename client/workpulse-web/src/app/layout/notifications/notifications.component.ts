import { Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { CollaborationService } from '../../core/services/collaboration.service';
import { Notification, PagedList } from '../../core/models';

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './notifications.component.html',
  styleUrl: './notifications.component.scss'
})
export class NotificationsComponent implements OnInit {
  private readonly collaboration = inject(CollaborationService);

  readonly open = signal(false);
  readonly loading = signal(false);
  readonly paged = signal<PagedList<Notification> | null>(null);
  readonly unreadCount = signal(0);

  ngOnInit(): void {
    this.refreshUnreadCount();
  }

  toggle(): void {
    this.open.update((v) => !v);
    if (this.open()) {
      this.load();
    }
  }

  close(): void {
    this.open.set(false);
  }

  load(page = 1): void {
    this.loading.set(true);
    this.collaboration.listNotifications(false, page, 15).subscribe({
      next: (res) => {
        this.paged.set(res);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  markRead(notification: Notification): void {
    if (notification.isRead) return;
    this.collaboration.markNotificationRead(notification.id).subscribe({
      next: () => {
        this.paged.update((p) =>
          p
            ? { ...p, items: p.items.map((n) => (n.id === notification.id ? { ...n, isRead: true } : n)) }
            : p
        );
        this.unreadCount.update((c) => Math.max(0, c - 1));
      }
    });
  }

  markAllRead(): void {
    this.collaboration.markAllNotificationsRead().subscribe({
      next: () => {
        this.paged.update((p) =>
          p ? { ...p, items: p.items.map((n) => ({ ...n, isRead: true })) } : p
        );
        this.unreadCount.set(0);
      }
    });
  }

  private refreshUnreadCount(): void {
    this.collaboration.listNotifications(true, 1, 1).subscribe({
      next: (res) => this.unreadCount.set(res.totalCount)
    });
  }
}
