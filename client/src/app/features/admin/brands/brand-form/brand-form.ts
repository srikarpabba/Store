import { Component, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { filter, switchMap } from 'rxjs';
import { AdminBrandService } from '../../services/admin-brand.service';
import { SaveBrandRequest } from '../../models/save-brand-request';
import { Brand } from '../../../shop/models/brand';
import { HasPendingChanges } from '../../../../core/guards/pending-changes.guard';
import { LoadingService } from '../../../../core/services/loading.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { ConfirmDialogService } from '../../../../shared/ui/confirm-dialog/confirm-dialog.service';

@Component({
  selector: 'app-brand-form',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatFormFieldModule,
    MatInputModule,
    MatCheckboxModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './brand-form.html',
  styleUrl: './brand-form.css',
})
export class BrandForm implements HasPendingChanges {

  private readonly formBuilder = inject(NonNullableFormBuilder);
  private readonly adminBrandService = inject(AdminBrandService);
  private readonly notificationService = inject(NotificationService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  readonly loading = inject(LoadingService);

  /** null when creating; the brand id when editing */
  readonly brandId = this.route.snapshot.paramMap.get('id');

  readonly isEdit = this.brandId !== null;

  /** Loaded brand in edit mode; drives the logo manager */
  readonly brand = signal<Brand | null>(null);

  readonly form = this.formBuilder.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    description: ['', Validators.maxLength(1000)],
    isFeatured: [false]
  });

  constructor() {
    if (this.brandId) {
      this.adminBrandService.getById(this.brandId).subscribe(brand => this.populate(brand));
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

    const { name, description, isFeatured } = this.form.getRawValue();

    const request: SaveBrandRequest = {
      name: name.trim(),
      description: description.trim() || null,
      isFeatured
    };

    if (this.brandId) {
      this.adminBrandService.update(this.brandId, request).subscribe({
        next: () => {
          this.notificationService.success('Brand updated.');
          this.form.markAsPristine();
        },
        // failures are toasted by the error interceptor
        error: () => { }
      });
    } else {
      this.adminBrandService.create(request).subscribe({
        next: id => {
          this.notificationService.success('Brand created. You can now upload a logo.');
          this.form.markAsPristine();
          this.router.navigate(['/admin/product-management/brands', id, 'edit']);
        },
        error: () => { }
      });
    }
  }

  // ---------- Logo (edit mode only) ----------

  onLogoSelected(input: HTMLInputElement): void {
    const file = input.files?.[0] ?? null;
    input.value = '';

    if (!this.brandId || !file) {
      return;
    }

    this.adminBrandService.uploadLogo(this.brandId, file).subscribe({
      next: () => {
        this.notificationService.success('Logo uploaded.');
        this.refreshBrand();
      },
      // failures are toasted by the error interceptor
      error: () => { }
    });
  }

  deleteLogo(): void {
    if (!this.brandId) {
      return;
    }

    const brandId = this.brandId;

    this.confirmDialog.confirm({
      title: 'Delete this logo?',
      message: 'The logo will be removed permanently.',
      confirmLabel: 'Delete',
      destructive: true
    }).pipe(
      filter(confirmed => confirmed),
      switchMap(() => this.adminBrandService.deleteLogo(brandId))
    ).subscribe({
      next: () => {
        this.notificationService.success('Logo deleted.');
        this.refreshBrand();
      },
      error: () => { }
    });
  }

  private refreshBrand(): void {
    if (this.brandId) {
      this.adminBrandService.getById(this.brandId).subscribe(brand => this.brand.set(brand));
    }
  }

  private populate(brand: Brand): void {
    this.brand.set(brand);

    this.form.patchValue({
      name: brand.name,
      description: brand.description ?? '',
      isFeatured: brand.isFeatured
    });
  }
}
