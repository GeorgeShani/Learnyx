import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';

interface Stat {
  iconSvg: SafeHtml;
  label: string;
  value: string;
}

interface Tab {
  value: string;
  label: string;
  iconSvg: SafeHtml;
}

type NotificationKey =
  | 'emailCourses'
  | 'emailPromotions'
  | 'emailNews'
  | 'pushCourses'
  | 'pushReminders'
  | 'smsReminders';

interface NotificationItem {
  key: NotificationKey;
  label: string;
  description: string;
}

type PrivacyKey = 'profileVisible' | 'progressVisible' | 'achievementsVisible';

interface PrivacyItem {
  key: PrivacyKey;
  label: string;
  description: string;
}

interface Achievement {
  title: string;
  date: string;
  icon: string;
}

// ---- Component ----
@Component({
  selector: 'app-profile',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss'],
})
export class ProfileComponent {
  activeTab: string = 'general';
  isEditing = false;
  showPassword = false;

  profileForm: FormGroup;
  passwordForm: FormGroup;

  stats: Stat[] = [];
  tabs: Tab[] = [];

  constructor(private fb: FormBuilder, private sanitizer: DomSanitizer) {
    this.profileForm = this.fb.group({
      firstName: [this.profile.firstName, Validators.required],
      lastName: [this.profile.lastName, Validators.required],
      email: [this.profile.email, [Validators.required, Validators.email]],
      phone: [this.profile.phone],
      location: [this.profile.location],
      timezone: [this.profile.timezone],
      bio: [this.profile.bio],
      website: [this.profile.website],
      linkedin: [this.profile.linkedin],
      github: [this.profile.github],
    });

    this.passwordForm = this.fb.group({
      currentPassword: ['', Validators.required],
      newPassword: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', Validators.required],
    });

    this.stats = [
      {
        iconSvg: this.sanitizer.bypassSecurityTrustHtml(
          '<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="#3B82F6" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-book-open-icon lucide-book-open"><path d="M12 7v14"/><path d="M3 18a1 1 0 0 1-1-1V4a1 1 0 0 1 1-1h5a4 4 0 0 1 4 4 4 4 0 0 1 4-4h5a1 1 0 0 1 1 1v13a1 1 0 0 1-1 1h-6a3 3 0 0 0-3 3 3 3 0 0 0-3-3z"/></svg>'
        ),
        label: 'Courses Completed',
        value: '12',
      },
      {
        iconSvg: this.sanitizer.bypassSecurityTrustHtml(
          '<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="#EAB308" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-award-icon lucide-award"><path d="m15.477 12.89 1.515 8.526a.5.5 0 0 1-.81.47l-3.58-2.687a1 1 0 0 0-1.197 0l-3.586 2.686a.5.5 0 0 1-.81-.469l1.514-8.526"/><circle cx="12" cy="8" r="6"/></svg>'
        ),
        label: 'Certificates Earned',
        value: '8',
      },
      {
        iconSvg: this.sanitizer.bypassSecurityTrustHtml(
          '<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="#22C55E" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-trending-up-icon lucide-trending-up"><path d="M16 7h6v6"/><path d="m22 7-8.5 8.5-5-5L2 17"/></svg>'
        ),
        label: 'Learning Hours',
        value: '156',
      },
      {
        iconSvg: this.sanitizer.bypassSecurityTrustHtml(
          '<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="#A855F7" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-calendar-icon lucide-calendar"><path d="M8 2v4"/><path d="M16 2v4"/><rect width="18" height="18" x="3" y="4" rx="2"/><path d="M3 10h18"/></svg>'
        ),
        label: 'Learning Streak',
        value: '23 days',
      },
    ];

    this.tabs = [
      {
        value: 'general',
        label: 'General',
        iconSvg: this.sanitizer.bypassSecurityTrustHtml(
          '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-user-icon lucide-user"><path d="M19 21v-2a4 4 0 0 0-4-4H9a4 4 0 0 0-4 4v2"/><circle cx="12" cy="7" r="4"/></svg>'
        ),
      },
      {
        value: 'notifications',
        label: 'Notifications',
        iconSvg: this.sanitizer.bypassSecurityTrustHtml(
          '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-bell-icon lucide-bell"><path d="M10.268 21a2 2 0 0 0 3.464 0"/><path d="M3.262 15.326A1 1 0 0 0 4 17h16a1 1 0 0 0 .74-1.673C19.41 13.956 18 12.499 18 8A6 6 0 0 0 6 8c0 4.499-1.411 5.956-2.738 7.326"/></svg>'
        ),
      },
      {
        value: 'privacy',
        label: 'Privacy',
        iconSvg: this.sanitizer.bypassSecurityTrustHtml(
          '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-shield-icon lucide-shield"><path d="M20 13c0 5-3.5 7.5-7.66 8.95a1 1 0 0 1-.67-.01C7.5 20.5 4 18 4 13V6a1 1 0 0 1 1-1c2 0 4.5-1.2 6.24-2.72a1.17 1.17 0 0 1 1.52 0C14.51 3.81 17 5 19 5a1 1 0 0 1 1 1z"/></svg>'
        ),
      },
      {
        value: 'security',
        label: 'Security',
        iconSvg: this.sanitizer.bypassSecurityTrustHtml(
          '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-key-icon lucide-key"><path d="m15.5 7.5 2.3 2.3a1 1 0 0 0 1.4 0l2.1-2.1a1 1 0 0 0 0-1.4L19 4"/><path d="m21 2-9.6 9.6"/><circle cx="7.5" cy="15.5" r="5.5"/></svg>'
        ),
      },
      {
        value: 'achievements',
        label: 'Achievements',
        iconSvg: this.sanitizer.bypassSecurityTrustHtml(
          '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-award-icon lucide-award"><path d="m15.477 12.89 1.515 8.526a.5.5 0 0 1-.81.47l-3.58-2.687a1 1 0 0 0-1.197 0l-3.586 2.686a.5.5 0 0 1-.81-.469l1.514-8.526"/><circle cx="12" cy="8" r="6"/></svg>'
        ),
      },
    ];
  }

