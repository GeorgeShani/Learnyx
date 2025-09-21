import {
  Component,
  ViewChild,
  ElementRef,
  OnInit,
  AfterViewChecked,
  OnDestroy,
} from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChatApiService } from '../../services/chat-api.service';
import { ChatStateService } from '../../services/chat-state.service';
import { SignalRService } from '@core/services/signalr.service';
import {
  ConversationDto,
  MessageDto,
  SendMessageRequest,
  CreateConversationRequest,
  ConversationType,
} from '../../models/messaging.model';
import { Subscription } from 'rxjs';
import { TokenService } from '@core/services/token.service';
import { MarkdownComponent } from "ngx-markdown";

@Component({
  selector: 'app-messaging',
  imports: [CommonModule, FormsModule, MarkdownComponent],
  templateUrl: './messaging.component.html',
  styleUrl: './messaging.component.scss',
})
export class MessagingComponent implements OnInit, AfterViewChecked, OnDestroy {
  @ViewChild('messagesContainer') messagesContainer!: ElementRef;

  activeTab: 'teachers' | 'students' | 'ai' = 'teachers';
  searchQuery = '';
  selectedContact: any = null;
  selectedConversation: ConversationDto | null = null;
  messages: MessageDto[] = [];
  newMessage = '';

  contacts: any[] = [];
  conversations: ConversationDto[] = [];
  subscriptions: Subscription[] = [];

  private shouldScrollToBottom = false;

  constructor(
    private router: Router,
    private chatApiService: ChatApiService,
    private chatStateService: ChatStateService,
    private signalRService: SignalRService,
    private tokenService: TokenService
  ) {}

  ngOnInit() {
    this.loadData();
    this.setupSubscriptions();
    this.signalRService.startConnection(this.tokenService.getToken() ?? '');
  }

  ngAfterViewChecked() {
    if (this.shouldScrollToBottom) {
      this.scrollToBottom();
      this.shouldScrollToBottom = false;
    }
  }

  ngOnDestroy() {
    this.subscriptions.forEach((sub) => sub.unsubscribe());
    this.signalRService.stopConnection();
  }

  loadData() {
    // Load conversations from API
    this.chatApiService.getConversations().subscribe({
      next: (conversations) => {
        this.conversations = conversations;
        this.chatStateService.setConversations(conversations);
        this.extractContactsFromConversations();
      },
      error: (error) => {
        console.error('Error loading conversations:', error);
      },
    });
  }

  setupSubscriptions() {
    // Subscribe to conversations updates
    const conversationsSub = this.chatStateService.conversations$.subscribe(
      (conversations) => {
        this.conversations = conversations;
        this.extractContactsFromConversations();
      }
    );

    // Subscribe to messages updates
    const messagesSub =
      this.chatStateService.activeConversationMessages$.subscribe(
        (messages) => {
          this.messages = messages;
          this.shouldScrollToBottom = true;
        }
      );

    // Subscribe to SignalR message received
    const signalRSub = this.signalRService.messageReceived$.subscribe(
      (message) => {
        if (message) {
          this.chatStateService.addMessage(message);
           this.shouldScrollToBottom = true;
        }
      }
    );

    // Subscribe to SignalR connection status
    const connectionSub = this.signalRService.isConnected$.subscribe(
      (isConnected) => {
        if (isConnected && this.selectedConversation) {
          this.signalRService.joinConversation(this.selectedConversation.id);
        }
      }
    );

    this.subscriptions.push(
      conversationsSub,
      messagesSub,
      signalRSub,
      connectionSub
    );
  }

  extractContactsFromConversations() {
    this.contacts = this.conversations.map((conv) => ({
      id: conv.otherUserId,
      name: conv.otherUserName || 'Unknown User',
      role: conv.isAssistantConversation ? 'AI' : this.determineUserRole(conv),
      avatar: conv.otherUserAvatar,
      isOnline: true, // This would need to be tracked separately
      conversation: conv,
    }));
  }

  private determineUserRole(conversation: ConversationDto): string {
    // For now, we'll determine role based on conversation type
    // This could be enhanced with actual user role data from the backend
    return conversation.type === ConversationType.UserToUser
      ? 'Teacher'
      : 'Student';
  }

