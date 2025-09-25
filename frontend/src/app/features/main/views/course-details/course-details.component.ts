import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';

interface Instructor {
  name: string;
  title: string;
  avatar: string;
  rating: number;
  students: number;
  courses: number;
  bio: string;
}

interface Lesson {
  title: string;
  duration: string;
  free: boolean;
}

interface CurriculumSection {
  section: string;
  lectures: number;
  duration: string;
  lessons: Lesson[];
}

interface Review {
  id: number;
  user: string;
  avatar: string;
  rating: number;
  date: string;
  content: string;
}

interface Course {
  id: number;
  title: string;
  subtitle: string;
  instructor: Instructor;
  rating: number;
  reviewCount: number;
  students: number;
  price: number;
  originalPrice: number;
  duration: string;
  lessons: number;
  level: string;
  language: string;
  lastUpdated: string;
  category: string;
  tags: string[];
  image: string;
  previewVideo: string;
  bestseller: boolean;
  certificate: boolean;
  downloadable: boolean;
  lifetime: boolean;
  description: string;
  whatYoullLearn: string[];
  requirements: string[];
  curriculum: CurriculumSection[];
  reviews: Review[];
}

interface RelatedCourse {
  id: number;
  title: string;
  image: string;
  rating: number;
  price: number;
}

@Component({
  selector: 'app-course-details',
  imports: [CommonModule, RouterModule],
  templateUrl: './course-details.component.html',
  styleUrl: './course-details.component.scss',
})
export class CourseDetailsComponent implements OnInit {
  courseId: string | null = null;
  isEnrolled = false;
  activeTab = 'overview';

