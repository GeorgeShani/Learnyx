import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';

interface Category {
  id: string;
  name: string;
  icon: string;
  count: number;
}

interface FAQ {
  id: number;
  category: string;
  question: string;
  answer: string;
  popular: boolean;
}

interface ContactOption {
  icon: string;
  title: string;
  description: string;
  availability: string;
  action: string;
}

interface Resource {
  title: string;
  description: string;
  icon: string;
  link: string;
}

@Component({
  selector: 'app-support',
  imports: [FormsModule, CommonModule],
  templateUrl: './support.component.html',
  styleUrl: './support.component.scss',
})
export class SupportComponent {
  searchTerm = '';
  selectedCategory = 'all';
  openFaqs: Set<number> = new Set();

  categories: Category[] = [
    { id: 'all', name: 'All Topics', icon: 'BookOpen', count: 24 },
    { id: 'getting-started', name: 'Getting Started', icon: 'BookOpen', count: 6, },
    { id: 'courses', name: 'Courses & Learning', icon: 'Video', count: 8 },
    { id: 'account', name: 'Account & Profile', icon: 'Users', count: 5 },
    { id: 'billing', name: 'Billing & Payments', icon: 'CreditCard', count: 4 },
    { id: 'technical', name: 'Technical Issues', icon: 'Settings', count: 3 },
    { id: 'privacy', name: 'Privacy & Security', icon: 'Shield', count: 2 },
  ];

  faqs: FAQ[] = [
    {
      id: 1,
      category: 'getting-started',
      question: 'How do I create an account?',
      answer:
        "To create an account, click the 'Sign Up' button in the top right corner of the homepage. You can register using your email address or sign up with Google/Facebook for faster access.",
      popular: true,
    },
    {
      id: 2,
      category: 'getting-started',
      question: 'How do I enroll in a course?',
      answer:
        "Browse our course catalog, click on any course that interests you, and click the 'Enroll Now' button. Free courses will be added to your dashboard immediately, while paid courses require payment confirmation.",
      popular: true,
    },
    {
      id: 3,
      category: 'courses',
      question: 'Can I download course videos for offline viewing?',
      answer:
        'Yes! Premium subscribers can download course videos for offline viewing using our mobile app. Look for the download icon next to each lesson.',
      popular: true,
    },
    {
      id: 4,
      category: 'courses',
      question: 'How do I track my learning progress?',
      answer:
        'Your progress is automatically tracked as you complete lessons. You can view detailed progress analytics in your student dashboard, including completion percentages, time spent, and quiz scores.',
      popular: false,
    },
    {
      id: 5,
      category: 'courses',
      question: 'What happens after I complete a course?',
      answer:
        "Upon course completion, you'll receive a certificate of completion that you can download and share on LinkedIn. The course will remain accessible in your library for future reference.",
      popular: true,
    },
    {
      id: 6,
      category: 'account',
      question: 'How do I reset my password?',
      answer:
        "Click 'Forgot Password' on the login page, enter your email address, and we'll send you a reset link. Follow the instructions in the email to create a new password.",
      popular: false,
    },
    {
      id: 7,
      category: 'account',
      question: 'Can I change my email address?',
      answer:
        "Yes, you can update your email address in your profile settings. Go to Profile > General > Personal Information and update your email. You'll need to verify the new email address.",
      popular: false,
    },
    {
      id: 8,
      category: 'billing',
      question: 'What payment methods do you accept?',
      answer:
        'We accept all major credit cards (Visa, MasterCard, American Express), PayPal, and bank transfers in select regions. All payments are processed securely through Stripe.',
      popular: true,
    },
    {
      id: 9,
      category: 'billing',
      question: 'Can I get a refund?',
      answer:
        "We offer a 30-day money-back guarantee for all paid courses. If you're not satisfied, contact our support team within 30 days of purchase for a full refund.",
      popular: false,
    },
    {
      id: 10,
      category: 'technical',
      question: 'The video player is not working. What should I do?',
      answer:
        'Try refreshing the page first. If the issue persists, clear your browser cache, disable ad blockers, or try a different browser. Contact support if you continue experiencing issues.',
      popular: false,
    },
    {
      id: 11,
      category: 'privacy',
      question: 'How do you protect my personal information?',
      answer:
        'We use industry-standard encryption and security measures to protect your data. We never sell your personal information to third parties. See our Privacy Policy for detailed information.',
      popular: false,
    },
    {
      id: 12,
      category: 'getting-started',
      question: 'Is there a mobile app?',
      answer:
        'Yes! Download the Learnyx mobile app from the App Store or Google Play Store. The app includes offline video downloads, progress syncing, and push notifications.',
      popular: true,
    },
  ];

