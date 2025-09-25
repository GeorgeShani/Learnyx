export interface CourseSearchResult {
  id: number;
  title: string;
  instructor: string;
  rating: number;
  reviewCount: number;
  students: number;
  price: number;
  originalPrice: number;
  duration: string;
  lessons: number;
  level: string;
  category: string;
  tags: string[];
  thumbnailUrl: string | null;
  isBestseller: boolean;
  updatedAt: string;
}

export interface SearchCoursesDTO {
  courses: CourseSearchResult[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
}

export interface SearchCoursesRequest {
  searchQuery?: string;
  categories: string[];
  level?: string; // enum on server
  priceRange?: 'free' | 'under-50' | '50-100' | 'over-100';
  sortBy?: 'newest' | 'rating' | 'price-low' | 'price-high' | 'popular';
  page: number;
  pageSize: number;
}

export interface CategoryDTO {
  id: string;
  name: string;
  count: number;
}

export interface CourseSettingsRequest {
  enableQA: boolean;
  enableReviews: boolean;
  enableDiscussions: boolean;
  autoApprove: boolean;
  issueCertificates: boolean;
  sendCompletionEmails: boolean;
}

export interface LessonRequest {
  title: string;
  type: string; // enum on server
  duration?: string;
  content?: string;
  isFree: boolean;
  order: number;
  resources?: string[];
}

export interface CourseSectionRequest {
  title: string;
  order: number;
  lessons: LessonRequest[];
}

export interface CreateCourseRequest {
  title: string;
  subtitle?: string;
  description?: string;
  category?: string;
  level?: string; // enum on server
  language: string;
  isFree: boolean;
  price: number;
  tags: string[];
  requirements: string[];
  learningOutcomes: string[];
  settings: CourseSettingsRequest;
  sections: CourseSectionRequest[];
}

export interface CourseDTO {
  id: number;
  title: string;
  status: string; // enum on server
  message: string;
  totalSections: number;
  totalLessons: number;
}

export interface CourseMediaUploadRequest {
  thumbnail?: File | null;
  previewVideo?: File | null;
}
