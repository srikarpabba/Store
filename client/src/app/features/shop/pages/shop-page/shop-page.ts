import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { combineLatest } from 'rxjs';
import { ShopService } from '../../services/shop.service';
import { StorefrontService } from '../../services/storefront.service';
import { ShopSection } from '../../models/enums/shop-section';
import { Product } from '../../models/product';
import { StorefrontCategoryItem, StorefrontSectionType } from '../../models/storefront-section';
import { TitleCasePipe } from '@angular/common';
import { PricePipe } from '../../../../shared/pipes/price.pipe';
import { BannerSlide, BannerSlider } from '../../../../shared/ui/banner-slider/banner-slider';

@Component({
  selector: 'app-shop-page',
  imports: [TitleCasePipe, PricePipe, RouterLink, BannerSlider],
  templateUrl: './shop-page.html',
  styleUrl: './shop-page.css',
})
export class ShopPage {
  private route = inject(ActivatedRoute);

  private shopService = inject(ShopService);

  private storefrontService = inject(StorefrontService);

  readonly section = signal('');

  readonly products = signal<Product[]>([]);

  readonly bannerSlides = signal<BannerSlide[]>([]);

  readonly categories = signal<StorefrontCategoryItem[]>([]);

  ngOnInit(): void {

    combineLatest([
      this.route.paramMap,
      this.route.queryParamMap
    ]).subscribe(([params, queryParams]) => {

      const section =
        (params.get('section') as ShopSection) ?? ShopSection.New;

      const search = queryParams.get('search') ?? undefined;

      const category = queryParams.get('category') ?? undefined;

      this.section.set(section);

      this.loadProducts(section, search, category);

      this.loadStorefrontSections(section);

    });
  }

  private loadProducts(section: ShopSection, search?: string, category?: string): void {

    this.shopService
      .loadSection(section, search, category)
      .subscribe(result => this.products.set(result.items));
  }

  private loadStorefrontSections(section: ShopSection): void {

    if (section !== ShopSection.Men && section !== ShopSection.Women) {
      this.bannerSlides.set([]);
      this.categories.set([]);
      return;
    }

    this.storefrontService.getSections(section).subscribe(response => {

      const bannerSection = response.sections.find(s => s.type === StorefrontSectionType.Banner);
      const categorySection = response.sections.find(s => s.type === StorefrontSectionType.Category);

      this.bannerSlides.set((bannerSection?.items as BannerSlide[] | undefined) ?? []);
      this.categories.set((categorySection?.items as StorefrontCategoryItem[] | undefined) ?? []);
    });
  }
}
