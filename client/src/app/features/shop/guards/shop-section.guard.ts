import { CanMatchFn } from '@angular/router';
import { ShopSection } from '../models/enums/shop-section';

/**
 * Lets the `:section` route match only real shop sections,
 * so unknown URLs fall through to the wildcard 404 route.
 */
export const shopSectionGuard: CanMatchFn = (_route, segments) => {
    const section = segments[0]?.path;

    return Object.values(ShopSection).includes(section as ShopSection);
};
