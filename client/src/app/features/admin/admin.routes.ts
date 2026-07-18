import { Routes } from '@angular/router';
import { AdminDashboard } from './pages/dashboard/admin-dashboard';
import { ProductManagement } from './pages/product-management/product-management';
import { StoreLook } from './pages/store-look/store-look';
import { AdminProducts } from './pages/products/admin-products';
import { ProductForm } from './pages/products/product-form/product-form';
import { AdminCategories } from './pages/categories/admin-categories';
import { CategoryForm } from './pages/categories/category-form/category-form';
import { AdminBrands } from './pages/brands/admin-brands';
import { BrandForm } from './pages/brands/brand-form/brand-form';
import { AdminColors } from './pages/colors/admin-colors';
import { ColorForm } from './pages/colors/color-form/color-form';
import { AdminSizes } from './pages/sizes/admin-sizes';
import { SizeForm } from './pages/sizes/size-form/size-form';
import { AdminBanners } from './pages/banners/admin-banners';
import { BannerForm } from './pages/banners/banner-form/banner-form';
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
      },
      {
        path: 'colors',
        component: AdminColors,
        title: 'Colors | Admin | Store'
      },
      {
        path: 'colors/new',
        component: ColorForm,
        canDeactivate: [pendingChangesGuard],
        title: 'New Color | Admin | Store'
      },
      {
        path: 'colors/:id/edit',
        component: ColorForm,
        canDeactivate: [pendingChangesGuard],
        title: 'Edit Color | Admin | Store'
      },
      {
        path: 'sizes',
        component: AdminSizes,
        title: 'Sizes | Admin | Store'
      },
      {
        path: 'sizes/new',
        component: SizeForm,
        canDeactivate: [pendingChangesGuard],
        title: 'New Size | Admin | Store'
      },
      {
        path: 'sizes/:id/edit',
        component: SizeForm,
        canDeactivate: [pendingChangesGuard],
        title: 'Edit Size | Admin | Store'
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
