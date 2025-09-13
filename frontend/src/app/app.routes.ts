import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./layout/main-layout/main-layout.component').then(
        (m) => m.MainLayoutComponent
      ),
    loadChildren: () =>
      import('./features/main/main.routes').then((m) => m.routes),
  },
  {
    path: 'auth',
    loadComponent: () =>
      import('./layout/auth-layout/auth-layout.component').then(
        (m) => m.AuthLayoutComponent
      ),
    loadChildren: () =>
      import('./features/auth/auth.routes').then((m) => m.routes),
  },
  {
    path: 'dashboard',
    loadComponent: () =>
      import('./layout/dashboard-layout/dashboard-layout.component').then(
        (m) => m.DashboardLayoutComponent
      ),
    loadChildren: () =>
      import('./features/dashboard/dashboard.routes').then((m) => m.routes),
  },
  {
    path: 'learning',
    loadComponent: () =>
      import('./layout/learning-layout/learning-layout.component').then(
        (m) => m.LearningLayoutComponent
      ),
    loadChildren: () =>
      import('./features/learning/learning.routes').then((m) => m.routes),
  },
  {
    path: '**',
    loadComponent: () =>
      import('./layout/error-layout/error-layout.component').then(
        (m) => m.ErrorLayoutComponent
      ),
  },
];
