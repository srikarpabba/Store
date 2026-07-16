import { HttpErrorResponse, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

/**
 * Attaches the Bearer token to API requests and, on a 401,
 * refreshes the session once and retries the original request.
 */
export const authInterceptor: HttpInterceptorFn = (req, next) => {

    const authService = inject(AuthService);

    const isAuthRequest = req.url.includes('/auth/');

    const authorized = (request: HttpRequest<unknown>) => {
        const token = authService.accessToken;
        return token && !isAuthRequest
            ? request.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
            : request;
    };

    return next(authorized(req)).pipe(
        catchError((error: HttpErrorResponse) => {

            const canRefresh = error.status === 401
                && !isAuthRequest
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
