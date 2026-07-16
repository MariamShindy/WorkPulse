import { Injectable, inject, signal } from '@angular/core';
import {
  HttpTransportType,
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
  LogLevel
} from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthService } from './auth.service';
import { TenantService } from './tenant.service';

export interface TaskRealtimeEvent {
  type: 'TaskUpdated' | 'TaskMoved';
  payload: unknown;
}

@Injectable({ providedIn: 'root' })
export class RealtimeService {
  private readonly auth = inject(AuthService);
  private readonly tenant = inject(TenantService);

  private connection: HubConnection | null = null;
  private readonly joinedTeams = new Set<string>();
  private readonly joinedTasks = new Set<string>();

  readonly connected = signal(false);
  readonly taskEvents$ = new Subject<TaskRealtimeEvent>();
  readonly onlineUsers = signal<string[]>([]);

  async connect(): Promise<void> {
    if (this.connection || !this.auth.isAuthenticated() || !this.tenant.hasTenant()) {
      return;
    }

    // Long polling is the only browser transport that can carry the custom
    // X-Tenant-Id header the hub uses to resolve the tenant group.
    this.connection = new HubConnectionBuilder()
      .withUrl(environment.hubUrl, {
        accessTokenFactory: () => this.auth.accessToken() ?? '',
        headers: { 'X-Tenant-Id': this.tenant.tenantId() ?? '' },
        transport: HttpTransportType.LongPolling
      })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build();

    this.connection.on('TaskUpdated', (payload) =>
      this.taskEvents$.next({ type: 'TaskUpdated', payload }));
    this.connection.on('TaskMoved', (payload) =>
      this.taskEvents$.next({ type: 'TaskMoved', payload }));
    this.connection.on('UserOnline', (payload: { userId: string }) =>
      this.onlineUsers.update((users) =>
        users.includes(payload.userId) ? users : [...users, payload.userId]));
    this.connection.on('UserOffline', (payload: { userId: string }) =>
      this.onlineUsers.update((users) => users.filter((u) => u !== payload.userId)));

    this.connection.onreconnected(async () => {
      this.connected.set(true);
      for (const teamId of this.joinedTeams) {
        await this.invokeSafe('JoinTeam', teamId);
      }
      for (const taskId of this.joinedTasks) {
        await this.invokeSafe('JoinTask', taskId);
      }
    });
    this.connection.onclose(() => this.connected.set(false));

    try {
      await this.connection.start();
      this.connected.set(true);
    } catch {
      this.connected.set(false);
    }
  }

  async disconnect(): Promise<void> {
    if (!this.connection) return;
    const conn = this.connection;
    this.connection = null;
    this.joinedTeams.clear();
    this.joinedTasks.clear();
    this.connected.set(false);
    await conn.stop().catch(() => undefined);
  }

  async joinTeam(teamId: string): Promise<void> {
    this.joinedTeams.add(teamId);
    await this.invokeSafe('JoinTeam', teamId);
  }

  async leaveTeam(teamId: string): Promise<void> {
    this.joinedTeams.delete(teamId);
    await this.invokeSafe('LeaveTeam', teamId);
  }

  async joinTask(taskId: string): Promise<void> {
    this.joinedTasks.add(taskId);
    await this.invokeSafe('JoinTask', taskId);
  }

  async leaveTask(taskId: string): Promise<void> {
    this.joinedTasks.delete(taskId);
    await this.invokeSafe('LeaveTask', taskId);
  }

  private async invokeSafe(method: string, ...args: unknown[]): Promise<void> {
    if (this.connection?.state !== HubConnectionState.Connected) return;
    await this.connection.invoke(method, ...args).catch(() => undefined);
  }
}
