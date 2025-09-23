import { CommonModule } from '@angular/common';
import {
  Component,
  Input,
  Output,
  EventEmitter,
  ViewChild,
  ElementRef,
  OnInit,
  OnDestroy,
} from '@angular/core';

interface CourseModule {
  id: string;
  title: string;
  description: string;
  duration: string;
  videoUrl: string;
  completed: boolean;
}

@Component({
  selector: 'app-video-player',
  imports: [CommonModule],
  templateUrl: './video-player.component.html',
  styleUrl: './video-player.component.scss',
})
export class VideoPlayerComponent implements OnInit, OnDestroy {
  @Input() module!: CourseModule;
  @Output() moduleComplete = new EventEmitter<string>();

  @ViewChild('videoElement') videoElement!: ElementRef<HTMLVideoElement>;

  // Video state
  isPlaying = false;
  currentTime = 0;
  duration = 0;
  volume = 75;
  isMuted = false;
  playbackRate = 1;
  progressPercentage = 0;

  // UI state
  showSettings = false;
  isInFullscreen = false;

  // Configuration
  playbackSpeeds = [0.5, 0.75, 1, 1.25, 1.5, 2];

  ngOnInit() {
    // Listen for fullscreen changes
    document.addEventListener(
      'fullscreenchange',
      this.onFullscreenChange.bind(this)
    );
  }

  ngOnDestroy() {
    document.removeEventListener(
      'fullscreenchange',
      this.onFullscreenChange.bind(this)
    );
  }

  // Video event handlers
  handleTimeUpdate(): void {
    if (this.videoElement?.nativeElement) {
      this.currentTime = this.videoElement.nativeElement.currentTime;
      this.updateProgressPercentage();

      // Mark as complete when 90% watched
      const video = this.videoElement.nativeElement;
      if (video.currentTime / video.duration > 0.9 && !this.module.completed) {
        this.moduleComplete.emit(this.module.id);
      }
    }
  }

  handleLoadedMetadata(): void {
    if (this.videoElement?.nativeElement) {
      this.duration = this.videoElement.nativeElement.duration;
      this.updateProgressPercentage();
    }
  }

  setIsPlaying(playing: boolean): void {
    this.isPlaying = playing;
  }

  onVideoEnded(): void {
    this.isPlaying = false;
    // Mark as complete when video ends
    if (!this.module.completed) {
      this.moduleComplete.emit(this.module.id);
    }
  }

  private updateProgressPercentage(): void {
    this.progressPercentage =
      this.duration > 0 ? (this.currentTime / this.duration) * 100 : 0;
  }

  // Video controls
  togglePlay(): void {
    if (this.videoElement?.nativeElement) {
      if (this.isPlaying) {
        this.videoElement.nativeElement.pause();
      } else {
        this.videoElement.nativeElement.play();
      }
    }
  }

  skipBackward(): void {
    if (this.videoElement?.nativeElement) {
      const newTime = Math.max(0, this.currentTime - 10);
      this.videoElement.nativeElement.currentTime = newTime;
      this.currentTime = newTime;
    }
  }

  skipForward(): void {
    if (this.videoElement?.nativeElement) {
      const newTime = Math.min(this.duration, this.currentTime + 10);
      this.videoElement.nativeElement.currentTime = newTime;
      this.currentTime = newTime;
    }
  }

  onProgressClick(event: MouseEvent): void {
    if (this.videoElement?.nativeElement) {
      const rect = (event.currentTarget as HTMLElement).getBoundingClientRect();
      const percent = (event.clientX - rect.left) / rect.width;
      const newTime = percent * this.duration;
      this.videoElement.nativeElement.currentTime = newTime;
      this.currentTime = newTime;
    }
  }

  handleVolumeChange(event: Event): void {
    const target = event.target as HTMLInputElement;
    const newVolume = parseInt(target.value);
    this.volume = newVolume;

    if (this.videoElement?.nativeElement) {
      this.videoElement.nativeElement.volume = newVolume / 100;
    }

    if (newVolume === 0) {
      this.isMuted = true;
    } else if (this.isMuted) {
      this.isMuted = false;
    }
  }

  toggleMute(): void {
    if (this.videoElement?.nativeElement) {
      const newMutedState = !this.isMuted;
      this.isMuted = newMutedState;
      this.videoElement.nativeElement.muted = newMutedState;
    }
  }

  toggleSettings(): void {
    this.showSettings = !this.showSettings;
  }

  setPlaybackSpeed(speed: number): void {
    this.playbackRate = speed;
    if (this.videoElement?.nativeElement) {
      this.videoElement.nativeElement.playbackRate = speed;
    }
    this.showSettings = false;
  }

  toggleFullscreen(): void {
    const video = this.videoElement.nativeElement;
    if (document.fullscreenElement) {
      document.exitFullscreen();
    } else {
      video.requestFullscreen();
    }
  }

  private onFullscreenChange(): void {
    this.isInFullscreen = !!document.fullscreenElement;
  }

  formatTime(time: number): string {
    if (isNaN(time)) return '0:00';
    const minutes = Math.floor(time / 60);
    const seconds = Math.floor(time % 60);
    return `${minutes}:${seconds.toString().padStart(2, '0')}`;
  }
}