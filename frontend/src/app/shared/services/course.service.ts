import { Injectable } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiService } from '@core/services/api.service';
import {
  CategoryDTO,
  CourseDTO,
  CourseMediaUploadRequest,
  CreateCourseRequest,
  SearchCoursesDTO,
  SearchCoursesRequest,
} from '@shared/models/course.model';

@Injectable({
  providedIn: 'root',
})
export class CourseService {
  constructor(private api: ApiService) {}

  searchCourses(payload: SearchCoursesRequest): Observable<SearchCoursesDTO> {
    let params = new HttpParams()
      .set('page', payload.page)
      .set('pageSize', payload.pageSize);

    if (payload.searchQuery)
      params = params.set('searchQuery', payload.searchQuery);
    if (payload.level) params = params.set('level', payload.level);
    if (payload.priceRange)
      params = params.set('priceRange', payload.priceRange);
    if (payload.sortBy) params = params.set('sortBy', payload.sortBy);
    if (payload.categories && payload.categories.length) {
      payload.categories.forEach((c) => {
        params = params.append('categories', c);
      });
    }

    return this.api.get<SearchCoursesDTO>('/api/courses/search', params);
  }

  getCategories(): Observable<CategoryDTO[]> {
    return this.api.get<CategoryDTO[]>('/api/courses/categories');
  }

  createCourse(body: CreateCourseRequest): Observable<CourseDTO> {
    return this.api.post<CourseDTO>('/api/courses', body);
  }

  uploadCourseMedia(
    courseId: number,
    request: CourseMediaUploadRequest
  ): Observable<{ message: string }> {
    const formData = new FormData();
    if (request.thumbnail)
      formData.append('thumbnail', request.thumbnail, request.thumbnail.name);
    if (request.previewVideo)
      formData.append(
        'previewVideo',
        request.previewVideo,
        request.previewVideo.name
      );
    return this.api.post<{ message: string }>(
      `/api/courses/${courseId}/media`,
      formData
    );
  }

  publishCourse(courseId: number): Observable<{ message: string }> {
    return this.api.post<{ message: string }>(
      `/api/courses/${courseId}/publish`,
      {}
    );
  }
}
