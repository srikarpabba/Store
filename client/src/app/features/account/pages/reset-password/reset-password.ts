import { Component, inject } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../../../core/auth/auth.service';
import { passwordMatchValidator, passwordStrengthValidator } from '../../validators/password.validators';
import { LoadingService } from '../../../../core/services/loading.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { PasswordField } from '../../../../shared/ui/password-field/password-field';

@Component({
  selector: 'app-reset-password',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    PasswordField
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
}
