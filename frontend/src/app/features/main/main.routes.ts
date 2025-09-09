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
    path: 'courses/:id',
    loadComponent: () =>
      import('./views/course-details/course-details.component').then(
        (m) => m.CourseDetailsComponent
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
      import('./views/about/about.component').then((m) => m.AboutComponent),
  },
  {
    path: 'contact',
    loadComponent: () =>
      import('./views/contact/contact.component').then(
        (m) => m.ContactComponent
      ),
  },
  {
    path: 'help',
    loadComponent: () =>
      import('./views/support/support.component').then(
        (m) => m.SupportComponent
      ),
  },
  {
    path: 'terms',
    loadComponent: () =>
      import('./views/terms/terms.component').then((m) => m.TermsComponent),
  },
  {
    path: 'privacy',
    loadComponent: () =>
      import('./views/privacy/privacy.component').then(
        (m) => m.PrivacyComponent
      ),
  },
  {
    path: 'careers',
    loadComponent: () =>
      import('./views/careers/careers.component').then(
        (m) => m.CareersComponent
      ),
  },
];
