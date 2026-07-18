import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { filter, switchMap } from 'rxjs';
import { AdminSizeService } from '../../services/admin-size.service';
import { Size } from '../../../shop/models/size';
import { NotificationService } from '../../../../core/services/notification.service';
import { ConfirmDialogService } from '../../../../shared/ui/confirm-dialog/confirm-dialog.service';

@Component({
  selector: 'app-admin-sizes',
  imports: [RouterLink, MatButtonModule, MatIconModule],
  templateUrl: './admin-sizes.html',
  styleUrl: './admin-sizes.css',
})
export class AdminSizes {

  private readonly adminSizeService = inject(AdminSizeService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly notificationService = inject(NotificationService);

  readonly sizes = signal<Size[]>([]);
  readonly isLoading = signal(true);

  constructor() {
    this.load();
  }

  load(): void {
    this.isLoading.set(true);

    this.adminSizeService.getAll().subscribe({
      next: sizes => {
        this.sizes.set(sizes);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  deleteSize(size: Size): void {
    this.confirmDialog.confirm({
      title: 'Delete this size?',
      message: `"${size.name}" will be removed permanently. Sizes used by a product variant can't be deleted.`,
      confirmLabel: 'Delete',
      destructive: true
    }).pipe(
      filter(confirmed => confirmed),
      switchMap(() => this.adminSizeService.delete(size.id))
    ).subscribe({
      next: () => {
        this.notificationService.success('Size deleted.');
        this.load();
      },
      // failures (e.g. size in use) are toasted by the error interceptor
      error: () => { }
    });
  }
}
