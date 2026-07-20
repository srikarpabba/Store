import { Component, ElementRef, computed, effect, inject, signal, viewChild } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute, Router } from '@angular/router';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ShopSection } from '../../models/enums/shop-section';
import { CategoryLookup } from '../../models/category-lookup';
import { Lookup } from '../../models/lookup';
import { Product } from '../../models/product';
import { ProductFacets, FacetCount } from '../../models/product-facets';
import { ProductFilters } from '../../models/product-filters';
import { ProductService } from '../../services/product.service';
import { ProductCard } from '../../components/product-card/product-card';
import { NotificationService } from '../../../../core/services/notification.service';
import { slugify } from '../../../../shared/utils/slug';

type ListingTarget =
  | { kind: 'category'; category: CategoryLookup }
  | { kind: 'brand'; brand: Lookup };

/**
 * Category/brand landing page (`/men/t-shirt`, `/men/nike`) — all products
 * for the section's gender matching whichever the slug resolves to, with
 * sidebar filters (subcategory, size, color, price — plus brand, on a
 * category page) and the same infinite-scroll grid as the shop page.
 */
@Component({
  selector: 'app-storefront-listing',
  imports: [MatProgressSpinnerModule, ProductCard],
  templateUrl: './storefront-listing.html',
  styleUrl: './storefront-listing.css',
})
export class StorefrontListing {

  private static readonly PAGE_SIZE = 24;

  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly title = inject(Title);
  private readonly productService = inject(ProductService);
  private readonly notificationService = inject(NotificationService);

  readonly section = signal('');

  private readonly slug = signal('');

  readonly filters = signal<ProductFilters | null>(null);

  /** What this page is for, resolved from the filters payload by slugified
      name — a category (must be tagged with the section's gender; an empty
      genderIds means it applies to every gender) or, failing that, a brand
      (brands aren't gender-scoped, so any match applies). */
  readonly target = computed<ListingTarget | null>(() => {
    const filters = this.filters();
    const slug = this.slug();

    if (!filters || !slug) {
      return null;
    }

    const genderId = filters.genders.find(g => g.name === this.genderName)?.id;

    const category = filters.categories.find(c =>
      slugify(c.name) === slug &&
      (c.genderIds.length === 0 || (!!genderId && c.genderIds.includes(genderId))));

    if (category) {
      return { kind: 'category', category };
    }

    const brand = filters.brands.find(b => slugify(b.name) === slug);

    return brand ? { kind: 'brand', brand } : null;
  });

  readonly targetName = computed(() => {
    const target = this.target();

    if (!target) {
      return null;
    }

    return target.kind === 'category' ? target.category.name : target.brand.name;
  });

  /** Sidebar "Categories" options: a category page scopes these to its own
      subcategories; a brand page has no single parent category, so every
      subcategory is offered (further narrowed by facet counts below). */
  readonly availableSubcategories = computed(() => {
    const target = this.target();

    if (!target) {
      return [];
    }

    if (target.kind === 'brand') {
      return this.filters()?.subcategories ?? [];
    }

    return (this.filters()?.subcategories ?? []).filter(s => s.categoryId === target.category.id);
  });

  /** Sidebar options: only sizes the current category is tagged with — a
      category with no tagged sizes (or a brand page, with no category at
      all) allows all of them */
  readonly availableSizes = computed(() => {
    const sizes = this.filters()?.sizes ?? [];
    const target = this.target();

    if (!target || target.kind === 'brand' || target.category.sizeIds.length === 0) {
      return sizes;
    }

    return sizes.filter(size => target.category.sizeIds.includes(size.id));
  });

  readonly selectedSubcategories = signal<ReadonlySet<string>>(new Set());

  /** Brand filter is only shown on category pages — a brand page is
      already scoped to one brand */
  readonly selectedBrands = signal<ReadonlySet<string>>(new Set());

  readonly selectedSizes = signal<ReadonlySet<string>>(new Set());

  readonly selectedColors = signal<ReadonlySet<string>>(new Set());

  /** Per-option product counts for the current selection (self-excluding
      per facet, computed server-side). Null until the first load. */
  readonly facets = signal<ProductFacets | null>(null);

  /** Stale-response guard for facet loads */
  private facetGeneration = 0;

  /** Per-section option search boxes */
  readonly subcategorySearch = signal('');

