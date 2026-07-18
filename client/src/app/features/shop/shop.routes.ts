import { Routes } from '@angular/router';
import { ShopPage } from './pages/shop-page/shop-page';
import { ProductDetails } from './pages/product-details/product-details';

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
    path: ':id',
    component: ProductDetails
  }
];
