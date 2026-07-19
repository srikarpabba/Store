import { CanMatchFn } from '@angular/router';

const GUID_PATTERN = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;

/**
 * Product ids are GUIDs; category slugs never are. Lets `/:section/:id`
 * (product details) match only real ids, so `/:section/:categorySlug`
 * (category listing) can catch everything else.
 */
export const productIdGuard: CanMatchFn = (_route, segments) => {
    const last = segments[segments.length - 1]?.path ?? '';

    return GUID_PATTERN.test(last);
};
