import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./layout/main-layout/main-layout.component').then(m => m.MainLayoutComponent),
    loadChildren: () => import("./features/main/main.routes").then(m => m.routes)
  },
  {
    path: 'auth',
    loadComponent: () => import("./layout/auth-layout/auth-layout.component").then(m => m.AuthLayoutComponent),
    loadChildren: () => import("./features/auth/auth.routes").then(m => m.routes)
  },
];
