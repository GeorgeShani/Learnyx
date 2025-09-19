import { Injectable } from '@angular/core';

export interface Message {
  id: string;
  senderId: string;
  receiverId: string;
  content: string;
  timestamp: Date;
  isRead: boolean;
  type: 'text' | 'file' | 'image';
}

export interface Conversation {
  id: string;
  participants: string[];
  lastMessage: Message;
  unreadCount: number;
  type: 'instructor' | 'student' | 'ai';
}

export interface Contact {
  id: string;
  name: string;
  role: 'INSTRUCTOR' | 'STUDENT' | 'AI';
  avatar?: string | null;
  isOnline: boolean;
  lastSeen?: Date;
  expertise?: string[];
  courses?: string[];
}

@Injectable({
  providedIn: 'root',
})
export class MessagingService {
  private mockContacts: Contact[] = [
    {
      id: 'instructor-1',
      name: 'Dr. Sarah Johnson',
      role: 'INSTRUCTOR',
      avatar: null,
      isOnline: true,
      expertise: ['Web Development', 'React', 'JavaScript'],
      courses: ['Complete Web Development Bootcamp', 'Advanced React Patterns'],
    },
    {
      id: 'instructor-2',
      name: 'Prof. Michael Chen',
      role: 'INSTRUCTOR',
      avatar:
        'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=150&h=150&fit=crop&crop=face',
      isOnline: false,
      lastSeen: new Date(Date.now() - 30 * 60 * 1000),
      expertise: ['Data Science', 'Machine Learning', 'Python'],
      courses: ['Data Science Fundamentals', 'ML with Python'],
    },
    {
      id: 'instructor-3',
      name: 'Emily Rodriguez',
      role: 'INSTRUCTOR',
      avatar:
        'https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=150&h=150&fit=crop&crop=face',
      isOnline: true,
      expertise: ['UI/UX Design', 'Figma', 'Design Systems'],
      courses: ['UI/UX Design Masterclass', 'Design Systems 101'],
    },
    {
      id: 'student-1',
      name: 'Alex Thompson',
      role: 'STUDENT',
      avatar:
        'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=150&h=150&fit=crop&crop=face',
      isOnline: true,
      courses: ['Web Development', 'React Fundamentals'],
    },
    {
      id: 'student-2',
      name: 'Maria Garcia',
      role: 'STUDENT',
      avatar:
        'https://images.unsplash.com/photo-1487412720507-e7ab37603c6f?w=150&h=150&fit=crop&crop=face',
      isOnline: false,
      lastSeen: new Date(Date.now() - 2 * 60 * 60 * 1000),
      courses: ['Data Science', 'Python Programming'],
    },
    {
      id: 'student-3',
      name: 'David Kim',
      role: 'STUDENT',
      avatar:
        'https://images.unsplash.com/photo-1500648767791-00dcc994a43e?w=150&h=150&fit=crop&crop=face',
      isOnline: true,
      courses: ['UI/UX Design', 'Web Development'],
    },
    {
      id: 'ai-assistant',
      name: 'Learnyx AI Assistant',
      role: 'AI',
      avatar: '🤖',
      isOnline: true,
      expertise: ['Learning Support', 'Course Recommendations', 'Study Tips'],
    },
  ];

  private mockMessages: Message[] = [
    {
      id: 'msg-1',
      senderId: 'instructor-1',
      receiverId: 'current-user',
      content:
        'Hi! I saw you enrolled in my Web Development course. How are you finding the React modules?',
      timestamp: new Date(Date.now() - 30 * 60 * 1000),
      isRead: true,
      type: 'text',
    },
    {
      id: 'msg-2',
      senderId: 'current-user',
      receiverId: 'instructor-1',
      content:
        "Hello Dr. Johnson! The course is amazing. I'm currently on the useState hook lesson. Could you clarify the difference between functional and class components?",
      timestamp: new Date(Date.now() - 25 * 60 * 1000),
      isRead: true,
      type: 'text',
    },
    {
      id: 'msg-3',
      senderId: 'instructor-1',
      receiverId: 'current-user',
      content:
        "Great question! Functional components are the modern approach. They're simpler, more performant, and work better with hooks. I'll create a bonus video explaining this in detail.",
      timestamp: new Date(Date.now() - 20 * 60 * 1000),
      isRead: true,
      type: 'text',
    },
    {
      id: 'msg-4',
      senderId: 'student-1',
      receiverId: 'current-user',
      content:
        'Hey! Are you also taking the React course? Want to form a study group?',
      timestamp: new Date(Date.now() - 60 * 60 * 1000),
      isRead: false,
      type: 'text',
    },
    {
      id: 'msg-5',
      senderId: 'student-3',
      receiverId: 'current-user',
      content:
        'I found this great resource for UI/UX inspiration. Check it out: https://dribbble.com',
      timestamp: new Date(Date.now() - 120 * 60 * 1000),
      isRead: false,
      type: 'text',
    },
    {
      id: 'msg-6',
      senderId: 'ai-assistant',
      receiverId: 'current-user',
      content:
        "Welcome to Learnyx! I'm here to help you with your learning journey. You can ask me about courses, study tips, or any learning-related questions. How can I assist you today?",
      timestamp: new Date(Date.now() - 180 * 60 * 1000),
      isRead: true,
      type: 'text',
    },
  ];

  private mockConversations: Conversation[] = [
    {
      id: 'conv-1',
      participants: ['instructor-1', 'current-user'],
      lastMessage: this.mockMessages.find((m) => m.id === 'msg-3')!,
      unreadCount: 0,
      type: 'instructor',
    },
    {
      id: 'conv-2',
      participants: ['student-1', 'current-user'],
      lastMessage: this.mockMessages.find((m) => m.id === 'msg-4')!,
      unreadCount: 1,
      type: 'student',
    },
    {
      id: 'conv-3',
      participants: ['student-3', 'current-user'],
      lastMessage: this.mockMessages.find((m) => m.id === 'msg-5')!,
      unreadCount: 1,
      type: 'student',
    },
    {
      id: 'conv-4',
      participants: ['ai-assistant', 'current-user'],
      lastMessage: this.mockMessages.find((m) => m.id === 'msg-6')!,
      unreadCount: 0,
      type: 'ai',
    },
  ];

  getContacts(): Contact[] {
    return this.mockContacts;
  }

  getConversations(): Conversation[] {
    return this.mockConversations;
  }

  getContactById(id: string): Contact | undefined {
    return this.mockContacts.find((contact) => contact.id === id);
  }

  getMessagesByConversation(conversationId: string): Message[] {
    const conversation = this.mockConversations.find(
      (c) => c.id === conversationId
    );
    if (!conversation) return [];

    return this.mockMessages.filter(
      (message) =>
        conversation.participants.includes(message.senderId) &&
        conversation.participants.includes(message.receiverId)
    );
  }

  getConversationsByType(
    type: 'instructor' | 'student' | 'ai'
  ): Conversation[] {
    return this.mockConversations.filter((conv) => conv.type === type);
  }
}
