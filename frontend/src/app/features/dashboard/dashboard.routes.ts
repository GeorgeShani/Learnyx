import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'create-course',
    loadComponent: () =>
      import('./views/create-course/create-course.component').then(
        (m) => m.CreateCourseComponent
      ),
  },
  {
    path: 'student',
    loadComponent: () =>
      import('./views/student-dashboard/student-dashboard.component').then(
        (m) => m.StudentDashboardComponent
      ),
  },
  {
    path: 'teacher',
    loadComponent: () =>
      import('./views/teacher-dashboard/teacher-dashboard.component').then(
        (m) => m.TeacherDashboardComponent
      ),
  },
  {
    path: 'admin',
    loadComponent: () =>
      import('./views/admin-dashboard/admin-dashboard.component').then(
        (m) => m.AdminDashboardComponent
      ),
  },
];