  contactOptions: ContactOption[] = [
    {
      icon: 'MessageSquare',
      title: 'Live Chat',
      description: 'Get instant help from our support team',
      availability: '24/7 Available',
      action: 'Start Chat',
    },
    {
      icon: 'Mail',
      title: 'Email Support',
      description: 'Send us a detailed message',
      availability: 'Response within 24h',
      action: 'Send Email',
    },
    {
      icon: 'Phone',
      title: 'Phone Support',
      description: 'Speak directly with our team',
      availability: 'Mon-Fri, 9AM-6PM EST',
      action: 'Call Now',
    },
  ];

  resources: Resource[] = [
    {
      title: 'User Guide',
      description: 'Complete guide to using Learnyx',
      icon: 'BookOpen',
      link: '#',
    },
    {
      title: 'Video Tutorials',
      description: 'Step-by-step video guides',
      icon: 'Video',
      link: '#',
    },
    {
      title: 'Mobile App Guide',
      description: 'How to use our mobile app',
      icon: 'Download',
      link: '#',
    },
  ];

  setSelectedCategory(categoryId: string): void {
    this.selectedCategory = categoryId;
  }

  getCategoryButtonClass(categoryId: string): string {
    return `category-button ${
      this.selectedCategory === categoryId ? 'active' : ''
    }`;
  }

  getFilteredFaqs(): FAQ[] {
    return this.faqs.filter((faq) => {
      const matchesSearch =
        faq.question.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        faq.answer.toLowerCase().includes(this.searchTerm.toLowerCase());
      const matchesCategory =
        this.selectedCategory === 'all' ||
        faq.category === this.selectedCategory;
      return matchesSearch && matchesCategory;
    });
  }

  getPopularFaqs(): FAQ[] {
    return this.faqs.filter((faq) => faq.popular).slice(0, 4);
  }

  getFaqResultsTitle(): string {
    if (this.searchTerm) {
      return `Search Results (${this.getFilteredFaqs().length})`;
    } else if (this.selectedCategory === 'all') {
      return 'All Questions';
    } else {
      const category = this.categories.find(
        (cat) => cat.id === this.selectedCategory
      );
      return category ? category.name : 'Questions';
    }
  }

  toggleFaq(faqId: number): void {
    if (this.openFaqs.has(faqId)) {
      this.openFaqs.delete(faqId);
    } else {
      this.openFaqs.add(faqId);
    }
  }

