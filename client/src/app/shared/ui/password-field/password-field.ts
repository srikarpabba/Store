import { Component, input, signal } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';

/**
 * Password input with prefix icon, its own visibility toggle and the
 * standard required / minlength / passwordStrength error messages.
 * Each instance manages visibility independently.
 */
@Component({
  selector: 'app-password-field',
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './password-field.html',
  styleUrl: './password-field.css',
})
export class PasswordField {

  readonly control = input.required<FormControl<string>>();

  readonly label = input('Password');

  readonly autocomplete = input('new-password');

  readonly prefixIcon = input('lock_outline');

  /** Shown under the field (enables dynamic subscript sizing) */
  readonly hint = input<string | null>(null);

  /** Hides the visibility toggle for fields that should never reveal */
  readonly toggleable = input(true);

  readonly requiredMessage = input('Password is required');

  readonly hide = signal(true);

  togglePasswordVisibility(): void {
    this.hide.update(hidden => !hidden);
  }
}
