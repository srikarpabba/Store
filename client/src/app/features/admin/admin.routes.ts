import { Routes } from '@angular/router';
import { AdminDashboard } from './dashboard/admin-dashboard';
import { AdminProducts } from './products/admin-products';
import { ProductForm } from './products/product-form/product-form';
import { AdminCategories } from './categories/admin-categories';
import { CategoryForm } from './categories/category-form/category-form';
import { AdminBrands } from './brands/admin-brands';
import { BrandForm } from './brands/brand-form/brand-form';
import { pendingChangesGuard } from '../../core/guards/pending-changes.guard';

export const ADMIN_ROUTES: Routes = [
  {
    path: '',
    component: AdminDashboard,
    title: 'Admin Dashboard | Store'
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
];
