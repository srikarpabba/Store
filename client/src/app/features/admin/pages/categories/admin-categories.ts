import { Component, computed, inject, signal } from '@angular/core';
import { CdkDrag, CdkDragDrop, CdkDragHandle, CdkDropList, moveItemInArray } from '@angular/cdk/drag-drop';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { filter, switchMap } from 'rxjs';
import { AdminCategoryService, CategoryGenderFilter } from '../../services/admin-category.service';
import { Category } from '../../../shop/models/category';
import { Lookup } from '../../../shop/models/lookup';
import { ProductService } from '../../../shop/services/product.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { ConfirmDialogService } from '../../../../shared/ui/confirm-dialog/confirm-dialog.service';
import { Pagination } from '../../../../shared/ui/pagination/pagination';

interface GenderTab {
  label: string;
  value: CategoryGenderFilter | null;
}

/** Tab label → the gender name the backend knows */
const GENDER_NAMES: Record<CategoryGenderFilter, string> = {
  Men: 'Male',
  Women: 'Female',
  Unisex: 'Unisex'
};

@Component({
  selector: 'app-admin-categories',
  imports: [CdkDrag, CdkDragHandle, CdkDropList, RouterLink, MatButtonModule, MatIconModule, Pagination],
  templateUrl: './admin-categories.html',
  styleUrl: './admin-categories.css',
})
export class AdminCategories {

  private static readonly PAGE_SIZE = 25;

  private readonly adminCategoryService = inject(AdminCategoryService);
  private readonly productService = inject(ProductService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly notificationService = inject(NotificationService);

  readonly tabs: GenderTab[] = [
    { label: 'All', value: null },
    { label: 'Men', value: 'Men' },
    { label: 'Women', value: 'Women' },
    { label: 'Unisex', value: 'Unisex' }
  ];

  readonly activeTab = signal<GenderTab>(this.tabs[0]);

  readonly categories = signal<Category[]>([]);
  readonly totalCount = signal(0);
  readonly pageIndex = signal(1);
  readonly totalPages = signal(0);
  readonly isLoading = signal(true);

  private readonly genders = signal<Lookup[]>([]);

  /** Dragging needs one specific gender's full list: the order is
      per-gender, and the strict reorder endpoint wants every tagged
      category — so not on "All", and not on a partial (paged) view. */
  readonly canReorder = computed(() =>
    this.activeTab().value !== null
    && this.totalPages() <= 1
    && this.categories().length > 1);

  constructor() {
    this.productService.getFilters().subscribe(filters => this.genders.set(filters.genders));
    this.load();
  }

  dropCategory(event: CdkDragDrop<Category[]>): void {
    const tab = this.activeTab().value;

    if (!tab || event.previousIndex === event.currentIndex) {
      return;
    }

    const genderId = this.genders().find(g => g.name === GENDER_NAMES[tab])?.id;

    if (!genderId) {
      return;
    }

    const reordered = [...this.categories()];
    moveItemInArray(reordered, event.previousIndex, event.currentIndex);

    // optimistic: keep the dropped order on screen while the server saves
    this.categories.set(reordered);

    this.adminCategoryService.reorder(genderId, reordered.map(c => c.id)).subscribe({
      next: () => this.notificationService.success('Category order saved.'),
      // failures are toasted by the error interceptor — reload to revert
      error: () => this.load()
    });
  }

  selectTab(tab: GenderTab): void {
    this.activeTab.set(tab);
    this.pageIndex.set(1);
    this.load();
  }

  goToPage(page: number): void {
    this.pageIndex.set(page);
    this.load();
  }

  load(): void {
    this.isLoading.set(true);

    this.adminCategoryService.getAll(
      this.pageIndex(),
      AdminCategories.PAGE_SIZE,
      this.activeTab().value ?? undefined
    ).subscribe({
      next: response => {
        this.categories.set(response.items);
        this.totalCount.set(response.totalCount);
        this.totalPages.set(response.totalPages);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  firstPhoto(category: Category): string | null {
    return category.genders.find(g => g.photo)?.photo ?? null;
  }

  deleteCategory(category: Category): void {
    this.confirmDialog.confirm({
      title: 'Delete this category?',
      message: `"${category.name}" will be removed permanently. Categories used by a product can't be deleted.`,
      confirmLabel: 'Delete',
      destructive: true
    }).pipe(
      filter(confirmed => confirmed),
      switchMap(() => this.adminCategoryService.delete(category.id))
    ).subscribe({
      next: () => {
        this.notificationService.success('Category deleted.');
        // last item on the page gone — step back a page when possible
        if (this.categories().length === 1 && this.pageIndex() > 1) {
          this.pageIndex.update(page => page - 1);
        }
        this.load();
      },
      // failures (e.g. category in use) are toasted by the error interceptor
      error: () => { }
    });
  }
}
