import { CommonModule } from '@angular/common';
import { Component, ElementRef, HostListener } from '@angular/core';

interface Notification {
  id: string;
  type: 'course' | 'achievement' | 'message' | 'system' | 'assignment';
  title: string;
  message: string;
  time: string;
  read: boolean;
  actionUrl?: string;
}

@Component({
  selector: 'app-notification-center',
  imports: [CommonModule],
  templateUrl: './notification-center.component.html',
  styleUrl: './notification-center.component.scss',
})
export class NotificationCenterComponent {
  isDropdownOpen = false;

  notifications: Notification[] = [
    {
      id: '1',
      type: 'course',
      title: 'New Course Enrolled',
      message: 'You have successfully enrolled in "Advanced React Development"',
      time: '5 minutes ago',
      read: false,
      actionUrl: '/courses/1',
    },
    {
      id: '2',
      type: 'achievement',
      title: 'Achievement Unlocked!',
      message: 'Congratulations! You earned the "Fast Learner" badge',
      time: '1 hour ago',
      read: false,
    },
    {
      id: '3',
      type: 'assignment',
      title: 'Assignment Due Soon',
      message: 'React Hooks project is due in 2 days',
      time: '3 hours ago',
      read: true,
      actionUrl: '/assignments/1',
    },
    {
      id: '4',
      type: 'message',
      title: 'New Message',
      message: 'Your instructor replied to your question',
      time: '1 day ago',
      read: true,
      actionUrl: '/messages',
    },
    {
      id: '5',
      type: 'system',
      title: 'System Maintenance',
      message: 'Scheduled maintenance will occur tonight at 2 AM EST',
      time: '2 days ago',
      read: true,
    },
  ];

  constructor(private elementRef: ElementRef) {}

  get unreadCount(): number {
    return this.notifications.filter((n) => !n.read).length;
  }

  toggleDropdown(): void {
    this.isDropdownOpen = !this.isDropdownOpen;
  }

  closeDropdown(): void {
    this.isDropdownOpen = false;
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: Event): void {
    if (!this.elementRef.nativeElement.contains(event.target)) {
      this.closeDropdown();
    }
  }

  markAsRead(id: string): void {
    this.notifications = this.notifications.map((notification) =>
      notification.id === id ? { ...notification, read: true } : notification
    );
  }

  markAllAsRead(): void {
    this.notifications = this.notifications.map((notification) => ({
      ...notification,
      read: true,
    }));
  }

  deleteNotification(event: Event, id: string): void {
    event.stopPropagation();
    this.notifications = this.notifications.filter((n) => n.id !== id);
  }

  handleNotificationClick(notification: Notification): void {
    if (!notification.read) {
      this.markAsRead(notification.id);
    }

    if (notification.actionUrl) {
      // Handle navigation - in a real app, you'd use Angular Router
      console.log('Navigate to:', notification.actionUrl);
    }
  }

  viewAllNotifications(): void {
    // Handle navigation to full notifications page
    console.log('View all notifications');
    this.closeDropdown();
  }
}
