import { inject, Service } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { PromotionApi } from '../../shop/api/promotion-api';
import { Promotion } from '../../shop/models/promotion';
import { SavePromotionBatchRequest, SavePromotionRequest } from '../models/save-promotion-request';

@Service()
export class AdminPromotionService {

    private readonly http = inject(HttpClient);

    private readonly apiUrl = environment.apiUrl;

    getAll() {
        return this.http.get<Promotion[]>(`${this.apiUrl}${PromotionApi.promotions}`);
    }

    getById(id: string) {
        return this.http.get<Promotion>(`${this.apiUrl}${PromotionApi.details(id)}`);
    }

    create(request: SavePromotionRequest) {
        return this.http.post<string>(`${this.apiUrl}${PromotionApi.promotions}`, request);
    }

    createBatch(request: SavePromotionBatchRequest) {
        return this.http.post<string[]>(`${this.apiUrl}${PromotionApi.promotions}/batch`, request);
    }

    update(id: string, request: SavePromotionRequest) {
        return this.http.put<void>(`${this.apiUrl}${PromotionApi.details(id)}`, request);
    }

    delete(id: string) {
        return this.http.delete<void>(`${this.apiUrl}${PromotionApi.details(id)}`);
    }
}
