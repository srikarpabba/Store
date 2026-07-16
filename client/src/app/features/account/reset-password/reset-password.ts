import { Component, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
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
  selector: 'app-reset-password',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './reset-password.html',
  styleUrl: '../auth.css',
})
export class ResetPassword {

  private readonly formBuilder = inject(NonNullableFormBuilder);
  private readonly authService = inject(AuthService);
  private readonly notificationService = inject(NotificationService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  readonly loading = inject(LoadingService);

  private readonly email = this.route.snapshot.queryParamMap.get('email') ?? '';
  private readonly token = this.route.snapshot.queryParamMap.get('token') ?? '';

  /** False when the page was opened without the emailed link's parameters */
  readonly hasValidLink = this.email.length > 0 && this.token.length > 0;

  readonly form = this.formBuilder.group({
    password: ['', [Validators.required, Validators.minLength(8), passwordStrengthValidator]],
    confirmPassword: ['', Validators.required]
  }, { validators: passwordMatchValidator });

  readonly hidePassword = signal(true);

  constructor() {
    // Opening a reset link is account recovery — if a session is active
    // (e.g. the user logged in after requesting the reset), end it so the
    // flow finishes signed out. The API revokes refresh tokens on reset.
    if (this.hasValidLink && this.authService.isAuthenticated()) {
      this.authService.logout();
    }
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.authService.resetPassword({
      email: this.email,
      token: this.token,
      newPassword: this.form.getRawValue().password
    }).subscribe({
      next: () => {
        this.authService.logout();
        this.notificationService.success('Password reset successfully. Please log in.');
        this.router.navigateByUrl('/account/login');
      },
      // failures are toasted by the error interceptor
      error: () => { }
    });
  }

  togglePasswordVisibility(): void {
    this.hidePassword.update(hidden => !hidden);
  }
}
