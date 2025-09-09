import { Component, HostListener } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

interface Stat {
  label: string;
  value: string;
  change: string;
  iconSvg: SafeHtml;
  trend: string;
}

interface User {
  id: number;
  name: string;
  email: string;
  role: string;
  status: string;
  joinDate: string;
  courses: number;
}

interface Course {
  id: number;
  title: string;
  instructor: string;
  category: string;
  submittedAt: string;
  status: string;
}

interface Alert {
  id: number;
  type: string;
  message: string;
  time: string;
}

@Component({
  selector: 'app-admin-dashboard',
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.scss',
})
export class AdminDashboardComponent {
  activeTab = 'overview';
  searchQuery = '';
  openDropdown: number | null = null;

  stats: Stat[] = [];

  constructor(private sanitizer: DomSanitizer) {
    this.stats = [
      {
        label: 'Total Users',
        value: '12,543',
        change: '12',
        iconSvg: this.sanitizer.bypassSecurityTrustHtml(
          '<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="#ffffff" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-users-icon lucide-users"><path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2"/><path d="M16 3.128a4 4 0 0 1 0 7.744"/><path d="M22 21v-2a4 4 0 0 0-3-3.87"/><circle cx="9" cy="7" r="4"/></svg>'
        ),
        trend: 'up',
      },
      {
        label: 'Active Courses',
        value: '1,247',
        change: '8',
        iconSvg: this.sanitizer.bypassSecurityTrustHtml(
          '<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="#ffffff" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-book-open-icon lucide-book-open"><path d="M12 7v14"/><path d="M3 18a1 1 0 0 1-1-1V4a1 1 0 0 1 1-1h5a4 4 0 0 1 4 4 4 4 0 0 1 4-4h5a1 1 0 0 1 1 1v13a1 1 0 0 1-1 1h-6a3 3 0 0 0-3 3 3 3 0 0 0-3-3z"/></svg>'
        ),
        trend: 'up',
      },
      {
        label: 'Monthly Revenue',
        value: '$45,678',
        change: '15',
        iconSvg: this.sanitizer.bypassSecurityTrustHtml(
          '<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="#ffffff" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-dollar-sign-icon lucide-dollar-sign"><line x1="12" x2="12" y1="2" y2="22"/><path d="M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"/></svg>'
        ),
        trend: 'up',
      },
      {
        label: 'Course Completions',
        value: '3,421',
        change: '23',
        iconSvg: this.sanitizer.bypassSecurityTrustHtml(
          '<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="#ffffff" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-trending-up-icon lucide-trending-up"><path d="M16 7h6v6"/><path d="m22 7-8.5 8.5-5-5L2 17"/></svg>'
        ),
        trend: 'up',
      },
    ];
  }

  pendingCourses: Course[] = [
    {
      id: 1,
      title: 'Advanced Machine Learning',
      instructor: 'Dr. Sarah Chen',
      category: 'Technology',
      submittedAt: '2024-01-15',
      status: 'pending',
    },
    {
      id: 2,
      title: 'Digital Marketing Strategy',
      instructor: 'Mike Johnson',
      category: 'Business',
      submittedAt: '2024-01-14',
      status: 'review',
    },
    {
      id: 3,
      title: 'Creative Writing Workshop',
      instructor: 'Emma Wilson',
      category: 'Arts',
      submittedAt: '2024-01-13',
      status: 'pending',
    },
  ];

  // Original user data (unfiltered)
  private allUsers: User[] = [
    {
      id: 1,
      name: 'Alice Johnson',
      email: 'alice@example.com',
      role: 'Student',
      status: 'Active',
      joinDate: '2024-01-15',
      courses: 3,
    },
    {
      id: 2,
      name: 'Bob Smith',
      email: 'bob@example.com',
      role: 'Teacher',
      status: 'Active',
      joinDate: '2024-01-14',
      courses: 2,
    },
    {
      id: 3,
      name: 'Carol Brown',
      email: 'carol@example.com',
      role: 'Student',
      status: 'Inactive',
      joinDate: '2024-01-13',
      courses: 1,
    },
    {
      id: 4,
      name: 'David Wilson',
      email: 'david@example.com',
      role: 'Teacher',
      status: 'Active',
      joinDate: '2024-01-12',
      courses: 5,
    },
    {
      id: 5,
      name: 'Eva Martinez',
      email: 'eva@example.com',
      role: 'Student',
      status: 'Active',
      joinDate: '2024-01-11',
      courses: 2,
    },
    {
      id: 6,
      name: 'Frank Thompson',
      email: 'frank@example.com',
      role: 'Admin',
      status: 'Active',
      joinDate: '2024-01-10',
      courses: 0,
    },
    {
      id: 7,
      name: 'Grace Lee',
      email: 'grace@example.com',
      role: 'Student',
      status: 'Suspended',
      joinDate: '2024-01-09',
      courses: 1,
    },
    {
      id: 8,
      name: 'Henry Davis',
      email: 'henry@example.com',
      role: 'Teacher',
      status: 'Active',
      joinDate: '2024-01-08',
      courses: 3,
    },
  ];

  // Filtered user data (displayed in UI)
  recentUsers: User[] = [...this.allUsers];

  systemAlerts: Alert[] = [
    {
      id: 1,
      type: 'warning',
      message: 'High server load detected',
      time: '5 minutes ago',
    },
    {
      id: 2,
      type: 'info',
      message: 'Weekly backup completed successfully',
      time: '2 hours ago',
    },
    {
      id: 3,
      type: 'error',
      message: 'Payment gateway error reported',
      time: '4 hours ago',
    },
  ];

  setActiveTab(tab: string): void {
    this.activeTab = tab;
  }

  toggleDropdown(userId: number): void {
    this.openDropdown = this.openDropdown === userId ? null : userId;
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: Event): void {
    const target = event.target as HTMLElement;
    if (!target.closest('.dropdown')) {
      this.openDropdown = null;
    }
  }

  // User search functionality
  onSearchInput(): void {
    const query = this.searchQuery.trim().toLowerCase();

    if (query.length === 0) {
      this.clearSearch();
      return;
    }

    this.searchUsers(query);
  }

  private searchUsers(query: string): void {
    this.recentUsers = this.allUsers.filter(
      (user) =>
        user.name.toLowerCase().includes(query) ||
        user.email.toLowerCase().includes(query) ||
        user.role.toLowerCase().includes(query) ||
        user.status.toLowerCase().includes(query)
    );
  }

  clearSearch(): void {
    this.searchQuery = '';
    this.recentUsers = [...this.allUsers];
  }

  // Get search results count
  getSearchResultsCount(): number {
    return this.recentUsers.length;
  }

  // Check if search is active
  isSearchActive(): boolean {
    return this.searchQuery.trim().length > 0;
  }

  exportData(): void {
    console.log('Exporting data...');
    // Implement export functionality
  }

  addUser(): void {
    console.log('Adding new user...');
    // Implement add user functionality
  }

  viewProfile(userId: number): void {
    console.log('Viewing profile for user:', userId);
    // Implement view profile functionality
  }

  editUser(userId: number): void {
    console.log('Editing user:', userId);
    // Implement edit user functionality
  }

  suspendUser(userId: number): void {
    console.log('Suspending user:', userId);
    // Implement suspend user functionality
  }

  viewCourse(courseId: number): void {
    console.log('Viewing course:', courseId);
    // Implement view course functionality
  }

  approveCourse(courseId: number): void {
    console.log('Approving course:', courseId);
    // Implement approve course functionality
  }

  rejectCourse(courseId: number): void {
    console.log('Rejecting course:', courseId);
    // Implement reject course functionality
  }
}
