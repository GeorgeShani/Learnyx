import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: ':courseId/:lessonId',
    loadComponent: () =>
      import('./views/learn/learn.component').then((m) => m.LearnComponent),
  },
  {
    path: 'chat',
    loadComponent: () =>
      import('./views/messaging/messaging.component').then(
        (m) => m.MessagingComponent
      ),
  },
];
