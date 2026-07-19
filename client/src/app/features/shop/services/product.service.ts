import { inject, Service } from '@angular/core';
import { ProductQuery } from '../models/product-query';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Product } from '../models/product';
import { ProductDetails } from '../models/product-details';
import { PagedResponse } from '../models/paged-response';
import { ProductApi } from '../api/product-api';
import { ProductFilters } from '../models/product-filters';
import { ProductFacets } from '../models/product-facets';
import { ProductSort } from '../models/enums/product-sort';

interface GraphQlResponse<T> {
    data: T | null;
    errors?: { message: string }[];
}

interface ProductsGraphQlData {
    products: PagedResponse<Product>;
}

@Service()
export class ProductService {

    private readonly http = inject(HttpClient);

    private readonly apiUrl = environment.apiUrl;

    private static readonly DEFAULT_PAGE_INDEX = 1;
    private static readonly DEFAULT_PAGE_SIZE = 20;

    // The API's GraphQL schema auto-generates enum names from the C# enum
    // members using SCREAMING_SNAKE_CASE — doesn't match the PascalCase
    // values ProductSort already uses for the REST query string.
    private static readonly SORT_GRAPHQL_NAMES: Record<ProductSort, string> = {
        [ProductSort.Newest]: 'NEWEST',
        [ProductSort.PriceLowToHigh]: 'PRICE_LOW_TO_HIGH',
        [ProductSort.PriceHighToLow]: 'PRICE_HIGH_TO_LOW',
        [ProductSort.Rating]: 'RATING',
        [ProductSort.Name]: 'NAME',
    };

    private static readonly PRODUCTS_QUERY = `
        query Products($input: GetProductsQueryInput!) {
            products(input: $input) {
                items {
                    id name startingPrice rating image
                    category { id name }
                    subcategory { id name }
                    colors {
                        productColorId colorId colorName hexCode
                        photos { id fileName isMain }
                    }
                }
                pageIndex
                pageSize
                totalCount
                totalPages
                hasPrevious
                hasNext
            }
        }
    `;

    getProducts(query: ProductQuery) {
        return this.http.get<PagedResponse<Product>>(
            `${this.apiUrl}${ProductApi.products}`,
            {
                params: this.buildParams(query)
            }
        );
    }

    /**
     * Same data as getProducts, over GraphQL instead of REST — used by the
     * shop pages' infinite-scroll grid.
     *
     * HotChocolate returns query-level failures (e.g. validation errors) as
     * HTTP 200 with an `errors` array in the body, so the shared
     * errorInterceptor never sees them — check `errors` here and surface it
     * as a stream error the same way a REST failure would look to a caller.
     */
    getProductsGraphQl(query: ProductQuery): Observable<PagedResponse<Product>> {

        const input = {
            search: query.search,
            brands: query.brands,
            categories: query.categories,
            subcategories: query.subcategories,
            colors: query.colors,
            sizes: query.sizes,
            genders: query.genders,
            minPrice: query.minPrice,
            maxPrice: query.maxPrice,
            sort: query.sort ? ProductService.SORT_GRAPHQL_NAMES[query.sort] : undefined,
            pageIndex: query.pageIndex ?? ProductService.DEFAULT_PAGE_INDEX,
            pageSize: query.pageSize ?? ProductService.DEFAULT_PAGE_SIZE,
            // Only the shop grid's interactive cards (color swap, hover
            // slider) need this — getProductsGraphQl is exclusively used by
            // that path today (via ShopService.loadSection).
            includeColors: true
        };

        return this.http.post<GraphQlResponse<ProductsGraphQlData>>(
            environment.graphqlUrl,
            { query: ProductService.PRODUCTS_QUERY, variables: { input } }
        ).pipe(
            map(response => {
                if (response.errors?.length) {
                    throw new Error(response.errors[0].message);
                }

                return response.data!.products;
            })
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

    /** Counts per filter option for the given selection — paging/sort
        fields of the query are ignored server-side. */
    getFacets(query: ProductQuery) {
        return this.http.get<ProductFacets>(
            `${this.apiUrl}${ProductApi.facets}`,
            {
                params: this.buildParams(query)
            }
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

        query.subcategories?.forEach(subcategory =>
            params = params.append('subcategories', subcategory));

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
