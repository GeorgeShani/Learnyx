import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router } from '@angular/router';

interface Stat {
  iconName: string;
  label: string;
  value: string;
}

interface Value {
  iconName: string;
  title: string;
  description: string;
}

interface TeamMember {
  name: string;
  role: string;
  avatar: string;
  bio: string;
}

interface Feature {
  iconName: string;
  title: string;
  description: string;
}

@Component({
  selector: 'app-about',
  imports: [CommonModule],
  templateUrl: './about.component.html',
  styleUrl: './about.component.scss',
})
export class AboutComponent {
  stats: Stat[] = [
    {
      iconName: 'assets/icons/users.svg',
      label: 'Active Learners',
      value: '50K+',
    },
    {
      iconName: 'assets/icons/book-open.svg',
      label: 'Courses Available',
      value: '1,200+',
    },
    {
      iconName: 'assets/icons/award.svg',
      label: 'Expert Instructors',
      value: '500+',
    },
    {
      iconName: 'assets/icons/globe.svg',
      label: 'Countries Reached',
      value: '120+',
    },
  ];

  values: Value[] = [
    {
      iconName: 'assets/icons/lightbulb.svg',
      title: 'Innovation',
      description:
        'We constantly evolve our platform with cutting-edge technology to enhance the learning experience.',
    },
    {
      iconName: 'assets/icons/heart.svg',
      title: 'Accessibility',
      description:
        'Quality education should be accessible to everyone, regardless of location or background.',
    },
    {
      iconName: 'assets/icons/target.svg',
      title: 'Excellence',
      description:
        'We maintain the highest standards in course quality and instructor expertise.',
    },
    {
      iconName: 'assets/icons/users.svg',
      title: 'Community',
      description:
        'Learning is better together. We foster a supportive community of learners and educators.',
    },
  ];

  team: TeamMember[] = [
    {
      name: 'Alex Chen',
      role: 'CEO & Founder',
      avatar:
        'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=150&h=150&fit=crop&crop=face',
      bio: 'Former EdTech executive with 15+ years experience in online education.',
    },
    {
      name: 'Sarah Johnson',
      role: 'CTO & Co-Founder',
      avatar:
        'https://www.georgetown.edu/wp-content/uploads/2022/02/Jkramerheadshot-scaled-e1645036825432-1050x1050-c-default.jpg',
      bio: 'Tech visionary who led engineering teams at major tech companies.',
    },
    {
      name: 'Dr. Michael Rodriguez',
      role: 'Head of Education',
      avatar:
        'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=150&h=150&fit=crop&crop=face',
      bio: 'Former university dean with expertise in curriculum development and pedagogy.',
    },
  ];

  features: Feature[] = [
    {
      iconName: 'assets/icons/zap.svg',
      title: 'AI-Powered Learning',
      description: 'Personalized learning paths adapted to your pace and style',
    },
    {
      iconName: 'assets/icons/shield.svg',
      title: 'Quality Assurance',
      description:
        'All courses are reviewed by industry experts before publication',
    },
    {
      iconName: 'assets/icons/trending-up.svg',
      title: 'Progress Tracking',
      description: 'Detailed analytics to monitor your learning journey',
    },
    {
      iconName: 'assets/icons/check-circle.svg',
      title: 'Certification',
      description: 'Industry-recognized certificates upon course completion',
    },
  ];

  constructor(private router: Router) { }
  
  navigate(route: string) {
    this.router.navigate([route]);
  }
}
