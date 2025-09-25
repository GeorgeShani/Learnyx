import { CommonModule, DatePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { Profile } from '@shared/models/profile.model';
import { ProfileService } from '@shared/services/profile.service';
import { AssignmentService } from '@shared/services/assignment.service';
import {
  AssignmentDTO,
  SubmissionDTO,
  GradeSubmissionRequest,
  AssignmentStatus,
} from '@shared/models/assignments.model';

interface Teacher {
  name: string;
  avatar?: string | null;
  title: string;
  rating: number;
  totalStudents: number;
  totalCourses: number;
  totalRevenue: number;
  monthlyRevenue: number;
  joinDate: string | null;
}

interface Course {
  id: number;
  title: string;
  status: string;
  students: number;
  rating: number;
  reviews: number;
  revenue: number;
  lastUpdated: string;
  image: string;
  lessons: number;
  duration: string;
}

interface Analytics {
  totalViews: number;
  courseCompletions: number;
  averageRating: number;
  responseRate: number;
  weeklyStats: Array<{
    day: string;
    students: number;
    revenue: number;
  }>;
}

interface Activity {
  type: string;
  message: string;
  time: string;
  icon: SafeHtml;
}

interface Task {
  task: string;
  priority: string;
  dueDate: string;
}

interface Tab {
  value: string;
  label: string;
}

interface Assignment {
  id: number;
  title: string;
  courseTitle: string;
  dueDate: string;
  totalSubmissions: number;
  gradedSubmissions: number;
  pendingSubmissions: number;
  status: 'active' | 'closed' | 'draft';
}

interface Submission {
  id: number;
  assignmentId: number;
  assignmentTitle: string;
  studentName: string;
  studentAvatar?: string;
  submittedAt: string;
  status: 'submitted' | 'graded' | 'late';
  grade?: number;
  maxGrade: number;
  submissionType: 'text' | 'file' | 'both';
  textContent?: string;
  files?: Array<{
    name: string;
    size: number;
    url: string;
  }>;
  feedback?: string;
}

@Component({
  selector: 'app-teacher-dashboard',
  imports: [CommonModule, FormsModule],
  providers: [DatePipe],
  templateUrl: './teacher-dashboard.component.html',
  styleUrl: './teacher-dashboard.component.scss',
})
export class TeacherDashboardComponent implements OnInit {
  activeTab = 'overview';

  tabs: Tab[] = [
    { value: 'overview', label: 'Overview' },
    { value: 'courses', label: 'My Courses' },
    { value: 'analytics', label: 'Analytics' },
    { value: 'students', label: 'Students' },
    { value: 'submissions', label: 'Submissions' },
  ];

  teacher!: Teacher;

  myCourses: Course[] = [
    {
      id: 1,
      title: 'Complete Web Development Bootcamp',
      status: 'published',
      students: 89234,
      rating: 4.9,
      reviews: 12543,
      revenue: 89340,
      lastUpdated: '2 weeks ago',
      image:
        'https://images.unsplash.com/photo-1461749280684-dccba630e2f6?w=300',
      lessons: 156,
      duration: '40 hours',
    },
    {
      id: 2,
      title: 'Advanced JavaScript Concepts',
      status: 'published',
      students: 45678,
      rating: 4.8,
      reviews: 8932,
      revenue: 45680,
      lastUpdated: '1 week ago',
      image:
        'https://images.unsplash.com/photo-1579468118864-1b9ea3c0db4a?w=300',
      lessons: 89,
      duration: '28 hours',
    },
    {
      id: 3,
      title: 'React Performance Optimization',
      status: 'draft',
      students: 0,
      rating: 0,
      reviews: 0,
      revenue: 0,
      lastUpdated: '3 days ago',
      image:
        'https://images.unsplash.com/photo-1633356122102-3fe601e05bd2?w=300',
      lessons: 45,
      duration: '15 hours',
    },
  ];

  analytics: Analytics = {
    totalViews: 234567,
    courseCompletions: 12543,
    averageRating: 4.85,
    responseRate: 98,
    weeklyStats: [
      { day: 'Mon', students: 120, revenue: 450 },
      { day: 'Tue', students: 98, revenue: 380 },
      { day: 'Wed', students: 145, revenue: 520 },
      { day: 'Thu', students: 187, revenue: 690 },
      { day: 'Fri', students: 210, revenue: 780 },
      { day: 'Sat', students: 156, revenue: 590 },
      { day: 'Sun', students: 134, revenue: 500 },
    ],
  };

  recentActivity: Activity[] = [];

  pendingTasks: Task[] = [
    {
      task: 'Respond to 8 student questions',
      priority: 'high',
      dueDate: 'Today',
    },
    {
      task: 'Update course materials for JavaScript course',
      priority: 'medium',
      dueDate: 'This week',
    },
    {
      task: 'Review and publish React course',
      priority: 'high',
      dueDate: 'In 3 days',
    },
    {
      task: 'Plan next course content',
      priority: 'low',
      dueDate: 'Next week',
    },
  ];

  assignments: Assignment[] = [
    {
      id: 1,
      title: 'JavaScript Fundamentals Quiz',
      courseTitle: 'Complete Web Development Bootcamp',
      dueDate: '2024-01-15',
      totalSubmissions: 45,
      gradedSubmissions: 32,
      pendingSubmissions: 13,
      status: 'active',
    },
    {
      id: 2,
      title: 'React Component Project',
      courseTitle: 'Advanced JavaScript Concepts',
      dueDate: '2024-01-20',
      totalSubmissions: 28,
      gradedSubmissions: 15,
      pendingSubmissions: 13,
      status: 'active',
    },
    {
      id: 3,
      title: 'Final Portfolio Website',
      courseTitle: 'Complete Web Development Bootcamp',
      dueDate: '2024-01-25',
      totalSubmissions: 12,
      gradedSubmissions: 0,
      pendingSubmissions: 12,
      status: 'active',
    },
  ];

  submissions: Submission[] = [
    {
      id: 1,
      assignmentId: 1,
      assignmentTitle: 'JavaScript Fundamentals Quiz',
      studentName: 'Alice Johnson',
      studentAvatar:
        'https://images.unsplash.com/photo-1494790108755-2616b612b786?w=150',
      submittedAt: '2024-01-14T10:30:00Z',
      status: 'submitted',
      maxGrade: 100,
      submissionType: 'text',
      textContent: 'Here are my answers to the JavaScript quiz...',
    },
    {
      id: 2,
      assignmentId: 2,
      assignmentTitle: 'React Component Project',
      studentName: 'Bob Smith',
      studentAvatar:
        'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=150',
      submittedAt: '2024-01-19T15:45:00Z',
      status: 'graded',
      grade: 85,
      maxGrade: 100,
      submissionType: 'both',
      textContent:
        'I built a todo list component with the following features...',
      files: [
        { name: 'TodoComponent.jsx', size: 2048, url: '#' },
        { name: 'styles.css', size: 1024, url: '#' },
      ],
      feedback:
        'Great work! The component structure is clean and well-organized.',
    },
    {
      id: 3,
      assignmentId: 3,
      assignmentTitle: 'Final Portfolio Website',
      studentName: 'Carol Davis',
      submittedAt: '2024-01-24T09:15:00Z',
      status: 'late',
      maxGrade: 100,
      submissionType: 'file',
      files: [{ name: 'portfolio.zip', size: 5120, url: '#' }],
    },
  ];

  selectedAssignment: Assignment | null = null;
  filteredSubmissions: Submission[] = [];
  selectedSubmission: Submission | null = null;
  showGradingModal = false;
  gradingForm = {
    grade: 0,
    feedback: '',
  };
  isGrading = false;
  realAssignments: AssignmentDTO[] = [];
  realSubmissions: SubmissionDTO[] = [];

  constructor(
    private router: Router,
    private sanitizer: DomSanitizer,
    private profileService: ProfileService,
    private datePipe: DatePipe,
    private assignmentService: AssignmentService
  ) {
    this.profileService.getProfile().subscribe({
      next: (profile: Profile) => {
        this.teacher = {
          name: `${profile.firstName} ${profile.lastName}`,
          avatar: profile.avatar ?? null,
          title: 'Senior Full-Stack Developer',
          rating: 4.9,
          totalStudents: 89234,
          totalCourses: 12,
          totalRevenue: 125000,
          monthlyRevenue: 12500,
          joinDate: this.datePipe.transform(profile.createdAt, 'MMMM yyyy'),
        };
      },
      error: (error) => {
        console.error('Error fetching the profile:', error);
      },
    });

    this.recentActivity = [
      {
        type: 'enrollment',
        message: '25 new students enrolled in Web Development Bootcamp',
        time: '2 hours ago',
        icon: this.sanitizer.bypassSecurityTrustHtml(
          '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="#22C55E" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-users-icon lucide-users"><path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2"/><path d="M16 3.128a4 4 0 0 1 0 7.744"/><path d="M22 21v-2a4 4 0 0 0-3-3.87"/><circle cx="9" cy="7" r="4"/></svg>'
        ),
      },
      {
        type: 'review',
        message: 'New 5-star review on Advanced JavaScript',
        time: '4 hours ago',
        icon: this.sanitizer.bypassSecurityTrustHtml(
          '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="#eab308" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-star-icon lucide-star"><path d="M11.525 2.295a.53.53 0 0 1 .95 0l2.31 4.679a2.123 2.123 0 0 0 1.595 1.16l5.166.756a.53.53 0 0 1 .294.904l-3.736 3.638a2.123 2.123 0 0 0-.611 1.878l.882 5.14a.53.53 0 0 1-.771.56l-4.618-2.428a2.122 2.122 0 0 0-1.973 0L6.396 21.01a.53.53 0 0 1-.77-.56l.881-5.139a2.122 2.122 0 0 0-.611-1.879L2.16 9.795a.53.53 0 0 1 .294-.906l5.165-.755a2.122 2.122 0 0 0 1.597-1.16z"/></svg>'
        ),
      },
      {
        type: 'question',
        message: '3 new Q&A questions need responses',
        time: '6 hours ago',
        icon: this.sanitizer.bypassSecurityTrustHtml(
          '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="#3b82f6" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-message-circle-icon lucide-message-circle"><path d="M2.992 16.342a2 2 0 0 1 .094 1.167l-1.065 3.29a1 1 0 0 0 1.236 1.168l3.413-.998a2 2 0 0 1 1.099.092 10 10 0 1 0-4.777-4.719"/></svg>'
        ),
      },
      {
        type: 'completion',
        message: '142 students completed React course this week',
        time: '1 day ago',
        icon: this.sanitizer.bypassSecurityTrustHtml(
          '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="#a855f7" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-circle-check-big-icon lucide-circle-check-big"><path d="M21.801 10A10 10 0 1 1 17 3.335"/><path d="m9 11 3 3L22 4"/></svg>'
        ),
      },
    ];
  }

  ngOnInit(): void {
    this.loadTeacherAssignments();
  }

  loadTeacherAssignments(): void {
    // Load assignments for all courses the teacher has
    // For now, we'll use the mock data but this could be enhanced to load real assignments
    this.myCourses.forEach((course) => {
      this.assignmentService.getCourseAssignments(course.id).subscribe({
        next: (assignments: AssignmentDTO[]) => {
          this.realAssignments.push(...assignments);
          // Load submissions for each assignment
          assignments.forEach((assignment) => {
            this.loadAssignmentSubmissions(assignment.id);
          });
        },
        error: (error) => {
          console.error(
            `Error loading assignments for course ${course.id}:`,
            error
          );
        },
      });
    });
  }

  loadAssignmentSubmissions(assignmentId: number): void {
    this.assignmentService.getAssignmentSubmissions(assignmentId).subscribe({
      next: (submissions: SubmissionDTO[]) => {
        this.realSubmissions.push(...submissions);
        // Update the mock assignments with real submission data
        this.updateAssignmentsWithRealData();
      },
      error: (error) => {
        console.error(
          `Error loading submissions for assignment ${assignmentId}:`,
          error
        );
      },
    });
  }

  updateAssignmentsWithRealData(): void {
    // Update the mock assignments with real submission data
    this.assignments = this.realAssignments.map((assignment) => {
      const assignmentSubmissions = this.realSubmissions.filter(
        (s) => s.assignmentId === assignment.id
      );
      const totalSubmissions = assignmentSubmissions.length;
      const gradedSubmissions = assignmentSubmissions.filter(
        (s) => s.status === AssignmentStatus.Graded
      ).length;
      const pendingSubmissions = assignmentSubmissions.filter(
        (s) => s.status === AssignmentStatus.Submitted
      ).length;

      return {
        id: assignment.id,
        title: assignment.title,
        courseTitle: this.getCourseTitle(assignment.id), // You might need to implement this
        dueDate: assignment.dueDate,
        totalSubmissions,
        gradedSubmissions,
        pendingSubmissions,
        status: assignment.isVisible
          ? 'active'
          : ('draft' as 'active' | 'closed' | 'draft'),
      };
    });
  }

  private getCourseTitle(assignmentId: number): string {
    // This is a simplified implementation - you might want to store course info differently
    const assignment = this.realAssignments.find((a) => a.id === assignmentId);
    return assignment ? `Course ${assignment.id}` : 'Unknown Course';
  }

  setActiveTab(tab: string): void {
    this.activeTab = tab;
  }

  getInitials(name: string): string {
    return name
      .split(' ')
      .map((n) => n[0])
      .join('');
  }

  getTopCourses(): Course[] {
    return this.myCourses.slice(0, 2);
  }

  getBarHeight(students: number): number {
    return (students / 250) * 100;
  }

  navigateToCreateCourse(): void {
    this.router.navigate(['/dashboard/teacher/create-course']);
  }

  editCourse(courseId: number): void {
    this.router.navigate(['/dashboard/teacher/courses', courseId]);
  }

  viewCourse(courseId: number): void {
    this.router.navigate(['/courses', courseId]);
  }

  selectAssignment(assignment: Assignment): void {
    this.selectedAssignment = assignment;
    // Filter real submissions for this assignment
    const realSubmissions = this.realSubmissions.filter(
      (s) => s.assignmentId === assignment.id
    );
    // Convert to local Submission interface
    this.filteredSubmissions = realSubmissions.map(
      this.convertToLocalSubmission
    );
  }

  viewSubmission(submission: Submission): void {
    this.selectedSubmission = submission;
  }

  openGradingModal(submission: Submission): void {
    this.selectedSubmission = submission;
    this.gradingForm.grade = submission.grade || 0;
    this.gradingForm.feedback = submission.feedback || '';
    this.showGradingModal = true;
  }

  closeGradingModal(): void {
    this.showGradingModal = false;
    this.selectedSubmission = null;
    this.gradingForm = { grade: 0, feedback: '' };
  }

  submitGrade(): void {
    if (!this.selectedSubmission || this.isGrading) return;

    this.isGrading = true;
    const request: GradeSubmissionRequest = {
      grade: this.gradingForm.grade,
      feedback: this.gradingForm.feedback.trim() || undefined,
    };

    this.assignmentService
      .gradeSubmission(this.selectedSubmission.id, request)
      .subscribe({
        next: (response) => {
          this.isGrading = false;

          // Update the submission locally
          this.selectedSubmission!.grade = this.gradingForm.grade;
          this.selectedSubmission!.feedback = this.gradingForm.feedback;
          this.selectedSubmission!.status = 'graded';

          // Update the real submission data
          const realSubmission = this.realSubmissions.find(
            (s) => s.id === this.selectedSubmission!.id
          );
          if (realSubmission) {
            realSubmission.grade = this.gradingForm.grade;
            realSubmission.feedback = this.gradingForm.feedback;
            realSubmission.status = AssignmentStatus.Graded;
          }

          // Update assignment stats
          if (this.selectedAssignment) {
            this.selectedAssignment.gradedSubmissions++;
            this.selectedAssignment.pendingSubmissions--;
          }

          this.closeGradingModal();
        },
        error: (error) => {
          this.isGrading = false;
          console.error('Error grading submission:', error);
          // You might want to show an error message to the user
        },
      });
  }

  getStatusColor(status: string): string {
    switch (status) {
      case 'submitted':
        return '#3b82f6';
      case 'graded':
        return '#22c55e';
      case 'late':
        return '#ef4444';
      default:
        return '#64748b';
    }
  }

  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return (
      Number.parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
    );
  }

  downloadFile(file: any): void {
    // Implementation for file download
    console.log('Downloading file:', file.name);
  }

  getTotalSubmissions(): number {
    return this.assignments.reduce((sum, a) => sum + a.totalSubmissions, 0);
  }

  getPendingSubmissions(): number {
    return this.assignments.reduce((sum, a) => sum + a.pendingSubmissions, 0);
  }

  getGradedSubmissions(): number {
    return this.assignments.reduce((sum, a) => sum + a.gradedSubmissions, 0);
  }

  private convertToLocalSubmission(dto: SubmissionDTO): Submission {
    return {
      id: dto.id,
      assignmentId: dto.assignmentId,
      assignmentTitle: dto.assignmentTitle,
      studentName: dto.studentName,
      studentAvatar: undefined, // You might want to add avatar support
      submittedAt: dto.submittedAt || '',
      status: this.mapSubmissionStatus(dto.status),
      grade: dto.grade,
      maxGrade: dto.maxPoints,
      submissionType: this.mapSubmissionType(dto),
      textContent: dto.textContent,
      files: dto.files.map((file) => ({
        name: file.originalFileName,
        size: file.fileSize,
        url: file.downloadUrl,
      })),
      feedback: dto.feedback,
    };
  }

  private mapSubmissionStatus(status: AssignmentStatus): Submission['status'] {
    switch (status) {
      case AssignmentStatus.Submitted:
        return 'submitted';
      case AssignmentStatus.Graded:
        return 'graded';
      case AssignmentStatus.Overdue:
        return 'late';
      default:
        return 'submitted';
    }
  }

  private mapSubmissionType(dto: SubmissionDTO): Submission['submissionType'] {
    // This is a simplified mapping - you might want to get this from the assignment
    if (dto.textContent && dto.files.length > 0) {
      return 'both';
    } else if (dto.files.length > 0) {
      return 'file';
    } else {
      return 'text';
    }
  }
}
