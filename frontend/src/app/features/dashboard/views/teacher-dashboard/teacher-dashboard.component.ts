import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { Router } from '@angular/router';

interface Teacher {
  name: string;
  avatar: string;
  title: string;
  rating: number;
  totalStudents: number;
  totalCourses: number;
  totalRevenue: number;
  monthlyRevenue: number;
  joinDate: string;
}

interface Course {
  id: number;
  title: string;
  status: string;
  students: number;
  rating: number;
  reviews: number;
  revenue: number;
  lastUpdated: string;
  image: string;
  lessons: number;
  duration: string;
}

interface Analytics {
  totalViews: number;
  courseCompletions: number;
  averageRating: number;
  responseRate: number;
  weeklyStats: Array<{
    day: string;
    students: number;
    revenue: number;
  }>;
}

interface Activity {
  type: string;
  message: string;
  time: string;
  icon: SafeHtml;
}

interface Task {
  task: string;
  priority: string;
  dueDate: string;
}

interface Tab {
  value: string;
  label: string;
}

@Component({
  selector: 'app-teacher-dashboard',
  imports: [CommonModule],
  templateUrl: './teacher-dashboard.component.html',
  styleUrl: './teacher-dashboard.component.scss',
})
export class TeacherDashboardComponent {
  activeTab = 'overview';

  tabs: Tab[] = [
    { value: 'overview', label: 'Overview' },
    { value: 'courses', label: 'My Courses' },
    { value: 'analytics', label: 'Analytics' },
    { value: 'students', label: 'Students' },
  ];

  teacher: Teacher = {
    name: 'Sarah Johnson',
    avatar: '',
    title: 'Senior Full-Stack Developer',
    rating: 4.9,
    totalStudents: 89234,
    totalCourses: 12,
    totalRevenue: 125000,
    monthlyRevenue: 12500,
    joinDate: 'January 2022',
  };

  myCourses: Course[] = [
    {
      id: 1,
      title: 'Complete Web Development Bootcamp',
      status: 'published',
      students: 89234,
      rating: 4.9,
      reviews: 12543,
      revenue: 89340,
      lastUpdated: '2 weeks ago',
      image:
        'https://images.unsplash.com/photo-1461749280684-dccba630e2f6?w=300',
      lessons: 156,
      duration: '40 hours',
    },
    {
      id: 2,
      title: 'Advanced JavaScript Concepts',
      status: 'published',
      students: 45678,
      rating: 4.8,
      reviews: 8932,
      revenue: 45680,
      lastUpdated: '1 week ago',
      image:
        'https://images.unsplash.com/photo-1579468118864-1b9ea3c0db4a?w=300',
      lessons: 89,
      duration: '28 hours',
    },
    {
      id: 3,
      title: 'React Performance Optimization',
      status: 'draft',
      students: 0,
      rating: 0,
      reviews: 0,
      revenue: 0,
      lastUpdated: '3 days ago',
      image:
        'https://images.unsplash.com/photo-1633356122102-3fe601e05bd2?w=300',
      lessons: 45,
      duration: '15 hours',
    },
  ];

  analytics: Analytics = {
    totalViews: 234567,
    courseCompletions: 12543,
    averageRating: 4.85,
    responseRate: 98,
    weeklyStats: [
      { day: 'Mon', students: 120, revenue: 450 },
      { day: 'Tue', students: 98, revenue: 380 },
      { day: 'Wed', students: 145, revenue: 520 },
      { day: 'Thu', students: 187, revenue: 690 },
      { day: 'Fri', students: 210, revenue: 780 },
      { day: 'Sat', students: 156, revenue: 590 },
      { day: 'Sun', students: 134, revenue: 500 },
    ],
  };

  recentActivity: Activity[] = [];

  pendingTasks: Task[] = [
    {
      task: 'Respond to 8 student questions',
      priority: 'high',
      dueDate: 'Today',
    },
    {
      task: 'Update course materials for JavaScript course',
      priority: 'medium',
      dueDate: 'This week',
    },
    {
      task: 'Review and publish React course',
      priority: 'high',
      dueDate: 'In 3 days',
    },
    {
      task: 'Plan next course content',
      priority: 'low',
      dueDate: 'Next week',
    },
  ];

  constructor(private router: Router, private sanitizer: DomSanitizer) {
    this.recentActivity = [
      {
        type: 'enrollment',
        message: '25 new students enrolled in Web Development Bootcamp',
        time: '2 hours ago',
        icon: this.sanitizer.bypassSecurityTrustHtml(
          '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="#22C55E" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-users-icon lucide-users"><path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2"/><path d="M16 3.128a4 4 0 0 1 0 7.744"/><path d="M22 21v-2a4 4 0 0 0-3-3.87"/><circle cx="9" cy="7" r="4"/></svg>'
        ),
      },
      {
        type: 'review',
        message: 'New 5-star review on Advanced JavaScript',
        time: '4 hours ago',
        icon: this.sanitizer.bypassSecurityTrustHtml(
          '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="#eab308" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-star-icon lucide-star"><path d="M11.525 2.295a.53.53 0 0 1 .95 0l2.31 4.679a2.123 2.123 0 0 0 1.595 1.16l5.166.756a.53.53 0 0 1 .294.904l-3.736 3.638a2.123 2.123 0 0 0-.611 1.878l.882 5.14a.53.53 0 0 1-.771.56l-4.618-2.428a2.122 2.122 0 0 0-1.973 0L6.396 21.01a.53.53 0 0 1-.77-.56l.881-5.139a2.122 2.122 0 0 0-.611-1.879L2.16 9.795a.53.53 0 0 1 .294-.906l5.165-.755a2.122 2.122 0 0 0 1.597-1.16z"/></svg>'
        ),
      },
      {
        type: 'question',
        message: '3 new Q&A questions need responses',
        time: '6 hours ago',
        icon: this.sanitizer.bypassSecurityTrustHtml(
          '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="#3b82f6" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-message-circle-icon lucide-message-circle"><path d="M2.992 16.342a2 2 0 0 1 .094 1.167l-1.065 3.29a1 1 0 0 0 1.236 1.168l3.413-.998a2 2 0 0 1 1.099.092 10 10 0 1 0-4.777-4.719"/></svg>'
        ),
      },
      {
        type: 'completion',
        message: '142 students completed React course this week',
        time: '1 day ago',
        icon: this.sanitizer.bypassSecurityTrustHtml(
          '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="#a855f7" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-circle-check-big-icon lucide-circle-check-big"><path d="M21.801 10A10 10 0 1 1 17 3.335"/><path d="m9 11 3 3L22 4"/></svg>'
        ),
      },
    ];
  }

  setActiveTab(tab: string): void {
    this.activeTab = tab;
  }

  getInitials(name: string): string {
    return name
      .split(' ')
      .map((n) => n[0])
      .join('');
  }

  getTopCourses(): Course[] {
    return this.myCourses.slice(0, 2);
  }

  getBarHeight(students: number): number {
    return (students / 250) * 100;
  }

  navigateToCreateCourse(): void {
    this.router.navigate(['/dashboard/teacher/create-course']);
  }

  editCourse(courseId: number): void {
    this.router.navigate(['/dashboard/teacher/courses', courseId]);
  }

  viewCourse(courseId: number): void {
    this.router.navigate(['/courses', courseId]);
  }
}
