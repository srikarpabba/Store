import { AbstractControl, ValidationErrors } from '@angular/forms';

/** Requires an uppercase letter, a lowercase letter, a number and a special character. */
export function passwordStrengthValidator(control: AbstractControl): ValidationErrors | null {
    const value: string = control.value;

    if (!value) {
        return null;
    }

    const hasUpperCase = /[A-Z]/.test(value);
    const hasLowerCase = /[a-z]/.test(value);
    const hasNumeric = /[0-9]/.test(value);
    const hasSpecialChar = /[^a-zA-Z0-9]/.test(value);

    const passwordValid = hasUpperCase && hasLowerCase && hasNumeric && hasSpecialChar;

    return passwordValid ? null : { passwordStrength: true };
}

/** Group validator: `password` and `confirmPassword` must be equal. */
export function passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
    const password = control.get('password');
    const confirmPassword = control.get('confirmPassword');

    if (!password || !confirmPassword) {
        return null;
    }

    return password.value === confirmPassword.value ? null : { passwordMismatch: true };
}
