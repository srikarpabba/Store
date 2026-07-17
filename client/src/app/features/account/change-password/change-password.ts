import { Component, inject } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../../core/auth/auth.service';
import { UserService } from '../services/user.service';
import { passwordMatchValidator, passwordStrengthValidator } from '../validators/password.validators';
import { LoadingService } from '../../../core/services/loading.service';
import { NotificationService } from '../../../core/services/notification.service';
import { PasswordField } from '../../../shared/ui/password-field/password-field';

@Component({
  selector: 'app-change-password',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    PasswordField
  ],
  templateUrl: './change-password.html',
  styleUrl: '../auth.css',
})
export class ChangePassword {

  private readonly formBuilder = inject(NonNullableFormBuilder);
  private readonly authService = inject(AuthService);
  private readonly userService = inject(UserService);
  private readonly notificationService = inject(NotificationService);
  private readonly router = inject(Router);

  readonly loading = inject(LoadingService);

  readonly form = this.formBuilder.group({
    currentPassword: ['', Validators.required],
    password: ['', [Validators.required, Validators.minLength(8), passwordStrengthValidator]],
    confirmPassword: ['', Validators.required]
  }, { validators: passwordMatchValidator });

  constructor() {
    // Google sign-in accounts without a local password can't provide a
    // current password — they set one via the set password flow instead
    this.userService.getProfile().subscribe(profile => {
      if (!profile.hasPassword) {
        this.router.navigateByUrl('/account/set-password', { replaceUrl: true });
      }
    });
  }

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
}
