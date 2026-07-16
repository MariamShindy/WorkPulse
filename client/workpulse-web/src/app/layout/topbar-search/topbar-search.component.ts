import { Component, ElementRef, inject, signal, viewChild } from '@angular/core';
import { Router } from '@angular/router';
import { Subject, debounceTime, distinctUntilChanged, filter, switchMap } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { SearchService } from '../../core/services/search.service';
import { SearchResult } from '../../core/models';

@Component({
  selector: 'app-topbar-search',
  standalone: true,
  templateUrl: './topbar-search.component.html',
  styleUrl: './topbar-search.component.scss'
})
export class TopbarSearchComponent {
  private readonly search = inject(SearchService);
  private readonly router = inject(Router);
  private readonly query$ = new Subject<string>();

  readonly input = viewChild<ElementRef<HTMLInputElement>>('searchInput');
  readonly results = signal<SearchResult[]>([]);
  readonly open = signal(false);
  readonly searching = signal(false);

  constructor() {
    this.query$
      .pipe(
        debounceTime(250),
        distinctUntilChanged(),
        filter((q) => q.length >= 2),
        switchMap((q) => {
          this.searching.set(true);
          return this.search.search(q, 'All', 15);
        }),
        takeUntilDestroyed()
      )
      .subscribe({
        next: (results) => {
          this.results.set(results);
          this.open.set(true);
          this.searching.set(false);
        },
        error: () => this.searching.set(false)
      });
  }

  onInput(value: string): void {
    if (value.trim().length < 2) {
      this.open.set(false);
      this.results.set([]);
      return;
    }
    this.query$.next(value.trim());
  }

  select(result: SearchResult): void {
    this.close();
    const inputEl = this.input()?.nativeElement;
    if (inputEl) inputEl.value = '';

    switch (result.entityType.toLowerCase()) {
      case 'task':
      case 'comment':
        this.router.navigate(['/board'], {
          queryParams: { teamId: result.teamId ?? undefined, taskId: result.id }
        });
        break;
      case 'project':
        this.router.navigate(['/projects']);
        break;
      case 'team':
        this.router.navigate(['/teams', result.id]);
        break;
      default:
        this.router.navigate(['/dashboard']);
    }
  }

  close(): void {
    this.open.set(false);
  }

  typeIcon(entityType: string): string {
    switch (entityType.toLowerCase()) {
      case 'task':
        return '☑';
      case 'project':
        return '▣';
      case 'team':
        return '◎';
      case 'comment':
        return '❝';
      default:
        return '◇';
    }
  }
}
