import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-server-error',
  imports: [RouterLink, MatButtonModule, MatIconModule],
  templateUrl: './server-error.html',
  styleUrl: './server-error.css',
})
export class ServerError {

  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  /** Retries the page the user was on when the connection failed */
  tryAgain(): void {
    const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') ?? '/';
    this.router.navigateByUrl(returnUrl);
  }
}
