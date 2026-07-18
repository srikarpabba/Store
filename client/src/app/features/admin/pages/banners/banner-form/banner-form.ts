import { Component, inject, signal } from '@angular/core';
import { TitleCasePipe } from '@angular/common';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { AdminBannerService } from '../../../services/admin-banner.service';
import { SaveBannerRequest } from '../../../models/save-banner-request';
import { Banner } from '../../../../shop/models/banner';
import { HasPendingChanges } from '../../../../../core/guards/pending-changes.guard';
import { LoadingService } from '../../../../../core/services/loading.service';
import { NotificationService } from '../../../../../core/services/notification.service';

@Component({
  selector: 'app-banner-form',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    TitleCasePipe,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatCheckboxModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './banner-form.html',
  styleUrl: './banner-form.css',
})
export class BannerForm implements HasPendingChanges {

  private readonly formBuilder = inject(NonNullableFormBuilder);
  private readonly adminBannerService = inject(AdminBannerService);
  private readonly notificationService = inject(NotificationService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  readonly loading = inject(LoadingService);

  readonly storefronts = ['men', 'women', 'kids'];

  /** null when creating; the banner id when editing */
  readonly bannerId = this.route.snapshot.paramMap.get('id');

  readonly isEdit = this.bannerId !== null;

  /** Loaded banner in edit mode; drives the image manager */
  readonly banner = signal<Banner | null>(null);

  readonly form = this.formBuilder.group({
    storefront: ['men', Validators.required],
    title: ['', Validators.maxLength(200)],
    link: ['', Validators.maxLength(2048)],
    sortOrder: [0, Validators.required],
    isActive: [true]
  });

  constructor() {
    if (this.bannerId) {
      this.adminBannerService.getById(this.bannerId).subscribe(banner => this.populate(banner));
    }
  }

  hasPendingChanges(): boolean {
    return this.form.dirty;
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { storefront, title, link, sortOrder, isActive } = this.form.getRawValue();

    const request: SaveBannerRequest = {
      storefront,
      title: title.trim() || null,
      link: link.trim() || null,
      sortOrder,
      isActive
    };

    if (this.bannerId) {
      this.adminBannerService.update(this.bannerId, request).subscribe({
        next: () => {
          this.notificationService.success('Banner updated.');
          this.form.markAsPristine();
        },
        error: () => { }
      });
    } else {
      this.adminBannerService.create(request).subscribe({
        next: id => {
          this.notificationService.success('Banner created. You can now upload an image.');
          this.form.markAsPristine();
          this.router.navigate(['/admin/store-look/banners', id, 'edit']);
        },
        error: () => { }
      });
    }
  }

  // ---------- Image (edit mode only) ----------

  onImageSelected(input: HTMLInputElement): void {
    const file = input.files?.[0] ?? null;
    input.value = '';

    if (!this.bannerId || !file) {
      return;
    }

    this.adminBannerService.uploadImage(this.bannerId, file).subscribe({
      next: () => {
        this.notificationService.success('Image uploaded.');
        this.refreshBanner();
      },
      error: () => { }
    });
  }

  private refreshBanner(): void {
    if (this.bannerId) {
      this.adminBannerService.getById(this.bannerId).subscribe(banner => this.banner.set(banner));
    }
  }

  private populate(banner: Banner): void {
    this.banner.set(banner);

    this.form.patchValue({
      storefront: banner.storefront,
      title: banner.title ?? '',
      link: banner.link ?? '',
      sortOrder: banner.sortOrder,
      isActive: banner.isActive
    });
  }
}
