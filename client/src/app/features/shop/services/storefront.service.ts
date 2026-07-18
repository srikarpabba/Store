import { inject, Service } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { StorefrontApi } from '../api/storefront-api';
import { StorefrontSections } from '../models/storefront-section';

@Service()
export class StorefrontService {

    private readonly http = inject(HttpClient);

    private readonly apiUrl = environment.apiUrl;

    getSections(storefront: string) {
        return this.http.get<StorefrontSections>(`${this.apiUrl}${StorefrontApi.sections(storefront)}`);
    }
}
