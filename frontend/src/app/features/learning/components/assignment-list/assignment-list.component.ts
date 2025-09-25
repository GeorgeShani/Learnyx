import { CommonModule } from '@angular/common';
import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AssignmentService } from '@shared/services/assignment.service';
import {
  AssignmentDTO,
  SubmissionDTO,
  SubmitAssignmentRequest,
  AssignmentStatus,
  SubmissionType,
} from '@shared/models/assignments.model';

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

@Component({
  selector: 'app-assignment-list',
  imports: [CommonModule, FormsModule],
  templateUrl: './assignment-list.component.html',
  styleUrl: './assignment-list.component.scss',
})
export class AssignmentListComponent implements OnInit {
  @Input() assignments: Assignment[] = [];
  @Input() courseId: number | null = null;
  @Output() submitAssignment = new EventEmitter<{
    assignmentId: string;
    submission: any;
  }>();

  selectedAssignment: Assignment | null = null;
  submissionText = '';
  submissionFiles: File[] = [];
  showSubmissionDialog = false;
  showFeedbackDialog = false;
  showToast = false;
  toastMessage = '';
  toastTitle = '';
  isSubmitting = false;

  getStatusClass(status: Assignment['status']): string {
    return status;
  }

  formatDate(dateString: string | undefined): string {
    if (!dateString) return '';
    return new Date(dateString).toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric',
    });
  }

  getDaysUntilDue(dueDate: string): number {
    const due = new Date(dueDate);
    const now = new Date();
    const diffTime = due.getTime() - now.getTime();
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    return diffDays;
  }

  openSubmissionDialog(assignment: Assignment): void {
    this.selectedAssignment = assignment;
    this.showSubmissionDialog = true;
    this.submissionText = '';
    this.submissionFiles = [];
  }

  closeSubmissionDialog(): void {
    this.showSubmissionDialog = false;
    this.selectedAssignment = null;
    this.submissionText = '';
    this.submissionFiles = [];
  }

  openFeedbackDialog(assignment: Assignment): void {
    this.selectedAssignment = assignment;
    this.showFeedbackDialog = true;
  }

  closeFeedbackDialog(): void {
    this.showFeedbackDialog = false;
    this.selectedAssignment = null;
  }

  onFileSelected(event: Event): void {
    const target = event.target as HTMLInputElement;
    if (target.files && target.files.length > 0) {
      const files = Array.from(target.files);
      if (files.length > 10) {
        this.showToastMessage(
          'Too many files',
          'You can upload up to 10 files maximum.'
        );
        return;
      }
      this.submissionFiles = files;
    }
  }

  removeFile(index: number): void {
    this.submissionFiles = this.submissionFiles.filter((_, i) => i !== index);
  }

  showToastMessage(title: string, message: string): void {
    this.toastTitle = title;
    this.toastMessage = message;
    this.showToast = true;
    setTimeout(() => {
      this.showToast = false;
    }, 3000);
  }

  canSubmit(): boolean {
    if (!this.selectedAssignment) return false;

    const hasText = this.submissionText.trim().length > 0;
    const hasFiles = this.submissionFiles.length > 0;

    switch (this.selectedAssignment.submissionType) {
      case 'text':
        return hasText;
      case 'file':
        return hasFiles;
      case 'both':
        return hasText || hasFiles;
      default:
        return false;
    }
  }

  formatFileSize(bytes: number): string {
    return (bytes / 1024 / 1024).toFixed(2) + ' MB';
  }

  constructor(private assignmentService: AssignmentService) {}

  ngOnInit(): void {
    // Load assignments if courseId is provided
    if (this.courseId) {
      this.loadAssignments();
    }
  }

  loadAssignments(): void {
    if (!this.courseId) return;

    this.assignmentService.getCourseAssignments(this.courseId).subscribe({
      next: (assignments: AssignmentDTO[]) => {
        // Convert AssignmentDTO to local Assignment interface
        this.assignments = assignments.map(this.convertToLocalAssignment);
      },
      error: (error) => {
        console.error('Error loading assignments:', error);
        this.showToastMessage('Error', 'Failed to load assignments');
      },
    });
  }

  private convertToLocalAssignment(dto: AssignmentDTO): Assignment {
    return {
      id: dto.id.toString(),
      title: dto.title,
      description: dto.description,
      dueDate: dto.dueDate,
      maxPoints: dto.maxPoints,
      status: this.mapAssignmentStatus(dto),
      submissionType: this.mapSubmissionType(dto.submissionType),
      grade: dto.studentSubmission?.grade,
      feedback: dto.studentSubmission?.feedback,
      submittedAt: dto.studentSubmission?.submittedAt,
    };
  }

  private mapAssignmentStatus(dto: AssignmentDTO): Assignment['status'] {
    if (dto.studentSubmission) {
      switch (dto.studentSubmission.status) {
        case AssignmentStatus.Submitted:
          return 'submitted';
        case AssignmentStatus.Graded:
          return 'graded';
        case AssignmentStatus.Overdue:
          return 'overdue';
        default:
          return 'pending';
      }
    }
    return dto.isOverdue ? 'overdue' : 'pending';
  }

  private mapSubmissionType(
    type: SubmissionType
  ): Assignment['submissionType'] {
    switch (type) {
      case SubmissionType.Text:
        return 'text';
      case SubmissionType.File:
        return 'file';
      case SubmissionType.Both:
        return 'both';
      default:
        return 'text';
    }
  }

  submitAssignmentVoid(): void {
    if (!this.selectedAssignment || !this.canSubmit() || this.isSubmitting)
      return;

    this.isSubmitting = true;
    const assignmentId = parseInt(this.selectedAssignment.id);

    const request: SubmitAssignmentRequest = {
      assignmentId: assignmentId,
      textContent: this.submissionText.trim() || undefined,
    };

    this.assignmentService
      .submitAssignment(
        assignmentId,
        request,
        this.submissionFiles.length > 0 ? this.submissionFiles : undefined
      )
      .subscribe({
        next: (submission: SubmissionDTO) => {
          this.isSubmitting = false;

          // Update the assignment status locally
          const assignmentIndex = this.assignments.findIndex(
            (a) => a.id === this.selectedAssignment!.id
          );
          if (assignmentIndex !== -1) {
            this.assignments[assignmentIndex].status = 'submitted';
            this.assignments[assignmentIndex].submittedAt =
              submission.submittedAt;
          }

          this.showToastMessage(
            'Assignment Submitted!',
            `Your submission for "${
              this.selectedAssignment!.title
            }" has been received.`
          );

          // Emit the submission event for parent components
          this.submitAssignment.emit({
            assignmentId: this.selectedAssignment!.id,
            submission: {
              type: this.selectedAssignment!.submissionType,
              content: this.submissionText,
              files: this.submissionFiles,
              submittedAt: submission.submittedAt,
            },
          });

          this.closeSubmissionDialog();
        },
        error: (error) => {
          this.isSubmitting = false;
          console.error('Error submitting assignment:', error);
          this.showToastMessage(
            'Submission Failed',
            'There was an error submitting your assignment. Please try again.'
          );
        },
      });
  }
}
