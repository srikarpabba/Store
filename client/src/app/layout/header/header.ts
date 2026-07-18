import { CommonModule } from '@angular/common';
import { Component, HostListener, ElementRef, signal, inject } from '@angular/core';
import { MatBadgeModule } from '@angular/material/badge';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { NAV_ITEMS } from '../../shared/constants/navigation';
import { AuthService } from '../../core/auth/auth.service';
import { LoadingService } from '../../core/services/loading.service';
import { PricePipe } from '../../shared/pipes/price.pipe';
import { Search } from '../../features/shop/components/search/search';

@Component({
  selector: 'app-header',
  imports: [
    CommonModule,
    RouterLink,
    RouterLinkActive,
    MatIconModule,
    MatButtonModule,
    MatBadgeModule,
    MatMenuModule,
    MatDividerModule,
    MatProgressBarModule,
    PricePipe,
    Search,
  ],
  templateUrl: './header.html',
  styleUrl: './header.css',
})
export class Header {
  private readonly router = inject(Router);

  /** Auth state driving the account menu */
  readonly auth = inject(AuthService);

  /** API activity driving the progress bar under the header */
  readonly loading = inject(LoadingService);

  /** Primary nav, edit to match catalog */
  readonly navItems = NAV_ITEMS;

  /** Order value above which shipping is free, shown in the utility bar */
  readonly freeShippingThreshold = 999;

  /** Mobile nav drawer open state */
  readonly isMobileMenuOpen = signal(false);

  /** Account dropdown open state — plain CSS dropdown, no animations module needed */
  readonly isAccountMenuOpen = signal(false);

  /** True once the user has scrolled past the top — used for the shadow/border */
  readonly isScrolled = signal(false);

  /** Replace with real state from a cart/wishlist service (signal, store, etc.) */
  readonly cartCount = signal(2);
  readonly wishlistCount = signal(3);

  constructor(private readonly elementRef: ElementRef<HTMLElement>) { }

  @HostListener('window:scroll')
  onWindowScroll(): void {
    this.isScrolled.set(window.scrollY > 4);
  }

  // Close the account dropdown on any click outside the header
  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (
      this.isAccountMenuOpen() &&
      !this.elementRef.nativeElement.contains(event.target as Node)
    ) {
      this.isAccountMenuOpen.set(false);
    }
  }

  @HostListener('document:keydown.escape')
  onEscape(): void {
    this.isAccountMenuOpen.set(false);
  }

  toggleMobileMenu(): void {
    this.isMobileMenuOpen.update((open) => !open);
  }

  closeMobileMenu(): void {
    this.isMobileMenuOpen.set(false);
  }

  toggleAccountMenu(): void {
    this.isAccountMenuOpen.update((open) => !open);
  }

  closeAccountMenu(): void {
    this.isAccountMenuOpen.set(false);
  }

  signOut(): void {
    this.auth.logout();
    this.closeAccountMenu();
    this.router.navigateByUrl('/');
  }
}
