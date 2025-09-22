import { Injectable } from '@angular/core';
import {
  ConversationDto,
  MessageDto,
} from '@features/learning/models/messaging.model';
import { BehaviorSubject, combineLatest, map, Observable } from 'rxjs';

export interface UserPresence {
  userId: number;
  isOnline: boolean;
  lastSeen?: Date | string; // Allow both Date and string
}

@Injectable({
  providedIn: 'root',
})
export class ChatStateService {
  private conversationsSubject = new BehaviorSubject<ConversationDto[]>([]);
  private messagesSubject = new BehaviorSubject<{
    [conversationId: number]: MessageDto[];
  }>({});

  private activeConversationIdSubject = new BehaviorSubject<number | null>(
    null
  );
  private typingUsersSubject = new BehaviorSubject<{
    [conversationId: number]: string[];
  }>({});

  // User presence state
  private userPresenceSubject = new BehaviorSubject<{
    [userId: number]: UserPresence;
  }>({});

  public conversations$ = this.conversationsSubject.asObservable();
  public messages$ = this.messagesSubject.asObservable();
  public activeConversationId$ =
    this.activeConversationIdSubject.asObservable();
  public typingUsers$ = this.typingUsersSubject.asObservable();
  public userPresence$ = this.userPresenceSubject.asObservable();

  public activeConversationMessages$: Observable<MessageDto[]> = combineLatest([
    this.messages$,
    this.activeConversationId$,
  ]).pipe(
    map(([messages, activeId]) => (activeId ? messages[activeId] || [] : []))
  );

  // Conversations
  setConversations(conversations: ConversationDto[]): void {
    this.conversationsSubject.next(conversations);
  }

  addConversation(conversation: ConversationDto): void {
    const current = this.conversationsSubject.value;
    this.conversationsSubject.next([conversation, ...current]);
  }

  updateConversation(
    conversationId: number,
    updates: Partial<ConversationDto>
  ): void {
    const current = this.conversationsSubject.value;
    const index = current.findIndex((c) => c.id === conversationId);
    if (index !== -1) {
      current[index] = { ...current[index], ...updates };
      this.conversationsSubject.next([...current]);
    }
  }

  // Messages
  setMessages(conversationId: number, messages: MessageDto[]): void {
    const current = this.messagesSubject.value;
    current[conversationId] = messages;
    this.messagesSubject.next({ ...current });
  }

  addMessage(message: MessageDto): void {
    const current = this.messagesSubject.value;
    if (!current[message.conversationId]) {
      current[message.conversationId] = [];
    }
    current[message.conversationId].push(message);
    this.messagesSubject.next({ ...current });
  }

  updateMessage(messageId: number, updates: Partial<MessageDto>): void {
    const current = this.messagesSubject.value;
    for (const conversationId in current) {
      const messages = current[conversationId];
      const index = messages.findIndex((m) => m.id === messageId);
      if (index !== -1) {
        messages[index] = { ...messages[index], ...updates };
        this.messagesSubject.next({ ...current });
        break;
      }
    }
  }

  removeMessage(messageId: number): void {
    const current = this.messagesSubject.value;
    for (const conversationId in current) {
      const messages = current[conversationId];
      const index = messages.findIndex((m) => m.id === messageId);
      if (index !== -1) {
        messages.splice(index, 1);
        this.messagesSubject.next({ ...current });
        break;
      }
    }
  }

  // Active conversation
  setActiveConversation(conversationId: number | null): void {
    this.activeConversationIdSubject.next(conversationId);
  }

  // Typing indicators
  setUserTyping(conversationId: number, userName: string): void {
    const current = this.typingUsersSubject.value;
    if (!current[conversationId]) {
      current[conversationId] = [];
    }
    if (!current[conversationId].includes(userName)) {
      current[conversationId].push(userName);
      this.typingUsersSubject.next({ ...current });
    }
  }

  removeUserTyping(conversationId: number, userName: string): void {
    const current = this.typingUsersSubject.value;
    if (current[conversationId]) {
      current[conversationId] = current[conversationId].filter(
        (u: any) => u !== userName
      );
      this.typingUsersSubject.next({ ...current });
    }
  }

  // User presence methods
  setUserOnline(userId: number, lastSeen?: Date): void {
    const current = this.userPresenceSubject.value;
    current[userId] = {
      userId,
      isOnline: true,
      lastSeen,
    };
    this.userPresenceSubject.next({ ...current });
  }

  setUserOffline(userId: number, lastSeen: Date): void {
    const current = this.userPresenceSubject.value;
    current[userId] = {
      userId,
      isOnline: false,
      lastSeen,
    };
    this.userPresenceSubject.next({ ...current });
  }

  setOnlineUsers(userIds: number[]): void {
    const current = this.userPresenceSubject.value;

    // Mark all existing users as offline first
    Object.keys(current).forEach((userId) => {
      const id = parseInt(userId);
      if (!userIds.includes(id)) {
        current[id] = {
          ...current[id],
          isOnline: false,
          lastSeen: current[id].lastSeen || new Date(),
        };
      }
    });

    // Mark online users as online
    userIds.forEach((userId) => {
      current[userId] = {
        userId,
        isOnline: true,
        lastSeen: undefined, // Clear lastSeen for online users
      };
    });

    this.userPresenceSubject.next({ ...current });
  }

  isUserOnline(userId: number): boolean {
    return this.userPresenceSubject.value[userId]?.isOnline || false;
  }

  getUserPresence(userId: number): UserPresence | null {
    return this.userPresenceSubject.value[userId] || null;
  }

  getLastSeenText(userId: number): string {
    const presence = this.getUserPresence(userId);

    if (!presence) {
      return 'Unknown';
    }

    if (presence.isOnline) {
      return 'Online';
    }

    if (presence.lastSeen) {
      const now = new Date();
      // Convert to Date object if it's a string
      const lastSeen =
        presence.lastSeen instanceof Date
          ? presence.lastSeen
          : new Date(presence.lastSeen);
      const diffInMinutes = Math.floor(
        (now.getTime() - lastSeen.getTime()) / (1000 * 60)
      );

      if (diffInMinutes < 1) {
        return 'Just now';
      } else if (diffInMinutes < 60) {
        return `${diffInMinutes} min ago`;
      } else if (diffInMinutes < 1440) {
        // 24 hours
        const hours = Math.floor(diffInMinutes / 60);
        return `${hours} hour${hours > 1 ? 's' : ''} ago`;
      } else {
        const days = Math.floor(diffInMinutes / 1440);
        return `${days} day${days > 1 ? 's' : ''} ago`;
      }
    }

    return 'Offline';
  }

  // Existing getter methods
  getConversations(): ConversationDto[] {
    return this.conversationsSubject.value;
  }

  getMessages(conversationId: number): MessageDto[] {
    return this.messagesSubject.value[conversationId] || [];
  }
}
