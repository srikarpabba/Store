import { Component, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../services/auth.service';
import { passwordMatchValidator, passwordStrengthValidator } from '../validators/password.validators';
import { LoadingService } from '../../../shared/services/loading.service';
import { NotificationService } from '../../../shared/services/notification.service';

@Component({
  selector: 'app-change-password',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './change-password.html',
  styleUrl: '../auth.css',
})
export class ChangePassword {

  private readonly formBuilder = inject(NonNullableFormBuilder);
  private readonly authService = inject(AuthService);
  private readonly notificationService = inject(NotificationService);
  private readonly router = inject(Router);

  readonly loading = inject(LoadingService);

  readonly form = this.formBuilder.group({
    currentPassword: ['', Validators.required],
    password: ['', [Validators.required, Validators.minLength(8), passwordStrengthValidator]],
    confirmPassword: ['', Validators.required]
  }, { validators: passwordMatchValidator });

  readonly hidePassword = signal(true);

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { currentPassword, password } = this.form.getRawValue();

    this.authService.changePassword({
      currentPassword,
      newPassword: password
    }).subscribe({
      next: () => {
        this.notificationService.success('Password changed successfully.');
        this.router.navigateByUrl('/');
      },
      // failures are toasted by the error interceptor
      error: () => { }
    });
  }

  togglePasswordVisibility(): void {
    this.hidePassword.update(hidden => !hidden);
  }
}
