import { AfterViewInit, Component, ElementRef, inject, viewChild } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../../../core/auth/auth.service';
import { GoogleAuthService } from '../../services/google-auth.service';
import { emailValidator } from '../../validators/email.validator';
import { passwordStrengthValidator } from '../../validators/password.validators';
import { LoadingService } from '../../../../core/services/loading.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { PasswordField } from '../../../../shared/ui/password-field/password-field';

@Component({
  selector: 'app-login',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatCheckboxModule,
    MatProgressSpinnerModule,
    PasswordField
  ],
  templateUrl: './login.html',
  styleUrl: '../auth.css',
})
export class Login implements AfterViewInit {

  private readonly formBuilder = inject(NonNullableFormBuilder);
  private readonly authService = inject(AuthService);
  private readonly googleAuthService = inject(GoogleAuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly notificationService = inject(NotificationService);

  readonly loading = inject(LoadingService);

  private readonly googleButton =
    viewChild.required<ElementRef<HTMLElement>>('googleButton');

  readonly form = this.formBuilder.group({
    email: ['', [Validators.required, emailValidator]],
    password: ['', [Validators.required, Validators.minLength(8), passwordStrengthValidator]],
    rememberMe: [false]
  });

  ngAfterViewInit(): void {
    this.googleAuthService
      .renderButton(
        this.googleButton().nativeElement,
        'signin_with',
        idToken => this.loginWithGoogle(idToken))
      .catch(() => this.notificationService.error('Google sign-in is unavailable right now.'));
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { email, password, rememberMe } = this.form.getRawValue();

    this.authService.login({ email, password }, rememberMe).subscribe({
      next: () => {
        this.notificationService.success('Logged in successfully. Welcome back!');
        this.navigateBack();
      },
      // failures are toasted by the error interceptor
      error: () => { }
    });
  }

  private loginWithGoogle(idToken: string): void {
    this.authService.loginWithGoogle(idToken).subscribe({
      next: (response) => {
        this.notificationService.success(response.isNewUser
          ? 'Account created with Google. Welcome!'
          : 'Logged in with Google. Welcome back!');
        this.navigateBack();
      },
      error: () => { }
    });
  }

  private navigateBack(): void {
    const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') ?? '/';
    this.router.navigateByUrl(returnUrl);
  }
}
