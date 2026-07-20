import { Component, inject, output, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { catchError, debounceTime, distinctUntilChanged, map, of, switchMap, tap } from 'rxjs';
import { Product } from '../../../features/shop/models/product';
import { ProductService } from '../../../features/shop/services/product.service';
import { PricePipe } from '../../pipes/price.pipe';

/**
 * Debounced product search-and-select, self-contained (own query/results/
 * open state per instance) so it drops cleanly into a list of rows — e.g.
 * one per line of a bulk sale. Emits the picked product (or null on clear)
 * rather than implementing ControlValueAccessor, since every caller so far
 * only ever sets the id by picking a result, never externally.
 */
@Component({
  selector: 'app-product-picker',
  imports: [ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatIconModule, PricePipe],
  templateUrl: './product-picker.html',
  styleUrl: './product-picker.css',
})
export class ProductPicker {

  private static readonly MIN_QUERY_LENGTH = 2;
  private static readonly SUGGESTION_COUNT = 8;

  private readonly productService = inject(ProductService);

  readonly productSelected = output<Product | null>();

  readonly selected = signal<Product | null>(null);

  readonly query = new FormControl('', { nonNullable: true });

  readonly results = signal<Product[]>([]);
  readonly isSearching = signal(false);
  readonly isPanelOpen = signal(false);

  constructor() {
    this.query.valueChanges.pipe(
      map(value => value.trim()),
      tap(term => {
        if (term.length < ProductPicker.MIN_QUERY_LENGTH) {
          this.isPanelOpen.set(false);
        }
      }),
      debounceTime(300),
      distinctUntilChanged(),
      switchMap(term => {
        if (term.length < ProductPicker.MIN_QUERY_LENGTH) {
          return of(null);
        }

        this.isSearching.set(true);
        this.isPanelOpen.set(true);

        return this.productService.getProducts({
          search: term,
          pageIndex: 1,
          pageSize: ProductPicker.SUGGESTION_COUNT
        }).pipe(catchError(() => of(null)));
      }),
      takeUntilDestroyed()
    ).subscribe(response => {
      this.isSearching.set(false);

      if (response !== null) {
        this.results.set(response.items);
      }
    });
  }

  openPanel(): void {
    this.isPanelOpen.set(true);
  }

  closePanel(): void {
    // Deferred so a click on a result fires before the panel closes
    setTimeout(() => this.isPanelOpen.set(false), 150);
  }

  select(product: Product): void {
    this.selected.set(product);
    this.query.setValue(product.name, { emitEvent: false });
    this.isPanelOpen.set(false);
    this.productSelected.emit(product);
  }

  clear(): void {
    this.selected.set(null);
    this.query.setValue('', { emitEvent: false });
    this.productSelected.emit(null);
  }
}
