import { inject, Service } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { CategoryApi } from '../../shop/api/category-api';
import { Category } from '../../shop/models/category';
import { SaveCategoryRequest } from '../models/save-category-request';

@Service()
export class AdminCategoryService {

    private readonly http = inject(HttpClient);

    private readonly apiUrl = environment.apiUrl;

    getAll() {
        return this.http.get<Category[]>(`${this.apiUrl}${CategoryApi.categories}`);
    }

    getById(id: string) {
        return this.http.get<Category>(`${this.apiUrl}${CategoryApi.details(id)}`);
    }

    create(request: SaveCategoryRequest) {
        return this.http.post<string>(`${this.apiUrl}${CategoryApi.categories}`, request);
    }

    update(id: string, request: SaveCategoryRequest) {
        return this.http.put<void>(`${this.apiUrl}${CategoryApi.details(id)}`, request);
    }

    delete(id: string) {
        return this.http.delete<void>(`${this.apiUrl}${CategoryApi.details(id)}`);
    }

    uploadGenderPhoto(categoryId: string, genderId: string, file: File) {
        const form = new FormData();
        form.append('file', file);

        return this.http.post<void>(`${this.apiUrl}${CategoryApi.photo(categoryId, genderId)}`, form);
    }

    deleteGenderPhoto(categoryId: string, genderId: string) {
        return this.http.delete<void>(`${this.apiUrl}${CategoryApi.photo(categoryId, genderId)}`);
    }
}