  course: Course = {
    id: 1,
    title: 'Complete Web Development Bootcamp 2024',
    subtitle:
      'Master HTML, CSS, JavaScript, React, Node.js and become a full-stack developer',
    instructor: {
      name: 'John Smith',
      title: 'Senior Full-Stack Developer',
      avatar:
        'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=150&h=150&fit=crop&crop=face',
      rating: 4.9,
      students: 125000,
      courses: 12,
      bio: "John is a senior full-stack developer with 8+ years of experience at top tech companies. He's passionate about making web development accessible to everyone.",
    },
    rating: 4.9,
    reviewCount: 12543,
    students: 89234,
    price: 99,
    originalPrice: 149,
    duration: '40 hours',
    lessons: 156,
    level: 'Beginner',
    language: 'English',
    lastUpdated: 'January 2024',
    category: 'Web Development',
    tags: ['HTML', 'CSS', 'JavaScript', 'React', 'Node.js', 'MongoDB'],
    image: 'https://images.unsplash.com/photo-1461749280684-dccba630e2f6?w=800',
    previewVideo:
      'https://images.unsplash.com/photo-1516321318423-f06f85e504b3?w=800',
    bestseller: true,
    certificate: true,
    downloadable: true,
    lifetime: true,
    description: `Master web development from scratch and become a professional full-stack developer. This comprehensive course covers everything you need to know to build modern, responsive websites and web applications.

You'll start with the fundamentals of HTML, CSS, and JavaScript, then progress to advanced topics including React, Node.js, and database integration. By the end of this course, you'll have built multiple real-world projects and have the skills to work as a professional web developer.`,

    whatYoullLearn: [
      'Build responsive websites using HTML5, CSS3, and JavaScript',
      'Master React.js and create dynamic user interfaces',
      'Develop full-stack applications with Node.js and Express',
      'Work with databases using MongoDB and Mongoose',
      'Deploy applications to production environments',
      'Follow industry best practices and modern development workflows',
      'Build a portfolio of real-world projects',
      'Understand version control with Git and GitHub',
    ],

    requirements: [
      'No prior programming experience required',
      'A computer with internet connection',
      'Willingness to learn and practice coding',
      'Basic computer skills (file management, web browsing)',
    ],

    curriculum: [
      {
        section: 'Getting Started',
        lectures: 12,
        duration: '2 hours',
        lessons: [
          { title: 'Welcome to the Course', duration: '5:30', free: true },
          {
            title: 'Setting Up Your Development Environment',
            duration: '15:45',
            free: true,
          },
          {
            title: 'Introduction to Web Development',
            duration: '12:20',
            free: false,
          },
          { title: 'How the Internet Works', duration: '18:30', free: false },
        ],
      },
      {
        section: 'HTML Fundamentals',
        lectures: 18,
        duration: '4 hours',
        lessons: [
          {
            title: 'HTML Structure and Syntax',
            duration: '20:15',
            free: false,
          },
          { title: 'Common HTML Tags', duration: '25:30', free: false },
          { title: 'Forms and Input Elements', duration: '30:45', free: false },
          { title: 'Semantic HTML', duration: '22:10', free: false },
        ],
      },
      {
        section: 'CSS Mastery',
        lectures: 24,
        duration: '6 hours',
        lessons: [
          { title: 'CSS Fundamentals', duration: '18:20', free: false },
          { title: 'Flexbox Layout', duration: '35:15', free: false },
          { title: 'CSS Grid', duration: '28:45', free: false },
          { title: 'Responsive Design', duration: '42:30', free: false },
        ],
      },
      {
        section: 'JavaScript Essentials',
        lectures: 32,
        duration: '8 hours',
        lessons: [
          { title: 'JavaScript Basics', duration: '25:30', free: false },
          { title: 'DOM Manipulation', duration: '35:20', free: false },
          { title: 'Event Handling', duration: '28:15', free: false },
          { title: 'Async JavaScript', duration: '45:10', free: false },
        ],
      },
      {
        section: 'React Development',
        lectures: 28,
        duration: '7 hours',
        lessons: [
          { title: 'Introduction to React', duration: '22:30', free: false },
          { title: 'Components and JSX', duration: '30:45', free: false },
          { title: 'State and Props', duration: '35:20', free: false },
          { title: 'Hooks Deep Dive', duration: '40:15', free: false },
        ],
      },
      {
        section: 'Backend with Node.js',
        lectures: 26,
        duration: '6.5 hours',
        lessons: [
          { title: 'Node.js Fundamentals', duration: '28:30', free: false },
          { title: 'Express.js Framework', duration: '35:45', free: false },
          { title: 'REST API Development', duration: '42:20', free: false },
          {
            title: 'Authentication & Security',
            duration: '38:15',
            free: false,
          },
        ],
      },
      {
        section: 'Database Integration',
        lectures: 16,
        duration: '4 hours',
        lessons: [
          { title: 'Introduction to MongoDB', duration: '25:30', free: false },
          { title: 'Mongoose ODM', duration: '30:20', free: false },
          { title: 'Database Design', duration: '35:45', free: false },
          { title: 'Data Relationships', duration: '28:15', free: false },
        ],
      },
      {
        section: 'Deployment & Production',
        lectures: 8,
        duration: '2.5 hours',
        lessons: [
          { title: 'Git Version Control', duration: '20:30', free: false },
          { title: 'Deploying to Heroku', duration: '25:45', free: false },
          { title: 'Environment Variables', duration: '18:20', free: false },
          { title: 'Performance Optimization', duration: '22:15', free: false },
        ],
      },
    ],

    reviews: [
      {
        id: 1,
        user: 'Alex Thompson',
        avatar:
          'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=50',
        rating: 5,
        date: '2 weeks ago',
        content:
          "This course exceeded my expectations! Sarah's teaching style is clear and engaging. I went from zero programming knowledge to building my own web applications. The projects are practical and really help solidify the concepts.",
      },
      {
        id: 2,
        user: 'Maria Garcia',
        avatar:
          'https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=50',
        rating: 5,
        date: '1 month ago',
        content:
          'Absolutely fantastic course! The curriculum is well-structured and up-to-date with current industry standards. The support from the instructor and community is amazing. Already landed my first developer job!',
      },
      {
        id: 3,
        user: 'David Chen',
        avatar:
          'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=50',
        rating: 4,
        date: '3 weeks ago',
        content:
          'Great content and excellent explanation of complex concepts. The hands-on projects really helped me understand how everything fits together. Would definitely recommend to anyone starting their web development journey.',
      },
    ],
  };

  relatedCourses: RelatedCourse[] = [
    {
      id: 11,
      title: 'Advanced JavaScript Course',
      image: 'https://images.unsplash.com/photo-1550745166?w=80',
      rating: 4.9,
      price: 89,
    },
    {
      id: 12,
      title: 'Advanced React Course',
      image: 'https://images.unsplash.com/photo-1550745167?w=80',
      rating: 4.8,
      price: 99,
    },
    {
      id: 13,
      title: 'Advanced Node.js Course',
      image: 'https://images.unsplash.com/photo-1550745168?w=80',
      rating: 4.7,
      price: 109,
    },
  ];

  constructor(private router: Router, private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.courseId = this.route.snapshot.paramMap.get('id');
    // In a real app, you would fetch course data based on the ID
  }

  setActiveTab(tab: string): void {
    this.activeTab = tab;
  }

  toggleEnrollment(): void {
    this.isEnrolled = !this.isEnrolled;
    // In a real app, you would handle enrollment logic here
  }

  navigateToMessages(): void {
    this.router.navigate(['/learning/chat']);
  }

  navigateToCourse(): void {
    this.router.navigate(['/learning/1']);
  }

  getDiscountPercentage(): number {
    return Math.round(
      (1 - this.course.price / this.course.originalPrice) * 100
    );
  }

  getStarArray(count: number): number[] {
    return Array(count).fill(0);
  }
}
