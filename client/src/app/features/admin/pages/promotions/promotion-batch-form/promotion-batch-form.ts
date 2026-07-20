import { Component, inject, signal } from '@angular/core';
import { FormArray, FormControl, FormGroup, NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { AdminPromotionService } from '../../../services/admin-promotion.service';
import { SavePromotionBatchRequest } from '../../../models/save-promotion-request';
import { Product } from '../../../../shop/models/product';
import { ProductFilters } from '../../../../shop/models/product-filters';
import { ProductService } from '../../../../shop/services/product.service';
import { ProductPicker } from '../../../../../shared/ui/product-picker/product-picker';
import { nowLocalInputValue, toIsoOrNull } from '../../../../../shared/utils/datetime-local';
import { HasPendingChanges } from '../../../../../core/guards/pending-changes.guard';
import { LoadingService } from '../../../../../core/services/loading.service';
import { NotificationService } from '../../../../../core/services/notification.service';

type RowScope = 'product' | 'brand';

type RowGroup = FormGroup<{
  scope: FormControl<RowScope>;
  productId: FormControl<string>;
  brandId: FormControl<string>;
  discountPercentage: FormControl<number>;
  startsAt: FormControl<string>;
  endsAt: FormControl<string>;
  isActive: FormControl<boolean>;
}>;

@Component({
  selector: 'app-promotion-batch-form',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatCheckboxModule,
    MatProgressSpinnerModule,
    ProductPicker
  ],
  templateUrl: './promotion-batch-form.html',
  styleUrl: './promotion-batch-form.css',
})
export class PromotionBatchForm implements HasPendingChanges {

  private readonly formBuilder = inject(NonNullableFormBuilder);
  private readonly productService = inject(ProductService);
  private readonly adminPromotionService = inject(AdminPromotionService);
  private readonly notificationService = inject(NotificationService);
  private readonly router = inject(Router);

  readonly loading = inject(LoadingService);

  readonly filters = signal<ProductFilters | null>(null);

  /** Floors every Starts/Ends picker (defaults and rows) so a past date can't be selected */
  readonly minDateTime = nowLocalInputValue();

  readonly name = this.formBuilder.control('', [Validators.required, Validators.maxLength(200)]);

  /** Applied to each new row as its starting point — rows can override any of these afterward */
  readonly defaults = this.formBuilder.group({
    discountPercentage: [10, [Validators.required, Validators.min(0.01), Validators.max(100)]],
    startsAt: [''],
    endsAt: [''],
    isActive: [true]
  });

  readonly rows = this.formBuilder.array<RowGroup>([]);

  constructor() {
    this.productService.getFilters().subscribe(filters => this.filters.set(filters));
    this.addRow();
  }

  hasPendingChanges(): boolean {
    return this.name.dirty || this.defaults.dirty || this.rows.dirty;
  }

  addRow(): void {
    const d = this.defaults.getRawValue();

    this.rows.push(this.buildRow({
      scope: 'product',
      productId: '',
      brandId: '',
      discountPercentage: d.discountPercentage,
      startsAt: d.startsAt,
      endsAt: d.endsAt,
      isActive: d.isActive
    }));
  }

  removeRow(index: number): void {
    this.rows.removeAt(index);
  }

  selectRowScope(row: RowGroup, scope: RowScope): void {
    row.controls.scope.setValue(scope);
    row.markAsDirty();

    if (scope === 'product') {
      row.controls.brandId.setValue('');
    } else {
      row.controls.productId.setValue('');
    }
  }

  onRowProductSelected(row: RowGroup, product: Product | null): void {
    row.controls.productId.setValue(product?.id ?? '');
    row.markAsDirty();
  }

  submit(): void {
    if (this.name.invalid || this.defaults.invalid || this.rows.invalid) {
      this.name.markAsTouched();
      this.rows.markAllAsTouched();
      return;
    }

    const rowValues = this.rows.getRawValue();

    if (rowValues.length === 0) {
      this.notificationService.error('Add at least one product or brand.');
      return;
    }

    const missingScope = rowValues.some(row =>
      (row.scope === 'product' && !row.productId) || (row.scope === 'brand' && !row.brandId));

    if (missingScope) {
      this.notificationService.error('Every row needs a product or brand picked.');
      return;
    }

    const request: SavePromotionBatchRequest = {
      name: this.name.value.trim(),
      items: rowValues.map(row => ({
        discountPercentage: row.discountPercentage,
        startsAtUtc: toIsoOrNull(row.startsAt),
        endsAtUtc: toIsoOrNull(row.endsAt),
        isActive: row.isActive,
        productId: row.scope === 'product' ? row.productId : null,
        brandId: row.scope === 'brand' ? row.brandId : null
      }))
    };

    this.adminPromotionService.createBatch(request).subscribe({
      next: ids => {
        this.notificationService.success(`${ids.length} sale${ids.length === 1 ? '' : 's'} created.`);
        this.name.markAsPristine();
        this.defaults.markAsPristine();
        this.rows.markAsPristine();
        this.router.navigateByUrl('/admin/promotions/sales');
      },
      // failures are toasted by the error interceptor
      error: () => { }
    });
  }

  private buildRow(value: {
    scope: RowScope;
    productId: string;
    brandId: string;
    discountPercentage: number;
    startsAt: string;
    endsAt: string;
    isActive: boolean;
  }): RowGroup {
    return this.formBuilder.group({
      scope: this.formBuilder.control<RowScope>(value.scope),
      productId: [value.productId],
      brandId: [value.brandId],
      discountPercentage: [value.discountPercentage, [Validators.required, Validators.min(0.01), Validators.max(100)]],
      startsAt: [value.startsAt],
      endsAt: [value.endsAt],
      isActive: [value.isActive]
    }) as RowGroup;
  }
}
