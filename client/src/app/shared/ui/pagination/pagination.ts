import { Component, computed, input, output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

/** '…' marks a gap between page numbers */
export type PageItem = number | '…';

/**
 * Numbered pagination with previous/next. Emits the 1-based page to load;
 * the parent owns the state and reloads its data on (pageChange).
 */
@Component({
  selector: 'app-pagination',
  imports: [MatButtonModule, MatIconModule],
  templateUrl: './pagination.html',
  styleUrl: './pagination.css',
})
export class Pagination {

  /** Window of numbered pages shown around the current one */
  private static readonly SIBLINGS = 1;

  readonly pageIndex = input.required<number>();

  readonly totalPages = input.required<number>();

  readonly pageChange = output<number>();

  readonly pages = computed<PageItem[]>(() => {
    const current = this.pageIndex();
    const total = this.totalPages();

    const wanted = new Set<number>([1, total]);

    for (let page = current - Pagination.SIBLINGS; page <= current + Pagination.SIBLINGS; page++) {
      if (page >= 1 && page <= total) {
        wanted.add(page);
      }
    }

    const sorted = [...wanted].sort((a, b) => a - b);
    const items: PageItem[] = [];

    for (const [index, page] of sorted.entries()) {
      if (index > 0) {
        const previous = sorted[index - 1];
        if (page - previous === 2) {
          items.push(previous + 1);
        } else if (page - previous > 2) {
          items.push('…');
        }
      }
      items.push(page);
    }

    return items;
  });

  select(page: PageItem): void {
    if (typeof page === 'number' && page !== this.pageIndex() && page >= 1 && page <= this.totalPages()) {
      this.pageChange.emit(page);
    }
  }

  previous(): void {
    this.select(this.pageIndex() - 1);
  }

  next(): void {
    this.select(this.pageIndex() + 1);
  }
}
