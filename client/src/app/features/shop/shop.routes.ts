import { Routes } from '@angular/router';
import { ShopPage } from './pages/shop-page/shop-page';
import { ProductDetails } from './pages/product-details/product-details';

export const SHOP_ROUTES: Routes = [
  {
    path: '',
    component: ShopPage
  },
  {
    path: ':id',
    component: ProductDetails
  }
];