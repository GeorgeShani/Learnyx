// Enums
export enum SubmissionType {
  Text = 0,
  File = 1,
  Both = 2,
}

export enum AssignmentStatus {
  Pending = 0,
  Submitted = 1,
  Graded = 2,
  Overdue = 3,
}

export enum ResourceType {
  PDF = 0,
  ZIP = 1,
  Link = 2,
  Code = 3,
  Video = 4,
  Image = 5,
  Document = 6,
}

// DTOs
export interface AssignmentDTO {
  id: number;
  title: string;
  description: string;
  instructions: string;
  dueDate: string; // ISO string
  maxPoints: number;
  submissionType: SubmissionType;
  allowLateSubmission: boolean;
  latePenaltyPercentage: number;
  isVisible: boolean;
  order: number;
  lessonId?: number;
  lessonTitle?: string;
  resources: AssignmentResourceDTO[];
  studentSubmission?: SubmissionDTO;
  createdAt: string; // ISO string
  updatedAt: string; // ISO string

  // Computed properties
  isOverdue: boolean;
  daysUntilDue: number;
}

export interface SubmissionDTO {
  id: number;
  textContent?: string;
  status: AssignmentStatus;
  submittedAt?: string; // ISO string
  gradedAt?: string; // ISO string
  grade?: number;
  feedback?: string;
  isLate: boolean;
  daysLate: number;
  files: SubmissionFileDTO[];

  // Student info (for instructor view)
  studentId: number;
  studentName: string;
  studentEmail: string;

  // Assignment info
  assignmentId: number;
  assignmentTitle: string;
  maxPoints: number;

  // Computed properties
  gradePercentage?: number;
}

export interface AssignmentResourceDTO {
  id: number;
  name: string;
  type: ResourceType;
  downloadUrl?: string;
  externalUrl?: string;
  sizeDisplay: string;
  description?: string;
  order: number;
  isRequired: boolean;
}

export interface SubmissionFileDTO {
  id: number;
  originalFileName: string;
  mimeType: string;
  fileSize: number;
  description?: string;
  order: number;
  downloadUrl: string;
  fileSizeFormatted: string;
}

export interface StudentAssignmentSummaryDTO {
  courseId: number;
  courseTitle: string;
  totalAssignments: number;
  completedAssignments: number;
  pendingAssignments: number;
  gradedAssignments: number;
  averageGrade: number;
  upcomingAssignments: AssignmentDTO[];
  recentSubmissions: SubmissionDTO[];
}

// Request Models
export interface CreateAssignmentRequest {
  title: string;
  description: string;
  instructions: string;
  dueDate: string; // ISO string
  maxPoints: number;
  submissionType: SubmissionType;
  allowLateSubmission: boolean;
  latePenaltyPercentage: number;
  isVisible: boolean;
  order: number;
  lessonId?: number;
  resources: CreateAssignmentResourceRequest[];
}

export interface CreateAssignmentResourceRequest {
  name: string;
  type: ResourceType;
  externalUrl?: string;
  description?: string;
  order: number;
  isRequired: boolean;
}

export interface SubmitAssignmentRequest {
  assignmentId: number;
  textContent?: string;
}

export interface GradeSubmissionRequest {
  grade: number;
  feedback?: string;
}

// API Response Models
export interface ApiResponse<T> {
  data?: T;
  message?: string;
  errors?: string[];
  success: boolean;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
