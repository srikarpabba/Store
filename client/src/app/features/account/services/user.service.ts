import { inject, Service } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { UserApi } from '../api/user-api';
import { Address, SaveAddressRequest } from '../models/address';
import { Profile } from '../models/profile';
import { UpdateProfileRequest } from '../models/update-profile-request';

@Service()
export class UserService {

    private readonly http = inject(HttpClient);

    private readonly apiUrl = environment.apiUrl;

    getProfile() {
        return this.http.get<Profile>(`${this.apiUrl}${UserApi.me}`);
    }

    updateProfile(request: UpdateProfileRequest) {
        return this.http.put<void>(`${this.apiUrl}${UserApi.me}`, request);
    }

    resendEmailConfirmation() {
        return this.http.post<void>(`${this.apiUrl}${UserApi.resendConfirmation}`, {});
    }

    getAddresses() {
        return this.http.get<Address[]>(`${this.apiUrl}${UserApi.addresses}`);
    }

    addAddress(request: SaveAddressRequest) {
        return this.http.post<string>(`${this.apiUrl}${UserApi.addresses}`, request);
    }

    updateAddress(id: string, request: SaveAddressRequest) {
        return this.http.put<void>(`${this.apiUrl}${UserApi.address(id)}`, request);
    }

    deleteAddress(id: string) {
        return this.http.delete<void>(`${this.apiUrl}${UserApi.address(id)}`);
    }

    setDefaultAddress(id: string) {
        return this.http.put<void>(`${this.apiUrl}${UserApi.defaultAddress(id)}`, {});
    }
}
