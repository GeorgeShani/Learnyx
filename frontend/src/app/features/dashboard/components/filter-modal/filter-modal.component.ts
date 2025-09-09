// filter-modal.component.ts
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

export interface CourseFilters {
  status: 'all' | 'in-progress' | 'completed';
  category: string;
  instructor: string;
  rating: number;
  progress: {
    min: number;
    max: number;
  };
  sortBy: 'title' | 'progress' | 'rating' | 'date-enrolled' | 'date-completed';
  sortOrder: 'asc' | 'desc';
}

@Component({
  selector: 'app-filter-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: "./filter-modal.component.html",
  styleUrl: "./filter-modal.component.scss",
})
export class FilterModalComponent {
  @Input() isOpen = false;
  @Input() currentFilters: CourseFilters = this.getDefaultFilters();
  @Input() availableInstructors: string[] = [];

  @Output() filtersChanged = new EventEmitter<CourseFilters>();
  @Output() modalClosed = new EventEmitter<void>();

  tempFilters: CourseFilters = { ...this.currentFilters };

  ngOnChanges() {
    if (this.isOpen) {
      this.tempFilters = { ...this.currentFilters };
    }
  }

  private getDefaultFilters(): CourseFilters {
    return {
      status: 'all',
      category: '',
      instructor: '',
      rating: 0,
      progress: {
        min: 0,
        max: 100,
      },
      sortBy: 'title',
      sortOrder: 'asc',
    };
  }

  setRating(rating: number): void {
    this.tempFilters.rating = rating;
  }

  clearRating(): void {
    this.tempFilters.rating = 0;
  }

  resetFilters(): void {
    this.tempFilters = this.getDefaultFilters();
  }

  applyFilters(): void {
    this.filtersChanged.emit({ ...this.tempFilters });
    this.closeModal();
  }

  closeModal(): void {
    this.modalClosed.emit();
  }
}
