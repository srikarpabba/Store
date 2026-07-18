import { Routes } from '@angular/router';
import { AdminDashboard } from './dashboard/admin-dashboard';
import { ProductManagement } from './product-management/product-management';
import { StoreLook } from './store-look/store-look';
import { AdminProducts } from './products/admin-products';
import { ProductForm } from './products/product-form/product-form';
import { AdminCategories } from './categories/admin-categories';
import { CategoryForm } from './categories/category-form/category-form';
import { AdminBrands } from './brands/admin-brands';
import { BrandForm } from './brands/brand-form/brand-form';
import { AdminBanners } from './banners/admin-banners';
import { BannerForm } from './banners/banner-form/banner-form';
import { pendingChangesGuard } from '../../core/guards/pending-changes.guard';

export const ADMIN_ROUTES: Routes = [
  {
    path: '',
    component: AdminDashboard,
    title: 'Admin Dashboard | Store'
  },
  {
    path: 'product-management',
    children: [
      {
        path: '',
        component: ProductManagement,
        title: 'Product Management | Admin | Store'
      },
      {
        path: 'products',
        component: AdminProducts,
        title: 'Products | Admin | Store'
      },
      {
        path: 'products/new',
        component: ProductForm,
        canDeactivate: [pendingChangesGuard],
        title: 'New Product | Admin | Store'
      },
      {
        path: 'products/:id/edit',
        component: ProductForm,
        canDeactivate: [pendingChangesGuard],
        title: 'Edit Product | Admin | Store'
      },
      {
        path: 'categories',
        component: AdminCategories,
        title: 'Categories | Admin | Store'
      },
      {
        path: 'categories/new',
        component: CategoryForm,
        canDeactivate: [pendingChangesGuard],
        title: 'New Category | Admin | Store'
      },
      {
        path: 'categories/:id/edit',
        component: CategoryForm,
        canDeactivate: [pendingChangesGuard],
        title: 'Edit Category | Admin | Store'
      },
      {
        path: 'brands',
        component: AdminBrands,
        title: 'Brands | Admin | Store'
      },
      {
        path: 'brands/new',
        component: BrandForm,
        canDeactivate: [pendingChangesGuard],
        title: 'New Brand | Admin | Store'
      },
      {
        path: 'brands/:id/edit',
        component: BrandForm,
        canDeactivate: [pendingChangesGuard],
        title: 'Edit Brand | Admin | Store'
      }
    ]
  },
  {
    path: 'store-look',
    children: [
      {
        path: '',
        component: StoreLook,
        title: 'Store Look | Admin | Store'
      },
      {
        path: 'banners',
        component: AdminBanners,
        title: 'Banners | Admin | Store'
      },
      {
        path: 'banners/new',
        component: BannerForm,
        canDeactivate: [pendingChangesGuard],
        title: 'New Banner | Admin | Store'
      },
      {
        path: 'banners/:id/edit',
        component: BannerForm,
        canDeactivate: [pendingChangesGuard],
        title: 'Edit Banner | Admin | Store'
      }
    ]
  }
];
