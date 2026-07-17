import { Routes } from '@angular/router';
import { AdminDashboard } from './dashboard/admin-dashboard';
import { AdminProducts } from './products/admin-products';
import { ProductForm } from './products/product-form/product-form';
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
  }
];
