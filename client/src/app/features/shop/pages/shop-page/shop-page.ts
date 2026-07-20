import { Component, ElementRef, effect, inject, signal, viewChild } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { combineLatest } from 'rxjs';
import { ShopService } from '../../services/shop.service';
import { StorefrontService } from '../../services/storefront.service';
import { ShopSection } from '../../models/enums/shop-section';
import { Product } from '../../models/product';
import { StorefrontBrandItem, StorefrontCategoryItem, StorefrontSectionType } from '../../models/storefront-section';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { BannerSlide, BannerSlider } from '../../../../shared/ui/banner-slider/banner-slider';
import { NotificationService } from '../../../../core/services/notification.service';
import { ProductCard } from '../../components/product-card/product-card';
import { slugify } from '../../../../shared/utils/slug';

@Component({
  selector: 'app-shop-page',
  imports: [RouterLink, BannerSlider, MatProgressSpinnerModule, ProductCard],
  templateUrl: './shop-page.html',
  styleUrl: './shop-page.css',
})
export class ShopPage {
  private route = inject(ActivatedRoute);

  private shopService = inject(ShopService);

  private storefrontService = inject(StorefrontService);

  private notificationService = inject(NotificationService);

  /** Exposed for the category-tile links */
  readonly slugify = slugify;

  readonly section = signal('');

  readonly products = signal<Product[]>([]);

  readonly bannerSlides = signal<BannerSlide[]>([]);

  readonly categories = signal<StorefrontCategoryItem[]>([]);

  readonly newArrivals = signal<Product[]>([]);

  readonly featuredBrands = signal<StorefrontBrandItem[]>([]);

  /** 0 = nothing loaded yet for the current section/search/category */
  readonly pageIndex = signal(0);

  readonly hasNext = signal(false);

  readonly isLoadingMore = signal(false);

  readonly isSentinelIntersecting = signal(false);

  private readonly scrollSentinel = viewChild<ElementRef<HTMLElement>>('scrollSentinel');

  /** Bumped on every section/search/category change, so the observer effect
   *  below re-subscribes and re-reads the sentinel's *current* on-screen
   *  state — Men/Women start with it below the fold (banner + categories
   *  above it), so nothing loads until the shopper actually scrolls there;
   *  New/Sale have no banner/categories, so the sentinel starts in view and
   *  page 1 loads immediately, all through the same code path. */
  private readonly resetTrigger = signal(0);

  /** Bumped alongside resetTrigger so a slow, now-stale request can't overwrite newer results */
  private generation = 0;

  readonly activeCategory = signal<string | undefined>(undefined);

  private currentSection: ShopSection = ShopSection.New;

  private currentSearch?: string;

  /** Only re-fetch banners/categories when the section itself changes — a
   *  category/search filter change should only reload the product grid. */
  private loadedStorefrontSection: ShopSection | null = null;

  /** False on Men/Women until the storefront sections (banner + categories +
   *  new arrivals) have loaded. Until then the page is artificially short and
   *  the sentinel would sit spuriously in view, eager-loading products before
   *  the shopper ever scrolls. Gate the observer on this so it only attaches
   *  once the page has its real height. */
  private readonly storefrontReady = signal(false);

  constructor() {
    effect(onCleanup => {
      const sentinel = this.scrollSentinel();
      this.resetTrigger(); // re-run on every section change
      const ready = this.storefrontReady(); // …and once the sections have landed

      if (!sentinel || !ready) {
        return;
      }

      // Creating a fresh observer and calling .observe() always triggers an
      // initial callback reporting the sentinel's *current* intersection
      // state (per the IntersectionObserver spec, independent of Angular's
      // render timing) — that's what decides whether the new section's
      // first page loads immediately or waits for a scroll.
      const observer = new IntersectionObserver(entries => {
        const isIntersecting = entries[0]?.isIntersecting ?? false;
        this.isSentinelIntersecting.set(isIntersecting);

        if (isIntersecting) {
          this.loadMore();
        }
      });

      observer.observe(sentinel.nativeElement);

      onCleanup(() => observer.disconnect());
    });
  }

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

