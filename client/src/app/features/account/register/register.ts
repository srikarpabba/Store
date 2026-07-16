import { AfterViewInit, Component, ElementRef, inject, signal, viewChild } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { switchMap } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { GoogleAuthService } from '../services/google-auth.service';
import { passwordMatchValidator, passwordStrengthValidator } from '../validators/password.validators';
import { LoadingService } from '../../../shared/services/loading.service';
import { NotificationService } from '../../../shared/services/notification.service';

@Component({
  selector: 'app-register',
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatCheckboxModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './register.html',
  styleUrl: '../auth.css',
})
export class Register implements AfterViewInit {

  private readonly formBuilder = inject(NonNullableFormBuilder);
  private readonly authService = inject(AuthService);
  private readonly googleAuthService = inject(GoogleAuthService);
  private readonly router = inject(Router);
  private readonly notificationService = inject(NotificationService);

  readonly loading = inject(LoadingService);

  private readonly googleButton =
    viewChild.required<ElementRef<HTMLElement>>('googleButton');

  readonly form = this.formBuilder.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8), passwordStrengthValidator]],
    confirmPassword: ['', Validators.required],
    acceptTerms: [false, Validators.requiredTrue]
  }, { validators: passwordMatchValidator });

  readonly hidePassword = signal(true);

  ngAfterViewInit(): void {
    this.googleAuthService
      .renderButton(
        this.googleButton().nativeElement,
        'signup_with',
        idToken => this.loginWithGoogle(idToken))
      .catch(() => this.notificationService.error('Google sign-up is unavailable right now.'));
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { confirmPassword, acceptTerms, ...request } = this.form.getRawValue();

    this.authService.register(request).pipe(
      switchMap(() => this.authService.login({
        email: request.email,
        password: request.password
      }))
    ).subscribe({
      next: () => {
        this.notificationService.success('Account created successfully. Welcome!');
        this.router.navigateByUrl('/');
      },
      // failures are toasted by the error interceptor
      error: () => { }
    });
  }

  togglePasswordVisibility(): void {
    this.hidePassword.update(hidden => !hidden);
  }

  private loginWithGoogle(idToken: string): void {
    this.authService.loginWithGoogle(idToken).subscribe({
      next: (response) => {
        this.notificationService.success(response.isNewUser
          ? 'Account created with Google. Welcome!'
          : 'You already have an account — logged in with Google.');
        this.router.navigateByUrl('/');
      },
      error: () => { }
    });
  }
}
