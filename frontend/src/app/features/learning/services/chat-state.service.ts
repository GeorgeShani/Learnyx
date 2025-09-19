import { Injectable } from '@angular/core';
import {
  ConversationDto,
  MessageDto,
} from '@features/learning/models/messaging.model';
import { BehaviorSubject, combineLatest, map, Observable } from 'rxjs';

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

  public conversations$ = this.conversationsSubject.asObservable();
  public messages$ = this.messagesSubject.asObservable();
  public activeConversationId$ =
    this.activeConversationIdSubject.asObservable();
  public typingUsers$ = this.typingUsersSubject.asObservable();

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

  getConversations(): ConversationDto[] {
    return this.conversationsSubject.value;
  }

  getMessages(conversationId: number): MessageDto[] {
    return this.messagesSubject.value[conversationId] || [];
  }
}
