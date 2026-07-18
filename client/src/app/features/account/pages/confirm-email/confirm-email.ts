import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../../../core/auth/auth.service';

type ConfirmationState = 'confirming' | 'confirmed' | 'invalid';

@Component({
  selector: 'app-confirm-email',
  imports: [RouterLink, MatButtonModule, MatIconModule, MatProgressSpinnerModule],
  templateUrl: './confirm-email.html',
  styleUrl: '../auth.css',
})
export class ConfirmEmail {

  private readonly authService = inject(AuthService);
  private readonly route = inject(ActivatedRoute);

  readonly state = signal<ConfirmationState>('confirming');

  constructor() {
    const email = this.route.snapshot.queryParamMap.get('email');
    const token = this.route.snapshot.queryParamMap.get('token');

    if (!email || !token) {
      this.state.set('invalid');
      return;
    }

    this.authService.confirmEmail(email, token).subscribe({
      next: () => this.state.set('confirmed'),
      error: () => this.state.set('invalid')
    });
  }
}
