import { inject, Service } from '@angular/core';
import { ProductQuery } from '../models/product-query';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { Product } from '../models/product';
import { ProductDetails } from '../models/product-details';
import { PagedResponse } from '../models/paged-response';
import { ProductApi } from '../api/product-api';
import { ProductFilters } from '../models/product-filters';

@Service()
export class ProductService {

    private readonly http = inject(HttpClient);

    private readonly apiUrl = environment.apiUrl;

    private static readonly DEFAULT_PAGE_INDEX = 1;
    private static readonly DEFAULT_PAGE_SIZE = 20;

    getProducts(query: ProductQuery) {
        return this.http.get<PagedResponse<Product>>(
            `${this.apiUrl}${ProductApi.products}`,
            {
                params: this.buildParams(query)
            }
        );
    }

    getProduct(id: string) {
        return this.http.get<ProductDetails>(
            `${this.apiUrl}${ProductApi.details(id)}`
        );
    }

    getFilters() {
        return this.http.get<ProductFilters>(
            `${this.apiUrl}${ProductApi.filters}`
        );
    }

    private buildParams(query: ProductQuery): HttpParams {

        let params = new HttpParams();

        if (query.search) {
            params = params.set('search', query.search);
        }

        query.brands?.forEach(brand =>
            params = params.append('brands', brand));

        query.categories?.forEach(category =>
            params = params.append('categories', category));

        query.colors?.forEach(color =>
            params = params.append('colors', color));

        query.sizes?.forEach(size =>
            params = params.append('sizes', size));

        query.genders?.forEach(gender =>
            params = params.append('genders', gender));

        if (query.minPrice != null) {
            params = params.set('minPrice', query.minPrice);
        }

        if (query.maxPrice != null) {
            params = params.set('maxPrice', query.maxPrice);
        }

        if (query.sort != null) {
            params = params.set('sort', query.sort);
        }

        params = params
            .set('pageIndex', query.pageIndex ?? ProductService.DEFAULT_PAGE_INDEX)
            .set('pageSize', query.pageSize ?? ProductService.DEFAULT_PAGE_SIZE);

        return params;
    }
}
