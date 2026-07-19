import { Routes } from '@angular/router';
import { ShopPage } from './pages/shop-page/shop-page';
import { ProductDetails } from './pages/product-details/product-details';
import { CategoryListing } from './pages/category-listing/category-listing';
import { productIdGuard } from './guards/product-id.guard';

export const SHOP_ROUTES: Routes = [
  {
    path: '',
    component: ShopPage,
    title: route => {
      const section = route.paramMap.get('section') ?? 'Shop';
      return `${section.charAt(0).toUpperCase()}${section.slice(1)} | Store`;
    }
  },
  {
    // GUID segments are product ids…
    path: ':id',
    component: ProductDetails,
    canMatch: [productIdGuard]
  },
  {
    // …anything else is a category slug (e.g. /women/t-shirt)
    path: ':categorySlug',
    component: CategoryListing
  }
];
