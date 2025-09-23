import { CommonModule } from '@angular/common';
import { Component, Input, Output, EventEmitter } from '@angular/core';

interface CourseModule {
  id: string;
  title: string;
  description: string;
  duration: string;
  videoUrl: string;
  completed: boolean;
}

@Component({
  selector: 'app-course-progress',
  imports: [CommonModule],
  templateUrl: './course-progress.component.html',
  styleUrl: './course-progress.component.scss',
})
export class CourseProgressComponent {
  @Input() modules: CourseModule[] = [];
  @Input() currentModuleId = '';
  @Input() progress = 0;
  @Output() moduleSelect = new EventEmitter<string>();

  getCompletedCount(): number {
    return this.modules.filter((m) => m.completed).length;
  }

  getRemainingCount(): number {
    return this.modules.length - this.getCompletedCount();
  }

  getTotalDuration(): string {
    const totalMinutes = this.modules.reduce((acc, module) => {
      const [minutes, seconds] = module.duration.split(':').map(Number);
      return acc + minutes + seconds / 60;
    }, 0);

    const hours = Math.floor(totalMinutes / 60);
    const mins = Math.round(totalMinutes % 60);
    return `${hours}h ${mins}m`;
  }

  selectModule(moduleId: string): void {
    this.moduleSelect.emit(moduleId);
  }
}
