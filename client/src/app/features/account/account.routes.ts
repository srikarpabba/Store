import { Routes } from '@angular/router';
import { AuthPage } from './auth-page/auth-page';
import { ChangePassword } from './change-password/change-password';
import { ConfirmEmail } from './confirm-email/confirm-email';
import { Dashboard } from './dashboard/dashboard';
import { ForgotPassword } from './forgot-password/forgot-password';
import { Login } from './login/login';
import { Register } from './register/register';
import { ResetPassword } from './reset-password/reset-password';
import { authGuard } from './guards/auth.guard';
import { roleGuard } from './guards/role.guard';

export const ACCOUNT_ROUTES: Routes = [
  {
    path: 'dashboard',
    component: Dashboard,
    canActivate: [roleGuard('Customer')],
    title: 'My Account | Store'
  },
  {
    path: 'forgot-password',
    component: ForgotPassword,
    title: 'Forgot Password | Store'
  },
  {
    path: 'reset-password',
    component: ResetPassword,
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
    path: '',
    component: AuthPage,
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
