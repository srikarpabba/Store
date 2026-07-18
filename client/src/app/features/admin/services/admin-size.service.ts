import { inject, Service } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { SizeApi } from '../../shop/api/size-api';
import { Size } from '../../shop/models/size';
import { SaveSizeRequest } from '../models/save-size-request';

@Service()
export class AdminSizeService {

    private readonly http = inject(HttpClient);

    private readonly apiUrl = environment.apiUrl;

    getAll() {
        return this.http.get<Size[]>(`${this.apiUrl}${SizeApi.sizes}`);
    }

    getById(id: string) {
        return this.http.get<Size>(`${this.apiUrl}${SizeApi.details(id)}`);
    }

    create(request: SaveSizeRequest) {
        return this.http.post<string>(`${this.apiUrl}${SizeApi.sizes}`, request);
    }

    update(id: string, request: SaveSizeRequest) {
        return this.http.put<void>(`${this.apiUrl}${SizeApi.details(id)}`, request);
    }

    delete(id: string) {
        return this.http.delete<void>(`${this.apiUrl}${SizeApi.details(id)}`);
    }
}
