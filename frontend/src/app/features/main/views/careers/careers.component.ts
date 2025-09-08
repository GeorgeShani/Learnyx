import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

interface JobOpening {
  title: string;
  department: string;
  location: string;
  type: string;
  description: string;
  requirements: string[];
}

interface Benefit {
  icon: SafeHtml;
  title: string;
  description: string;
}

@Component({
  selector: 'app-careers',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './careers.component.html',
  styleUrls: ['./careers.component.scss'],
})
export class CareersComponent {
  selectedJob: JobOpening | null = null;
  resumeModalOpen = false;
  cultureModalOpen = false;

  jobApplicationForm: FormGroup;
  resumeForm: FormGroup;

  jobOpenings: JobOpening[] = [
    {
      title: 'Senior Frontend Developer',
      department: 'Engineering',
      location: 'Remote',
      type: 'Full-time',
      description:
        'Join our engineering team to build the next generation of learning experiences using React, TypeScript, and modern web technologies.',
      requirements: [
        '5+ years React experience',
        'TypeScript expertise',
        'UI/UX design skills',
      ],
    },
    {
      title: 'Product Manager',
      department: 'Product',
      location: 'San Francisco, CA',
      type: 'Full-time',
      description:
        'Lead product strategy and development for our core learning platform, working closely with engineering and design teams.',
      requirements: [
        '3+ years product management',
        'EdTech experience',
        'Data-driven mindset',
      ],
    },
    {
      title: 'UX Designer',
      department: 'Design',
      location: 'Remote',
      type: 'Full-time',
      description:
        'Design intuitive and engaging learning experiences that help millions of students achieve their goals.',
      requirements: [
        'Strong portfolio',
        'Figma expertise',
        'User research experience',
      ],
    },
    {
      title: 'DevOps Engineer',
      department: 'Engineering',
      location: 'New York, NY',
      type: 'Full-time',
      description:
        'Build and maintain scalable infrastructure to support our growing platform and ensure 99.9% uptime.',
      requirements: ['AWS/GCP experience', 'Kubernetes', 'CI/CD pipelines'],
    },
    {
      title: 'Content Marketing Manager',
      department: 'Marketing',
      location: 'Remote',
      type: 'Contract',
      description:
        'Create compelling content that drives user engagement and showcases the value of online learning.',
      requirements: [
        'Content strategy',
        'SEO knowledge',
        'Educational background',
      ],
    },
  ];

  benefits: Benefit[];

  constructor(
    private formBuilder: FormBuilder,
    private sanitizer: DomSanitizer
  ) {
    this.jobApplicationForm = this.formBuilder.group({
      fullName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phone: [''],
      resume: ['', Validators.required],
      coverLetter: [''],
    });

    this.resumeForm = this.formBuilder.group({
      fullName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      resume: ['', Validators.required],
      interests: [''],
      message: [''],
    });

    this.benefits = [
      {
        icon: this.sanitizer.bypassSecurityTrustHtml(`
          <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="#5BCBF1" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-heart-icon lucide-heart">
            <path d="M2 9.5a5.5 5.5 0 0 1 9.591-3.676.56.56 0 0 0 .818 0A5.49 5.49 0 0 1 22 9.5c0 2.29-1.5 4-3 5.5l-5.492 5.313a2 2 0 0 1-3 .019L5 15c-1.5-1.5-3-3.2-3-5.5"/>
          </svg>
        `),
        title: 'Health & Wellness',
        description:
          'Comprehensive health insurance, mental health support, and wellness programs',
      },
      {
        icon: this.sanitizer.bypassSecurityTrustHtml(`
          <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="#5BCBF1" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-zap-icon lucide-zap">
            <path d="M4 14a1 1 0 0 1-.78-1.63l9.9-10.2a.5.5 0 0 1 .86.46l-1.92 6.02A1 1 0 0 0 13 10h7a1 1 0 0 1 .78 1.63l-9.9 10.2a.5.5 0 0 1-.86-.46l1.92-6.02A1 1 0 0 0 11 14z"/>
          </svg>
        `),
        title: 'Learning & Growth',
        description:
          'Unlimited learning budget, conference attendance, and internal skill-sharing',
      },
      {
        icon: this.sanitizer.bypassSecurityTrustHtml(`
          <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="#5BCBF1" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-target-icon lucide-target">
            <circle cx="12" cy="12" r="10"/>
            <circle cx="12" cy="12" r="6"/>
            <circle cx="12" cy="12" r="2"/>
          </svg>
        `),
        title: 'Work-Life Balance',
        description: 'Flexible hours, unlimited PTO, and remote-first culture',
      },
      {
        icon: this.sanitizer.bypassSecurityTrustHtml(`
          <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="#5BCBF1" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-award-icon lucide-award">
            <path d="m15.477 12.89 1.515 8.526a.5.5 0 0 1-.81.47l-3.58-2.687a1 1 0 0 0-1.197 0l-3.586 2.686a.5.5 0 0 1-.81-.469l1.514-8.526"/>
            <circle cx="12" cy="8" r="6"/>
          </svg>
        `),
        title: 'Equity & Rewards',
        description:
          'Competitive salary, equity packages, and performance bonuses',
      },
    ];
  }

  handleJobApplication(job: JobOpening): void {
    this.selectedJob = job;
  }

  closeJobModal(): void {
    this.selectedJob = null;
    this.jobApplicationForm.reset();
  }

  openResumeModal(): void {
    this.resumeModalOpen = true;
  }

  closeResumeModal(): void {
    this.resumeModalOpen = false;
    this.resumeForm.reset();
  }

  openCultureModal(): void {
    this.cultureModalOpen = true;
  }

  closeCultureModal(): void {
    this.cultureModalOpen = false;
  }

  handleApplicationSubmit(): void {
    if (this.jobApplicationForm.valid) {
      // Simulate form submission
      console.log('Job application submitted:', this.jobApplicationForm.value);
      this.showToast(
        'Application Submitted',
        "Thank you for applying! We'll be in touch within 5 business days."
      );
      this.closeJobModal();
    }
  }

  handleResumeSubmit(): void {
    if (this.resumeForm.valid) {
      // Simulate form submission
      console.log('Resume submitted:', this.resumeForm.value);
      this.showToast(
        'Resume Submitted',
        "Thank you! We'll review your resume and get back to you soon."
      );
      this.closeResumeModal();
    }
  }

  private showToast(title: string, message: string): void {
    // Simple toast implementation - in a real app, you'd use a proper toast service
    alert(`${title}: ${message}`);
  }
}
