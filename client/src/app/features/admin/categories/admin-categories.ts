import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { filter, switchMap } from 'rxjs';
import { AdminCategoryService } from '../services/admin-category.service';
import { Category } from '../../shop/models/category';
import { NotificationService } from '../../../core/services/notification.service';
import { ConfirmDialogService } from '../../../shared/ui/confirm-dialog/confirm-dialog.service';

@Component({
  selector: 'app-admin-categories',
  imports: [RouterLink, MatButtonModule, MatIconModule],
  templateUrl: './admin-categories.html',
  styleUrl: './admin-categories.css',
})
export class AdminCategories {

  private readonly adminCategoryService = inject(AdminCategoryService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly notificationService = inject(NotificationService);

  readonly categories = signal<Category[]>([]);
  readonly isLoading = signal(true);

  constructor() {
    this.load();
  }

  load(): void {
    this.isLoading.set(true);

    this.adminCategoryService.getAll().subscribe({
      next: categories => {
        this.categories.set(categories);
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
        this.load();
      },
      // failures (e.g. category in use) are toasted by the error interceptor
      error: () => { }
    });
  }
}
