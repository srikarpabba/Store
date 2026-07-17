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
  selector: 'app-set-password',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    PasswordField
  ],
  templateUrl: './set-password.html',
  styleUrl: '../auth.css',
})
export class SetPassword {

  private readonly formBuilder = inject(NonNullableFormBuilder);
  private readonly authService = inject(AuthService);
  private readonly userService = inject(UserService);
  private readonly notificationService = inject(NotificationService);
  private readonly router = inject(Router);

  readonly loading = inject(LoadingService);

  readonly form = this.formBuilder.group({
    password: ['', [Validators.required, Validators.minLength(8), passwordStrengthValidator]],
    confirmPassword: ['', Validators.required]
  }, { validators: passwordMatchValidator });

  constructor() {
    // Only for accounts without a local password — anyone who already
    // has one belongs on the change password page instead
    this.userService.getProfile().subscribe(profile => {
      if (profile.hasPassword) {
        this.router.navigateByUrl('/account/change-password', { replaceUrl: true });
      }
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.authService.setPassword(this.form.getRawValue().password).subscribe({
      next: () => {
        this.notificationService.success('Password set successfully. You can now log in with your email and password.');
        this.router.navigateByUrl('/account/dashboard');
      },
      // failures are toasted by the error interceptor
      error: () => { }
    });
  }
}
