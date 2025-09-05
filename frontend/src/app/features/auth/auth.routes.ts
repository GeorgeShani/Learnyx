import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'sign-up',
    loadComponent: () => import("./views/sign-up/sign-up.component").then(m => m.SignUpComponent)
  },
  {
    path: 'log-in',
    loadComponent: () => import("./views/log-in/log-in.component").then(m => m.LogInComponent)
  }
];