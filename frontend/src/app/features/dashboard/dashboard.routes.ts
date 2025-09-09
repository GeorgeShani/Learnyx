import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'teacher/create-course',
    loadComponent: () =>
      import('./views/course-builder/course-builder.component').then(
        (m) => m.CourseBuilderComponent
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
