import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthApi } from '../auth/auth-api';
import { NotificationService } from '../services/notification.service';
import { extractHttpErrorMessage } from '../../shared/utils/http-error';

/**
 * Global error handler.
 * - Status 0 (server unreachable / offline): the whole app is down,
 *   so navigate to the full-page connection screen instead of toasting.
 * - 404: the resource the page is about doesn't exist — show the
 *   not-found page (keeping the original URL in the address bar).
 * - Anything else: show the failure as a snackbar.
 * Expired-session 401s are recovered by the auth interceptor before
 * they ever reach this one. The refresh call itself is skipped —
 * when it fails, its error is rethrown into the original request's
 * chain and handled there, so handling it here would double up.
 */
export const errorInterceptor: HttpInterceptorFn = (req, next) => {

    const notificationService = inject(NotificationService);
    const router = inject(Router);

    const isRefreshRequest = req.url.includes(AuthApi.refreshToken);

    return next(req).pipe(
        catchError((error: HttpErrorResponse) => {

            if (!isRefreshRequest) {
                if (error.status === 0) {
                    if (!router.url.startsWith('/server-error')) {
                        router.navigate(
                            ['/server-error'],
                            { queryParams: { returnUrl: router.url } }
                        );
                    }
                } else if (error.status === 404) {
                    router.navigateByUrl('/not-found', { skipLocationChange: true });
                } else {
                    notificationService.error(extractHttpErrorMessage(error));
                }
            }

            return throwError(() => error);
        })
    );
};
