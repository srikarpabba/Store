import { computed, inject, Service, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { finalize, Observable, shareReplay, tap } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { AuthApi } from '../api/auth-api';
import { AccessTokensResponse } from '../models/access-tokens-response';
import { ChangePasswordRequest } from '../models/change-password-request';
import { GoogleAuthResponse } from '../models/google-auth-response';
import { LoginRequest } from '../models/login-request';
import { RegisterRequest } from '../models/register-request';
import { ResetPasswordRequest } from '../models/reset-password-request';
import { User } from '../models/user';

@Service()
export class AuthService {

    private readonly http = inject(HttpClient);

    private readonly apiUrl = environment.apiUrl;

    private static readonly ACCESS_TOKEN_KEY = 'store.access-token';
    private static readonly REFRESH_TOKEN_KEY = 'store.refresh-token';

    private readonly _currentUser = signal<User | null>(this.readUserFromStoredToken());

    /** The signed-in user decoded from the access token, or null */
    readonly currentUser = this._currentUser.asReadonly();

    readonly isAuthenticated = computed(() => this.currentUser() !== null);

    readonly isCustomer = computed(() => this.hasRole('Customer'));

    readonly isAdmin = computed(() => this.hasRole('Admin'));

    hasRole(role: string): boolean {
        return this.currentUser()?.roles.includes(role) ?? false;
    }

    /** Shared in-flight refresh so concurrent 401s trigger a single refresh call */
    private refreshInFlight$: Observable<AccessTokensResponse> | null = null;

    get accessToken(): string | null {
        return localStorage.getItem(AuthService.ACCESS_TOKEN_KEY)
            ?? sessionStorage.getItem(AuthService.ACCESS_TOKEN_KEY);
    }

    get refreshToken(): string | null {
        return localStorage.getItem(AuthService.REFRESH_TOKEN_KEY)
            ?? sessionStorage.getItem(AuthService.REFRESH_TOKEN_KEY);
    }

    /** True when the session is persisted across browser restarts */
    private get isPersistent(): boolean {
        return localStorage.getItem(AuthService.REFRESH_TOKEN_KEY) !== null;
    }

    login(request: LoginRequest, remember = true) {
        return this.http.post<AccessTokensResponse>(
            `${this.apiUrl}${AuthApi.login}`,
            request
        ).pipe(tap(tokens => this.startSession(tokens, remember)));
    }

    register(request: RegisterRequest) {
        return this.http.post<string>(
            `${this.apiUrl}${AuthApi.register}`,
            request
        );
    }

    loginWithGoogle(idToken: string) {
        return this.http.post<GoogleAuthResponse>(
            `${this.apiUrl}${AuthApi.google}`,
            { idToken }
        ).pipe(tap(tokens => this.startSession(tokens)));
    }

    forgotPassword(email: string) {
        return this.http.post<void>(
            `${this.apiUrl}${AuthApi.forgotPassword}`,
            { email }
        );
    }

    resetPassword(request: ResetPasswordRequest) {
        return this.http.post<void>(
            `${this.apiUrl}${AuthApi.resetPassword}`,
            request
        );
    }

    changePassword(request: ChangePasswordRequest) {
        return this.http.post<void>(
            `${this.apiUrl}${AuthApi.changePassword}`,
            request
        );
    }

    confirmEmail(email: string, token: string) {
        return this.http.post<void>(
            `${this.apiUrl}${AuthApi.confirmEmail}`,
            { email, token }
        );
    }

    refreshTokens() {
        const remember = this.isPersistent;

        this.refreshInFlight$ ??= this.http.post<AccessTokensResponse>(
            `${this.apiUrl}${AuthApi.refreshToken}`,
            { refreshToken: this.refreshToken }
        ).pipe(
            tap(tokens => this.startSession(tokens, remember)),
            finalize(() => this.refreshInFlight$ = null),
            shareReplay(1)
        );

        return this.refreshInFlight$;
    }

    logout(): void {
        for (const storage of [localStorage, sessionStorage]) {
            storage.removeItem(AuthService.ACCESS_TOKEN_KEY);
            storage.removeItem(AuthService.REFRESH_TOKEN_KEY);
        }

        this._currentUser.set(null);
    }

    private startSession(tokens: AccessTokensResponse, remember = true): void {
        const storage = remember ? localStorage : sessionStorage;
        const other = remember ? sessionStorage : localStorage;

        other.removeItem(AuthService.ACCESS_TOKEN_KEY);
        other.removeItem(AuthService.REFRESH_TOKEN_KEY);

        storage.setItem(AuthService.ACCESS_TOKEN_KEY, tokens.accessToken);
        storage.setItem(AuthService.REFRESH_TOKEN_KEY, tokens.refreshToken);

        this._currentUser.set(this.decodeUser(tokens.accessToken));
    }

    private readUserFromStoredToken(): User | null {
        const token = this.accessToken;
        return token ? this.decodeUser(token) : null;
    }

    private decodeUser(accessToken: string): User | null {
        try {
            const payload = JSON.parse(
                atob(accessToken.split('.')[1].replace(/-/g, '+').replace(/_/g, '/'))
            );

            // a single role arrives as a string, multiple as an array
            const roleClaim = payload.role;
            const roles: string[] = Array.isArray(roleClaim)
                ? roleClaim
                : roleClaim ? [roleClaim] : [];

            return {
                id: payload.sub,
                email: payload.email,
                roles
            };
        } catch {
            return null;
        }
    }
}
