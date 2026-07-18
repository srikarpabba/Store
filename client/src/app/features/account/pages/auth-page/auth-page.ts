import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-auth-page',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './auth-page.html',
  styleUrl: '../auth.css',
})
export class AuthPage {}