  createNewConversation(
    userId: number,
    type: ConversationType = ConversationType.UserToUser
  ): void {
    const request: CreateConversationRequest = {
      type,
      user2Id: userId,
    };

    this.chatApiService.createConversation(request).subscribe({
      next: (conversation) => {
        this.chatStateService.addConversation(conversation);
        this.extractContactsFromConversations();
      },
      error: (error) => {
        console.error('Error creating conversation:', error);
      },
    });
  }

  navigateToLogin() {
    this.router.navigate(['/auth/login']);
  }

  setActiveTab(tab: 'teachers' | 'students' | 'ai') {
    this.activeTab = tab;
    this.selectedContact = null;
    this.selectedConversation = null;
    this.messages = [];
    this.chatStateService.setActiveConversation(null);
  }

  getFilteredContacts(): any[] {
    const roleMap: Record<string, string[]> = {
      teachers: ['Teacher'],
      students: ['Student'],
      ai: ['AI'],
    };

    return this.contacts
      .filter((contact) => roleMap[this.activeTab]?.includes(contact.role))
      .filter((contact) =>
        contact.name.toLowerCase().includes(this.searchQuery.toLowerCase())
      );
  }

  selectContact(contact: any) {
    this.selectedContact = contact;
    this.selectedConversation = contact.conversation;

    if (this.selectedConversation) {
      this.chatStateService.setActiveConversation(this.selectedConversation.id);

      // Join SignalR conversation
      this.signalRService.joinConversation(this.selectedConversation.id);

      this.chatApiService
        .getConversationMessages(this.selectedConversation.id)
        .subscribe({
          next: (messages) => {
            this.chatStateService.setMessages(
              this.selectedConversation?.id ?? 0,
              messages
            );
            this.shouldScrollToBottom = true;
          },
          error: (error) => {
            console.error('Error loading messages:', error);
          },
        });
      
      this.chatApiService.markAllMessagesAsRead(this.selectedConversation.id);
    } else {
      this.messages = [];
      this.chatStateService.setActiveConversation(null);
    }
  }

  getConversationForContact(contact: any): ConversationDto | undefined {
    return contact.conversation;
  }

  getInitials(name: string): string {
    return name
      .split(' ')
      .map((n) => n[0])
      .join('')
      .toUpperCase();
  }

  getLastSeenText(contact: any): string {
    if (contact.isOnline) return 'Online';
    return 'Offline';
  }

  formatTime(timestamp: Date): string {
    return new Date(timestamp).toLocaleTimeString([], {
      hour: '2-digit',
      minute: '2-digit',
    });
  }

  getMessageContent(message: MessageDto): string {
    if (message.textContent) {
      return message.textContent;
    }
    if (message.contents && message.contents.length > 0) {
      return message.contents[0].textContent || 'File attachment';
    }
    return '';
  }

  sendMessage() {
    if (!this.newMessage.trim() || !this.selectedConversation) return;

    const request: SendMessageRequest = {
      textContent: this.newMessage,
    };

    // Use SignalR to send message for real-time delivery
    this.signalRService
      .sendMessage(this.selectedConversation?.id ?? 0, this.newMessage, [])
      .then(() => {
        this.newMessage = '';
        this.shouldScrollToBottom = true;

        if (this.selectedConversation?.type === ConversationType.UserToAssistant) {
          this.chatApiService.triggerAssistantResponse(this.selectedConversation.id);
        }
      })
      .catch((error) => {
        console.error('Error sending message via SignalR:', error);
        // Fallback to API call
        this.chatApiService
          .sendMessage(this.selectedConversation?.id ?? 0, request)
          .subscribe({
            next: (message) => {
              this.chatStateService.addMessage(message);
              this.newMessage = '';
              this.shouldScrollToBottom = true;
            },
            error: (error) => {
              console.error('Error sending message:', error);
            },
          });
      });
  }

  getCurrentUserId(): number {
    return this.tokenService.getUserId() || 0;
  }

  // Check if message is sent by current user
  isSentMessage(message: MessageDto): boolean {
    const currentUserId = this.getCurrentUserId();
    return message.senderId === currentUserId && !message.isFromAssistant;
  }

  // Check if message is received (from another user or assistant)
  isReceivedMessage(message: MessageDto): boolean {
    const currentUserId = this.getCurrentUserId();
    return message.senderId !== currentUserId || message.isFromAssistant;
  }

  private scrollToBottom() {
    if (this.messagesContainer) {
      const element = this.messagesContainer.nativeElement;
      element.scrollTop = element.scrollHeight;
    }
  }
}
