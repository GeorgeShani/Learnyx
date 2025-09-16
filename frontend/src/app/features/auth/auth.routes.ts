import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'signup',
    loadComponent: () =>
      import('./views/sign-up/sign-up.component').then(
        (m) => m.SignUpComponent
      ),
  },
  {
    path: 'login',
    loadComponent: () =>
      import('./views/log-in/log-in.component').then((m) => m.LogInComponent),
  },
  {
    path: 'forgot-password',
    loadComponent: () =>
      import('./views/forgot-password/forgot-password.component').then(
        (m) => m.ForgotPasswordComponent
      ),
  },
  {
    path: 'callback',
    loadComponent: () =>
      import('./views/callback/callback.component').then(
        (m) => m.CallbackComponent
      ),
  },
];
