import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AssignmentListComponent } from '@features/learning/components/assignment-list/assignment-list.component';
import { CourseProgressComponent } from '@features/learning/components/course-progress/course-progress.component';
import { VideoPlayerComponent } from '@features/learning/components/video-player/video-player.component';

interface Resource {
  name: string;
  type: 'pdf' | 'zip' | 'link' | 'code';
  size: string;
  url?: string;
}

interface CourseModule {
  id: string;
  title: string;
  description: string;
  duration: string;
  videoUrl: string;
  completed: boolean;
  resources: Resource[];
}

interface Assignment {
  id: string;
  title: string;
  description: string;
  dueDate: string;
  maxPoints: number;
  status: 'pending' | 'submitted' | 'graded' | 'overdue';
  submissionType: 'text' | 'file' | 'both';
  grade?: number;
  feedback?: string;
  submittedAt?: string;
}

interface Instructor {
  name: string;
  avatar: string;
}

interface CourseData {
  title: string;
  instructor: Instructor;
  modules: CourseModule[];
  assignments: Assignment[];
  progress: number;
  currentModule: string;
}

@Component({
  selector: 'app-course',
  imports: [
    CommonModule,
    AssignmentListComponent,
    VideoPlayerComponent,
    CourseProgressComponent,
  ],
  templateUrl: './course.component.html',
  styleUrl: './course.component.scss',
})
export class CourseComponent implements OnInit {
  courseData: CourseData = {
    title: 'Complete Web Development Bootcamp 2024',
    instructor: {
      name: 'John Smith',
      avatar:
        'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=150&h=150&fit=crop&crop=face',
    },
    modules: [
      {
        id: '1',
        title: 'Introduction to React',
        description: 'Learn the basics of React components and JSX',
        duration: '15:30',
        videoUrl:
          'https://echowave-musics-storage-bucket.s3.eu-north-1.amazonaws.com/videos/JavaScript+tutorial+for+beginners+%F0%9F%8C%90+-+Bro+Code+(720p%2C+h264%2C+youtube).mp4',
        completed: true,
        resources: [
          { name: 'React Quick Start Guide', type: 'pdf', size: '2.4 MB' },
          { name: 'React Component Examples', type: 'zip', size: '1.8 MB' },
          {
            name: 'React Official Documentation',
            type: 'link',
            size: 'External Link',
          },
          { name: 'JSX Syntax Cheatsheet', type: 'pdf', size: '650 KB' },
        ],
      },
      {
        id: '2',
        title: 'State Management',
        description: 'Understanding useState and useEffect hooks',
        duration: '22:45',
        videoUrl:
          'https://echowave-musics-storage-bucket.s3.eu-north-1.amazonaws.com/videos/JavaScript+tutorial+for+beginners+%F0%9F%8C%90+-+Bro+Code+(720p%2C+h264%2C+youtube).mp4',
        completed: false,
        resources: [
          { name: 'Hooks API Reference', type: 'pdf', size: '3.2 MB' },
          {
            name: 'State Management Best Practices',
            type: 'pdf',
            size: '1.5 MB',
          },
          { name: 'useState Examples', type: 'zip', size: '2.1 MB' },
          { name: 'useEffect Tutorial Code', type: 'zip', size: '1.7 MB' },
          {
            name: 'React Hooks Documentation',
            type: 'link',
            size: 'External Link',
          },
          { name: 'Hook Rules and Guidelines', type: 'pdf', size: '890 KB' },
        ],
      },
      {
        id: '3',
        title: 'Component Lifecycle',
        description:
          'Deep dive into React component lifecycle methods and hooks',
        duration: '18:20',
        videoUrl:
          'https://echowave-musics-storage-bucket.s3.eu-north-1.amazonaws.com/videos/JavaScript+tutorial+for+beginners+%F0%9F%8C%90+-+Bro+Code+(720p%2C+h264%2C+youtube).mp4',
        completed: false,
        resources: [
          { name: 'Component Lifecycle Diagram', type: 'pdf', size: '1.2 MB' },
          { name: 'Lifecycle Methods Examples', type: 'zip', size: '2.3 MB' },
          {
            name: 'useEffect vs Lifecycle Methods',
            type: 'pdf',
            size: '1.1 MB',
          },
          { name: 'Cleanup Functions Guide', type: 'pdf', size: '750 KB' },
        ],
      },
    ],
    assignments: [
      {
        id: '1',
        title: 'React Component Assignment',
        description: 'Create a functional React component with props',
        dueDate: '2024-02-15',
        maxPoints: 100,
        status: 'graded',
        submissionType: 'file',
        grade: 95,
        feedback:
          'Excellent work! Your component structure is clean and well-documented.',
        submittedAt: '2024-02-10',
      },
      {
        id: '2',
        title: 'State Management Exercise',
        description: 'Implement state management using hooks',
        dueDate: '2024-02-20',
        maxPoints: 100,
        status: 'pending',
        submissionType: 'both',
      },
    ],
    progress: 45,
    currentModule: '2',
  };

