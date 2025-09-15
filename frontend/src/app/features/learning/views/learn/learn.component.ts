import {
  Component,
  ElementRef,
  OnInit,
  ViewChild,
  OnDestroy,
} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

interface Resource {
  name: string;
  type: 'pdf' | 'zip' | 'link' | 'code';
  size: string;
  url?: string;
}

interface TranscriptItem {
  time: string;
  text: string;
}

interface ReplyType {
  id: number;
  user: string;
  text: string;
  timestamp: string;
  likes: number;
}

interface Question {
  id: number;
  user: string;
  question: string;
  answer?: string;
  timestamp: string;
  likes: number;
  replies?: ReplyType[];
}

interface Lesson {
  id: number;
  title: string;
  duration: string;
  completed: boolean;
  section: string;
  description: string;
  videoUrl: string;
  resources: Resource[];
  transcript: TranscriptItem[];
  questions: Question[];
}

interface Course {
  id: number;
  title: string;
  instructor: string;
  totalLessons: number;
  completedLessons: number;
  progress: number;
  curriculum: {
    section: string;
    lessons: Lesson[];
  }[];
}

@Component({
  selector: 'app-learn',
  imports: [CommonModule, FormsModule],
  templateUrl: './learn.component.html',
  styleUrl: './learn.component.scss',
})
export class LearnComponent {
  @ViewChild('videoElement') videoElement!: ElementRef<HTMLVideoElement>;

  // Route parameters
  courseId = '';
  lessonId = '';

  // Video state
  isPlaying = false;
  currentTime = 0;
  duration = 0;
  volume = 75;
  isMuted = false;
  playbackRate = 1;
  progressPercentage = 0;

  // UI state
  sidebarOpen = true;
  activeTab = 'overview';
  showSettings = false;
  isInFullscreen = false;
  replyingTo: number | null = null;

  // Form data
  newQuestion = '';
  replyText = '';

  // Data
  allLessons: Lesson[] = [];
  currentLesson: Lesson | null = null;
  currentLessonIndex = 0;
  course: Course = {} as Course;
  questions: Question[] = [];

  // Configuration
  playbackSpeeds = [0.5, 0.75, 1, 1.25, 1.5, 2];
  tabs = [
    { value: 'overview', label: 'Overview' },
    { value: 'resources', label: 'Resources' },
    { value: 'transcript', label: 'Transcript' },
    { value: 'qa', label: 'Q&A' },
  ];

  constructor(private route: ActivatedRoute, private router: Router) {
    this.initializeLessonsData();
  }

