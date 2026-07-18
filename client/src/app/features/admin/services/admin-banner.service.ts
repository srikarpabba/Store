import { inject, Service } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { BannerApi } from '../../shop/api/banner-api';
import { Banner } from '../../shop/models/banner';
import { SaveBannerRequest } from '../models/save-banner-request';

@Service()
export class AdminBannerService {

    private readonly http = inject(HttpClient);

    private readonly apiUrl = environment.apiUrl;

    getAll(storefront?: string) {
        const url = storefront
            ? `${this.apiUrl}${BannerApi.banners}?storefront=${storefront}`
            : `${this.apiUrl}${BannerApi.banners}`;

        return this.http.get<Banner[]>(url);
    }

    getById(id: string) {
        return this.http.get<Banner>(`${this.apiUrl}${BannerApi.details(id)}`);
    }

    create(request: SaveBannerRequest) {
        return this.http.post<string>(`${this.apiUrl}${BannerApi.banners}`, request);
    }

    update(id: string, request: SaveBannerRequest) {
        return this.http.put<void>(`${this.apiUrl}${BannerApi.details(id)}`, request);
    }

    delete(id: string) {
        return this.http.delete<void>(`${this.apiUrl}${BannerApi.details(id)}`);
    }

    uploadImage(id: string, file: File) {
        const form = new FormData();
        form.append('file', file);

        return this.http.post<void>(`${this.apiUrl}${BannerApi.image(id)}`, form);
    }
}
