import { inject, Service } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { SubcategoryApi } from '../../shop/api/subcategory-api';
import { Subcategory } from '../../shop/models/subcategory';
import { SaveSubcategoryRequest } from '../models/save-subcategory-request';

@Service()
export class AdminSubcategoryService {

    private readonly http = inject(HttpClient);

    private readonly apiUrl = environment.apiUrl;

    getAll() {
        return this.http.get<Subcategory[]>(`${this.apiUrl}${SubcategoryApi.subcategories}`);
    }

    create(request: SaveSubcategoryRequest) {
        return this.http.post<string>(`${this.apiUrl}${SubcategoryApi.subcategories}`, request);
    }

    update(id: string, request: SaveSubcategoryRequest) {
        return this.http.put<void>(`${this.apiUrl}${SubcategoryApi.details(id)}`, request);
    }

    delete(id: string) {
        return this.http.delete<void>(`${this.apiUrl}${SubcategoryApi.details(id)}`);
    }
}
