import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { filter, switchMap } from 'rxjs';
import { AdminBannerService } from '../../services/admin-banner.service';
import { Banner } from '../../../shop/models/banner';
import { NotificationService } from '../../../../core/services/notification.service';
import { ConfirmDialogService } from '../../../../shared/ui/confirm-dialog/confirm-dialog.service';

interface StorefrontTab {
  label: string;
  value: string | null;
}

@Component({
  selector: 'app-admin-banners',
  imports: [RouterLink, MatButtonModule, MatIconModule],
  templateUrl: './admin-banners.html',
  styleUrl: './admin-banners.css',
})
export class AdminBanners {

  private readonly adminBannerService = inject(AdminBannerService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly notificationService = inject(NotificationService);

  readonly tabs: StorefrontTab[] = [
    { label: 'All', value: null },
    { label: 'Men', value: 'men' },
    { label: 'Women', value: 'women' },
    { label: 'Kids', value: 'kids' }
  ];

  readonly activeTab = signal<StorefrontTab>(this.tabs[0]);

  readonly banners = signal<Banner[]>([]);
  readonly isLoading = signal(true);

  constructor() {
    this.load();
  }

  selectTab(tab: StorefrontTab): void {
    this.activeTab.set(tab);
    this.load();
  }

  load(): void {
    this.isLoading.set(true);

    this.adminBannerService.getAll(this.activeTab().value ?? undefined).subscribe({
      next: banners => {
        this.banners.set(banners);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  deleteBanner(banner: Banner): void {
    this.confirmDialog.confirm({
      title: 'Delete this banner?',
      message: `"${banner.title ?? 'This banner'}" will be removed permanently.`,
      confirmLabel: 'Delete',
      destructive: true
    }).pipe(
      filter(confirmed => confirmed),
      switchMap(() => this.adminBannerService.delete(banner.id))
    ).subscribe({
      next: () => {
        this.notificationService.success('Banner deleted.');
        this.load();
      },
      error: () => { }
    });
  }
}
