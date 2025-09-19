import { Routes } from '@angular/router';
import { roleGuard } from '@core/guards/role.guard';

export const routes: Routes = [
  {
    path: 'teacher/create-course',
    canActivate: [roleGuard],
    data: { roles: ['Teacher'] },
    loadComponent: () =>
      import('./views/course-builder/course-builder.component').then(
        (m) => m.CourseBuilderComponent
      ),
  },
  {
    path: 'student',
    canActivate: [roleGuard],
    data: { roles: ['Student'] },
    loadComponent: () =>
      import('./views/student-dashboard/student-dashboard.component').then(
        (m) => m.StudentDashboardComponent
      ),
  },
  {
    path: 'teacher',
    canActivate: [roleGuard],
    data: { roles: ['Teacher'] },
    loadComponent: () =>
      import('./views/teacher-dashboard/teacher-dashboard.component').then(
        (m) => m.TeacherDashboardComponent
      ),
  },
  {
    path: 'admin',
    canActivate: [roleGuard],
    data: { roles: ['Admin'] },
    loadComponent: () =>
      import('./views/admin-dashboard/admin-dashboard.component').then(
        (m) => m.AdminDashboardComponent
      ),
  },
];
