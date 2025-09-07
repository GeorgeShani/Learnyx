import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

interface Teacher {
  id: number;
  name: string;
  avatar: string;
  title: string;
  rating: number;
  students: number;
  courses: number;
  location: string;
  subjects: string[];
  bio: string;
  expertise: string[];
}

@Component({
  selector: 'app-teachers',
  imports: [CommonModule, FormsModule],
  templateUrl: './teachers.component.html',
  styleUrl: './teachers.component.scss',
})
export class TeachersComponent {
  searchTerm = '';
  selectedSubject = 'all';
  filteredTeachers: Teacher[] = [];

  teachers: Teacher[] = [
    {
      id: 1,
      name: 'Dr. Sarah Chen',
      avatar:
        'https://www.georgetown.edu/wp-content/uploads/2022/02/Jkramerheadshot-scaled-e1645036825432-1050x1050-c-default.jpg',
      title: 'Senior Data Science Instructor',
      rating: 4.9,
      students: 12500,
      courses: 8,
      location: 'San Francisco, CA',
      subjects: ['Data Science', 'Machine Learning', 'Python'],
      bio: 'Former Google ML engineer with 10+ years experience in AI and data analytics.',
      expertise: ['Python', 'TensorFlow', 'Statistics', 'Deep Learning'],
    },
    {
      id: 2,
      name: 'Prof. Michael Johnson',
      avatar:
        'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=150&h=150&fit=crop&crop=face',
      title: 'Full Stack Development Expert',
      rating: 4.8,
      students: 18200,
      courses: 12,
      location: 'New York, NY',
      subjects: ['Web Development', 'JavaScript', 'React'],
      bio: 'Senior software architect and former startup CTO with expertise in modern web technologies.',
      expertise: ['React', 'Node.js', 'TypeScript', 'AWS'],
    },
    {
      id: 3,
      name: 'Dr. Emily Rodriguez',
      avatar:
        'https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=150&h=150&fit=crop&crop=face',
      title: 'UX/UI Design Specialist',
      rating: 4.9,
      students: 9800,
      courses: 6,
      location: 'Los Angeles, CA',
      subjects: ['Design', 'UX/UI', 'Figma'],
      bio: 'Award-winning designer with experience at top tech companies and design agencies.',
      expertise: [
        'Figma',
        'Adobe Creative Suite',
        'User Research',
        'Prototyping',
      ],
    },
    {
      id: 4,
      name: 'James Wilson',
      avatar:
        'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=150&h=150&fit=crop&crop=face',
      title: 'DevOps & Cloud Engineer',
      rating: 4.7,
      students: 7600,
      courses: 5,
      location: 'Seattle, WA',
      subjects: ['DevOps', 'AWS', 'Docker'],
      bio: 'Cloud infrastructure expert helping students master modern deployment and scaling techniques.',
      expertise: ['AWS', 'Docker', 'Kubernetes', 'CI/CD'],
    },
    {
      id: 5,
      name: 'Dr. Lisa Park',
      avatar:
        'https://images.unsplash.com/photo-1489424731084-a5d8b219a5bb?w=150&h=150&fit=crop&crop=face',
      title: 'Mobile Development Lead',
      rating: 4.8,
      students: 11300,
      courses: 9,
      location: 'Austin, TX',
      subjects: ['Mobile Development', 'React Native', 'Flutter'],
      bio: 'Mobile app development expert with apps reaching millions of users worldwide.',
      expertise: ['React Native', 'Flutter', 'iOS', 'Android'],
    },
    {
      id: 6,
      name: 'Robert Kumar',
      avatar:
        'https://images.unsplash.com/photo-1560250097-0b93528c311a?w=150&h=150&fit=crop&crop=face',
      title: 'Cybersecurity Specialist',
      rating: 4.9,
      students: 8900,
      courses: 7,
      location: 'Boston, MA',
      subjects: ['Cybersecurity', 'Ethical Hacking', 'Network Security'],
      bio: 'Former security consultant helping students understand modern cybersecurity threats and defenses.',
      expertise: [
        'Penetration Testing',
        'Network Security',
        'Cryptography',
        'Risk Assessment',
      ],
    },
  ];

  subjects: string[] = [
    'all',
    'Data Science',
    'Web Development',
    'Design',
    'DevOps',
    'Mobile Development',
    'Cybersecurity',
  ];

  constructor(private router: Router) {
    this.filterTeachers();
  }

  onSearchChange(): void {
    this.filterTeachers();
  }

  onSubjectChange(): void {
    this.filterTeachers();
  }

  navigate(route: string): void {
    this.router.navigate([route]);
  }

  private filterTeachers(): void {
    this.filteredTeachers = this.teachers.filter((teacher) => {
      const matchesSearch =
        teacher.name.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        teacher.subjects.some((subject) =>
          subject.toLowerCase().includes(this.searchTerm.toLowerCase())
        );
      const matchesSubject =
        this.selectedSubject === 'all' ||
        teacher.subjects.includes(this.selectedSubject);
      return matchesSearch && matchesSubject;
    });
  }
}
