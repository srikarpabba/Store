import { Component, computed, inject, signal } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { switchMap } from 'rxjs';
import { ProductColorDetails, ProductDetails as ProductDetailsModel, ProductVariantDetails } from '../../models/product-details';
import { ProductService } from '../../services/product.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { PricePipe } from '../../../../shared/pipes/price.pipe';
import { Drawer } from '../../../../shared/ui/drawer/drawer';

interface SizeOption {
  sizeId: string;
  sizeName: string;
  /** false when the size has no stock (or no variant) for the selected color */
  available: boolean;
}

@Component({
  selector: 'app-product-details',
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatIconModule,
    PricePipe,
    Drawer
  ],
  templateUrl: './product-details.html',
  styleUrl: './product-details.css',
})
export class ProductDetails {

  private static readonly MAX_QUANTITY = 10;

  private readonly route = inject(ActivatedRoute);
  private readonly productService = inject(ProductService);
  private readonly notificationService = inject(NotificationService);
  private readonly title = inject(Title);

  readonly product = signal<ProductDetailsModel | null>(null);

  readonly selectedColorId = signal<string | null>(null);
  readonly selectedSizeId = signal<string | null>(null);
  readonly quantity = signal(1);
  readonly sizeGuideOpen = signal(false);

  readonly pincode = new FormControl('', { nonNullable: true });
  readonly deliveryMessage = signal<string | null>(null);

  readonly selectedColor = computed<ProductColorDetails | null>(() => {
    const product = this.product();
    return product?.colors.find(c => c.colorId === this.selectedColorId()) ?? null;
  });

  readonly photos = computed(() => this.selectedColor()?.photos ?? []);

  /** All sizes the product comes in, greyed out when the selected color has no stock */
  readonly sizes = computed<SizeOption[]>(() => {
    const product = this.product();

    if (!product) {
      return [];
    }

    const seen = new Map<string, string>();

    for (const variant of product.variants) {
      if (!seen.has(variant.sizeId)) {
        seen.set(variant.sizeId, variant.sizeName);
      }
    }

    return [...seen].map(([sizeId, sizeName]) => {
      const variant = this.findVariant(this.selectedColorId(), sizeId);
      return {
        sizeId,
        sizeName,
        available: variant !== null && variant.quantityInStock > 0
      };
    });
  });

  readonly selectedVariant = computed<ProductVariantDetails | null>(() =>
    this.findVariant(this.selectedColorId(), this.selectedSizeId()));

  /** Selected variant's price; otherwise the cheapest price of the selected color */
  readonly displayPrice = computed<number | null>(() => {
    const variant = this.selectedVariant();

    if (variant) {
      return variant.price;
    }

    const color = this.selectedColor();
    const product = this.product();

    const candidates = product?.variants
      .filter(v => color === null || v.productColorId === color.productColorId)
      .map(v => v.price) ?? [];

    return candidates.length > 0 ? Math.min(...candidates) : null;
  });

  readonly canAddToCart = computed(() =>
    (this.selectedVariant()?.quantityInStock ?? 0) > 0);

  constructor() {
    this.route.paramMap.pipe(
      switchMap(params => this.productService.getProduct(params.get('id') ?? ''))
    ).subscribe(product => {
      this.product.set(product);
      this.title.setTitle(`${product.name} | Store`);

      // preselect the first color that has stock (falling back to the first)
      const firstAvailable = product.colors.find(color =>
        product.variants.some(v => v.productColorId === color.productColorId && v.quantityInStock > 0));

      const colorId = (firstAvailable ?? product.colors[0])?.colorId ?? null;

      this.selectedColorId.set(colorId);
      this.selectedSizeId.set(this.firstAvailableSizeId(colorId));
      this.quantity.set(1);
    });
  }

  selectColor(color: ProductColorDetails): void {
    this.selectedColorId.set(color.colorId);

    // keep the current size if the new color still has it in stock;
    // otherwise fall back to the first size that is in stock
    const currentSizeId = this.selectedSizeId();
    const stillAvailable = currentSizeId !== null
      && (this.findVariant(color.colorId, currentSizeId)?.quantityInStock ?? 0) > 0;

    this.selectedSizeId.set(stillAvailable ? currentSizeId : this.firstAvailableSizeId(color.colorId));

    this.quantity.set(1);
  }

  selectSize(size: SizeOption): void {
    if (size.available) {
      this.selectedSizeId.set(size.sizeId);
      this.quantity.set(1);
    }
  }

  decreaseQuantity(): void {
    this.quantity.update(quantity => Math.max(1, quantity - 1));
  }

  increaseQuantity(): void {
    const stock = this.selectedVariant()?.quantityInStock ?? ProductDetails.MAX_QUANTITY;
    const max = Math.min(stock, ProductDetails.MAX_QUANTITY);
    this.quantity.update(quantity => Math.min(max, quantity + 1));
  }

  addToCart(): void {
    // cart lands in a later iteration — the buy box is wired up for it
    this.notificationService.info('Cart is coming soon.');
  }

  addToWishlist(): void {
    this.notificationService.info('Wishlist is coming soon.');
  }

  checkDelivery(): void {
    const pin = this.pincode.value.trim();

    if (!/^\d{6}$/.test(pin)) {
      this.deliveryMessage.set('Please enter a valid 6-digit pincode.');
      return;
    }

    // placeholder until a delivery-estimate API exists
    this.deliveryMessage.set(`Delivery available to ${pin}. Estimated 2–4 business days.`);
  }

  /** First size (in the product's variant order) that has stock for the given color */
  private firstAvailableSizeId(colorId: string | null): string | null {
    const product = this.product();

    if (!product) {
      return null;
    }

    const seenSizeIds = new Set<string>();

    for (const variant of product.variants) {
      if (seenSizeIds.has(variant.sizeId)) {
        continue;
      }

      seenSizeIds.add(variant.sizeId);

      const match = this.findVariant(colorId, variant.sizeId);

      if (match !== null && match.quantityInStock > 0) {
        return variant.sizeId;
      }
    }

    return null;
  }

  private findVariant(colorId: string | null, sizeId: string | null): ProductVariantDetails | null {
    const product = this.product();
    const color = product?.colors.find(c => c.colorId === colorId);

    if (!product || !color || sizeId === null) {
      return null;
    }

    return product.variants.find(v =>
      v.productColorId === color.productColorId && v.sizeId === sizeId) ?? null;
  }
}
