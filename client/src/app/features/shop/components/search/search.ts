import { Component, ElementRef, HostListener, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { catchError, debounceTime, distinctUntilChanged, map, of, switchMap, tap } from 'rxjs';
import { ProductService } from '../../services/product.service';
import { Product } from '../../models/product';
import { PricePipe } from '../../../../shared/pipes/price.pipe';

@Component({
  selector: 'app-search',
  imports: [ReactiveFormsModule, MatIconModule, PricePipe],
  templateUrl: './search.html',
  styleUrl: './search.css',
})
export class Search {

  private static readonly MIN_QUERY_LENGTH = 2;
  private static readonly SUGGESTION_COUNT = 6;

  private readonly productService = inject(ProductService);
  private readonly router = inject(Router);
  private readonly elementRef = inject(ElementRef<HTMLElement>);

  readonly query = new FormControl('', { nonNullable: true });

  readonly results = signal<Product[]>([]);
  readonly isLoading = signal(false);
  readonly isOpen = signal(false);

  constructor() {
    this.query.valueChanges.pipe(
      map(value => value.trim()),
      tap(term => {
        if (term.length < Search.MIN_QUERY_LENGTH) {
          this.close();
        }
      }),
      debounceTime(300),
      distinctUntilChanged(),
      switchMap(term => {
        if (term.length < Search.MIN_QUERY_LENGTH) {
          return of(null);
        }

        this.isLoading.set(true);
        this.isOpen.set(true);

        return this.productService.getProducts({
          search: term,
          pageIndex: 1,
          pageSize: Search.SUGGESTION_COUNT
        }).pipe(catchError(() => of(null)));
      }),
      takeUntilDestroyed()
    ).subscribe(response => {
      this.isLoading.set(false);

      if (response !== null) {
        this.results.set(response.items);
      }
    });
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (!this.elementRef.nativeElement.contains(event.target as Node)) {
      this.isOpen.set(false);
    }
  }

  onFocus(): void {
    if (this.query.value.trim().length >= Search.MIN_QUERY_LENGTH) {
      this.isOpen.set(true);
    }
  }

  goToProduct(product: Product): void {
    this.close();
    this.router.navigate(['/new', product.id]);
  }

  viewAllResults(): void {
    const term = this.query.value.trim();

    if (term.length < Search.MIN_QUERY_LENGTH) {
      return;
    }

    this.close();
    this.router.navigate(['/new'], { queryParams: { search: term } });
  }

  clear(): void {
    this.query.setValue('');
    this.close();
  }

  close(): void {
    this.isOpen.set(false);
    this.isLoading.set(false);
    this.results.set([]);
  }
}