  readonly brandSearch = signal('');

  readonly sizeSearch = signal('');

  readonly colorSearch = signal('');

  readonly subcategoryOptions = computed(() => StorefrontListing.withCounts(
    this.availableSubcategories().map(s => s.name),
    this.facets()?.subcategories,
    this.selectedSubcategories(),
    this.subcategorySearch()));

  readonly brandOptions = computed(() => StorefrontListing.withCounts(
    (this.filters()?.brands ?? []).map(b => b.name),
    this.facets()?.brands,
    this.selectedBrands(),
    this.brandSearch()));

  readonly sizeOptions = computed(() => StorefrontListing.withCounts(
    this.availableSizes().map(s => s.name),
    this.facets()?.sizes,
    this.selectedSizes(),
    this.sizeSearch()));

  readonly colorOptions = computed(() => {
    const counts = new Map(this.facets()?.colors.map(f => [f.name, f.count]) ?? []);
    const selected = this.selectedColors();
    const search = this.colorSearch();

    return (this.filters()?.colors ?? [])
      .filter(color => StorefrontListing.matchesSearch(color.name, search))
      .map(color => ({
        ...color,
        count: this.facets() ? counts.get(color.name) ?? 0 : null
      }))
      .filter(color => color.count === null || color.count > 0 || selected.has(color.name));
  });

  readonly minPrice = signal<number | null>(null);

  readonly maxPrice = signal<number | null>(null);

  readonly products = signal<Product[]>([]);

  /** 0 = nothing loaded yet for the current target/filter combination */
  readonly pageIndex = signal(0);

  readonly hasNext = signal(false);

  readonly isLoadingMore = signal(false);

  readonly isSentinelIntersecting = signal(false);

  private readonly scrollSentinel = viewChild<ElementRef<HTMLElement>>('scrollSentinel');

  /** Re-attaches the observer on every target/filter reset (see shop-page) */
  private readonly resetTrigger = signal(0);

  /** Stale-response guard, bumped with every reset */
  private generation = 0;

  private genderName = '';

