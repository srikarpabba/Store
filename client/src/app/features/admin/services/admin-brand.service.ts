import { inject, Service } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { BrandApi } from '../../shop/api/brand-api';
import { Brand } from '../../shop/models/brand';
import { PagedResponse } from '../../shop/models/paged-response';
import { SaveBrandRequest } from '../models/save-brand-request';

@Service()
export class AdminBrandService {

    private readonly http = inject(HttpClient);

    private readonly apiUrl = environment.apiUrl;

    getAll(pageIndex: number, pageSize: number) {

        const params = new HttpParams()
            .set('pageIndex', pageIndex)
            .set('pageSize', pageSize);

        return this.http.get<PagedResponse<Brand>>(
            `${this.apiUrl}${BrandApi.brands}`,
            { params }
        );
    }

    getById(id: string) {
        return this.http.get<Brand>(`${this.apiUrl}${BrandApi.details(id)}`);
    }

    create(request: SaveBrandRequest) {
        return this.http.post<string>(`${this.apiUrl}${BrandApi.brands}`, request);
    }

    update(id: string, request: SaveBrandRequest) {
        return this.http.put<void>(`${this.apiUrl}${BrandApi.details(id)}`, request);
    }

    delete(id: string) {
        return this.http.delete<void>(`${this.apiUrl}${BrandApi.details(id)}`);
    }

    uploadLogo(id: string, file: File) {
        const form = new FormData();
        form.append('file', file);

        return this.http.post<void>(`${this.apiUrl}${BrandApi.logo(id)}`, form);
    }

    deleteLogo(id: string) {
        return this.http.delete<void>(`${this.apiUrl}${BrandApi.logo(id)}`);
    }
}
