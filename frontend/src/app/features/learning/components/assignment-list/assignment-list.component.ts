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
  submissionType: 'text' | 'file';
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
  submissionFile: File | null = null;
  showSubmissionDialog = false;
  showFeedbackDialog = false;

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
    this.submissionFile = null;
  }

  closeSubmissionDialog(): void {
    this.showSubmissionDialog = false;
    this.selectedAssignment = null;
    this.submissionText = '';
    this.submissionFile = null;
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
      this.submissionFile = target.files[0];
    }
  }

  submitAssignmentVoid(): void {
    if (!this.selectedAssignment) return;

    const submission = {
      type: this.selectedAssignment.submissionType,
      content: this.submissionText,
      file: this.submissionFile,
      submittedAt: new Date().toISOString(),
    };

    this.submitAssignment.emit({
      assignmentId: this.selectedAssignment.id,
      submission: submission,
    });

    this.closeSubmissionDialog();
  }
}
