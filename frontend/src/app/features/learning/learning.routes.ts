import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'chat',
    loadComponent: () =>
      import('./views/messaging/messaging.component').then(
        (m) => m.MessagingComponent
      ),
  },
  {
    path: ':courseId',
    loadComponent: () =>
      import('./views/learn/learn.component').then((m) => m.LearnComponent),
  },
  {
    path: ':courseId/:lessonId',
    loadComponent: () =>
      import('./views/learn/learn.component').then((m) => m.LearnComponent),
  },
];
