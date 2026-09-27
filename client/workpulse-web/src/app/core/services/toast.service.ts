import { Injectable, computed, signal } from '@angular/core';

export type ToastKind = 'success' | 'error' | 'info';

export interface Toast {
  id: number;
  kind: ToastKind;
  message: string;
}

/** Errors and successes previously had nowhere to surface; this is the one place they land. */
@Injectable({ providedIn: 'root' })
export class ToastService {
  private static readonly DefaultDurationMs = 5_000;
  private static readonly ErrorDurationMs = 8_000;

  private nextId = 1;
  private readonly _toasts = signal<Toast[]>([]);
  private readonly timers = new Map<number, ReturnType<typeof setTimeout>>();

  readonly toasts = computed(() => this._toasts());

  success(message: string, durationMs = ToastService.DefaultDurationMs): number {
    return this.show('success', message, durationMs);
  }

  /** Errors linger longer — they usually carry something the user needs to read. */
  error(message: string, durationMs = ToastService.ErrorDurationMs): number {
    return this.show('error', message, durationMs);
  }

  info(message: string, durationMs = ToastService.DefaultDurationMs): number {
    return this.show('info', message, durationMs);
  }

  show(kind: ToastKind, message: string, durationMs = ToastService.DefaultDurationMs): number {
    const id = this.nextId++;
    this._toasts.update((list) => [...list, { id, kind, message }]);

    if (durationMs > 0) {
      this.timers.set(
        id,
        setTimeout(() => this.dismiss(id), durationMs)
      );
    }

    return id;
  }

  dismiss(id: number): void {
    const timer = this.timers.get(id);
    if (timer !== undefined) {
      clearTimeout(timer);
      this.timers.delete(id);
    }

    this._toasts.update((list) => list.filter((t) => t.id !== id));
  }

  clear(): void {
    for (const timer of this.timers.values()) {
      clearTimeout(timer);
    }
    this.timers.clear();
    this._toasts.set([]);
  }
}
