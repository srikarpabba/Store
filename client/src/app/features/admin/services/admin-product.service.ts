import { inject, Service } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { ProductApi } from '../../shop/api/product-api';
import { ProductDetails } from '../../shop/models/product-details';
import { SaveProductRequest } from '../models/save-product-request';

/** Admin-only product mutations; reads reuse the shop's ProductService */
@Service()
export class AdminProductService {

    private readonly http = inject(HttpClient);

    private readonly apiUrl = environment.apiUrl;

    getDetails(id: string) {
        return this.http.get<ProductDetails>(
            `${this.apiUrl}${ProductApi.details(id)}`
        );
    }

    create(request: SaveProductRequest) {
        return this.http.post<string>(
            `${this.apiUrl}${ProductApi.products}`,
            request
        );
    }

    update(id: string, request: SaveProductRequest) {
        return this.http.put<void>(
            `${this.apiUrl}${ProductApi.details(id)}`,
            request
        );
    }

    delete(id: string) {
        return this.http.delete<void>(
            `${this.apiUrl}${ProductApi.details(id)}`
        );
    }

    uploadImages(productId: string, productColorId: string, files: File[]) {
        const form = new FormData();
        form.append('productColorId', productColorId);

        for (const file of files) {
            form.append('files', file);
        }

        return this.http.post<void>(
            `${this.apiUrl}${ProductApi.details(productId)}/images`,
            form
        );
    }

    deleteImage(productId: string, photoId: string) {
        return this.http.delete<void>(
            `${this.apiUrl}${ProductApi.details(productId)}/images/${photoId}`
        );
    }

    setMainImage(productId: string, photoId: string) {
        return this.http.put<void>(
            `${this.apiUrl}${ProductApi.details(productId)}/images/${photoId}/main`,
            {}
        );
    }
}
