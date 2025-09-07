import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';

interface ContactInfo {
  iconPath: string;
  title: string;
  content: string;
  description: string;
  action: string;
}

interface FaqCategory {
  iconPath: string;
  title: string;
  description: string;
}

interface SocialPlatform {
  label: string;
  link: string;
}

@Component({
  selector: 'app-contact',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './contact.component.html',
  styleUrl: './contact.component.scss',
})
export class ContactComponent {
  contactForm: FormGroup;
  isSubmitting = false;

  contactInfo: ContactInfo[] = [
    {
      iconPath: 'assets/icons/mail-white.svg',
      title: 'Email Us',
      content: 'support@learnyx.io',
      description: 'Get in touch via email',
      action: 'mailto:support@learnyx.com',
    },
    {
      iconPath: 'assets/icons/phone.svg',
      title: 'Call Us',
      content: '+1 (555) 123-4567',
      description: 'Mon-Fri, 9AM-6PM EST',
      action: 'tel:+15551234567',
    },
    {
      iconPath: 'assets/icons/map-pin.svg',
      title: 'Visit Us',
      content: '123 Learning Street, Education City, EC 12345',
      description: 'Our headquarters',
      action: '#',
    },
    {
      iconPath: 'assets/icons/clock.svg',
      title: 'Support Hours',
      content: '24/7 Online Support',
      description: "We're always here to help",
      action: '#',
    },
  ];

  faqCategories: FaqCategory[] = [
    {
      iconPath: 'assets/icons/message-square.svg',
      title: 'General Inquiries',
      description: 'Questions about courses, pricing, or platform features',
    },
    {
      iconPath: 'assets/icons/help-circle.svg',
      title: 'Technical Support',
      description: 'Help with login issues, playback problems, or bugs',
    },
    {
      iconPath: 'assets/icons/building.svg',
      title: 'Business Partnerships',
      description: 'Corporate training, bulk licensing, or custom solutions',
    },
    {
      iconPath: 'assets/icons/users-cyan.svg',
      title: 'Instructor Applications',
      description: 'Apply to become an instructor or course creator',
    },
  ];

  socialPlatforms: SocialPlatform[] = [
    {
      label: 'Twitter',
      link: 'https://x.com/',
    },
    {
      label: 'LinkedIn',
      link: 'https://www.linkedin.com',
    },
    {
      label: 'Facebook',
      link: 'https://www.facebook.com/',
    },
    {
      label: 'Discord',
      link: 'https://discord.com/',
    },
  ];

  constructor(private formBuilder: FormBuilder, private router: Router) {
    this.contactForm = this.formBuilder.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      category: ['', Validators.required],
      subject: ['', [Validators.required, Validators.minLength(5)]],
      message: ['', [Validators.required, Validators.minLength(10)]],
    });
  }

  openContact(action: string): void {
    if (action.startsWith('mailto:') || action.startsWith('tel:')) {
      window.open(action, '_blank');
    }
  }

  async onSubmit(): Promise<void> {
    if (this.contactForm.valid) {
      this.isSubmitting = true;

      // Simulate API call
      await new Promise((resolve) => setTimeout(resolve, 2000));

      // Show success message (you would implement a toast service here)
      console.log('Message sent successfully!');
      alert(
        "Thank you for contacting us. We'll get back to you within 24 hours."
      );

      // Reset form
      this.contactForm.reset();
      this.isSubmitting = false;
    } else {
      // Mark all fields as touched to show validation errors
      Object.keys(this.contactForm.controls).forEach((key) => {
        this.contactForm.get(key)?.markAsTouched();
      });
    }
  }

  navigate(route: string): void {
    this.router.navigate([route]);
  }

  open(link: string): void {
    window.open(link, '_blank', 'noopener,noreferrer');
  }
}
