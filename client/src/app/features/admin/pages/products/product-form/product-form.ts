import { Component, computed, effect, inject, signal } from '@angular/core';
import { CdkDrag, CdkDragDrop, CdkDropList, moveItemInArray } from '@angular/cdk/drag-drop';
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
import { AdminProductService } from '../../../services/admin-product.service';
import { SaveProductRequest } from '../../../models/save-product-request';
import { ProductColorDetails, ProductDetails, ProductPhoto } from '../../../../shop/models/product-details';
import { ProductFilters } from '../../../../shop/models/product-filters';
import { ProductService } from '../../../../shop/services/product.service';
import { HasPendingChanges } from '../../../../../core/guards/pending-changes.guard';
import { LoadingService } from '../../../../../core/services/loading.service';
import { NotificationService } from '../../../../../core/services/notification.service';
import { ConfirmDialogService } from '../../../../../shared/ui/confirm-dialog/confirm-dialog.service';

type SizeRowGroup = FormGroup<{
  id: FormControl<string | null>;
  sizeId: FormControl<string>;
  price: FormControl<number>;
  quantityInStock: FormControl<number>;
  sku: FormControl<string>;
}>;

/** One color of the product with its per-size stock rows — maps onto the
    server's ProductColor + one variant per (color, size). */
type ColorVariantGroup = FormGroup<{
  colorId: FormControl<string>;
  sizes: FormArray<SizeRowGroup>;
}>;

