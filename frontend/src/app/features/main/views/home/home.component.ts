import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router } from '@angular/router';

interface Stat {
  label: string;
  value: string;
  iconPath: string;
}

interface Feature {
  title: string;
  description: string;
  iconPath: string;
}

interface Course {
  id: number;
  title: string;
  instructor: string;
  rating: number;
  students: number;
  price: number;
  duration: string;
  level: string;
  image: string;
}

@Component({
  selector: 'app-home',
  imports: [CommonModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class HomeComponent {
  stats: Stat[] = [
    {
      label: 'Active Students',
      value: '50K+',
      iconPath: 'assets/icons/users.svg',
    },
    {
      label: 'Expert Instructors',
      value: '1,200+',
      iconPath: 'assets/icons/award.svg',
    },
    {
      label: 'Courses Available',
      value: '5,000+',
      iconPath: 'assets/icons/book-open.svg',
    },
    {
      label: 'Success Rate',
      value: '95%',
      iconPath: 'assets/icons/trending-up.svg',
    },
  ];

  features: Feature[] = [
    {
      title: 'Interactive Learning',
      description: 'Engage with hands-on projects, quizzes, and real-world applications that make learning stick.',
      iconPath: 'assets/icons/zap.svg',
    },
    {
      title: 'Learn Anywhere',
      description: 'Access your courses from any device, anywhere in the world. Learn at your own pace.',
      iconPath: 'assets/icons/globe.svg',
    },
    {
      title: 'Certified Instructors',
      description: 'Learn from industry experts and certified professionals with proven track records.',
      iconPath: 'assets/icons/shield.svg',
    },
    {
      title: 'Personalized Path',
      description: 'AI-powered recommendations help you discover courses tailored to your goals and interests.',
      iconPath: 'assets/icons/sparkles.svg',
    },
  ];

  popularCourses: Course[] = [
    {
      id: 1,
      title: 'Complete Web Development Bootcamp',
      instructor: 'Sarah Johnson',
      rating: 4.9,
      students: 12543,
      price: 99,
      duration: '40 hours',
      level: 'Beginner',
      image:
        'https://images.unsplash.com/photo-1461749280684-dccba630e2f6?w=400',
    },
    {
      id: 2,
      title: 'Advanced React & Node.js',
      instructor: 'Michael Chen',
      rating: 4.8,
      students: 8932,
      price: 129,
      duration: '35 hours',
      level: 'Advanced',
      image:
        'https://images.unsplash.com/photo-1633356122102-3fe601e05bd2?w=400',
    },
    {
      id: 3,
      title: 'Data Science with Python',
      instructor: 'Dr. Emily Rodriguez',
      rating: 4.9,
      students: 15678,
      price: 149,
      duration: '50 hours',
      level: 'Intermediate',
      image: 'https://images.unsplash.com/photo-1551288049-bebda4e38f71?w=400',
    },
  ];

  constructor(private router: Router) {}

  navigateTo(route: string): void {
    this.router.navigate([route]);
  }
}
