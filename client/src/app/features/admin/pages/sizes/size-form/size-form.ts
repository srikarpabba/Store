import { Component, inject } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AdminSizeService } from '../../../services/admin-size.service';
import { SaveSizeRequest } from '../../../models/save-size-request';
import { Size } from '../../../../shop/models/size';
import { HasPendingChanges } from '../../../../../core/guards/pending-changes.guard';
import { LoadingService } from '../../../../../core/services/loading.service';
import { NotificationService } from '../../../../../core/services/notification.service';

@Component({
  selector: 'app-size-form',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './size-form.html',
  styleUrl: './size-form.css',
})
export class SizeForm implements HasPendingChanges {

  private readonly formBuilder = inject(NonNullableFormBuilder);
  private readonly adminSizeService = inject(AdminSizeService);
  private readonly notificationService = inject(NotificationService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  readonly loading = inject(LoadingService);

  /** null when creating; the size id when editing */
  readonly sizeId = this.route.snapshot.paramMap.get('id');

  readonly isEdit = this.sizeId !== null;

  readonly form = this.formBuilder.group({
    name: ['', [Validators.required, Validators.maxLength(10)]]
  });

  constructor() {
    if (this.sizeId) {
      this.adminSizeService.getById(this.sizeId).subscribe(size => this.populate(size));
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

    const { name } = this.form.getRawValue();

    const request: SaveSizeRequest = {
      name: name.trim()
    };

    if (this.sizeId) {
      this.adminSizeService.update(this.sizeId, request).subscribe({
        next: () => {
          this.notificationService.success('Size updated.');
          this.form.markAsPristine();
        },
        // failures (e.g. duplicate name) are toasted by the error interceptor
        error: () => { }
      });
    } else {
      this.adminSizeService.create(request).subscribe({
        next: () => {
          this.notificationService.success('Size created.');
          this.form.markAsPristine();
          this.router.navigate(['/admin/product-management/sizes']);
        },
        error: () => { }
      });
    }
  }

  private populate(size: Size): void {
    this.form.patchValue({
      name: size.name
    });
  }
}
