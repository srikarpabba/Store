import { Component, ElementRef, inject, viewChild } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AdminColorService } from '../../../services/admin-color.service';
import { SaveColorRequest } from '../../../models/save-color-request';
import { Color } from '../../../../shop/models/color';
import { HasPendingChanges } from '../../../../../core/guards/pending-changes.guard';
import { LoadingService } from '../../../../../core/services/loading.service';
import { NotificationService } from '../../../../../core/services/notification.service';

@Component({
  selector: 'app-color-form',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './color-form.html',
  styleUrl: './color-form.css',
})
export class ColorForm implements HasPendingChanges {

  private static readonly HEX_PATTERN = /^#[0-9A-Fa-f]{6}$/;

  private readonly formBuilder = inject(NonNullableFormBuilder);
  private readonly adminColorService = inject(AdminColorService);
  private readonly notificationService = inject(NotificationService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  readonly loading = inject(LoadingService);

  /** null when creating; the color id when editing */
  readonly colorId = this.route.snapshot.paramMap.get('id');

  readonly isEdit = this.colorId !== null;

  private readonly picker = viewChild<ElementRef<HTMLInputElement>>('picker');

  readonly form = this.formBuilder.group({
    name: ['', [Validators.required, Validators.maxLength(50)]],
    hexCode: ['#000000', [Validators.required, Validators.pattern(ColorForm.HEX_PATTERN)]]
  });

  constructor() {
    if (this.colorId) {
      this.adminColorService.getById(this.colorId).subscribe(color => this.populate(color));
    }

    // Mirror the hex into the native picker when it's typed or loaded. We only
    // write when it actually differs from what the picker already shows — so a
    // picker-originated change (drag) is a no-op here and never resets the
    // native cursor mid-drag. Native color inputs use lowercase #rrggbb.
    this.form.controls.hexCode.valueChanges
      .pipe(takeUntilDestroyed())
      .subscribe(value => {
        const el = this.picker()?.nativeElement;

        if (!el || !ColorForm.HEX_PATTERN.test(value)) {
          return;
        }

        const normalized = value.toLowerCase();

        if (el.value !== normalized) {
          el.value = normalized;
        }
      });
  }

  hasPendingChanges(): boolean {
    return this.form.dirty;
  }

  onPickerInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value.toUpperCase();

    this.form.controls.hexCode.setValue(value);
    this.form.controls.hexCode.markAsDirty();
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { name, hexCode } = this.form.getRawValue();

    const request: SaveColorRequest = {
      name: name.trim(),
      hexCode: hexCode.toUpperCase()
    };

    if (this.colorId) {
      this.adminColorService.update(this.colorId, request).subscribe({
        next: () => {
          this.notificationService.success('Color updated.');
          this.form.markAsPristine();
        },
        // failures (e.g. duplicate name) are toasted by the error interceptor
        error: () => { }
      });
    } else {
      this.adminColorService.create(request).subscribe({
        next: () => {
          this.notificationService.success('Color created.');
          this.form.markAsPristine();
          this.router.navigate(['/admin/product-management/colors']);
        },
        error: () => { }
      });
    }
  }

  private populate(color: Color): void {
    this.form.patchValue({
      name: color.name,
      hexCode: color.hexCode
    });
  }
}
