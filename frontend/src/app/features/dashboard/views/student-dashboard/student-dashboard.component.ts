import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { FilterModalComponent } from '@features/dashboard/components/filter-modal/filter-modal.component';

interface User {
  name: string;
  avatar: string;
  level: string;
  joinDate: string;
  totalCourses: number;
  completedCourses: number;
  totalHours: number;
  currentStreak: number;
  points: number;
  badges: Badge[];
}

interface Badge {
  name: string;
  icon: string;
  color: string;
}

interface Course {
  id: number;
  title: string;
  instructor: string;
  progress: number;
  totalLessons: number;
  completedLessons: number;
  lastAccessed: string;
  timeRemaining: string;
  image: string;
  category: string;
  nextLesson: string;
  rating?: number;
  dateEnrolled?: string;
  dateCompleted?: string;
}

interface CourseFilters {
  status: 'all' | 'in-progress' | 'completed';
  category: string;
  instructor: string;
  rating: number;
  progress: {
    min: number;
    max: number;
  };
  sortBy: 'title' | 'progress' | 'rating' | 'date-enrolled' | 'date-completed';
  sortOrder: 'asc' | 'desc';
}

interface CompletedCourse {
  id: number;
  title: string;
  instructor: string;
  completedDate: string;
  rating: number;
  certificate: boolean;
  image: string;
  category: string;
}

interface Deadline {
  course: string;
  task: string;
  dueDate: string;
  priority: 'high' | 'medium' | 'low';
}

interface Achievement {
  title: string;
  description: string;
  date: string;
  icon: string;
  color: string;
}

@Component({
  selector: 'app-student-dashboard',
  imports: [CommonModule, RouterModule, FilterModalComponent],
  templateUrl: './student-dashboard.component.html',
  styleUrl: './student-dashboard.component.scss',
})
export class StudentDashboardComponent {
  activeTab = 'overview';

  user: User = {
    name: 'Alex Thompson',
    avatar:
      'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=150',
    level: 'Intermediate Developer',
    joinDate: 'March 2023',
    totalCourses: 8,
    completedCourses: 5,
    totalHours: 156,
    currentStreak: 12,
    points: 2450,
    badges: [
      { name: 'First Course', icon: 'BookOpen', color: 'bg-blue' },
      { name: 'Fast Learner', icon: 'Zap', color: 'bg-yellow' },
      { name: 'Consistent', icon: 'Target', color: 'bg-green' },
      { name: 'JavaScript Master', icon: 'Award', color: 'bg-purple' },
    ],
  };

  // Original course data (unfiltered)
  private allEnrolledCourses: Course[] = [
    {
      id: 1,
      title: 'Complete Web Development Bootcamp',
      instructor: 'Sarah Johnson',
      progress: 75,
      totalLessons: 156,
      completedLessons: 117,
      lastAccessed: '2 hours ago',
      timeRemaining: '10 hours',
      image:
        'https://images.unsplash.com/photo-1461749280684-dccba630e2f6?w=300',
      category: 'Web Development',
      nextLesson: 'Building REST APIs with Express',
      rating: 4.8,
      dateEnrolled: '2024-01-15',
    },
    {
      id: 2,
      title: 'Advanced React Development',
      instructor: 'Michael Chen',
      progress: 45,
      totalLessons: 128,
      completedLessons: 58,
      lastAccessed: '1 day ago',
      timeRemaining: '18 hours',
      image:
        'https://images.unsplash.com/photo-1633356122102-3fe601e05bd2?w=300',
      category: 'React',
      nextLesson: 'State Management with Redux',
      rating: 4.6,
      dateEnrolled: '2024-02-20',
    },
    {
      id: 3,
      title: 'Data Science with Python',
      instructor: 'Dr. Emily Rodriguez',
      progress: 90,
      totalLessons: 189,
      completedLessons: 170,
      lastAccessed: '3 days ago',
      timeRemaining: '4 hours',
      image: 'https://images.unsplash.com/photo-1551288049-bebda4e38f71?w=300',
      category: 'Data Science',
      nextLesson: 'Machine Learning Models',
      rating: 4.9,
      dateEnrolled: '2023-12-10',
    },
  ];