  constructor() {
    this.productService.getFilters().subscribe(filters => this.filters.set(filters));

    this.route.paramMap.subscribe(params => {
      const section = params.get('section') ?? '';
      const slug = params.get('slug') ?? '';

      this.section.set(section);

      this.genderName =
        section === ShopSection.Men ? 'Male'
          : section === ShopSection.Women ? 'Female'
            : '';

      // Category/brand pages only exist for the gendered storefronts
      if (!this.genderName) {
        this.router.navigateByUrl('/not-found', { skipLocationChange: true });
        return;
      }

      this.resetState();
      this.slug.set(slug);
    });

    // Once the filters have loaded, an unmatched slug means neither a
    // category nor a brand exists for it (for this gender) — treat it like
    // any bad URL.
    effect(() => {
      const filters = this.filters();
      const slug = this.slug();

      if (!filters || !slug) {
        return;
      }

      const name = this.targetName();

      if (!name) {
        this.router.navigateByUrl('/not-found', { skipLocationChange: true });
        return;
      }

      this.title.setTitle(`${name} | Store`);
    });

    // Reload facet counts whenever the target or any filter selection
    // changes, so every option's count reflects the products it would show.
    effect(() => {
      const target = this.target();
      const subcategories = [...this.selectedSubcategories()];
      const brands = [...this.selectedBrands()];
      const sizes = [...this.selectedSizes()];
      const colors = [...this.selectedColors()];
      const minPrice = this.minPrice() ?? undefined;
      const maxPrice = this.maxPrice() ?? undefined;

      if (!target) {
        return;
      }

      const generation = ++this.facetGeneration;

      this.productService.getFacets({
        genders: [this.genderName],
        ...this.targetQueryFilter(target, brands),
        subcategories,
        sizes,
        colors,
        minPrice,
        maxPrice
      }).subscribe(facets => {
        if (generation === this.facetGeneration) {
          this.facets.set(facets);
        }
      });
    });

    // Same observer pattern as shop-page: attach only once the target has
    // resolved, so the first page loads when the sentinel is genuinely in
    // view, and re-check after each page for short result sets.
    effect(onCleanup => {
      const sentinel = this.scrollSentinel();
      this.resetTrigger();

      if (!sentinel || !this.target()) {
        return;
      }

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

  toggleSubcategory(name: string): void {
    this.selectedSubcategories.update(current => StorefrontListing.toggled(current, name));
    this.resetAndReload();
  }

  toggleBrand(name: string): void {
    this.selectedBrands.update(current => StorefrontListing.toggled(current, name));
    this.resetAndReload();
  }

  toggleSize(name: string): void {
    this.selectedSizes.update(current => StorefrontListing.toggled(current, name));
    this.resetAndReload();
  }

  toggleColor(name: string): void {
    this.selectedColors.update(current => StorefrontListing.toggled(current, name));
    this.resetAndReload();
  }

  applyPrice(min: string, max: string): void {
    const parsedMin = min === '' ? null : Number(min);
    const parsedMax = max === '' ? null : Number(max);

    this.minPrice.set(Number.isFinite(parsedMin ?? NaN) ? parsedMin : null);
    this.maxPrice.set(Number.isFinite(parsedMax ?? NaN) ? parsedMax : null);
    this.resetAndReload();
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

  private resetAndReload(): void {
    this.resetState();
    this.resetTrigger.update(n => n + 1);
  }

  private resetState(): void {
    this.generation++;
    this.products.set([]);
    this.pageIndex.set(0);
    this.hasNext.set(false);
    this.isLoadingMore.set(false);
  }

  /** category page: filters by category name, plus the Brand facet's
      selection; brand page: filters by brand name alone — there's no
      Brand facet to select from since the page is already brand-scoped. */
  private targetQueryFilter(target: ListingTarget, selectedBrands: string[]): { categories?: string[]; brands?: string[] } {
    return target.kind === 'category'
      ? { categories: [target.category.name], brands: selectedBrands }
      : { brands: [target.brand.name] };
  }

  private loadPage(pageIndex: number): void {

    const target = this.target();

    if (!target) {
      return;
    }

    const generation = this.generation;

    this.isLoadingMore.set(true);

    this.productService.getProductsGraphQl({
      genders: [this.genderName],
      ...this.targetQueryFilter(target, [...this.selectedBrands()]),
      subcategories: [...this.selectedSubcategories()],
      sizes: [...this.selectedSizes()],
      colors: [...this.selectedColors()],
      minPrice: this.minPrice() ?? undefined,
      maxPrice: this.maxPrice() ?? undefined,
      pageIndex,
      pageSize: StorefrontListing.PAGE_SIZE
    }).subscribe({
      next: result => {

        if (generation !== this.generation) {
          return;
        }

        this.products.update(current => [...current, ...result.items]);
        this.pageIndex.set(result.pageIndex);
        this.hasNext.set(result.hasNext);
        this.isLoadingMore.set(false);

        if (this.isSentinelIntersecting()) {
          this.loadMore();
        }
      },
      error: (err: unknown) => {

        if (generation !== this.generation) {
          return;
        }

        this.isLoadingMore.set(false);

        // GraphQL body-level failures come back HTTP 200 and never reach
        // the shared error interceptor — surface them here.
        if (err instanceof Error) {
          this.notificationService.error(err.message);
        }
      }
    });
  }

  private static toggled(current: ReadonlySet<string>, name: string): ReadonlySet<string> {
    const next = new Set(current);

    if (next.has(name)) {
      next.delete(name);
    } else {
      next.add(name);
    }

    return next;
  }

  /** Pairs option names with their facet count. Before the first facet
      response lands, counts are null (options all shown, no number).
      After it, zero-count options are hidden unless currently selected —
      unchecking one must stay possible. */
  private static withCounts(
    names: string[],
    facetCounts: FacetCount[] | undefined,
    selected: ReadonlySet<string>,
    search: string
  ): { name: string; count: number | null }[] {
    const visible = names.filter(name => StorefrontListing.matchesSearch(name, search));

    if (!facetCounts) {
      return visible.map(name => ({ name, count: null }));
    }

    const byName = new Map(facetCounts.map(f => [f.name, f.count]));

    return visible
      .map(name => ({ name, count: byName.get(name) ?? 0 }))
      .filter(option => option.count > 0 || selected.has(option.name));
  }

  private static matchesSearch(name: string, search: string): boolean {
    const query = search.trim().toLowerCase();

    return query === '' || name.toLowerCase().includes(query);
  }
}