  // ---- Profile ----
  profile = {
    firstName: 'John',
    lastName: 'Doe',
    email: 'john.doe@example.com',
    phone: '+1 (555) 123-4567',
    location: 'San Francisco, CA',
    timezone: 'America/Los_Angeles',
    bio: 'Passionate learner interested in data science and web development. Always looking to expand my skills and knowledge.',
    website: 'https://johndoe.dev',
    linkedin: 'https://linkedin.com/in/johndoe',
    github: 'https://github.com/johndoe',
  };

  // ---- Settings ----
  notifications: Record<NotificationKey, boolean> = {
    emailCourses: true,
    emailPromotions: false,
    emailNews: true,
    pushCourses: true,
    pushReminders: true,
    smsReminders: false,
  };

  privacy: Record<PrivacyKey, boolean> = {
    profileVisible: true,
    progressVisible: false,
    achievementsVisible: true,
  };

  emailNotifications: NotificationItem[] = [
    {
      key: 'emailCourses',
      label: 'Course Updates',
      description: 'New lessons, assignments, and announcements',
    },
    {
      key: 'emailPromotions',
      label: 'Promotions & Offers',
      description: 'Special deals and discounts',
    },
    {
      key: 'emailNews',
      label: 'Newsletter',
      description: 'Weekly learning tips and platform updates',
    },
  ];

  pushNotifications: NotificationItem[] = [
    {
      key: 'pushCourses',
      label: 'Course Reminders',
      description: 'Reminders for scheduled learning sessions',
    },
    {
      key: 'pushReminders',
      label: 'Study Reminders',
      description: 'Daily and weekly study reminders',
    },
  ];

  privacySettings: PrivacyItem[] = [
    {
      key: 'profileVisible',
      label: 'Public Profile',
      description: 'Make your profile visible to other learners',
    },
    {
      key: 'progressVisible',
      label: 'Learning Progress',
      description: 'Show your course progress to others',
    },
    {
      key: 'achievementsVisible',
      label: 'Achievements',
      description: 'Display your certificates and badges publicly',
    },
  ];

  achievements: Achievement[] = [
    { title: 'First Course Completed', date: '2024-01-15', icon: '🎯' },
    { title: 'Data Science Specialist', date: '2024-02-28', icon: '📊' },
    { title: 'Fast Learner', date: '2024-03-10', icon: '⚡' },
    { title: 'Community Helper', date: '2024-03-20', icon: '🤝' },
  ];

  setActiveTab(tab: string): void {
    this.activeTab = tab;
  }

  toggleEdit(): void {
    this.isEditing = !this.isEditing;
    if (!this.isEditing) {
      this.profileForm.patchValue(this.profile);
    }
  }

  handleSave(): void {
    if (this.profileForm.valid) {
      this.profile = { ...this.profile, ...this.profileForm.value };
      this.isEditing = false;
      console.log('Profile updated successfully');
    }
  }

  handleNotificationChange(key: NotificationKey, event: Event): void {
    const target = event.target as HTMLInputElement;
    this.notifications = {
      ...this.notifications,
      [key]: target.checked,
    };
  }

  handlePrivacyChange(key: PrivacyKey, event: Event): void {
    const target = event.target as HTMLInputElement;
    this.privacy = {
      ...this.privacy,
      [key]: target.checked,
    };
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  // ---- DomSanitizer for SVG ----
  getSafeSvg(svg: string): SafeHtml {
    return this.sanitizer.bypassSecurityTrustHtml(svg);
  }
}
