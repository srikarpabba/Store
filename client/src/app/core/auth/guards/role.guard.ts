import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../auth.service';

/**
 * Restricts a route to users holding the given role.
 * Unauthenticated users go to login (with a return url);
 * authenticated users lacking the role are sent home.
 */
export function roleGuard(role: string): CanActivateFn {
    return (_route, state) => {
        const authService = inject(AuthService);
        const router = inject(Router);

        if (!authService.isAuthenticated()) {
            return router.createUrlTree(
                ['/account/login'],
                { queryParams: { returnUrl: state.url } }
            );
        }

        return authService.hasRole(role)
            ? true
            : router.createUrlTree(['/']);
    };
}
