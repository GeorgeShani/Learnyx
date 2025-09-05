import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./views/home/home.component').then((m) => m.HomeComponent),
  },
  {
    path: 'courses',
    loadComponent: () =>
      import('./views/courses/courses.component').then(
        (m) => m.CoursesComponent
      ),
  },
  {
    path: 'teachers',
    loadComponent: () =>
      import('./views/teachers/teachers.component').then(
        (m) => m.TeachersComponent
      ),
  },
  {
    path: 'about',
    loadComponent: () =>
      import('./views/about/about.component').then(
        (m) => m.AboutComponent
      ),
  },
  {
    path: '**',
    loadComponent: () => import('./views/not-found/not-found.component').then(
      (m) => m.NotFoundComponent
    )
  },
];