@Component({
  selector: 'app-product-form',
  imports: [
    CdkDrag,
    CdkDropList,
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
    // category/subcategory start disabled — they unlock once a gender is
    // picked (category) and a category is picked (subcategory)
    categoryId: [{ value: '', disabled: true }, Validators.required],
    subcategoryId: [{ value: '', disabled: true }],
    brandId: ['', Validators.required],
    genderIds: [<string[]>[], Validators.required],
    variantColors: this.formBuilder.array<ColorVariantGroup>([])
  });

  get variantColors(): FormArray<ColorVariantGroup> {
    return this.form.controls.variantColors;
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

  /** Subcategories of the currently selected category — the dropdown is
      only meaningful once a category is chosen */
  readonly availableSubcategories = computed(() => {
    const categoryId = this.selectedCategoryId();

    if (!categoryId) {
      return [];
    }

    return (this.filters()?.subcategories ?? []).filter(s => s.categoryId === categoryId);
  });

  /** Sizes the selected category's products can use — a category with no
      tagged sizes allows all of them (same convention as genderIds) */
  readonly availableSizes = computed(() => {
    const sizes = this.filters()?.sizes ?? [];
    const categoryId = this.selectedCategoryId();

    if (!categoryId) {
      return sizes;
    }

    const category = this.filters()?.categories.find(c => c.id === categoryId);

    if (!category || category.sizeIds.length === 0) {
      return sizes;
    }

    return sizes.filter(size => category.sizeIds.includes(size.id));
  });

  constructor() {
    this.productService.getFilters().subscribe(filters => this.filters.set(filters));

    if (this.productId) {
      this.adminProductService.getDetails(this.productId).subscribe(product => this.populate(product));
    } else {
      this.addColorGroup();
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

    // gender gates category: no gender picked -> category locked
    effect(() => {
      const hasGenders = this.selectedGenderIds().length > 0;
      const control = this.form.controls.categoryId;

      if (hasGenders && control.disabled) {
        control.enable();
      } else if (!hasGenders && control.enabled) {
        control.setValue('');
        control.disable();
      }
    });

    // …and category gates subcategory
    effect(() => {
      const hasCategory = !!this.selectedCategoryId();
      const control = this.form.controls.subcategoryId;

      if (hasCategory && control.disabled) {
        control.enable();
      } else if (!hasCategory && control.enabled) {
        control.setValue('');
        control.disable();
      }
    });

    // variant sizes must stay within the selected category's sizes — clear
    // ones that fall outside when the category changes. Same filters-loaded
    // guard as below so an edit-mode prefill isn't wiped.
    effect(() => {
      const available = this.availableSizes();

      if (!this.filters()) {
        return;
      }

      for (const colorGroup of this.variantColors.controls) {
        for (const row of colorGroup.controls.sizes.controls) {
          const sizeId = row.controls.sizeId.value;

          if (sizeId && !available.some(size => size.id === sizeId)) {
            row.controls.sizeId.setValue('');
          }
        }
      }
    });

    // likewise a subcategory belongs to exactly one category — clear it
    // when the category changes to one it doesn't belong to. Skip until the
    // filters have loaded, or an edit-mode prefill that lands before the
    // filters response would be wiped by the then-empty available list.
    effect(() => {
      const available = this.availableSubcategories();
      const current = this.form.controls.subcategoryId.value;

      if (this.filters() && current && !available.some(subcategory => subcategory.id === current)) {
        this.form.controls.subcategoryId.setValue('');
      }
    });
  }

  hasPendingChanges(): boolean {
    return this.form.dirty;
  }

  addColorGroup(): void {
    this.variantColors.push(this.buildColorGroup());
    this.form.markAsDirty();
  }

  removeColorGroup(index: number): void {
    this.variantColors.removeAt(index);
    this.form.markAsDirty();
  }

  addSizeRow(colorGroup: ColorVariantGroup): void {
    colorGroup.controls.sizes.push(this.buildSizeRow());
    this.form.markAsDirty();
  }

  removeSizeRow(colorGroup: ColorVariantGroup, index: number): void {
    colorGroup.controls.sizes.removeAt(index);
    this.form.markAsDirty();
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    // flatten color groups × size rows into the API's (color, size) variants
    const variants = this.variantColors.getRawValue().flatMap(colorGroup =>
      colorGroup.sizes.map(row => ({
        // the create endpoint rejects unknown fields, so only send the
        // variant id when updating
        ...(this.isEdit ? { id: row.id } : {}),
        colorId: colorGroup.colorId,
        sizeId: row.sizeId,
        price: row.price,
        quantityInStock: row.quantityInStock,
        sku: row.sku.trim()
      })));

    if (variants.length === 0) {
      this.notificationService.error('Add at least one color with a size.');
      return;
    }

    const duplicateSizeInColor = this.variantColors.getRawValue().some(colorGroup =>
      new Set(colorGroup.sizes.map(row => row.sizeId)).size !== colorGroup.sizes.length);

    if (duplicateSizeInColor) {
      this.notificationService.error('A color has the same size listed twice.');
      return;
    }

    const { name, description, categoryId, subcategoryId, brandId, genderIds } = this.form.getRawValue();

    const request: SaveProductRequest = {
      name: name.trim(),
      description: description.trim(),
      categoryId,
      subcategoryId: subcategoryId || null,
      brandId,
      genderIds,
      variants
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
          this.router.navigate(['/admin/product-management/products', id, 'edit']);
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
      this.variantColors.clear();
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

  reorderPhotos(color: ProductColorDetails, event: CdkDragDrop<ProductPhoto[]>): void {
    if (!this.productId || event.previousIndex === event.currentIndex) {
      return;
    }

    const photos = [...color.photos];
    moveItemInArray(photos, event.previousIndex, event.currentIndex);

    // optimistic: keep the dropped order on screen while the server saves
    this.details.update(details => details && ({
      ...details,
      colors: details.colors.map(c =>
        c.productColorId === color.productColorId ? { ...c, photos } : c)
    }));

    this.adminProductService.reorderImages(
      this.productId,
      color.productColorId,
      photos.map(photo => photo.id)
    ).subscribe({
      // reload either way: on success the server re-sorts (main pinned
      // first); on failure it reverts. Failures are toasted by the
      // error interceptor.
      next: () => this.refreshImages(),
      error: () => this.refreshImages()
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
      subcategoryId: product.subcategory?.id ?? '',
      brandId: product.brand.id,
      genderIds: product.genders.map(gender => gender.id)
    });

    const colorByProductColor = new Map(
      product.colors.map(color => [color.productColorId, color.colorId]));

    // one group per color, its variants becoming the size rows
    const groupByColor = new Map<string, ColorVariantGroup>();

    for (const variant of product.variants) {
      const colorId = colorByProductColor.get(variant.productColorId) ?? '';

      let colorGroup = groupByColor.get(colorId);

      if (!colorGroup) {
        colorGroup = this.buildColorGroup(colorId);
        groupByColor.set(colorId, colorGroup);
        this.variantColors.push(colorGroup);
      }

      colorGroup.controls.sizes.push(this.buildSizeRow({
        id: variant.id,
        sizeId: variant.sizeId,
        price: variant.price,
        quantityInStock: variant.quantityInStock,
        sku: variant.sku
      }));
    }
  }

  private buildColorGroup(colorId = ''): ColorVariantGroup {
    return this.formBuilder.group({
      colorId: [colorId, Validators.required],
      sizes: this.formBuilder.array<SizeRowGroup>(colorId === '' ? [this.buildSizeRow()] : [])
    }) as ColorVariantGroup;
  }

  private buildSizeRow(value?: {
    id: string | null;
    sizeId: string;
    price: number;
    quantityInStock: number;
    sku: string;
  }): SizeRowGroup {
    return this.formBuilder.group({
      id: this.formBuilder.control<string | null>(value?.id ?? null),
      sizeId: [value?.sizeId ?? '', Validators.required],
      price: [value?.price ?? 0, [Validators.required, Validators.min(0.01)]],
      quantityInStock: [value?.quantityInStock ?? 0, [Validators.required, Validators.min(0)]],
      sku: [value?.sku ?? '', [Validators.required, Validators.maxLength(100), Validators.pattern(/^[A-Za-z0-9\-_]+$/)]]
    }) as SizeRowGroup;
  }
}
