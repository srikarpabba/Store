import { Component, computed, effect, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormArray, FormControl, FormGroup, NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatTooltipModule } from '@angular/material/tooltip';
import { filter, switchMap } from 'rxjs';
import { AdminProductService } from '../../services/admin-product.service';
import { SaveProductRequest } from '../../models/save-product-request';
import { ProductColorDetails, ProductDetails, ProductPhoto } from '../../../shop/models/product-details';
import { ProductFilters } from '../../../shop/models/product-filters';
import { ProductService } from '../../../shop/services/product.service';
import { HasPendingChanges } from '../../../../core/guards/pending-changes.guard';
import { LoadingService } from '../../../../core/services/loading.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { ConfirmDialogService } from '../../../../shared/ui/confirm-dialog/confirm-dialog.service';

type VariantGroup = FormGroup<{
  id: FormControl<string | null>;
  colorId: FormControl<string>;
  sizeId: FormControl<string>;
  price: FormControl<number>;
  quantityInStock: FormControl<number>;
  sku: FormControl<string>;
}>;

@Component({
  selector: 'app-product-form',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatTooltipModule
  ],
  templateUrl: './product-form.html',
  styleUrl: './product-form.css',
})
export class ProductForm implements HasPendingChanges {

  private readonly formBuilder = inject(NonNullableFormBuilder);
  private readonly productService = inject(ProductService);
  private readonly adminProductService = inject(AdminProductService);
  private readonly notificationService = inject(NotificationService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  readonly loading = inject(LoadingService);

  /** null when creating; the product id when editing */
  readonly productId = this.route.snapshot.paramMap.get('id');

  readonly isEdit = this.productId !== null;

  readonly filters = signal<ProductFilters | null>(null);

  /** Loaded product in edit mode; drives the image manager */
  readonly details = signal<ProductDetails | null>(null);

  readonly form = this.formBuilder.group({
    name: ['', [Validators.required, Validators.maxLength(200)]],
    description: ['', [Validators.required, Validators.maxLength(2000)]],
    categoryId: ['', Validators.required],
    brandId: ['', Validators.required],
    genderIds: [<string[]>[], Validators.required],
    variants: this.formBuilder.array<VariantGroup>([])
  });

  get variants(): FormArray<VariantGroup> {
    return this.form.controls.variants;
  }

  private readonly selectedGenderIds = toSignal(this.form.controls.genderIds.valueChanges, {
    initialValue: <string[]>[]
  });

  private readonly selectedCategoryId = toSignal(this.form.controls.categoryId.valueChanges, {
    initialValue: ''
  });

  /** Categories compatible with every currently selected gender —
      a category must be explicitly tagged with all of them */
  readonly availableCategories = computed(() => {
    const genderIds = this.selectedGenderIds();
    const categories = this.filters()?.categories ?? [];

    return genderIds.length === 0
      ? categories
      : categories.filter(category => genderIds.every(id => category.genderIds.includes(id)));
  });

  /** Genders the selected category is tagged with — narrows the other way.
      Never needs to prune an existing gender selection: availableCategories
      already only lists categories that are supersets of it, so whichever
      category gets picked is guaranteed compatible with what's selected. */
  readonly availableGenders = computed(() => {
    const categoryId = this.selectedCategoryId();
    const genders = this.filters()?.genders ?? [];

    if (!categoryId) {
      return genders;
    }

    const category = this.filters()?.categories.find(c => c.id === categoryId);

    return category ? genders.filter(g => category.genderIds.includes(g.id)) : genders;
  });

  /** Name of the selected category, only when it actually narrows the
      gender list — drives the hint under the Genders field */
  readonly selectedCategoryName = computed(() => {
    const categoryId = this.selectedCategoryId();

    if (!categoryId) {
      return null;
    }

    return this.filters()?.categories.find(c => c.id === categoryId)?.name ?? null;
  });

  constructor() {
    this.productService.getFilters().subscribe(filters => this.filters.set(filters));

    if (this.productId) {
      this.adminProductService.getDetails(this.productId).subscribe(product => this.populate(product));
    } else {
      this.addVariant();
    }

    // the previously selected category may no longer be valid once the
    // gender selection changes — clear it rather than submit a bad combo
    effect(() => {
      const available = this.availableCategories();
      const current = this.form.controls.categoryId.value;

      if (current && !available.some(category => category.id === current)) {
        this.form.controls.categoryId.setValue('');
      }
    });
  }

  hasPendingChanges(): boolean {
    return this.form.dirty;
  }

  addVariant(): void {
    this.variants.push(this.buildVariantGroup());
    this.form.markAsDirty();
  }

  removeVariant(index: number): void {
    this.variants.removeAt(index);
    this.form.markAsDirty();
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    if (this.variants.length === 0) {
      this.notificationService.error('Add at least one variant.');
      return;
    }

    const { name, description, categoryId, brandId, genderIds } = this.form.getRawValue();

    const request: SaveProductRequest = {
      name: name.trim(),
      description: description.trim(),
      categoryId,
      brandId,
      genderIds,
      variants: this.variants.getRawValue().map(variant => ({
        // the create endpoint rejects unknown fields, so only send the
        // variant id when updating
        ...(this.isEdit ? { id: variant.id } : {}),
        colorId: variant.colorId,
        sizeId: variant.sizeId,
        price: variant.price,
        quantityInStock: variant.quantityInStock,
        sku: variant.sku.trim()
      }))
    };

    if (this.productId) {
      this.adminProductService.update(this.productId, request).subscribe({
        next: () => {
          this.notificationService.success('Product updated.');
          // stay on the page; reload so new variants carry their
          // server-generated ids and a re-save can't duplicate them
          this.reloadProduct();
        },
        // failures are toasted by the error interceptor
        error: () => { }
      });
    } else {
      this.adminProductService.create(request).subscribe({
        next: id => {
          this.notificationService.success('Product created. You can now upload images.');
          this.form.markAsPristine();
          this.router.navigate(['/admin/products', id, 'edit']);
        },
        error: () => { }
      });
    }
  }

  private reloadProduct(): void {
    if (!this.productId) {
      return;
    }

    this.adminProductService.getDetails(this.productId).subscribe(product => {
      this.variants.clear();
      this.populate(product);
      this.form.markAsPristine();
    });
  }

  // ---------- Images (edit mode only) ----------

  onFilesSelected(color: ProductColorDetails, input: HTMLInputElement): void {
    const files = Array.from(input.files ?? []);
    input.value = '';

    if (!this.productId || files.length === 0) {
      return;
    }

    this.adminProductService.uploadImages(this.productId, color.productColorId, files).subscribe({
      next: () => {
        this.notificationService.success(files.length === 1
          ? 'Image uploaded.'
          : `${files.length} images uploaded.`);
        this.refreshImages();
      },
      // failures are toasted by the error interceptor
      error: () => { }
    });
  }

  deleteImage(photo: ProductPhoto): void {
    if (!this.productId) {
      return;
    }

    const productId = this.productId;

    this.confirmDialog.confirm({
      title: 'Delete this image?',
      message: 'The image will be removed permanently.',
      confirmLabel: 'Delete',
      destructive: true
    }).pipe(
      filter(confirmed => confirmed),
      switchMap(() => this.adminProductService.deleteImage(productId, photo.id))
    ).subscribe({
      next: () => {
        this.notificationService.success('Image deleted.');
        this.refreshImages();
      },
      error: () => { }
    });
  }

  setMainImage(photo: ProductPhoto): void {
    if (!this.productId || photo.isMain) {
      return;
    }

    this.adminProductService.setMainImage(this.productId, photo.id).subscribe({
      next: () => this.refreshImages(),
      error: () => { }
    });
  }

  /** Reloads only the details signal — never re-patches the form,
      so in-progress edits survive image operations */
  private refreshImages(): void {
    if (this.productId) {
      this.adminProductService.getDetails(this.productId).subscribe(product =>
        this.details.set(product));
    }
  }

  private populate(product: ProductDetails): void {
    this.details.set(product);

    this.form.patchValue({
      name: product.name,
      description: product.description,
      categoryId: product.category.id,
      brandId: product.brand.id,
      genderIds: product.genders.map(gender => gender.id)
    });

    const colorByProductColor = new Map(
      product.colors.map(color => [color.productColorId, color.colorId]));

    for (const variant of product.variants) {
      this.variants.push(this.buildVariantGroup({
        id: variant.id,
        colorId: colorByProductColor.get(variant.productColorId) ?? '',
        sizeId: variant.sizeId,
        price: variant.price,
        quantityInStock: variant.quantityInStock,
        sku: variant.sku
      }));
    }
  }

  private buildVariantGroup(value?: {
    id: string | null;
    colorId: string;
    sizeId: string;
    price: number;
    quantityInStock: number;
    sku: string;
  }): VariantGroup {
    return this.formBuilder.group({
      id: this.formBuilder.control<string | null>(value?.id ?? null),
      colorId: [value?.colorId ?? '', Validators.required],
      sizeId: [value?.sizeId ?? '', Validators.required],
      price: [value?.price ?? 0, [Validators.required, Validators.min(0.01)]],
      quantityInStock: [value?.quantityInStock ?? 0, [Validators.required, Validators.min(0)]],
      sku: [value?.sku ?? '', [Validators.required, Validators.maxLength(100), Validators.pattern(/^[A-Za-z0-9\-_]+$/)]]
    }) as VariantGroup;
  }
}
