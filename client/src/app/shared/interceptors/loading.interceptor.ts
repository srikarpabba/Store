import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { delay, finalize, identity } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LoadingService } from '../services/loading.service';

/**
 * Flags mutations (POST/PUT/PATCH/DELETE) as in-flight so buttons can show
 * a spinner. GETs are skipped — background reads like the search typeahead
 * shouldn't animate the page's submit button.
 * In development, responses are artificially delayed so loading states are
 * actually visible against a near-instant local API.
 */
export const loadingInterceptor: HttpInterceptorFn = (req, next) => {

    if (req.method === 'GET') {
        return next(req);
    }

    const loadingService = inject(LoadingService);

    loadingService.start();

    return next(req).pipe(
        environment.production ? identity : delay(500),
        finalize(() => loadingService.stop())
    );
};
