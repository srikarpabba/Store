import { Component, DestroyRef, computed, inject, input, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { Product } from '../../models/product';
import { ProductColorDetails } from '../../models/product-details';
import { PricePipe } from '../../../../shared/pipes/price.pipe';

/** Grid card for the shop listing — color swap, hover-to-cycle photos, wishlist toggle. */
@Component({
  selector: 'app-product-card',
  imports: [RouterLink, MatIconModule, PricePipe],
  templateUrl: './product-card.html',
  styleUrl: './product-card.css',
})
export class ProductCard {

  private static readonly HOVER_INTERVAL_MS = 1200;

  private readonly destroyRef = inject(DestroyRef);

  readonly product = input.required<Product>();

  readonly section = input.required<string>();

  /** null until the shopper explicitly clicks a swatch — kept null-by-default
   *  so navigating a product the shopper never touched doesn't append ?color= */
  readonly selectedColorId = signal<string | null>(null);

  readonly photoIndex = signal(0);

  readonly isHovering = signal(false);

  readonly wishlisted = signal(false);

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
  }

  selectColor(colorId: string): void {
    this.selectedColorId.set(colorId);
    this.photoIndex.set(0);
  }

  toggleWishlist(): void {
    this.wishlisted.update(value => !value);
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
