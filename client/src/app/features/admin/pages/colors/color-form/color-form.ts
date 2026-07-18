import { Component, inject } from '@angular/core';
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
import { ColorPicker } from '../../../../../shared/ui/color-picker/color-picker';
import { nearestColorName } from '../../../../../shared/ui/color-picker/named-colors';

@Component({
  selector: 'app-color-form',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    ColorPicker
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

  /** While true, the name field tracks the picked color's nearest name. Turns
   *  off the moment the admin types their own name (and stays off in edit
   *  mode, so a loaded name is never clobbered). */
  private autoName = true;

  /** Guards the name write below so our own auto-fill isn't mistaken for a
   *  manual edit. */
  private settingName = false;

  readonly form = this.formBuilder.group({
    name: ['', [Validators.required, Validators.maxLength(50)]],
    hexCode: ['#000000', [Validators.required, Validators.pattern(ColorForm.HEX_PATTERN)]]
  });

  constructor() {
    if (this.colorId) {
      this.adminColorService.getById(this.colorId).subscribe(color => this.populate(color));
    }

    // Suggest a name from the picked color (picker drag, preset, or typed hex),
    // until the admin takes over the name themselves.
    this.form.controls.hexCode.valueChanges
      .pipe(takeUntilDestroyed())
      .subscribe(hex => {
        if (this.autoName && ColorForm.HEX_PATTERN.test(hex)) {
          this.settingName = true;
          this.form.controls.name.setValue(nearestColorName(hex));
          this.settingName = false;
        }
      });

    this.form.controls.name.valueChanges
      .pipe(takeUntilDestroyed())
      .subscribe(() => {
        if (!this.settingName) {
          this.autoName = false;
        }
      });
  }

  hasPendingChanges(): boolean {
    return this.form.dirty;
  }

  onPickerChange(hex: string): void {
    this.form.controls.hexCode.setValue(hex.toUpperCase());
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
    // editing an existing color — keep its saved name, don't auto-overwrite
    this.autoName = false;

    this.form.patchValue({
      name: color.name,
      hexCode: color.hexCode
    });
  }
}