  ngOnInit() {
    // Get route parameters
    this.route.params.subscribe((params) => {
      this.courseId = params['courseId'] || '1';
      this.lessonId = params['lessonId'] || null;
      this.loadCurrentLesson();
    });

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

  private initializeLessonsData() {
    this.allLessons = [
      {
        id: 1,
        title: 'Welcome to the Course',
        duration: '5:30',
        completed: true,
        section: 'Getting Started',
        description:
          "Get introduced to the course structure and what you'll learn throughout this comprehensive web development journey.",
        videoUrl:
          'https://echowave-musics-storage-bucket.s3.eu-north-1.amazonaws.com/videos/JavaScript+tutorial+for+beginners+%F0%9F%8C%90+-+Bro+Code+(720p%2C+h264%2C+youtube).mp4',
        resources: [
          { name: 'Course Syllabus', type: 'pdf', size: '1.2 MB' },
          { name: 'Welcome Guide', type: 'pdf', size: '800 KB' },
        ],
        transcript: [
          {
            time: '00:15',
            text: "Welcome to the Complete Web Development Bootcamp! I'm excited to have you here.",
          },
          {
            time: '01:30',
            text: "In this course, we'll cover everything from HTML basics to advanced JavaScript concepts.",
          },
          {
            time: '03:45',
            text: "Let's start by understanding what web development is all about.",
          },
        ],
        questions: [
          {
            id: 1,
            user: 'Mike Johnson',
            question: 'What prerequisites do I need for this course?',
            answer:
              'No prerequisites needed! This course is designed for complete beginners.',
            timestamp: '2:15',
            likes: 15,
            replies: [
              {
                id: 1,
                user: 'Sarah Kim',
                text: "That's great! I was worried about not having experience.",
                timestamp: '2:20',
                likes: 3,
              },
            ],
          },
        ],
      },
      {
        id: 2,
        title: 'Setting Up Development Environment',
        duration: '15:45',
        completed: true,
        section: 'Getting Started',
        description:
          'Learn how to set up your development environment with VS Code, Node.js, and essential extensions.',
        videoUrl:
          'https://echowave-musics-storage-bucket.s3.eu-north-1.amazonaws.com/videos/JavaScript+tutorial+for+beginners+%F0%9F%8C%90+-+Bro+Code+(720p%2C+h264%2C+youtube).mp4',
        resources: [
          { name: 'VS Code Setup Guide', type: 'pdf', size: '2.1 MB' },
          { name: 'Essential Extensions List', type: 'pdf', size: '500 KB' },
          { name: 'Node.js Installation', type: 'link', size: 'External Link' },
        ],
        transcript: [
          {
            time: '00:30',
            text: "Let's start by downloading and installing Visual Studio Code, our main code editor.",
          },
          {
            time: '03:15',
            text: "Next, we'll install Node.js which will allow us to run JavaScript outside the browser.",
          },
          {
            time: '08:20',
            text: 'These extensions will make your coding experience much more productive.',
          },
        ],
        questions: [
          {
            id: 2,
            user: 'Alex Chen',
            question: 'Can I use a different code editor instead of VS Code?',
            answer:
              "While I recommend VS Code, you can use any editor you're comfortable with.",
            timestamp: '5:30',
            likes: 8,
          },
        ],
      },
      {
        id: 12,
        title: 'Introduction to JavaScript Functions',
        duration: '18:30',
        completed: false,
        section: 'JavaScript Essentials',
        description:
          'Learn how to create and use functions in JavaScript, including arrow functions, parameters, and return values.',
        videoUrl:
          'https://echowave-musics-storage-bucket.s3.eu-north-1.amazonaws.com/videos/JavaScript+tutorial+for+beginners+%F0%9F%8C%90+-+Bro+Code+(720p%2C+h264%2C+youtube).mp4',
        resources: [
          { name: 'Function Examples', type: 'zip', size: '856 KB' },
          { name: 'Practice Exercises', type: 'pdf', size: '1.2 MB' },
          {
            name: 'MDN Functions Reference',
            type: 'link',
            size: 'External Link',
          },
        ],
        transcript: [
          {
            time: '00:15',
            text: 'Welcome to this lesson on JavaScript functions. Functions are reusable blocks of code that perform specific tasks.',
          },
          {
            time: '01:30',
            text: 'Functions are one of the fundamental building blocks of JavaScript programming.',
          },
          {
            time: '03:45',
            text: "Let's start by creating our first function using the function declaration syntax.",
          },
        ],
        questions: [
          {
            id: 3,
            user: 'Alex Chen',
            question:
              "What's the difference between function declarations and function expressions?",
            answer:
              'Great question! Function declarations are hoisted to the top of their scope, while function expressions are not.',
            timestamp: '5:30',
            likes: 12,
            replies: [
              {
                id: 2,
                user: 'Emma Wilson',
                text: 'This explanation really helped me understand hoisting!',
                timestamp: '5:35',
                likes: 5,
              },
            ],
          },
        ],
      },
    ];

    this.course = {
      id: Number.parseInt(this.courseId || '1'),
      title: 'Complete Web Development Bootcamp',
      instructor: 'Sarah Johnson',
      totalLessons: 156,
      completedLessons: 47,
      progress: 30,
      curriculum: [
        { section: 'Getting Started', lessons: this.allLessons.slice(0, 2) },
        {
          section: 'JavaScript Essentials',
          lessons: this.allLessons.slice(2, 3),
        },
      ],
    };
  }

  private loadCurrentLesson() {
    if (this.lessonId) {
      this.currentLessonIndex = this.allLessons.findIndex(
        (lesson) => lesson.id === Number.parseInt(this.lessonId)
      );
    }

    if (this.currentLessonIndex >= 0) {
      this.currentLesson = this.allLessons[this.currentLessonIndex];
      this.questions = [...(this.currentLesson?.questions || [])];
    } else {
      this.currentLessonIndex = 0;
      this.currentLesson = this.allLessons[0];
      this.questions = [...(this.currentLesson?.questions || [])];
    }

    // Reset video state when lesson changes
    this.currentTime = 0;
    this.isPlaying = false;
  }

  // Video event handlers
  onTimeUpdate() {
    if (this.videoElement?.nativeElement) {
      this.currentTime = this.videoElement.nativeElement.currentTime;
      this.updateProgressPercentage();
    }
  }

  onLoadedMetadata() {
    if (this.videoElement?.nativeElement) {
      this.duration = this.videoElement.nativeElement.duration;
      this.updateProgressPercentage();
    }
  }

  onPlay() {
    this.isPlaying = true;
  }

  onPause() {
    this.isPlaying = false;
  }

  onVideoEnded() {
    this.isPlaying = false;
    // Auto-advance to next lesson after 2 seconds
    if (this.currentLessonIndex < this.allLessons.length - 1) {
      setTimeout(() => {
        this.goToNextLesson();
      }, 2000);
    }
  }

  private updateProgressPercentage() {
    this.progressPercentage =
      this.duration > 0 ? (this.currentTime / this.duration) * 100 : 0;
  }

  // Video controls
  togglePlayPause() {
    if (this.videoElement?.nativeElement) {
      if (this.isPlaying) {
        this.videoElement.nativeElement.pause();
      } else {
        this.videoElement.nativeElement.play();
      }
    }
  }

  skipBack() {
    if (this.videoElement?.nativeElement) {
      const newTime = Math.max(0, this.currentTime - 10);
      this.videoElement.nativeElement.currentTime = newTime;
      this.currentTime = newTime;
    }
  }

  skipForward() {
    if (this.videoElement?.nativeElement) {
      const newTime = Math.min(this.duration, this.currentTime + 10);
      this.videoElement.nativeElement.currentTime = newTime;
      this.currentTime = newTime;
    }
  }

  onProgressClick(event: MouseEvent) {
    if (this.videoElement?.nativeElement) {
      const rect = (event.currentTarget as HTMLElement).getBoundingClientRect();
      const percent = (event.clientX - rect.left) / rect.width;
      const newTime = percent * this.duration;
      this.videoElement.nativeElement.currentTime = newTime;
      this.currentTime = newTime;
    }
  }

  onVolumeChange(event: Event) {
    const target = event.target as HTMLInputElement;
    const newVolume = Number.parseInt(target.value);
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

  toggleMute() {
    if (this.videoElement?.nativeElement) {
      const newMutedState = !this.isMuted;
      this.isMuted = newMutedState;
      this.videoElement.nativeElement.muted = newMutedState;
    }
  }

  toggleSettings() {
    this.showSettings = !this.showSettings;
  }

  setPlaybackSpeed(speed: number) {
    this.playbackRate = speed;
    if (this.videoElement?.nativeElement) {
      this.videoElement.nativeElement.playbackRate = speed;
    }
    this.showSettings = false;
  }

  toggleFullscreen() {
    if (!document.fullscreenElement) {
      document.documentElement.requestFullscreen();
      this.isInFullscreen = true;
    } else {
      document.exitFullscreen();
      this.isInFullscreen = false;
    }
  }

  private onFullscreenChange() {
    // Handle fullscreen state changes if needed
  }

  // Navigation
  setSidebarOpen(open: boolean) {
    this.sidebarOpen = open;
  }

  navigateToDashboard() {
    this.router.navigate(['/dashboard/student']);
  }

  navigateToLesson(lessonId: number) {
    this.router.navigate(['/learning', this.courseId, lessonId]);
    this.currentTime = 0;
    this.isPlaying = false;
  }

  isCurrentLesson(sectionIndex: number, lessonIndex: number): boolean {
    const globalIndex =
      this.course.curriculum
        .slice(0, sectionIndex)
        .reduce((acc, s) => acc + s.lessons.length, 0) + lessonIndex;
    return globalIndex === this.currentLessonIndex;
  }

  goToPreviousLesson() {
    if (this.currentLessonIndex > 0) {
      const prevLesson = this.allLessons[this.currentLessonIndex - 1];
      this.navigateToLesson(prevLesson.id);
    }
  }

  goToNextLesson() {
    if (this.currentLessonIndex < this.allLessons.length - 1) {
      const nextLesson = this.allLessons[this.currentLessonIndex + 1];
      this.navigateToLesson(nextLesson.id);
    }
  }

  // Tabs
  setActiveTab(tab: string) {
    this.activeTab = tab;
  }

  // Resources
  downloadResource(resource: Resource) {
    console.log(`Downloading ${resource.name}...`);
    alert(`Downloaded ${resource.name}!`);
  }

  // Transcript
  jumpToTime(timeString: string) {
    const [minutes, seconds] = timeString.split(':').map(Number);
    const totalSeconds = minutes * 60 + seconds;

    if (this.videoElement?.nativeElement) {
      this.videoElement.nativeElement.currentTime = totalSeconds;
      this.currentTime = totalSeconds;
    }
  }

  // Q&A
  askQuestion() {
    if (this.newQuestion.trim()) {
      const question: Question = {
        id: this.questions.length + 1,
        user: 'You',
        question: this.newQuestion,
        timestamp: this.formatTime(this.currentTime),
        likes: 0,
        replies: [],
      };
      this.questions = [question, ...this.questions];
      this.newQuestion = '';
    }
  }

  likeQuestion(questionId: number) {
    this.questions = this.questions.map((q) =>
      q.id === questionId ? { ...q, likes: q.likes + 1 } : q
    );
  }

  likeReply(questionId: number, replyId: number) {
    this.questions = this.questions.map((q) =>
      q.id === questionId
        ? {
            ...q,
            replies: q.replies?.map((r) =>
              r.id === replyId ? { ...r, likes: r.likes + 1 } : r
            ),
          }
        : q
    );
  }

  toggleReply(questionId: number) {
    this.replyingTo = this.replyingTo === questionId ? null : questionId;
    this.replyText = '';
  }

  cancelReply() {
    this.replyingTo = null;
    this.replyText = '';
  }

  submitReply(questionId: number) {
    if (this.replyText.trim()) {
      const reply: ReplyType = {
        id: Date.now(),
        user: 'You',
        text: this.replyText,
        timestamp: this.formatTime(this.currentTime),
        likes: 0,
      };

      this.questions = this.questions.map((q) =>
        q.id === questionId
          ? { ...q, replies: [...(q.replies || []), reply] }
          : q
      );

      this.replyText = '';
      this.replyingTo = null;
    }
  }

  // Utility
  formatTime(seconds: number): string {
    if (isNaN(seconds)) return '0:00';
    const mins = Math.floor(seconds / 60);
    const secs = Math.floor(seconds % 60);
    return `${mins}:${secs.toString().padStart(2, '0')}`;
  }
}
