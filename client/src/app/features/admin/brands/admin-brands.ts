import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { filter, switchMap } from 'rxjs';
import { AdminBrandService } from '../services/admin-brand.service';
import { Brand } from '../../shop/models/brand';
import { NotificationService } from '../../../core/services/notification.service';
import { ConfirmDialogService } from '../../../shared/ui/confirm-dialog/confirm-dialog.service';

@Component({
  selector: 'app-admin-brands',
  imports: [RouterLink, MatButtonModule, MatIconModule],
  templateUrl: './admin-brands.html',
  styleUrl: './admin-brands.css',
})
export class AdminBrands {

  private readonly adminBrandService = inject(AdminBrandService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly notificationService = inject(NotificationService);

  readonly brands = signal<Brand[]>([]);
  readonly isLoading = signal(true);

  constructor() {
    this.load();
  }

  load(): void {
    this.isLoading.set(true);

    this.adminBrandService.getAll().subscribe({
      next: brands => {
        this.brands.set(brands);
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
        this.load();
      },
      // failures (e.g. brand in use) are toasted by the error interceptor
      error: () => { }
    });
  }
}
