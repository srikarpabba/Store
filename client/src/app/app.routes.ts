import { Routes } from '@angular/router';
import { roleGuard } from './core/auth/guards/role.guard';
import { shopSectionGuard } from './features/shop/guards/shop-section.guard';
import { NotFound } from './shared/pages/not-found/not-found';
import { PrivacyPolicy } from './shared/pages/legal/privacy-policy/privacy-policy';
import { ServerError } from './shared/pages/server-error/server-error';
import { Terms } from './shared/pages/legal/terms/terms';

export const routes: Routes = [
    {
        path: '',
        loadChildren: () =>
            import('./features/home/home.routes')
                .then(r => r.HOME_ROUTES),
        title: 'Store'
    },
    {
        path: 'account',
        loadChildren: () =>
            import('./features/account/account.routes')
                .then(r => r.ACCOUNT_ROUTES)
    },
    {
        path: 'admin',
        canActivate: [roleGuard('Admin')],
        loadChildren: () =>
            import('./features/admin/admin.routes')
                .then(r => r.ADMIN_ROUTES)
    },
    {
        path: ':section',
        canMatch: [shopSectionGuard],
        loadChildren: () =>
            import('./features/shop/shop.routes')
                .then(r => r.SHOP_ROUTES)
    },
    {
        path: 'terms-and-conditions',
        component: Terms,
        title: 'Terms & Conditions | Store'
    },
    {
        path: 'privacy-policy',
        component: PrivacyPolicy,
        title: 'Privacy Policy | Store'
    },
    {
        path: 'server-error',
        component: ServerError,
        title: 'Connection Problem | Store'
    },
    {
        path: '**',
        component: NotFound,
        title: 'Page Not Found | Store'
    }
];