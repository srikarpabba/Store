import { CommonModule } from '@angular/common';
import { Component, ElementRef, HostListener, computed, effect, inject, signal } from '@angular/core';
import { MatBadgeModule } from '@angular/material/badge';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';
import { LoadingService } from '../../core/services/loading.service';
import { Search } from '../../features/shop/components/search/search';
import { WishlistService } from '../../features/shop/services/wishlist.service';
import { NAV_ITEMS } from '../../shared/constants/navigation';
import { PricePipe } from '../../shared/pipes/price.pipe';
import { HeartBurst } from '../../shared/ui/heart-burst/heart-burst';

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
    HeartBurst,
  ],
  templateUrl: './header.html',
  styleUrl: './header.css',
})
export class Header {
  private readonly router = inject(Router);

  /** Auth state driving the account menu */
  readonly auth = inject(AuthService);

  private readonly wishlistService = inject(WishlistService);

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

  /** Replace with real state once cart exists */
  readonly cartCount = signal(2);

  readonly wishlistCount = computed(() => this.wishlistService.items().length);

  /** Pulses true just after the count rises (an add, not a remove) — drives
      the header heart's confetti burst. Doesn't fire on initial load, even
      if the wishlist already has items. */
  readonly justAddedToWishlist = signal(false);

  private previousWishlistCount = 0;
  private hasWishlistCountSettled = false;

  constructor(private readonly elementRef: ElementRef<HTMLElement>) {
    effect(() => {
      const count = this.wishlistCount();

      if (this.hasWishlistCountSettled && count > this.previousWishlistCount) {
        this.justAddedToWishlist.set(true);
        setTimeout(() => this.justAddedToWishlist.set(false), 600);
      }

      this.previousWishlistCount = count;
      this.hasWishlistCountSettled = true;
    });
  }

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
