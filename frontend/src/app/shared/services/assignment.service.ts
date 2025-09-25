import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '@core/services/api.service';
import {
  AssignmentDTO,
  SubmissionDTO,
  StudentAssignmentSummaryDTO,
  CreateAssignmentRequest,
  SubmitAssignmentRequest,
  GradeSubmissionRequest,
  ApiResponse,
} from '../models/assignments.model';

@Injectable({
  providedIn: 'root',
})
export class AssignmentService {
  constructor(private apiService: ApiService) {}

  createAssignment(
    courseId: number,
    request: CreateAssignmentRequest
  ): Observable<AssignmentDTO> {
    return this.apiService.post<AssignmentDTO>(
      `/api/assignments/courses/${courseId}`,
      request
    );
  }

  updateAssignment(
    assignmentId: number,
    request: CreateAssignmentRequest
  ): Observable<AssignmentDTO> {
    return this.apiService.put<AssignmentDTO>(
      `/api/assignments/${assignmentId}`,
      request
    );
  }

  deleteAssignment(assignmentId: number): Observable<{ message: string }> {
    return this.apiService.delete<{ message: string }>(
      `/api/assignments/${assignmentId}`
    );
  }

  getAssignment(assignmentId: number): Observable<AssignmentDTO> {
    return this.apiService.get<AssignmentDTO>(
      `/api/assignments/${assignmentId}`
    );
  }

  getCourseAssignments(courseId: number): Observable<AssignmentDTO[]> {
    return this.apiService.get<AssignmentDTO[]>(
      `/api/assignments/courses/${courseId}`
    );
  }

  submitAssignment(
    assignmentId: number,
    request: SubmitAssignmentRequest,
    files?: File[]
  ): Observable<SubmissionDTO> {
    const formData = new FormData();

    // Add text content if provided
    if (request.textContent) {
      formData.append('textContent', request.textContent);
    }

    // Add files if provided
    if (files && files.length > 0) {
      files.forEach((file) => {
        formData.append('files', file);
      });
    }

    return this.apiService.post<SubmissionDTO>(
      `/api/assignments/${assignmentId}/submit`,
      formData
    );
  }

  updateSubmission(
    submissionId: number,
    request: SubmitAssignmentRequest,
    files?: File[]
  ): Observable<SubmissionDTO> {
    const formData = new FormData();

    // Add text content if provided
    if (request.textContent) {
      formData.append('textContent', request.textContent);
    }

    // Add files if provided
    if (files && files.length > 0) {
      files.forEach((file) => {
        formData.append('files', file);
      });
    }

    return this.apiService.put<SubmissionDTO>(
      `/api/assignments/submissions/${submissionId}`,
      formData
    );
  }

  gradeSubmission(
    submissionId: number,
    request: GradeSubmissionRequest
  ): Observable<{ message: string }> {
    return this.apiService.post<{ message: string }>(
      `/api/assignments/submissions/${submissionId}/grade`,
      request
    );
  }

  getAssignmentSubmissions(assignmentId: number): Observable<SubmissionDTO[]> {
    return this.apiService.get<SubmissionDTO[]>(
      `/api/assignments/${assignmentId}/submissions`
    );
  }

  getMySubmission(assignmentId: number): Observable<SubmissionDTO> {
    return this.apiService.get<SubmissionDTO>(
      `/api/assignments/${assignmentId}/submissions/my`
    );
  }

  getStudentSummary(courseId: number): Observable<StudentAssignmentSummaryDTO> {
    return this.apiService.get<StudentAssignmentSummaryDTO>(
      `/api/assignments/courses/${courseId}/summary`
    );
  }

  downloadAssignmentResource(
    resourceId: number
  ): Observable<{ downloadUrl: string }> {
    return this.apiService.get<{ downloadUrl: string }>(
      `/api/assignments/resources/${resourceId}/download`
    );
  }

  downloadSubmissionFile(fileId: number): Observable<{ downloadUrl: string }> {
    return this.apiService.get<{ downloadUrl: string }>(
      `/api/assignments/submissions/files/${fileId}/download`
    );
  }

  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 Bytes';

    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));

    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
  }

  isAssignmentOverdue(dueDate: string): boolean {
    return new Date(dueDate) < new Date();
  }

  getDaysUntilDue(dueDate: string): number {
    const due = new Date(dueDate);
    const now = new Date();
    const diffTime = due.getTime() - now.getTime();
    return Math.ceil(diffTime / (1000 * 60 * 60 * 24));
  }

  getStatusDisplay(status: number): string {
    const statusMap: { [key: number]: string } = {
      0: 'Pending',
      1: 'Submitted',
      2: 'Graded',
      3: 'Overdue',
    };
    return statusMap[status] || 'Unknown';
  }


  getSubmissionTypeDisplay(type: number): string {
    const typeMap: { [key: number]: string } = {
      0: 'Text Only',
      1: 'File Upload',
      2: 'Text and File',
    };
    return typeMap[type] || 'Unknown';
  }

  getResourceTypeDisplay(type: number): string {
    const typeMap: { [key: number]: string } = {
      0: 'PDF',
      1: 'ZIP',
      2: 'Link',
      3: 'Code',
      4: 'Video',
      5: 'Image',
      6: 'Document',
    };
    return typeMap[type] || 'Unknown';
  }
}