  getCategoryIconPath(iconName: string): string {
    const iconPaths: { [key: string]: string } = {
      BookOpen:
        'M2 3h6a4 4 0 0 1 4 4v14a3 3 0 0 0-3-3H2z M22 3h-6a4 4 0 0 0-4 4v14a3 3 0 0 1 3-3h7z',
      Video:
        'M16 13 L21.223 16.482 A0.5 0.5 0 0 0 22 16.066 V7.87 A0.5 0.5 0 0 0 21.248 7.438 L16 10.5 M2 8 A2 2 0 0 1 4 6 H14 A2 2 0 0 1 16 8 V16 A2 2 0 0 1 14 18 H4 A2 2 0 0 1 2 16 Z',
      Users:
        'M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2 M16 3.128a4 4 0 0 1 0 7.744 M22 21v-2a4 4 0 0 0-3-3.87 M9 3a4 4 0 1 1 0 8a4 4 0 1 1 0-8Z',
      CreditCard:
        'M21 4H3c-1.1 0-2 .9-2 2v12c0 1.1.9 2 2 2h18c1.1 0 2-.9 2-2V6c0-1.1-.9-2-2-2z M2 10h20',
      Settings:
        'M12.22 2h-.44a2 2 0 0 0-2 2v.18a2 2 0 0 1-1 1.73l-.43.25a2 2 0 0 1-2 0l-.15-.08a2 2 0 0 0-2.73.73l-.22.38a2 2 0 0 0 .73 2.73l.15.1a2 2 0 0 1 1 1.72v.51a2 2 0 0 1-1 1.74l-.15.09a2 2 0 0 0-.73 2.73l.22.38a2 2 0 0 0 2.73.73l.15-.08a2 2 0 0 1 2 0l.43.25a2 2 0 0 1 1 1.73V20a2 2 0 0 0 2 2h.44a2 2 0 0 0 2-2v-.18a2 2 0 0 1 1-1.73l.43-.25a2 2 0 0 1 2 0l.15.08a2 2 0 0 0 2.73-.73l.22-.39a2 2 0 0 0-.73-2.73l-.15-.08a2 2 0 0 1-1-1.74v-.5a2 2 0 0 1 1-1.74l.15-.09a2 2 0 0 0 .73-2.73l-.22-.38a2 2 0 0 0-2.73-.73l-.15.08a2 2 0 0 1-2 0l-.43-.25a2 2 0 0 1-1-1.73V4a2 2 0 0 0-2-2z M12 15a3 3 0 1 0 0-6 3 3 0 0 0 0 6z',
      Shield: 'M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z',
    };
    return iconPaths[iconName] || '';
  }

  getContactIconPath(iconName: string): string {
    const iconPaths: { [key: string]: string } = {
      MessageSquare:
        'M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z',
      Mail: 'M4 4h16c1.1 0 2 .9 2 2v12c0 1.1-.9 2-2 2H4c-1.1 0-2-.9-2-2V6c0-1.1.9-2 2-2z M22 6l-10 7L2 6',
      Phone:
        'M22 16.92v3a2 2 0 0 1-2.18 2 19.79 19.79 0 0 1-8.63-3.07 19.5 19.5 0 0 1-6-6 19.79 19.79 0 0 1-3.07-8.67A2 2 0 0 1 4.11 2h3a2 2 0 0 1 2 1.72 12.84 12.84 0 0 0 .7 2.81 2 2 0 0 1-.45 2.11L8.09 9.91a16 16 0 0 0 6 6l1.27-1.27a2 2 0 0 1 2.11-.45 12.84 12.84 0 0 0 2.81.7A2 2 0 0 1 22 16.92z',
    };
    return iconPaths[iconName] || '';
  }

  getResourceIconPath(iconName: string): string {
    const iconPaths: { [key: string]: string } = {
      BookOpen:
        'M2 3h6a4 4 0 0 1 4 4v14a3 3 0 0 0-3-3H2z M22 3h-6a4 4 0 0 0-4 4v14a3 3 0 0 1 3-3h7z',
      Video:
        'M16 13 L21.223 16.482 A0.5 0.5 0 0 0 22 16.066 V7.87 A0.5 0.5 0 0 0 21.248 7.438 L16 10.5 M2 8 A2 2 0 0 1 4 6 H14 A2 2 0 0 1 16 8 V16 A2 2 0 0 1 14 18 H4 A2 2 0 0 1 2 16 Z',
      Download:
        'M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4 M7 10l5 5 5-5 M12 15V3',
    };
    return iconPaths[iconName] || '';
  }

  /*
    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="#ffffff" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
  <path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2
           M16 3.128a4 4 0 0 1 0 7.744
           M22 21v-2a4 4 0 0 0-3-3.87
           M9 3a4 4 0 1 1 0 8a4 4 0 1 1 0-8Z"/>
</svg>

  */
}