  private allCompletedCourses: CompletedCourse[] = [
    {
      id: 4,
      title: 'JavaScript Fundamentals',
      instructor: 'John Smith',
      completedDate: '2024-08-15',
      rating: 5,
      certificate: true,
      image:
        'https://images.unsplash.com/photo-1579468118864-1b9ea3c0db4a?w=300',
      category: 'JavaScript',
    },
    {
      id: 5,
      title: 'HTML & CSS Masterclass',
      instructor: 'Lisa Wang',
      completedDate: '2024-07-20',
      rating: 4,
      certificate: true,
      image: 'https://images.unsplash.com/photo-1558618047-f9c8ebf46bc8?w=300',
      category: 'Web Design',
    },
  ];

  // Filtered course data (what's displayed)
  enrolledCourses: Course[] = [...this.allEnrolledCourses];
  completedCourses: CompletedCourse[] = [...this.allCompletedCourses];

  upcomingDeadlines: Deadline[] = [
    {
      course: 'Complete Web Development Bootcamp',
      task: 'Final Project Submission',
      dueDate: 'In 3 days',
      priority: 'high',
    },
    {
      course: 'Advanced React Development',
      task: 'Quiz: Hooks and Context',
      dueDate: 'In 1 week',
      priority: 'medium',
    },
    {
      course: 'Data Science with Python',
      task: 'Assignment: Data Visualization',
      dueDate: 'In 2 weeks',
      priority: 'low',
    },
  ];

  achievements: Achievement[] = [
    {
      title: '100 Hours Learned',
      description: "You've completed over 100 hours of learning!",
      date: '1 week ago',
      icon: 'Clock',
      color: 'text-blue-500',
    },
    {
      title: 'Course Completed',
      description: 'Finished JavaScript Fundamentals',
      date: '2 weeks ago',
      icon: 'Trophy',
      color: 'text-yellow-500',
    },
    {
      title: 'Perfect Score',
      description: 'Got 100% on React Quiz',
      date: '3 weeks ago',
      icon: 'Star',
      color: 'text-purple-500',
    },
  ];

  showFilterModal = false;

  currentFilters: CourseFilters = {
    status: 'all',
    category: '',
    instructor: '',
    rating: 0,
    progress: { min: 0, max: 100 },
    sortBy: 'title',
    sortOrder: 'asc',
  };

  availableInstructors = [
    'Sarah Johnson',
    'Michael Chen',
    'Dr. Emily Rodriguez',
    'John Smith',
    'Lisa Wang',
  ];

  availableCategories = [
    'Web Development',
    'React',
    'Data Science',
    'JavaScript',
    'Web Design',
  ];

  constructor(private router: Router) {}

  setActiveTab(tab: string): void {
    this.activeTab = tab;
  }

  getInitials(name: string): string {
    return name
      .split(' ')
      .map((n) => n[0])
      .join('');
  }

  getFirstName(name: string): string {
    return name.split(' ')[0];
  }

  getOverallProgress(): number {
    return Math.round(
      (this.user.completedCourses / this.user.totalCourses) * 100
    );
  }

  getBadgeColorClass(color: string): string {
    return color;
  }

  openFilterModal(): void {
    this.showFilterModal = true;
  }

  onFiltersChanged(filters: CourseFilters): void {
    this.currentFilters = filters;
    this.applyFilters(filters);
  }

  onModalClosed(): void {
    this.showFilterModal = false;
  }

