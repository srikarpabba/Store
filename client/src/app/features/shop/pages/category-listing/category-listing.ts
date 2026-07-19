import { Component, ElementRef, computed, effect, inject, signal, viewChild } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute, Router } from '@angular/router';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ShopSection } from '../../models/enums/shop-section';
import { CategoryLookup } from '../../models/category-lookup';
import { Product } from '../../models/product';
import { ProductFacets, FacetCount } from '../../models/product-facets';
import { ProductFilters } from '../../models/product-filters';
import { ProductService } from '../../services/product.service';
import { ProductCard } from '../../components/product-card/product-card';
import { NotificationService } from '../../../../core/services/notification.service';
import { slugify } from '../../../../shared/utils/slug';

/**
 * Category landing page (`/men/t-shirt`) — all of a category's products for
 * the section's gender, with sidebar filters (subcategory, size, color,
 * price) and the same infinite-scroll grid as the shop page.
 */
@Component({
  selector: 'app-category-listing',
  imports: [MatProgressSpinnerModule, ProductCard],
  templateUrl: './category-listing.html',
  styleUrl: './category-listing.css',
})
export class CategoryListing {

  private static readonly PAGE_SIZE = 24;

  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly title = inject(Title);
  private readonly productService = inject(ProductService);
  private readonly notificationService = inject(NotificationService);

  readonly section = signal('');

  private readonly categorySlug = signal('');

  readonly filters = signal<ProductFilters | null>(null);

  /** The category this page is for, resolved from the filters payload by
      slugified name — must be tagged with the section's gender (an empty
      genderIds means the category applies to every gender). */
  readonly category = computed<CategoryLookup | null>(() => {
    const filters = this.filters();
    const slug = this.categorySlug();

    if (!filters || !slug) {
      return null;
    }

    const genderId = filters.genders.find(g => g.name === this.genderName)?.id;

    return filters.categories.find(c =>
      slugify(c.name) === slug &&
      (c.genderIds.length === 0 || (!!genderId && c.genderIds.includes(genderId)))) ?? null;
  });

  /** Sidebar options: only this category's subcategories */
  readonly availableSubcategories = computed(() => {
    const categoryId = this.category()?.id;

    if (!categoryId) {
      return [];
    }

    return (this.filters()?.subcategories ?? []).filter(s => s.categoryId === categoryId);
  });

  /** Sidebar options: only sizes this category is tagged with — a category
      with no tagged sizes allows all of them */
  readonly availableSizes = computed(() => {
    const sizes = this.filters()?.sizes ?? [];
    const categoryId = this.category()?.id;

    if (!categoryId) {
      return sizes;
    }

    const categoryLookup = this.filters()?.categories.find(c => c.id === categoryId);

    if (!categoryLookup || categoryLookup.sizeIds.length === 0) {
      return sizes;
    }

    return sizes.filter(size => categoryLookup.sizeIds.includes(size.id));
  });

  readonly selectedSubcategories = signal<ReadonlySet<string>>(new Set());

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

  readonly subcategoryOptions = computed(() => CategoryListing.withCounts(
    this.availableSubcategories().map(s => s.name),
    this.facets()?.subcategories,
    this.selectedSubcategories(),
    this.subcategorySearch()));

  readonly brandOptions = computed(() => CategoryListing.withCounts(
    (this.filters()?.brands ?? []).map(b => b.name),
    this.facets()?.brands,
    this.selectedBrands(),
    this.brandSearch()));

  readonly sizeOptions = computed(() => CategoryListing.withCounts(
    this.availableSizes().map(s => s.name),
    this.facets()?.sizes,
    this.selectedSizes(),
    this.sizeSearch()));

  readonly colorOptions = computed(() => {
    const counts = new Map(this.facets()?.colors.map(f => [f.name, f.count]) ?? []);
    const selected = this.selectedColors();
    const search = this.colorSearch();

    return (this.filters()?.colors ?? [])
      .filter(color => CategoryListing.matchesSearch(color.name, search))
      .map(color => ({
        ...color,
        count: this.facets() ? counts.get(color.name) ?? 0 : null
      }))
      .filter(color => color.count === null || color.count > 0 || selected.has(color.name));
  });

