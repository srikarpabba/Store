import { Component, computed, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { WishlistService } from '../../../shop/services/wishlist.service';
import { Product } from '../../../shop/models/product';
import { ProductCard } from '../../../shop/components/product-card/product-card';
import { ShopSection } from '../../../shop/models/enums/shop-section';

@Component({
  selector: 'app-wishlist',
  imports: [RouterLink, MatButtonModule, MatIconModule, ProductCard],
  templateUrl: './wishlist.html',
  styleUrl: './wishlist.css',
})
export class Wishlist {

  private readonly wishlistService = inject(WishlistService);

  readonly isLoaded = this.wishlistService.isLoaded;

  /** Product details don't key off the section param, so a wishlist page
      (which mixes men's and women's items) just needs any valid one for
      routing purposes. */
  readonly section = ShopSection.New;

  /** Reshaped for app-product-card, which the shop grid already uses —
      colors/category/rating aren't tracked at wishlist level, and the card
      degrades gracefully to a plain image + name + price without them. */
  readonly products = computed<Product[]>(() =>
    this.wishlistService.items().map(item => ({
      id: item.productId,
      name: item.productName,
      startingPrice: item.startingPrice,
      rating: 0,
      image: item.image,
      category: null,
      subcategory: null,
      discountPercentage: item.discountPercentage,
      saleEndsAtUtc: item.saleEndsAtUtc,
      colors: null
    })));
}
