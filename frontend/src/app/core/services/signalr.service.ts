import { Injectable } from '@angular/core';
import {
  MessageContentDto,
  MessageDto,
} from '@features/learning/models/messaging.model';
import { BehaviorSubject } from 'rxjs';
import * as signalR from '@microsoft/signalr';

@Injectable({
  providedIn: 'root',
})
export class SignalRService {
  private hubConnection?: signalR.HubConnection;
  private isConnectedSubject = new BehaviorSubject<boolean>(false);
  private messageReceivedSubject = new BehaviorSubject<MessageDto | null>(null);
  private assistantTypingSubject = new BehaviorSubject<boolean>(false);
  private userTypingSubject = new BehaviorSubject<{
    userId: number;
    userName: string;
  } | null>(null);
  private userJoinedSubject = new BehaviorSubject<{
    userId: number;
    userName: string;
  } | null>(null);
  private userLeftSubject = new BehaviorSubject<{
    userId: number;
    userName: string;
  } | null>(null);
  private messageDeletedSubject = new BehaviorSubject<number | null>(null);
  private messageEditedSubject = new BehaviorSubject<MessageDto | null>(null);
  private messageReadSubject = new BehaviorSubject<{
    messageId: number;
    userId: number;
  } | null>(null);
  private messagesMarkedAsReadSubject = new BehaviorSubject<number | null>(
    null
  );
  private errorSubject = new BehaviorSubject<string | null>(null);

  public isConnected$ = this.isConnectedSubject.asObservable();
  public messageReceived$ = this.messageReceivedSubject.asObservable();
  public userTyping$ = this.userTypingSubject.asObservable();
  public assistantTyping$ = this.assistantTypingSubject.asObservable();
  public userJoined$ = this.userJoinedSubject.asObservable();
  public userLeft$ = this.userLeftSubject.asObservable();
  public messageDeleted$ = this.messageDeletedSubject.asObservable();
  public messageEdited$ = this.messageEditedSubject.asObservable();
  public messageRead$ = this.messageReadSubject.asObservable();
  public messagesMarkedAsRead$ = this.messagesMarkedAsReadSubject.asObservable();
  public error$ = this.errorSubject.asObservable();

  public async startConnection(token: string): Promise<void> {
    if (this.hubConnection?.state === 'Connected') {
      return;
    }

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7188/chathub', {
        accessTokenFactory: () => token,
        skipNegotiation: true,
        transport: 1,
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();

    // Set up event listeners
    this.setupEventListeners();

    try {
      await this.hubConnection.start();
      this.isConnectedSubject.next(true);
      console.log('SignalR Connected');
    } catch (err) {
      console.error('SignalR Connection Error: ', err);
      this.isConnectedSubject.next(false);
    }
  }

  public async stopConnection(): Promise<void> {
    if (this.hubConnection) {
      await this.hubConnection.stop();
      this.isConnectedSubject.next(false);
    }
  }

  public async joinConversation(conversationId: number): Promise<void> {
    if (this.hubConnection) {
      await this.hubConnection.invoke('JoinConversation', conversationId);
    }
  }

  public async leaveConversation(conversationId: number): Promise<void> {
    if (this.hubConnection) {
      await this.hubConnection.invoke('LeaveConversation', conversationId);
    }
  }

  public async sendMessage(
    conversationId: number,
    textContent: string,
    contents: MessageContentDto[]
  ): Promise<void> {
    if (this.hubConnection) {
      await this.hubConnection.invoke(
        'SendMessage',
        conversationId,
        textContent,
        contents
      );
    }
  }

  public async markMessageAsRead(messageId: number): Promise<void> {
    if (this.hubConnection) {
      await this.hubConnection.invoke('MarkMessageAsRead', messageId);
    }
  }

  public async markAllMessagesAsRead(conversationId: number): Promise<void> {
    if (this.hubConnection) {
      await this.hubConnection.invoke('MarkAllMessagesAsRead', conversationId);
    }
  }

  public async startTyping(conversationId: number): Promise<void> {
    if (this.hubConnection) {
      await this.hubConnection.invoke('StartTyping', conversationId);
    }
  }

  public async stopTyping(conversationId: number): Promise<void> {
    if (this.hubConnection) {
      await this.hubConnection.invoke('StopTyping', conversationId);
    }
  }

  // Utility methods for clearing subjects
  public clearMessageEvents(): void {
    this.messageReceivedSubject.next(null);
    this.messageDeletedSubject.next(null);
    this.messageEditedSubject.next(null);
    this.messageReadSubject.next(null);
    this.messagesMarkedAsReadSubject.next(null);
  }

  public clearUserEvents(): void {
    this.userJoinedSubject.next(null);
    this.userLeftSubject.next(null);
    this.userTypingSubject.next(null);
  }

  public clearError(): void {
    this.errorSubject.next(null);
  }

  private setupEventListeners(): void {
    if (!this.hubConnection) return;

    // Message events
    this.hubConnection.on('ReceiveMessage', (message: MessageDto) => {
      this.messageReceivedSubject.next(message);
    });

    this.hubConnection.on('MessageDeleted', (messageId: number) => {
      this.messageDeletedSubject.next(messageId);
    });

    this.hubConnection.on('MessageEdited', (message: MessageDto) => {
      this.messageEditedSubject.next(message);
    });

    this.hubConnection.on(
      'MessageRead',
      (messageId: number, userId: number) => {
        this.messageReadSubject.next({ messageId, userId });
      }
    );

    this.hubConnection.on('MessagesMarkedAsRead', (userId: number) => {
      this.messagesMarkedAsReadSubject.next(userId);
    });

    // User events
    this.hubConnection.on('UserJoined', (userId: number, userName: string) => {
      this.userJoinedSubject.next({ userId, userName });
    });

    this.hubConnection.on('UserLeft', (userId: number, userName: string) => {
      this.userLeftSubject.next({ userId, userName });
    });

    this.hubConnection.on('UserTyping', (userId: number, userName: string) => {
      this.userTypingSubject.next({ userId, userName });
    });

    this.hubConnection.on(
      'UserStoppedTyping',
      (userId: number, userName: string) => {
        this.userTypingSubject.next(null);
      }
    );

    // Assistant events
    this.hubConnection.on('AssistantTyping', (isTyping: boolean) => {
      this.assistantTypingSubject.next(isTyping);
    });

    // Error handling
    this.hubConnection.on('Error', (error: string) => {
      console.error('SignalR Error:', error);
      this.errorSubject.next(error);
    });

    // Connection events
    this.hubConnection.onreconnected(() => {
      this.isConnectedSubject.next(true);
      console.log('SignalR Reconnected');
    });

    this.hubConnection.onreconnecting(() => {
      this.isConnectedSubject.next(false);
      console.log('SignalR Reconnecting...');
    });

    this.hubConnection.onclose(() => {
      this.isConnectedSubject.next(false);
      console.log('SignalR Disconnected');
    });
  }
}
