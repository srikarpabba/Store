import { inject, Service } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { ColorApi } from '../../shop/api/color-api';
import { Color } from '../../shop/models/color';
import { SaveColorRequest } from '../models/save-color-request';

@Service()
export class AdminColorService {

    private readonly http = inject(HttpClient);

    private readonly apiUrl = environment.apiUrl;

    getAll() {
        return this.http.get<Color[]>(`${this.apiUrl}${ColorApi.colors}`);
    }

    getById(id: string) {
        return this.http.get<Color>(`${this.apiUrl}${ColorApi.details(id)}`);
    }

    create(request: SaveColorRequest) {
        return this.http.post<string>(`${this.apiUrl}${ColorApi.colors}`, request);
    }

    update(id: string, request: SaveColorRequest) {
        return this.http.put<void>(`${this.apiUrl}${ColorApi.details(id)}`, request);
    }

    delete(id: string) {
        return this.http.delete<void>(`${this.apiUrl}${ColorApi.details(id)}`);
    }
}
