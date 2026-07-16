import { Routes } from '@angular/router';
import { AdminDashboard } from './dashboard/admin-dashboard';

export const ADMIN_ROUTES: Routes = [
  {
    path: '',
    component: AdminDashboard,
    title: 'Admin Dashboard | Store'
  }
];
