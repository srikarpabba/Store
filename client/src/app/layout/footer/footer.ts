import { Component, inject } from '@angular/core';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-footer',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './footer.html',
  styleUrl: './footer.css',
})
export class Footer {

  private readonly notificationService = inject(NotificationService);

  readonly currentYear = new Date().getFullYear();

  readonly email = new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.email] });

  subscribe(): void {
    if (this.email.invalid) {
      this.email.markAsTouched();
      this.notificationService.error('Please enter a valid email address.');
      return;
    }

    // newsletter backend lands later — acknowledge the signup for now
    this.notificationService.success('Thanks for subscribing!');
    this.email.reset();
  }
}
