import { Component, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { catchError, debounceTime, distinctUntilChanged, map, of, switchMap, tap } from 'rxjs';
import { AdminPromotionService } from '../../../services/admin-promotion.service';
import { SavePromotionRequest } from '../../../models/save-promotion-request';
import { Promotion } from '../../../../shop/models/promotion';
import { Product } from '../../../../shop/models/product';
import { ProductFilters } from '../../../../shop/models/product-filters';
import { ProductService } from '../../../../shop/services/product.service';
import { PricePipe } from '../../../../../shared/pipes/price.pipe';
import { nowLocalInputValue, toIsoOrNull, toLocalInputValue } from '../../../../../shared/utils/datetime-local';
import { HasPendingChanges } from '../../../../../core/guards/pending-changes.guard';
import { LoadingService } from '../../../../../core/services/loading.service';
import { NotificationService } from '../../../../../core/services/notification.service';

type PromotionScope = 'product' | 'brand';

@Component({
  selector: 'app-promotion-form',
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
    PricePipe
  ],
  templateUrl: './promotion-form.html',
  styleUrl: './promotion-form.css',
})
export class PromotionForm implements HasPendingChanges {

  private static readonly MIN_QUERY_LENGTH = 2;
  private static readonly SUGGESTION_COUNT = 8;

  private readonly formBuilder = inject(NonNullableFormBuilder);
  private readonly productService = inject(ProductService);
  private readonly adminPromotionService = inject(AdminPromotionService);
  private readonly notificationService = inject(NotificationService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  readonly loading = inject(LoadingService);

  /** null when creating; the promotion id when editing */
  readonly promotionId = this.route.snapshot.paramMap.get('id');

  readonly isEdit = this.promotionId !== null;

  readonly filters = signal<ProductFilters | null>(null);

  /** Floors the Starts/Ends pickers so a past date can't be selected */
  readonly minDateTime = nowLocalInputValue();

  readonly scope = signal<PromotionScope>('product');

  /** The chosen product, kept alongside its id purely so the field can
      display a name instead of a bare Guid */
  readonly selectedProduct = signal<Product | null>(null);

  readonly productQuery = this.formBuilder.control('');

  readonly productResults = signal<Product[]>([]);
  readonly isSearching = signal(false);
  readonly isProductPanelOpen = signal(false);

  readonly form = this.formBuilder.group({
    name: ['', [Validators.required, Validators.maxLength(200)]],
    discountPercentage: [10, [Validators.required, Validators.min(0.01), Validators.max(100)]],
    startsAt: [''],
    endsAt: [''],
    isActive: [true],
    productId: [{ value: '', disabled: false }],
    brandId: [{ value: '', disabled: true }]
  });

  constructor() {
    this.productService.getFilters().subscribe(filters => this.filters.set(filters));

    this.productQuery.valueChanges.pipe(
      map(value => value.trim()),
      tap(term => {
        if (term.length < PromotionForm.MIN_QUERY_LENGTH) {
          this.isProductPanelOpen.set(false);
        }
      }),
      debounceTime(300),
      distinctUntilChanged(),
      switchMap(term => {
        if (term.length < PromotionForm.MIN_QUERY_LENGTH) {
          return of(null);
        }

        this.isSearching.set(true);
        this.isProductPanelOpen.set(true);

        return this.productService.getProducts({
          search: term,
          pageIndex: 1,
          pageSize: PromotionForm.SUGGESTION_COUNT
        }).pipe(catchError(() => of(null)));
      }),
      takeUntilDestroyed()
    ).subscribe(response => {
      this.isSearching.set(false);

      if (response !== null) {
        this.productResults.set(response.items);
      }
    });

    if (this.promotionId) {
      this.adminPromotionService.getById(this.promotionId).subscribe(promotion => this.populate(promotion));
    }
  }

  hasPendingChanges(): boolean {
    return this.form.dirty;
  }

  selectScope(scope: PromotionScope): void {
    this.scope.set(scope);
    this.form.markAsDirty();

    if (scope === 'product') {
      this.form.controls.productId.enable();
      this.form.controls.brandId.disable();
      this.form.controls.brandId.setValue('');
    } else {
      this.form.controls.brandId.enable();
      this.form.controls.productId.disable();
      this.form.controls.productId.setValue('');
      this.selectedProduct.set(null);
      this.productQuery.setValue('');
    }
  }

  selectProduct(product: Product): void {
    this.selectedProduct.set(product);
    this.form.controls.productId.setValue(product.id);
    this.form.markAsDirty();
    this.productQuery.setValue(product.name, { emitEvent: false });
    this.isProductPanelOpen.set(false);
  }

  clearProduct(): void {
    this.selectedProduct.set(null);
    this.form.controls.productId.setValue('');
    this.productQuery.setValue('', { emitEvent: false });
  }

  closeProductPanel(): void {
    // Deferred so a click on a result fires before the panel closes
    setTimeout(() => this.isProductPanelOpen.set(false), 150);
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { name, discountPercentage, startsAt, endsAt, isActive, productId, brandId } = this.form.getRawValue();

    if (this.scope() === 'product' && !productId) {
      this.notificationService.error('Pick a product for this sale.');
      return;
    }

    if (this.scope() === 'brand' && !brandId) {
      this.notificationService.error('Pick a brand for this sale.');
      return;
    }

    const request: SavePromotionRequest = {
      name: name.trim(),
      discountPercentage,
      startsAtUtc: toIsoOrNull(startsAt),
      endsAtUtc: toIsoOrNull(endsAt),
      isActive,
      productId: this.scope() === 'product' ? productId : null,
      brandId: this.scope() === 'brand' ? brandId : null
    };

    if (this.promotionId) {
      this.adminPromotionService.update(this.promotionId, request).subscribe({
        next: () => {
          this.notificationService.success('Sale updated.');
          this.form.markAsPristine();
          this.router.navigateByUrl('/admin/promotions/sales');
        },
        // failures are toasted by the error interceptor
        error: () => { }
      });
    } else {
      this.adminPromotionService.create(request).subscribe({
        next: () => {
          this.notificationService.success('Sale created.');
          this.form.markAsPristine();
          this.router.navigateByUrl('/admin/promotions/sales');
        },
        error: () => { }
      });
    }
  }

  private populate(promotion: Promotion): void {
    const scope: PromotionScope = promotion.productId ? 'product' : 'brand';
    this.scope.set(scope);

    this.form.patchValue({
      name: promotion.name,
      discountPercentage: promotion.discountPercentage,
      startsAt: toLocalInputValue(promotion.startsAtUtc),
      endsAt: toLocalInputValue(promotion.endsAtUtc),
      isActive: promotion.isActive,
      productId: promotion.productId ?? '',
      brandId: promotion.brandId ?? ''
    });

    if (scope === 'product') {
      this.form.controls.productId.enable();
      this.form.controls.brandId.disable();

      if (promotion.productId && promotion.productName) {
        this.productQuery.setValue(promotion.productName, { emitEvent: false });
      }
    } else {
      this.form.controls.brandId.enable();
      this.form.controls.productId.disable();
    }
  }
}