  readonly minPrice = signal<number | null>(null);

  readonly maxPrice = signal<number | null>(null);

  readonly products = signal<Product[]>([]);

  /** 0 = nothing loaded yet for the current category/filter combination */
  readonly pageIndex = signal(0);

  readonly hasNext = signal(false);

  readonly isLoadingMore = signal(false);

  readonly isSentinelIntersecting = signal(false);

  private readonly scrollSentinel = viewChild<ElementRef<HTMLElement>>('scrollSentinel');

  /** Re-attaches the observer on every category/filter reset (see shop-page) */
  private readonly resetTrigger = signal(0);

  /** Stale-response guard, bumped with every reset */
  private generation = 0;

  private genderName = '';

  constructor() {
    this.productService.getFilters().subscribe(filters => this.filters.set(filters));

    this.route.paramMap.subscribe(params => {
      const section = params.get('section') ?? '';
      const slug = params.get('categorySlug') ?? '';

      this.section.set(section);

      this.genderName =
        section === ShopSection.Men ? 'Male'
          : section === ShopSection.Women ? 'Female'
            : '';

      // Category pages only exist for the gendered storefronts
      if (!this.genderName) {
        this.router.navigateByUrl('/not-found', { skipLocationChange: true });
        return;
      }

      this.resetState();
      this.categorySlug.set(slug);
    });

    // Once the filters have loaded, an unmatched slug means the category
    // doesn't exist (for this gender) — treat it like any bad URL.
    effect(() => {
      const filters = this.filters();
      const slug = this.categorySlug();

      if (!filters || !slug) {
        return;
      }

      const category = this.category();

      if (!category) {
        this.router.navigateByUrl('/not-found', { skipLocationChange: true });
        return;
      }

      this.title.setTitle(`${category.name} | Store`);
    });

    // Reload facet counts whenever the category or any filter selection
    // changes, so every option's count reflects the products it would show.
    effect(() => {
      const category = this.category();
      const subcategories = [...this.selectedSubcategories()];
      const brands = [...this.selectedBrands()];
      const sizes = [...this.selectedSizes()];
      const colors = [...this.selectedColors()];
      const minPrice = this.minPrice() ?? undefined;
      const maxPrice = this.maxPrice() ?? undefined;

      if (!category) {
        return;
      }

      const generation = ++this.facetGeneration;

      this.productService.getFacets({
        genders: [this.genderName],
        categories: [category.name],
        subcategories,
        brands,
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

    // Same observer pattern as shop-page: attach only once the category has
    // resolved, so the first page loads when the sentinel is genuinely in
    // view, and re-check after each page for short result sets.
    effect(onCleanup => {
      const sentinel = this.scrollSentinel();
      this.resetTrigger();

      if (!sentinel || !this.category()) {
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
    this.selectedSubcategories.update(current => CategoryListing.toggled(current, name));
    this.resetAndReload();
  }

  toggleBrand(name: string): void {
    this.selectedBrands.update(current => CategoryListing.toggled(current, name));
    this.resetAndReload();
  }

  toggleSize(name: string): void {
    this.selectedSizes.update(current => CategoryListing.toggled(current, name));
    this.resetAndReload();
  }

  toggleColor(name: string): void {
    this.selectedColors.update(current => CategoryListing.toggled(current, name));
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

  private loadPage(pageIndex: number): void {

    const category = this.category();

    if (!category) {
      return;
    }

    const generation = this.generation;

    this.isLoadingMore.set(true);

    this.productService.getProductsGraphQl({
      genders: [this.genderName],
      categories: [category.name],
      brands: [...this.selectedBrands()],
      subcategories: [...this.selectedSubcategories()],
      sizes: [...this.selectedSizes()],
      colors: [...this.selectedColors()],
      minPrice: this.minPrice() ?? undefined,
      maxPrice: this.maxPrice() ?? undefined,
      pageIndex,
      pageSize: CategoryListing.PAGE_SIZE
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
    const visible = names.filter(name => CategoryListing.matchesSearch(name, search));

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
