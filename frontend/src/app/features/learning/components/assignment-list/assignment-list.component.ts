import { CommonModule } from '@angular/common';
import { Component, Input, Output, EventEmitter } from '@angular/core';
import { FormsModule } from '@angular/forms';

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
export class AssignmentListComponent {
  @Input() assignments: Assignment[] = [];
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

  submitAssignmentVoid(): void {
    if (!this.selectedAssignment || !this.canSubmit()) return;

    const submission = {
      type: this.selectedAssignment.submissionType,
      content: this.submissionText,
      files: this.submissionFiles,
      submittedAt: new Date().toISOString(),
    };

    this.submitAssignment.emit({
      assignmentId: this.selectedAssignment.id,
      submission: submission,
    });

    this.showToastMessage(
      'Assignment Submitted!',
      `Your submission for "${this.selectedAssignment.title}" has been received.`
    );

    this.closeSubmissionDialog();
  }

  formatFileSize(bytes: number): string {
    return (bytes / 1024 / 1024).toFixed(2) + ' MB';
  }
}
