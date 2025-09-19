import { Injectable } from '@angular/core';
import { MessageContentDto, MessageDto } from '@core/models/messaging.model';
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

  public isConnected$ = this.isConnectedSubject.asObservable();
  public messageReceived$ = this.messageReceivedSubject.asObservable();
  public userTyping$ = this.userTypingSubject.asObservable();
  public assistantTyping$ = this.assistantTypingSubject.asObservable();

  constructor() {}

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

  private setupEventListeners(): void {
    if (!this.hubConnection) return;

    this.hubConnection.on('ReceiveMessage', (message: MessageDto) => {
      this.messageReceivedSubject.next(message);
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

    this.hubConnection.on('AssistantTyping', (isTyping: boolean) => {
      this.assistantTypingSubject.next(isTyping);
    });

    this.hubConnection.on(
      'MessageRead',
      (messageId: number, userId: number) => {
        // Handle message read status
        console.log(`Message ${messageId} read by user ${userId}`);
      }
    );

    this.hubConnection.on('Error', (error: string) => {
      console.error('SignalR Error:', error);
    });

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
