import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { filter, switchMap } from 'rxjs';
import { AdminCategoryService, CategoryGenderFilter } from '../../services/admin-category.service';
import { Category } from '../../../shop/models/category';
import { NotificationService } from '../../../../core/services/notification.service';
import { ConfirmDialogService } from '../../../../shared/ui/confirm-dialog/confirm-dialog.service';
import { Pagination } from '../../../../shared/ui/pagination/pagination';

interface GenderTab {
  label: string;
  value: CategoryGenderFilter | null;
}

@Component({
  selector: 'app-admin-categories',
  imports: [RouterLink, MatButtonModule, MatIconModule, Pagination],
  templateUrl: './admin-categories.html',
  styleUrl: './admin-categories.css',
})
export class AdminCategories {

  private static readonly PAGE_SIZE = 25;

  private readonly adminCategoryService = inject(AdminCategoryService);
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

  constructor() {
    this.load();
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