      this.currentSection = section;
      this.currentSearch = search;
      this.activeCategory.set(category);

      this.generation++;
      this.products.set([]);
      this.pageIndex.set(0);
      this.hasNext.set(false);
      this.isLoadingMore.set(false);

      this.loadStorefrontSections(section);

      // Don't eagerly load products here — see resetTrigger above.
      this.resetTrigger.update(n => n + 1);

    });
  }

  /**
   * Pill filtering is in-place UI state, not real navigation — it
   * deliberately doesn't touch the URL (no bookmarking/back-button support
   * for it), which also means there's no router navigation to trigger
   * Angular's default scroll-to-top.
   */
  selectCategory(category?: string): void {

    if (this.activeCategory() === category) {
      return;
    }

    this.activeCategory.set(category);

    this.generation++;
    this.products.set([]);
    this.pageIndex.set(0);
    this.hasNext.set(false);
    this.isLoadingMore.set(false);

    this.loadPage(1);
  }

  loadMore(): void {

    if (this.isLoadingMore()) {
      return;
    }

    if (this.pageIndex() > 0 && !this.hasNext()) {
      return;
    }

    this.loadPage(this.pageIndex() + 1);
  }

  private loadPage(pageIndex: number): void {

    const generation = this.generation;

    this.isLoadingMore.set(true);

    this.shopService
      .loadSection(this.currentSection, this.currentSearch, this.activeCategory(), pageIndex)
      .subscribe({
        next: result => {

          if (generation !== this.generation) {
            return;
          }

          this.products.update(current => [...current, ...result.items]);
          this.pageIndex.set(result.pageIndex);
          this.hasNext.set(result.hasNext);
          this.isLoadingMore.set(false);

          // IntersectionObserver only fires on crossing events — if the
          // sentinel is still visible after this page landed (e.g. a short
          // result set that doesn't fill the viewport), keep loading.
          if (this.isSentinelIntersecting()) {
            this.loadMore();
          }
        },
        error: (err: unknown) => {

          if (generation !== this.generation) {
            return;
          }

          this.isLoadingMore.set(false);

          // A real HttpErrorResponse (network/5xx/etc.) is already toasted
          // by the shared error interceptor; a GraphQL body-level failure
          // surfaces here as a plain Error instead, since HotChocolate
          // returns those with HTTP 200 and the interceptor never sees them.
          if (err instanceof Error) {
            this.notificationService.error(err.message);
          }
        }
      });
  }

  private loadStorefrontSections(section: ShopSection): void {

    if (section === this.loadedStorefrontSection) {
      return;
    }

    this.loadedStorefrontSection = section;

    // Every ShopSection is a real backend storefront now (New/Sale get
    // banners only — no gender-scoped categories/new-arrivals/brands).
    // Defer product loading until these land (see storefrontReady).
    this.storefrontReady.set(false);

    this.storefrontService.getSections(section).subscribe(response => {

      const bannerSection = response.sections.find(s => s.type === StorefrontSectionType.Banner);
      const categorySection = response.sections.find(s => s.type === StorefrontSectionType.Category);
      const newArrivalsSection = response.sections.find(s => s.key === 'new-arrivals');
      const featuredBrandsSection = response.sections.find(s => s.type === StorefrontSectionType.Brand);

      this.bannerSlides.set((bannerSection?.items as BannerSlide[] | undefined) ?? []);
      this.categories.set((categorySection?.items as StorefrontCategoryItem[] | undefined) ?? []);
      this.newArrivals.set((newArrivalsSection?.items as Product[] | undefined) ?? []);
      this.featuredBrands.set((featuredBrandsSection?.items as StorefrontBrandItem[] | undefined) ?? []);

      this.storefrontReady.set(true);
    });
  }
}
