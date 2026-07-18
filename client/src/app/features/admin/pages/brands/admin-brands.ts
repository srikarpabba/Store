import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { filter, switchMap } from 'rxjs';
import { AdminBrandService } from '../../services/admin-brand.service';
import { Brand } from '../../../shop/models/brand';
import { NotificationService } from '../../../../core/services/notification.service';
import { ConfirmDialogService } from '../../../../shared/ui/confirm-dialog/confirm-dialog.service';
import { Pagination } from '../../../../shared/ui/pagination/pagination';

@Component({
  selector: 'app-admin-brands',
  imports: [RouterLink, MatButtonModule, MatIconModule, Pagination],
  templateUrl: './admin-brands.html',
  styleUrl: './admin-brands.css',
})
export class AdminBrands {

  private static readonly PAGE_SIZE = 25;

  private readonly adminBrandService = inject(AdminBrandService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly notificationService = inject(NotificationService);

  readonly brands = signal<Brand[]>([]);
  readonly totalCount = signal(0);
  readonly pageIndex = signal(1);
  readonly totalPages = signal(0);
  readonly isLoading = signal(true);

  constructor() {
    this.load();
  }

  goToPage(page: number): void {
    this.pageIndex.set(page);
    this.load();
  }

  load(): void {
    this.isLoading.set(true);

    this.adminBrandService.getAll(this.pageIndex(), AdminBrands.PAGE_SIZE).subscribe({
      next: response => {
        this.brands.set(response.items);
        this.totalCount.set(response.totalCount);
        this.totalPages.set(response.totalPages);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  deleteBrand(brand: Brand): void {
    this.confirmDialog.confirm({
      title: 'Delete this brand?',
      message: `"${brand.name}" will be removed permanently. Brands used by a product can't be deleted.`,
      confirmLabel: 'Delete',
      destructive: true
    }).pipe(
      filter(confirmed => confirmed),
      switchMap(() => this.adminBrandService.delete(brand.id))
    ).subscribe({
      next: () => {
        this.notificationService.success('Brand deleted.');
        // last item on the page gone — step back a page when possible
        if (this.brands().length === 1 && this.pageIndex() > 1) {
          this.pageIndex.update(page => page - 1);
        }
        this.load();
      },
      // failures (e.g. brand in use) are toasted by the error interceptor
      error: () => { }
    });
  }
}
