import { Component, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { debounceTime, distinctUntilChanged, filter, switchMap } from 'rxjs';
import { AdminProductService } from '../../services/admin-product.service';
import { Product } from '../../../shop/models/product';
import { ProductService } from '../../../shop/services/product.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { PricePipe } from '../../../../shared/pipes/price.pipe';
import { ConfirmDialogService } from '../../../../shared/ui/confirm-dialog/confirm-dialog.service';
import { Pagination } from '../../../../shared/ui/pagination/pagination';

@Component({
  selector: 'app-admin-products',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatButtonModule,
    MatIconModule,
    PricePipe,
    Pagination
  ],
  templateUrl: './admin-products.html',
  styleUrl: './admin-products.css',
})
export class AdminProducts {

  private static readonly PAGE_SIZE = 25;

  private readonly productService = inject(ProductService);
  private readonly adminProductService = inject(AdminProductService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly notificationService = inject(NotificationService);

  readonly search = new FormControl('', { nonNullable: true });

  readonly products = signal<Product[]>([]);
  readonly totalCount = signal(0);
  readonly pageIndex = signal(1);
  readonly totalPages = signal(0);
  readonly isLoading = signal(true);

  constructor() {
    this.search.valueChanges.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      takeUntilDestroyed()
    ).subscribe(() => {
      this.pageIndex.set(1);
      this.load();
    });

    this.load();
  }

  load(): void {
    this.isLoading.set(true);

    this.productService.getProducts({
      search: this.search.value.trim() || undefined,
      pageIndex: this.pageIndex(),
      pageSize: AdminProducts.PAGE_SIZE
    }).subscribe({
      next: response => {
        this.products.set(response.items);
        this.totalCount.set(response.totalCount);
        this.totalPages.set(response.totalPages);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  goToPage(page: number): void {
    this.pageIndex.set(page);
    this.load();
  }

  deleteProduct(product: Product): void {
    this.confirmDialog.confirm({
      title: 'Delete this product?',
      message: `"${product.name}" and all its variants will be removed permanently.`,
      confirmLabel: 'Delete',
      destructive: true
    }).pipe(
      filter(confirmed => confirmed),
      switchMap(() => this.adminProductService.delete(product.id))
    ).subscribe({
      next: () => {
        this.notificationService.success('Product deleted.');
        // last item on the page gone — step back a page when possible
        if (this.products().length === 1 && this.pageIndex() > 1) {
          this.pageIndex.update(page => page - 1);
        }
        this.load();
      },
      // failures are toasted by the error interceptor
      error: () => { }
    });
  }
}
