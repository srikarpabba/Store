import { Routes } from '@angular/router';
import { AuthPage } from './pages/auth-page/auth-page';
import { ChangePassword } from './pages/change-password/change-password';
import { ConfirmEmail } from './pages/confirm-email/confirm-email';
import { Dashboard } from './pages/dashboard/dashboard';
import { ForgotPassword } from './pages/forgot-password/forgot-password';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';
import { ResetPassword } from './pages/reset-password/reset-password';
import { SetPassword } from './pages/set-password/set-password';
import { authGuard } from '../../core/auth/guards/auth.guard';
import { guestGuard } from '../../core/auth/guards/guest.guard';
import { roleGuard } from '../../core/auth/guards/role.guard';
import { pendingChangesGuard } from '../../core/guards/pending-changes.guard';

export const ACCOUNT_ROUTES: Routes = [
  {
    path: 'dashboard',
    component: Dashboard,
    canActivate: [roleGuard('Customer')],
    canDeactivate: [pendingChangesGuard],
    title: 'My Account | Store'
  },
  {
    path: 'forgot-password',
    component: ForgotPassword,
    canActivate: [guestGuard],
    title: 'Forgot Password | Store'
  },
  {
    path: 'reset-password',
    component: ResetPassword,
    canActivate: [guestGuard],
    title: 'Reset Password | Store'
  },
  {
    path: 'confirm-email',
    component: ConfirmEmail,
    title: 'Confirm Email | Store'
  },
  {
    path: 'change-password',
    component: ChangePassword,
    canActivate: [authGuard],
    title: 'Change Password | Store'
  },
  {
    path: 'set-password',
    component: SetPassword,
    canActivate: [authGuard],
    title: 'Set Password | Store'
  },
  {
    path: '',
    component: AuthPage,
    canActivate: [guestGuard],
    children: [
      {
        path: 'login',
        component: Login,
        title: 'Sign in | Store'
      },
      {
        path: 'register',
        component: Register,
        title: 'Create account | Store'
      },
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'login'
      }
    ]
  }
];