  currentModuleId: string = this.courseData.currentModule;
  activeTab = 'assignments';

  constructor(private router: Router) {}

  ngOnInit(): void {}

  goBack(): void {
    this.router.navigate(['/dashboard/student']);
  }

  getCurrentModule(): CourseModule {
    return (
      this.courseData.modules.find((m) => m.id === this.currentModuleId) ||
      this.courseData.modules[0]
    );
  }

  getCompletedModulesCount(): number {
    return this.courseData.modules.filter((m) => m.completed).length;
  }

  setActiveTab(tab: string): void {
    this.activeTab = tab;
  }

  setCurrentModule(moduleId: string): void {
    this.currentModuleId = moduleId;
  }

  handleModuleComplete(moduleId: string): void {
    this.courseData.modules = this.courseData.modules.map((m) =>
      m.id === moduleId ? { ...m, completed: true } : m
    );
    this.courseData.progress = Math.min(this.courseData.progress + 15, 100);
  }

  handleAssignmentSubmit(data: {
    assignmentId: string;
    submission: any;
  }): void {
    this.courseData.assignments = this.courseData.assignments.map((a) =>
      a.id === data.assignmentId
        ? {
            ...a,
            status: 'submitted' as const,
            submittedAt: new Date().toISOString(),
          }
        : a
    );
  }

  // Resource handling
  downloadResource(resource: Resource): void {
    console.log(`Downloading ${resource.name}...`);
    // Simulate download
    if (resource.url) {
      // If URL is provided, open it
      window.open(resource.url, '_blank');
    } else {
      // Simulate file download
      alert(`Downloaded ${resource.name}!`);
    }
  }

  /*
  downloadResource(resource: Resource): void {
    console.log(`Downloading ${resource.name}...`);

    if (resource.url) {
      // Trigger a real download from URL
      const a = document.createElement('a');
      a.href = resource.url;
      a.download = resource.name; // Suggested filename
      a.target = '_blank';
      a.click();
    } else {
      // Simulate file content if no URL
      const content = `This is the content of ${resource.name}`;
      const blob = new Blob([content], { type: 'text/plain' });
      const url = URL.createObjectURL(blob);

      const a = document.createElement('a');
      a.href = url;
      a.download = resource.name;
      a.click();

      URL.revokeObjectURL(url); // Clean up
    }
  }
  */

  getAverageGrade(): string {
    const gradedAssignments = this.courseData.assignments.filter(
      (a) => a.grade
    );
    if (gradedAssignments.length === 0) return '0.0';

    const average =
      gradedAssignments.reduce((acc, a) => acc + (a.grade || 0), 0) /
      gradedAssignments.length;
    return average.toFixed(1);
  }

  getCompletedAssignments(): number {
    return this.courseData.assignments.filter((a) => a.status === 'graded')
      .length;
  }

  getPendingAssignments(): number {
    return this.courseData.assignments.length - this.getCompletedAssignments();
  }

  getGradedAssignments(): Assignment[] {
    return this.courseData.assignments.filter((a) => a.grade);
  }

  getGradeBadgeClass(grade: number | undefined, maxPoints: number): string {
    if (!grade) return 'average';
    const percentage = (grade / maxPoints) * 100;
    if (percentage >= 90) return 'excellent';
    if (percentage >= 80) return 'good';
    return 'average';
  }

  formatDate(dateString: string | undefined): string {
    if (!dateString) return '';
    return new Date(dateString).toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric',
    });
  }

  messageInstructor(): void {
    this.router.navigate(['/learning/chat']);
  }
}
