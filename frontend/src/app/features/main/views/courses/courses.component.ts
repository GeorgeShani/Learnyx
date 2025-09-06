import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

interface Category {
  id: string;
  name: string;
  count: number;
}

interface Course {
  id: number;
  title: string;
  instructor: string;
  rating: number;
  reviewCount: number;
  students: number;
  price: number;
  originalPrice: number;
  duration: string;
  lessons: number;
  level: string;
  category: string;
  tags: string[];
  image: string;
  bestseller: boolean;
  updated: string;
}

@Component({
  selector: 'app-courses',
  imports: [CommonModule, FormsModule],
  templateUrl: './courses.component.html',
  styleUrl: './courses.component.scss',
})
export class CoursesComponent {
  searchQuery = '';
  selectedCategory = '';
  selectedLevel = '';
  selectedPrice = '';
  sortBy = 'popular';
  showFilters = false;
  selectedCategories: string[] = [];
  filteredCourses: Course[] = [];

  categories: Category[] = [
    { id: 'web-dev', name: 'Web Development', count: 245 },
    { id: 'design', name: 'Design', count: 189 },
    { id: 'business', name: 'Business', count: 156 },
    { id: 'marketing', name: 'Marketing', count: 134 },
    { id: 'photography', name: 'Photography', count: 98 },
    { id: 'music', name: 'Music', count: 87 },
    { id: 'health', name: 'Health & Fitness', count: 76 },
    { id: 'language', name: 'Languages', count: 65 },
  ];

  courses: Course[] = [
    {
      id: 1,
      title: 'Complete Web Development Bootcamp 2024',
      instructor: 'Sarah Johnson',
      rating: 4.9,
      reviewCount: 12543,
      students: 89234,
      price: 99,
      originalPrice: 149,
      duration: '40 hours',
      lessons: 156,
      level: 'Beginner',
      category: 'Web Development',
      tags: ['HTML', 'CSS', 'JavaScript', 'React'],
      image:
        'https://images.unsplash.com/photo-1461749280684-dccba630e2f6?w=400',
      bestseller: true,
      updated: '2024-01',
    },
    {
      id: 2,
      title: 'Advanced React & Node.js Development',
      instructor: 'Michael Chen',
      rating: 4.8,
      reviewCount: 8932,
      students: 45678,
      price: 129,
      originalPrice: 199,
      duration: '35 hours',
      lessons: 128,
      level: 'Advanced',
      category: 'Web Development',
      tags: ['React', 'Node.js', 'MongoDB', 'Express'],
      image:
        'https://images.unsplash.com/photo-1633356122102-3fe601e05bd2?w=400',
      bestseller: false,
      updated: '2024-02',
    },
    {
      id: 3,
      title: 'Data Science with Python Complete Course',
      instructor: 'Dr. Emily Rodriguez',
      rating: 4.9,
      reviewCount: 15678,
      students: 67890,
      price: 149,
      originalPrice: 229,
      duration: '50 hours',
      lessons: 189,
      level: 'Intermediate',
      category: 'Data Science',
      tags: ['Python', 'Pandas', 'NumPy', 'Matplotlib'],
      image: 'https://images.unsplash.com/photo-1551288049-bebda4e38f71?w=400',
      bestseller: true,
      updated: '2024-01',
    },
    {
      id: 4,
      title: 'UI/UX Design Masterclass',
      instructor: 'Alex Thompson',
      rating: 4.7,
      reviewCount: 9876,
      students: 34567,
      price: 89,
      originalPrice: 139,
      duration: '28 hours',
      lessons: 98,
      level: 'Beginner',
      category: 'Design',
      tags: ['Figma', 'Adobe XD', 'Prototyping', 'User Research'],
      image: 'https://images.unsplash.com/photo-1561070791-2526d30994b5?w=400',
      bestseller: false,
      updated: '2024-02',
    },
    {
      id: 5,
      title: 'Digital Marketing Strategy 2024',
      instructor: 'Lisa Chang',
      rating: 4.6,
      reviewCount: 7654,
      students: 23456,
      price: 79,
      originalPrice: 129,
      duration: '22 hours',
      lessons: 87,
      level: 'Intermediate',
      category: 'Marketing',
      tags: ['SEO', 'Social Media', 'Content Marketing', 'Analytics'],
      image:
        'https://images.unsplash.com/photo-1460925895917-afdab827c52f?w=400',
      bestseller: false,
      updated: '2024-03',
    },
    {
      id: 6,
      title: 'Photography Fundamentals & Advanced Techniques',
      instructor: 'James Wilson',
      rating: 4.8,
      reviewCount: 5432,
      students: 18765,
      price: 99,
      originalPrice: 159,
      duration: '32 hours',
      lessons: 124,
      level: 'All Levels',
      category: 'Photography',
      tags: ['DSLR', 'Lightroom', 'Composition', 'Portrait'],
      image:
        'https://images.unsplash.com/photo-1502920917128-1aa500764cbd?w=400',
      bestseller: true,
      updated: '2024-01',
    },
  ];

