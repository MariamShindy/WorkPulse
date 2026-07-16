import { Component, computed, input, output } from '@angular/core';
import { PagedList } from '../../core/models';

@Component({
  selector: 'app-paginator',
  standalone: true,
  template: `
    @if (paged(); as p) {
      @if (p.totalCount > 0) {
        <div class="paginator">
          <span class="info">
            {{ rangeStart() }}–{{ rangeEnd() }} of {{ p.totalCount }}
          </span>
          <div class="controls">
            <button
              type="button"
              class="btn btn-ghost btn-sm"
              [disabled]="!p.hasPreviousPage"
              (click)="pageChange.emit(p.page - 1)">
              Previous
            </button>
            <span class="page">Page {{ p.page }} / {{ p.totalPages }}</span>
            <button
              type="button"
              class="btn btn-ghost btn-sm"
              [disabled]="!p.hasNextPage"
              (click)="pageChange.emit(p.page + 1)">
              Next
            </button>
          </div>
        </div>
      }
    }
  `,
  styles: `
    .paginator {
      display: flex;
      align-items: center;
      justify-content: space-between;
      gap: 0.75rem;
      flex-wrap: wrap;
      margin-top: 1rem;
      font-size: 0.85rem;
      color: var(--text-muted);
    }

    .controls {
      display: flex;
      align-items: center;
      gap: 0.6rem;
    }
  `
})
export class PaginatorComponent {
  readonly paged = input.required<PagedList<unknown> | null>();
  readonly pageChange = output<number>();

  readonly rangeStart = computed(() => {
    const p = this.paged();
    return p ? (p.page - 1) * p.pageSize + 1 : 0;
  });

  readonly rangeEnd = computed(() => {
    const p = this.paged();
    return p ? Math.min(p.page * p.pageSize, p.totalCount) : 0;
  });
}
