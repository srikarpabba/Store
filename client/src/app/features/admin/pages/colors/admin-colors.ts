import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { filter, switchMap } from 'rxjs';
import { AdminColorService } from '../../services/admin-color.service';
import { Color } from '../../../shop/models/color';
import { NotificationService } from '../../../../core/services/notification.service';
import { ConfirmDialogService } from '../../../../shared/ui/confirm-dialog/confirm-dialog.service';

@Component({
  selector: 'app-admin-colors',
  imports: [RouterLink, MatButtonModule, MatIconModule],
  templateUrl: './admin-colors.html',
  styleUrl: './admin-colors.css',
})
export class AdminColors {

  private readonly adminColorService = inject(AdminColorService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly notificationService = inject(NotificationService);

  readonly colors = signal<Color[]>([]);
  readonly isLoading = signal(true);

  constructor() {
    this.load();
  }

  load(): void {
    this.isLoading.set(true);

    this.adminColorService.getAll().subscribe({
      next: colors => {
        this.colors.set(colors);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  deleteColor(color: Color): void {
    this.confirmDialog.confirm({
      title: 'Delete this color?',
      message: `"${color.name}" will be removed permanently. Colors used by a product can't be deleted.`,
      confirmLabel: 'Delete',
      destructive: true
    }).pipe(
      filter(confirmed => confirmed),
      switchMap(() => this.adminColorService.delete(color.id))
    ).subscribe({
      next: () => {
        this.notificationService.success('Color deleted.');
        this.load();
      },
      // failures (e.g. color in use) are toasted by the error interceptor
      error: () => { }
    });
  }
}
