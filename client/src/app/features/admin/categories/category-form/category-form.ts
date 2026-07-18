import { Component, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatTooltipModule } from '@angular/material/tooltip';
import { filter, switchMap } from 'rxjs';
import { AdminCategoryService } from '../../services/admin-category.service';
import { SaveCategoryRequest } from '../../models/save-category-request';
import { Category, CategoryGenderInfo } from '../../../shop/models/category';
import { Lookup } from '../../../shop/models/lookup';
import { ProductService } from '../../../shop/services/product.service';
import { HasPendingChanges } from '../../../../core/guards/pending-changes.guard';
import { LoadingService } from '../../../../core/services/loading.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { ConfirmDialogService } from '../../../../shared/ui/confirm-dialog/confirm-dialog.service';

@Component({
  selector: 'app-category-form',
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
  templateUrl: './category-form.html',
  styleUrl: './category-form.css',
})
export class CategoryForm implements HasPendingChanges {

  private readonly formBuilder = inject(NonNullableFormBuilder);
  private readonly productService = inject(ProductService);
  private readonly adminCategoryService = inject(AdminCategoryService);
  private readonly notificationService = inject(NotificationService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  readonly loading = inject(LoadingService);

  /** null when creating; the category id when editing */
  readonly categoryId = this.route.snapshot.paramMap.get('id');

  readonly isEdit = this.categoryId !== null;

  readonly allGenders = signal<Lookup[]>([]);

  /** Loaded category in edit mode; drives the per-gender photo manager */
  readonly category = signal<Category | null>(null);

  readonly form = this.formBuilder.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    description: ['', Validators.maxLength(1000)],
    genderIds: [<string[]>[], Validators.required]
  });

  constructor() {
    this.productService.getFilters().subscribe(filters => this.allGenders.set(filters.genders));

    if (this.categoryId) {
      this.adminCategoryService.getById(this.categoryId).subscribe(category => this.populate(category));
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

    const { name, description, genderIds } = this.form.getRawValue();

    const request: SaveCategoryRequest = {
      name: name.trim(),
      description: description.trim() || null,
      genderIds
    };

    if (this.categoryId) {
      this.adminCategoryService.update(this.categoryId, request).subscribe({
        next: () => {
          this.notificationService.success('Category updated.');
          this.reloadCategory();
        },
        // failures are toasted by the error interceptor
        error: () => { }
      });
    } else {
      this.adminCategoryService.create(request).subscribe({
        next: id => {
          this.notificationService.success('Category created. You can now upload photos.');
          this.form.markAsPristine();
          this.router.navigate(['/admin/product-management/categories', id, 'edit']);
        },
        error: () => { }
      });
    }
  }

  private reloadCategory(): void {
    if (!this.categoryId) {
      return;
    }

    this.adminCategoryService.getById(this.categoryId).subscribe(category => {
      this.populate(category);
      this.form.markAsPristine();
    });
  }

  // ---------- Per-gender photo (edit mode only) ----------

  onFileSelected(gender: CategoryGenderInfo, input: HTMLInputElement): void {
    const file = input.files?.[0] ?? null;
    input.value = '';

    if (!this.categoryId || !file) {
      return;
    }

    this.adminCategoryService.uploadGenderPhoto(this.categoryId, gender.genderId, file).subscribe({
      next: () => {
        this.notificationService.success('Photo uploaded.');
        this.refreshCategory();
      },
      // failures are toasted by the error interceptor
      error: () => { }
    });
  }

  deletePhoto(gender: CategoryGenderInfo): void {
    if (!this.categoryId) {
      return;
    }

    const categoryId = this.categoryId;

    this.confirmDialog.confirm({
      title: 'Delete this photo?',
      message: 'The photo will be removed permanently.',
      confirmLabel: 'Delete',
      destructive: true
    }).pipe(
      filter(confirmed => confirmed),
      switchMap(() => this.adminCategoryService.deleteGenderPhoto(categoryId, gender.genderId))
    ).subscribe({
      next: () => {
        this.notificationService.success('Photo deleted.');
        this.refreshCategory();
      },
      error: () => { }
    });
  }

  /** Reloads only the category signal — never re-patches the form,
      so in-progress edits survive photo operations */
  private refreshCategory(): void {
    if (this.categoryId) {
      this.adminCategoryService.getById(this.categoryId).subscribe(category =>
        this.category.set(category));
    }
  }

  private populate(category: Category): void {
    this.category.set(category);

    this.form.patchValue({
      name: category.name,
      description: category.description ?? '',
      genderIds: category.genders.map(g => g.genderId)
    });
  }
}
