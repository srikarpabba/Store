import { Component, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../../../core/auth/auth.service';
import { emailValidator } from '../../validators/email.validator';
import { LoadingService } from '../../../../core/services/loading.service';

@Component({
  selector: 'app-forgot-password',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './forgot-password.html',
  styleUrl: '../auth.css',
})
export class ForgotPassword {

  private readonly formBuilder = inject(NonNullableFormBuilder);
  private readonly authService = inject(AuthService);

  readonly loading = inject(LoadingService);

  readonly form = this.formBuilder.group({
    email: ['', [Validators.required, emailValidator]]
  });

  readonly emailSent = signal(false);

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.authService.forgotPassword(this.form.getRawValue().email).subscribe({
      next: () => this.emailSent.set(true),
      // failures are toasted by the error interceptor
      error: () => { }
    });
  }
}
