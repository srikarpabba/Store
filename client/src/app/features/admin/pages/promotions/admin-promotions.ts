import { Component, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { filter, switchMap } from 'rxjs';
import { AdminPromotionService } from '../../services/admin-promotion.service';
import { Promotion } from '../../../shop/models/promotion';
import { NotificationService } from '../../../../core/services/notification.service';
import { ConfirmDialogService } from '../../../../shared/ui/confirm-dialog/confirm-dialog.service';

@Component({
  selector: 'app-admin-promotions',
  imports: [RouterLink, DatePipe, MatButtonModule, MatIconModule],
  templateUrl: './admin-promotions.html',
  styleUrl: './admin-promotions.css',
})
export class AdminPromotions {

  private readonly adminPromotionService = inject(AdminPromotionService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly notificationService = inject(NotificationService);

  readonly promotions = signal<Promotion[]>([]);
  readonly isLoading = signal(true);

  constructor() {
    this.load();
  }

  load(): void {
    this.isLoading.set(true);

    this.adminPromotionService.getAll().subscribe({
      next: promotions => {
        this.promotions.set(promotions);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  /** True once the promotion's end date has passed — still shown, since
      deleting history isn't the point, but flagged as no longer live. */
  isExpired(promotion: Promotion): boolean {
    return !!promotion.endsAtUtc && new Date(promotion.endsAtUtc) < new Date();
  }

  deletePromotion(promotion: Promotion): void {
    this.confirmDialog.confirm({
      title: 'Delete this sale?',
      message: `"${promotion.name}" will be removed permanently.`,
      confirmLabel: 'Delete',
      destructive: true
    }).pipe(
      filter(confirmed => confirmed),
      switchMap(() => this.adminPromotionService.delete(promotion.id))
    ).subscribe({
      next: () => {
        this.notificationService.success('Sale deleted.');
        this.load();
      },
      error: () => { }
    });
  }
}
