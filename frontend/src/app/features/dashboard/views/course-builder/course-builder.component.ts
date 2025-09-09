import { CommonModule } from '@angular/common';
import { Component, ElementRef, ViewChild } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  FormControl,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';

interface Lesson {
  id: string;
  title: string;
  type: 'video' | 'text' | 'quiz';
  duration: number;
  content: string;
  resources: string[];
}

interface Section {
  id: string;
  title: string;
  lessons: Lesson[];
}

interface CourseSettings {
  enableQA: boolean;
  enableReviews: boolean;
  enableDiscussions: boolean;
  autoApprove: boolean;
  issueCertificates: boolean;
  sendCompletionEmails: boolean;
}

@Component({
  selector: 'app-course-builder',
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './course-builder.component.html',
  styleUrl: './course-builder.component.scss',
})
export class CourseBuilderComponent {
  // UI State
  activeTab = 'basics';

  // Forms
  courseForm: FormGroup;
  priceControl: FormControl;

  // Course Data
  tags: string[] = [];
  requirements: string[] = [];
  learningOutcomes: string[] = [];
  sections: Section[] = [{ id: '1', title: 'Introduction', lessons: [] }];

  // Input Fields
  currentTag = '';
  currentRequirement = '';
  currentOutcome = '';

  // Settings
  isFree = false;
  settings: CourseSettings = {
    enableQA: false,
    enableReviews: true,
    enableDiscussions: false,
    autoApprove: true,
    issueCertificates: true,
    sendCompletionEmails: false,
  };

  // File Upload
  @ViewChild('thumbnailInput') thumbnailInput!: ElementRef<HTMLInputElement>;
  @ViewChild('videoInput') videoInput!: ElementRef<HTMLInputElement>;

  thumbnailFile: File | null = null;
  videoFile: File | null = null;
  thumbnailDragover = false;
  videoDragover = false;

  constructor(private formBuilder: FormBuilder, private router: Router) {
    // ===== INITIALIZATION =====
    this.priceControl = new FormControl(0);

    this.courseForm = this.formBuilder.group({
      title: ['', Validators.required],
      subtitle: [''],
      description: [''],
      category: [''],
      level: [''],
      language: ['English'],
    });
  }

  // ===== UI CONTROLS =====
  setActiveTab(tab: string): void {
    this.activeTab = tab;
  }

  // ===== TAG MANAGEMENT =====
  addTag(): void {
    const trimmedTag = this.currentTag.trim();
    if (trimmedTag && !this.tags.includes(trimmedTag)) {
      this.tags.push(trimmedTag);
      this.currentTag = '';
    }
  }

  removeTag(tagToRemove: string): void {
    this.tags = this.tags.filter((tag) => tag !== tagToRemove);
  }

  onTagKeyPress(event: KeyboardEvent): void {
    if (event.key === 'Enter') {
      event.preventDefault();
      this.addTag();
    }
  }

  // ===== REQUIREMENTS MANAGEMENT =====
  addRequirement(): void {
    const trimmedRequirement = this.currentRequirement.trim();
    if (trimmedRequirement) {
      this.requirements.push(trimmedRequirement);
      this.currentRequirement = '';
    }
  }

  onRequirementKeyPress(event: KeyboardEvent): void {
    if (event.key === 'Enter') {
      event.preventDefault();
      this.addRequirement();
    }
  }

  // ===== LEARNING OUTCOMES MANAGEMENT =====
  addOutcome(): void {
    const trimmedOutcome = this.currentOutcome.trim();
    if (trimmedOutcome) {
      this.learningOutcomes.push(trimmedOutcome);
      this.currentOutcome = '';
    }
  }

  onOutcomeKeyPress(event: KeyboardEvent): void {
    if (event.key === 'Enter') {
      event.preventDefault();
      this.addOutcome();
    }
  }

  // ===== SECTION MANAGEMENT =====
  addSection(): void {
    const newSection: Section = {
      id: Date.now().toString(),
      title: `Section ${this.sections.length + 1}`,
      lessons: [],
    };
    this.sections.push(newSection);
  }

  removeSection(sectionId: string): void {
    this.sections = this.sections.filter((section) => section.id !== sectionId);
  }

  // ===== LESSON MANAGEMENT =====
  addLesson(sectionId: string): void {
    const newLesson: Lesson = {
      id: Date.now().toString(),
      title: 'New Lesson',
      type: 'video',
      duration: 0,
      content: '',
      resources: [],
    };

    this.sections = this.sections.map((section) =>
      section.id === sectionId
        ? { ...section, lessons: [...section.lessons, newLesson] }
        : section
    );
  }

