import { AbstractControl, ValidationErrors } from '@angular/forms';

/**
 * Stricter than Angular's `Validators.email`, which follows the WHATWG spec
 * and accepts domains without a TLD (e.g. `user@h`). Real customer emails
 * always have a dot-separated domain, so require one to catch typos early.
 * Returns the same `email` error key as the built-in validator.
 */
export function emailValidator(control: AbstractControl): ValidationErrors | null {
    const value: string = control.value;

    if (!value) {
        return null;
    }

    return /^[^\s@]+@[^\s@]+\.[^\s@]{2,}$/.test(value) ? null : { email: true };
}
