import { Routes } from '@angular/router';
import { roleGuard } from '@core/guards/role.guard';

export const routes: Routes = [
  {
    path: 'chat',
    canActivate: [roleGuard],
    data: { roles: ['Student', 'Teacher'] },
    loadComponent: () =>
      import('./views/messaging/messaging.component').then(
        (m) => m.MessagingComponent
      ),
  },
  {
    path: ':courseId',
    canActivate: [roleGuard],
    data: { roles: ['Student', 'Teacher'] },
    loadComponent: () =>
      import('./views/learn/learn.component').then((m) => m.LearnComponent),
  },
  {
    path: ':courseId/:lessonId',
    canActivate: [roleGuard],
    data: { roles: ['Student', 'Teacher'] },
    loadComponent: () =>
      import('./views/learn/learn.component').then((m) => m.LearnComponent),
  },
];