  removeLesson(sectionId: string, lessonId: string): void {
    this.sections = this.sections.map((section) =>
      section.id === sectionId
        ? {
            ...section,
            lessons: section.lessons.filter((lesson) => lesson.id !== lessonId),
          }
        : section
    );
  }

  // ===== FILE UPLOAD CONTROLS =====
  triggerThumbnailUpload(): void {
    this.thumbnailInput.nativeElement.click();
  }

  triggerVideoUpload(): void {
    this.videoInput.nativeElement.click();
  }

  onThumbnailSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.handleFileSelect(input.files[0], 'thumbnail');
    }
  }

  onVideoSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.handleFileSelect(input.files[0], 'video');
    }
  }

  removeThumbnail(): void {
    this.thumbnailFile = null;
    this.thumbnailInput.nativeElement.value = '';
  }

  removeVideo(): void {
    this.videoFile = null;
    this.videoInput.nativeElement.value = '';
  }

  // ===== DRAG & DROP HANDLERS =====
  onDragOver(event: DragEvent, type: 'thumbnail' | 'video'): void {
    event.preventDefault();
    this.setDragState(type, true);
  }

  onDragLeave(type: 'thumbnail' | 'video'): void {
    this.setDragState(type, false);
  }

  onDrop(event: DragEvent, type: 'thumbnail' | 'video'): void {
    event.preventDefault();
    this.setDragState(type, false);

    const files = event.dataTransfer?.files;
    if (files && files.length > 0) {
      this.handleFileSelect(files[0], type);
    }
  }

  // ===== FILE HANDLING =====
  private handleFileSelect(file: File, fileType: 'thumbnail' | 'video'): void {
    const isValidFile = this.validateFile(file, fileType);
    if (!isValidFile) return;

    if (fileType === 'thumbnail') {
      this.thumbnailFile = file;
    } else {
      this.videoFile = file;
    }
  }

  private validateFile(file: File, fileType: 'thumbnail' | 'video'): boolean {
    const isImage = file.type.startsWith('image/');
    const isVideo = file.type.startsWith('video/');

    if (fileType === 'thumbnail' && !isImage) {
      alert('Please select an image file for the thumbnail.');
      return false;
    }

    if (fileType === 'video' && !isVideo) {
      alert('Please select a video file for the preview.');
      return false;
    }

    return true;
  }

  private setDragState(type: 'thumbnail' | 'video', isDragging: boolean): void {
    if (type === 'thumbnail') {
      this.thumbnailDragover = isDragging;
    } else {
      this.videoDragover = isDragging;
    }
  }

  getFileSize(file: File): string {
    return (file.size / (1024 * 1024)).toFixed(2);
  }

  getUploadedFiles(): { thumbnail: File | null; video: File | null } {
    return {
      thumbnail: this.thumbnailFile,
      video: this.videoFile,
    };
  }

  // ===== COURSE SUBMISSION =====
  submitCourseData(): void {
    const files = this.getUploadedFiles();

    if (!this.validateUploadedFiles(files)) return;

    const formData = new FormData();
    formData.append('thumbnail', files.thumbnail!);
    formData.append('video', files.video!);

    this.onCourseDataSubmit(formData);
  }

  private validateUploadedFiles(files: {
    thumbnail: File | null;
    video: File | null;
  }): boolean {
    if (!files.thumbnail) {
      alert('Please upload a thumbnail image.');
      return false;
    }

    if (!files.video) {
      alert('Please upload a preview video.');
      return false;
    }

    return true;
  }

  private onCourseDataSubmit(formData: FormData): void {
    console.log('Course data ready for submission:', {
      thumbnailFile: this.thumbnailFile,
      videoFile: this.videoFile,
      formData,
    });

    alert('Files ready for upload! Check console for details.');
  }

  // ===== COURSE ACTIONS =====
  handleSaveDraft(): void {
    const courseData = this.buildCourseData();
    console.log('Saving draft...', {
      course: courseData,
      sections: this.sections,
    });
  }

  handlePreview(): void {
    this.router.navigate(['/courses/preview']);
  }

  handlePublish(): void {
    const courseData = this.buildCourseData();
    console.log('Publishing course...', {
      course: courseData,
      sections: this.sections,
    });
    this.router.navigate(['/dashboard/teacher']);
  }

  // ===== UTILITIES =====
  private buildCourseData() {
    return {
      ...this.courseForm.value,
      price: this.priceControl.value,
      tags: this.tags,
      requirements: this.requirements,
      learningOutcomes: this.learningOutcomes,
      isFree: this.isFree,
      settings: this.settings,
    };
  }
}
