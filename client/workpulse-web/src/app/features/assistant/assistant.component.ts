import { Component, ElementRef, effect, inject, signal, viewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AiService } from '../../core/services/ai.service';
import { AiChatMessage } from '../../core/models';
import { AssistantMarkdownPipe } from './assistant-markdown.pipe';
import { apiErrorMessage } from '../../core/utils/api-error';

interface ChatEntry {
  role: 'user' | 'assistant';
  content: string;
  toolsUsed?: string[];
  pending?: boolean;
}

interface Suggestion {
  icon: string;
  label: string;
  prompt: string;
}

@Component({
  selector: 'app-assistant',
  standalone: true,
  imports: [CommonModule, FormsModule, AssistantMarkdownPipe],
  templateUrl: './assistant.component.html',
  styleUrl: './assistant.component.scss'
})
export class AssistantComponent {
  private readonly ai = inject(AiService);

  private readonly scrollAnchor = viewChild<ElementRef<HTMLDivElement>>('anchor');

  readonly entries = signal<ChatEntry[]>([]);
  readonly draft = signal('');
  readonly sending = signal(false);
  readonly error = signal<string | null>(null);

  readonly suggestions: Suggestion[] = [
    { icon: '👥', label: 'Workload', prompt: 'Who has the most tasks right now, and is anyone overloaded?' },
    { icon: '📅', label: 'Overdue', prompt: 'Is anyone behind schedule? List the overdue tasks and who owns them.' },
    { icon: '📊', label: 'Monthly KPIs', prompt: 'Give me the KPIs for the last 3 months — created, completed and overdue.' },
    { icon: '🧭', label: 'Overview', prompt: 'Give me a quick overview of the whole workspace.' }
  ];

  constructor() {
    // Auto-scroll to the newest message whenever the transcript changes.
    effect(() => {
      this.entries();
      queueMicrotask(() => this.scrollAnchor()?.nativeElement.scrollIntoView({ behavior: 'smooth' }));
    });
  }

  useSuggestion(s: Suggestion): void {
    this.draft.set(s.prompt);
    this.send();
  }

  send(): void {
    const text = this.draft().trim();
    if (!text || this.sending()) return;

    this.error.set(null);
    this.entries.update((list) => [...list, { role: 'user', content: text }]);
    this.draft.set('');
    this.sending.set(true);

    // Placeholder assistant bubble with typing indicator.
    this.entries.update((list) => [...list, { role: 'assistant', content: '', pending: true }]);

    const history: AiChatMessage[] = this.entries()
      .filter((e) => !e.pending)
      .map((e) => ({ role: e.role, content: e.content }));

    this.ai.chat(history).subscribe({
      next: (res) => {
        this.entries.update((list) => {
          const next = [...list];
          const idx = next.findIndex((e) => e.pending);
          if (idx !== -1) {
            next[idx] = { role: 'assistant', content: res.reply, toolsUsed: res.toolsUsed };
          }
          return next;
        });
        this.sending.set(false);
      },
      error: (err) => {
        this.entries.update((list) => list.filter((e) => !e.pending));
        this.error.set(this.resolveError(err));
        this.sending.set(false);
      }
    });
  }

  onKeydown(event: Event): void {
    const e = event as KeyboardEvent;
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      this.send();
    }
  }

  clear(): void {
    this.entries.set([]);
    this.error.set(null);
  }

  toolLabel(tool: string): string {
    switch (tool) {
      case 'get_member_workload': return 'Member workload';
      case 'get_monthly_kpis': return 'Monthly KPIs';
      case 'get_overdue_tasks': return 'Overdue tasks';
      case 'get_workspace_summary': return 'Workspace summary';
      default: return tool;
    }
  }

  private resolveError(err: { status?: number; error?: { detail?: string; description?: string; title?: string } }): string {
    if (err.status === 401) {
      return 'Your session expired. Please log in again, then retry.';
    }
    if (err.status === 0) {
      return 'Could not reach the API. Check that the server and Ollama are running.';
    }
    return apiErrorMessage(err, 'The assistant could not respond. Please try again.');
  }
}
