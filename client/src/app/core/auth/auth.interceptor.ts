import { HttpErrorResponse, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthApi } from './auth-api';
import { AuthService } from './auth.service';

/**
 * Attaches the Bearer token to API requests and, on a 401,
 * refreshes the session once and retries the original request.
 */
export const authInterceptor: HttpInterceptorFn = (req, next) => {

    const authService = inject(AuthService);

    // The refresh call authenticates via the refresh token in its body —
    // attaching the (possibly expired) access token or refreshing on its
    // 401 would loop. Every other request gets the token when one exists;
    // some /auth/ endpoints (set/change password) require it.
    const isRefreshRequest = req.url.includes(AuthApi.refreshToken);

    const authorized = (request: HttpRequest<unknown>) => {
        const token = authService.accessToken;
        return token && !isRefreshRequest
            ? request.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
            : request;
    };

    return next(authorized(req)).pipe(
        catchError((error: HttpErrorResponse) => {

            const canRefresh = error.status === 401
                && !isRefreshRequest
                && authService.refreshToken !== null;

            if (!canRefresh) {
                return throwError(() => error);
            }

            return authService.refreshTokens().pipe(
                switchMap(() => next(authorized(req))),
                catchError(refreshError => {
                    authService.logout();
                    return throwError(() => refreshError);
                })
            );
        })
    );
};