  constructor(private router: Router) {}

  ngOnInit(): void {
    this.filterCourses();
  }

  onSearchChange(): void {
    this.filterCourses();
  }

  onCategoryChange(categoryName: string, event: any): void {
    if (event.target.checked) {
      this.selectedCategories.push(categoryName);
    } else {
      this.selectedCategories = this.selectedCategories.filter(
        (c) => c !== categoryName
      );
    }
    this.filterCourses();
  }

  onLevelChange(): void {
    this.filterCourses();
  }

  onPriceChange(): void {
    this.filterCourses();
  }

  onSortChange(): void {
    this.sortCourses();
  }

  toggleFilters(): void {
    this.showFilters = !this.showFilters;
  }

  clearAllFilters(): void {
    this.searchQuery = '';
    this.selectedCategory = '';
    this.selectedLevel = '';
    this.selectedPrice = '';
    this.filterCourses();
  }

  filterCourses(): void {
    this.filteredCourses = this.courses.filter((course) => {
      const matchesSearch =
        course.title.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
        course.instructor
          .toLowerCase()
          .includes(this.searchQuery.toLowerCase()) ||
        course.tags.some((tag) =>
          tag.toLowerCase().includes(this.searchQuery.toLowerCase())
        );

      const matchesCategory =
        this.selectedCategories.length === 0 ||
        this.selectedCategories.includes(course.category);

      const matchesLevel =
        !this.selectedLevel || course.level === this.selectedLevel;
      const matchesPrice =
        !this.selectedPrice ||
        (this.selectedPrice === 'free' && course.price === 0) ||
        (this.selectedPrice === 'paid' && course.price > 0) ||
        (this.selectedPrice === 'under-50' && course.price < 50) ||
        (this.selectedPrice === '50-100' &&
          course.price >= 50 &&
          course.price <= 100) ||
        (this.selectedPrice === 'over-100' && course.price > 100);

      return matchesSearch && matchesCategory && matchesLevel && matchesPrice;
    });

    this.sortCourses();
  }

  sortCourses(): void {
    switch (this.sortBy) {
      case 'newest':
        this.filteredCourses.sort((a, b) => b.updated.localeCompare(a.updated));
        break;
      case 'rating':
        this.filteredCourses.sort((a, b) => b.rating - a.rating);
        break;
      case 'price-low':
        this.filteredCourses.sort((a, b) => a.price - b.price);
        break;
      case 'price-high':
        this.filteredCourses.sort((a, b) => b.price - a.price);
        break;
      case 'popular':
      default:
        this.filteredCourses.sort((a, b) => b.students - a.students);
        break;
    }
  }

  viewCourseDetails(courseId: number): void {
    this.router.navigate(['/courses', courseId]);
  }

  loadMoreCourses(): void {
    // Implementation for loading more courses
    console.log('Loading more courses...');
  }
}
