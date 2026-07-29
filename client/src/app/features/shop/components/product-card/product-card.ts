import { Component, DestroyRef, computed, effect, inject, input, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { Product } from '../../models/product';
import { ProductColorDetails } from '../../models/product-details';
import { WishlistService } from '../../services/wishlist.service';
import { AuthService } from '../../../../core/auth/auth.service';
import { PricePipe } from '../../../../shared/pipes/price.pipe';
import { applyDiscount } from '../../../../shared/utils/discount';
import { HeartBurst } from '../../../../shared/ui/heart-burst/heart-burst';

/** Grid card for the shop listing — color swap, hover-to-cycle photos, wishlist toggle. */
@Component({
  selector: 'app-product-card',
  imports: [RouterLink, MatIconModule, PricePipe, HeartBurst],
  templateUrl: './product-card.html',
  styleUrl: './product-card.css',
})
export class ProductCard {

  private static readonly HOVER_INTERVAL_MS = 1200;

  private readonly destroyRef = inject(DestroyRef);
  private readonly wishlistService = inject(WishlistService);
  private readonly authService = inject(AuthService);

  readonly product = input.required<Product>();

  readonly section = input.required<string>();

  /** null until the shopper explicitly clicks a swatch — kept null-by-default
   *  so navigating a product the shopper never touched doesn't append ?color= */
  readonly selectedColorId = signal<string | null>(null);

  readonly photoIndex = signal(0);

  readonly isHovering = signal(false);

  readonly wishlisted = computed(() => this.wishlistService.isWishlisted(this.product().id));

  /** Pulses true just after the heart is filled (not on removal, and not
      for an item that was already wishlisted on load) — drives the
      confetti burst. */
  readonly justWishlisted = signal(false);

  private hasWishlistStateSettled = false;

  /** Wishlisting is Customer-only server-side — shown for guests too
      (clicking it prompts sign-in), hidden only for a signed-in
      Admin/Manager who'd otherwise hit a dead-end 403. */
  readonly showWishlist = computed(() =>
    !this.authService.isAuthenticated() || this.authService.isCustomer());

  private hoverIntervalId: ReturnType<typeof setInterval> | null = null;

  // Only colors that actually have photos. A swatch for a photo-less color
  // has no image to show and would fall back to another color's photo —
  // a guaranteed swatch/image mismatch — so it's hidden on the card.
  readonly colors = computed<ProductColorDetails[]>(() =>
    (this.product().colors ?? []).filter(c => c.photos.length > 0));

  // No stock/variant data exists at list level (unlike product-details), so
  // there's no "first in-stock color" to prefer — just the first listed one.
  readonly selectedColor = computed<ProductColorDetails | null>(() => {
    const colors = this.colors();
    const selectedId = this.selectedColorId();
    return colors.find(c => c.colorId === selectedId) ?? colors[0] ?? null;
  });

  readonly photos = computed(() => this.selectedColor()?.photos ?? []);

  /** Discounted price, or null when the product isn't currently on sale */
  readonly salePrice = computed(() => {
    const discount = this.product().discountPercentage;
    return discount ? applyDiscount(this.product().startingPrice, discount) : null;
  });

  readonly displayedPhoto = computed(() => {
    const photos = this.photos();
    return photos.length === 0 ? null : photos[this.photoIndex() % photos.length];
  });

  readonly queryParams = computed(() => {
    const explicitId = this.selectedColorId();
    return explicitId ? { color: explicitId } : {};
  });

  constructor() {
    this.destroyRef.onDestroy(() => this.clearHoverInterval());

    // Fires the burst only on a true false->true transition — the first
    // time this runs just establishes the starting state (which may
    // already be wishlisted, e.g. on the wishlist page itself) without
    // celebrating anything.
    effect(() => {
      const isWishlisted = this.wishlisted();

      if (this.hasWishlistStateSettled && isWishlisted) {
        this.justWishlisted.set(true);
        setTimeout(() => this.justWishlisted.set(false), 600);
      }

      this.hasWishlistStateSettled = true;
    });
  }

  selectColor(colorId: string): void {
    this.selectedColorId.set(colorId);
    this.photoIndex.set(0);
  }

  toggleWishlist(): void {
    this.wishlistService.toggle(this.product().id);
  }

  onMouseEnter(): void {

    this.isHovering.set(true);

    const photos = this.photos();

    if (photos.length <= 1) {
      return;
    }

    this.photoIndex.set(0);

    this.hoverIntervalId = setInterval(() => {
      this.photoIndex.update(i => (i + 1) % photos.length);
    }, ProductCard.HOVER_INTERVAL_MS);
  }

  onMouseLeave(): void {
    this.isHovering.set(false);
    this.clearHoverInterval();
    this.photoIndex.set(0);
  }

  private clearHoverInterval(): void {
    if (this.hoverIntervalId !== null) {
      clearInterval(this.hoverIntervalId);
      this.hoverIntervalId = null;
    }
  }
}
