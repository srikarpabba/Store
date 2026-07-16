import { Component, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Observable } from 'rxjs';
import { Address } from '../models/address';
import { Profile } from '../models/profile';
import { UserService } from '../services/user.service';
import { LoadingService } from '../../../shared/services/loading.service';
import { NotificationService } from '../../../shared/services/notification.service';

@Component({
  selector: 'app-dashboard',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatTooltipModule
  ],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard {

  private readonly formBuilder = inject(NonNullableFormBuilder);
  private readonly userService = inject(UserService);
  private readonly notificationService = inject(NotificationService);

  readonly loading = inject(LoadingService);

  readonly profile = signal<Profile | null>(null);
  readonly addresses = signal<Address[]>([]);

  /** null = form closed; '' = adding; otherwise the id being edited */
  readonly editingAddressId = signal<string | null>(null);

  readonly profileForm = this.formBuilder.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    phoneNumber: ['']
  });

  readonly addressForm = this.formBuilder.group({
    line1: ['', Validators.required],
    line2: [''],
    city: ['', Validators.required],
    state: ['', Validators.required],
    postalCode: ['', Validators.required],
    country: ['India', Validators.required]
  });

  constructor() {
    this.loadProfile();
    this.loadAddresses();
  }

  // ---------- Profile ----------

  saveProfile(): void {
    if (this.profileForm.invalid) {
      this.profileForm.markAllAsTouched();
      return;
    }

    const { firstName, lastName, email, phoneNumber } = this.profileForm.getRawValue();
    const emailChanged = email.trim().toLowerCase() !== this.profile()?.email.toLowerCase();

    this.userService.updateProfile({
      firstName,
      lastName,
      email,
      phoneNumber: phoneNumber.trim() || null
    }).subscribe({
      next: () => {
        this.notificationService.success(emailChanged
          ? 'Profile updated. Please verify your new email address — we sent you a link.'
          : 'Profile updated successfully.');
        this.loadProfile();
      },
      // failures are toasted by the error interceptor
      error: () => { }
    });
  }

  resendVerification(): void {
    this.userService.resendEmailConfirmation().subscribe({
      next: () => this.notificationService.success('Verification email sent. Check your inbox.'),
      error: () => { }
    });
  }

  // ---------- Addresses ----------

  startAddAddress(): void {
    this.addressForm.reset({ country: 'India' });
    this.editingAddressId.set('');
  }

  startEditAddress(address: Address): void {
    this.addressForm.setValue({
      line1: address.line1,
      line2: address.line2 ?? '',
      city: address.city,
      state: address.state,
      postalCode: address.postalCode,
      country: address.country
    });
    this.editingAddressId.set(address.id);
  }

  cancelAddressForm(): void {
    this.editingAddressId.set(null);
  }

  saveAddress(): void {
    if (this.addressForm.invalid) {
      this.addressForm.markAllAsTouched();
      return;
    }

    const { line1, line2, city, state, postalCode, country } = this.addressForm.getRawValue();
    const request = { line1, line2: line2.trim() || null, city, state, postalCode, country };

    const editingId = this.editingAddressId();

    const save$: Observable<unknown> = editingId
      ? this.userService.updateAddress(editingId, request)
      : this.userService.addAddress(request);

    save$.subscribe({
      next: () => {
        this.notificationService.success(editingId ? 'Address updated.' : 'Address added.');
        this.editingAddressId.set(null);
        this.loadAddresses();
      },
      error: () => { }
    });
  }

  deleteAddress(address: Address): void {
    this.userService.deleteAddress(address.id).subscribe({
      next: () => {
        this.notificationService.success('Address removed.');
        this.loadAddresses();
      },
      error: () => { }
    });
  }

  setDefaultAddress(address: Address): void {
    this.userService.setDefaultAddress(address.id).subscribe({
      next: () => {
        this.notificationService.success('Default delivery address updated.');
        this.loadAddresses();
      },
      error: () => { }
    });
  }

  // ---------- Loaders ----------

  private loadProfile(): void {
    this.userService.getProfile().subscribe(profile => {
      this.profile.set(profile);
      this.profileForm.patchValue({
        firstName: profile.firstName,
        lastName: profile.lastName,
        email: profile.email,
        phoneNumber: profile.phoneNumber ?? ''
      });
    });
  }

  private loadAddresses(): void {
    this.userService.getAddresses().subscribe(addresses =>
      this.addresses.set(addresses));
  }
}