  private applyFilters(filters: CourseFilters): void {
    let filteredEnrolled: Course[] = [];
    let filteredCompleted: CompletedCourse[] = [];

    // Determine which courses to include based on status filter
    if (filters.status === 'all' || filters.status === 'in-progress') {
      filteredEnrolled = [...this.allEnrolledCourses];
    }

    if (filters.status === 'all' || filters.status === 'completed') {
      filteredCompleted = [...this.allCompletedCourses];
    }

    // Apply filters to enrolled courses
    if (filteredEnrolled.length > 0) {
      filteredEnrolled = filteredEnrolled.filter((course) => {
        // Category filter
        if (filters.category && course.category !== filters.category) {
          return false;
        }

        // Instructor filter
        if (filters.instructor && course.instructor !== filters.instructor) {
          return false;
        }

        // Rating filter
        if (
          filters.rating > 0 &&
          (!course.rating || course.rating < filters.rating)
        ) {
          return false;
        }

        // Progress filter
        if (
          course.progress < filters.progress.min ||
          course.progress > filters.progress.max
        ) {
          return false;
        }

        return true;
      });

      // Sort enrolled courses
      filteredEnrolled = this.sortCourses(
        filteredEnrolled,
        filters.sortBy,
        filters.sortOrder
      );
    }

    // Apply filters to completed courses
    if (filteredCompleted.length > 0) {
      filteredCompleted = filteredCompleted.filter((course) => {
        // Category filter
        if (filters.category && course.category !== filters.category) {
          return false;
        }

        // Instructor filter
        if (filters.instructor && course.instructor !== filters.instructor) {
          return false;
        }

        // Rating filter
        if (filters.rating > 0 && course.rating < filters.rating) {
          return false;
        }

        return true;
      });

      // Sort completed courses
      filteredCompleted = this.sortCompletedCourses(
        filteredCompleted,
        filters.sortBy,
        filters.sortOrder
      );
    }

    // Update the displayed courses
    this.enrolledCourses = filteredEnrolled;
    this.completedCourses = filteredCompleted;
  }

  private sortCourses(
    courses: Course[],
    sortBy: string,
    sortOrder: 'asc' | 'desc'
  ): Course[] {
    return courses.sort((a, b) => {
      let comparison = 0;

      switch (sortBy) {
        case 'title':
          comparison = a.title.localeCompare(b.title);
          break;
        case 'progress':
          comparison = a.progress - b.progress;
          break;
        case 'rating':
          comparison = (a.rating || 0) - (b.rating || 0);
          break;
        case 'date-enrolled':
          const dateA = new Date(a.dateEnrolled || '').getTime();
          const dateB = new Date(b.dateEnrolled || '').getTime();
          comparison = dateA - dateB;
          break;
        default:
          comparison = a.title.localeCompare(b.title);
      }

      return sortOrder === 'desc' ? -comparison : comparison;
    });
  }

  private sortCompletedCourses(
    courses: CompletedCourse[],
    sortBy: string,
    sortOrder: 'asc' | 'desc'
  ): CompletedCourse[] {
    return courses.sort((a, b) => {
      let comparison = 0;

      switch (sortBy) {
        case 'title':
          comparison = a.title.localeCompare(b.title);
          break;
        case 'rating':
          comparison = a.rating - b.rating;
          break;
        case 'date-completed':
          const dateA = new Date(a.completedDate).getTime();
          const dateB = new Date(b.completedDate).getTime();
          comparison = dateA - dateB;
          break;
        default:
          comparison = a.title.localeCompare(b.title);
      }

      return sortOrder === 'desc' ? -comparison : comparison;
    });
  }

  // Method to reset filters
  resetFilters(): void {
    this.currentFilters = {
      status: 'all',
      category: '',
      instructor: '',
      rating: 0,
      progress: { min: 0, max: 100 },
      sortBy: 'title',
      sortOrder: 'asc',
    };
    this.applyFilters(this.currentFilters);
  }

  // Method to get filter summary for UI display
  getActiveFiltersCount(): number {
    let count = 0;
    if (this.currentFilters.status !== 'all') count++;
    if (this.currentFilters.category) count++;
    if (this.currentFilters.instructor) count++;
    if (this.currentFilters.rating > 0) count++;
    if (
      this.currentFilters.progress.min > 0 ||
      this.currentFilters.progress.max < 100
    )
      count++;
    return count;
  }

  navigateToCourse(courseId: number): void {
    this.router.navigate(['/learn', courseId]);
  }

  navigateToCourses(): void {
    this.router.navigate(['/courses']);
  }
}
