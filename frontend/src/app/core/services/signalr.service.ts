import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';

@Injectable({
  providedIn: 'root',
})
export class SignalRService {
  private hubConnection!: signalR.HubConnection;

  messages: { user: string; message: string }[] = [];
  onlineUsers: Set<string> = new Set();
  lastSeen: { [user: string]: string } = {};
  unreadCount: { [user: string]: number } = {};

  startConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7188/chathub')
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('SignalR connected'))
      .catch((err) => console.error('SignalR error: ' + err));

    // 1. Messages
    this.hubConnection.on('ReceiveMessage', (user: string, message: string) => {
      this.messages.push({ user, message });

      // increment unread if chat not active
      if (!this.isActiveChat(user)) {
        this.unreadCount[user] = (this.unreadCount[user] || 0) + 1;
      }
    });

    // 2. Online
    this.hubConnection.on('UserOnline', (user: string) => {
      this.onlineUsers.add(user);
      delete this.lastSeen[user]; // reset last seen
    });

    // 3. Offline
    this.hubConnection.on('UserOffline', (user: string, lastSeen: string) => {
      this.onlineUsers.delete(user);
      this.lastSeen[user] = lastSeen;
    });
  }

  sendMessage(user: string, message: string) {
    this.hubConnection
      .invoke('SendMessage', user, message)
      .catch((err) => console.error(err));
  }

  // Dummy method – replace with actual active chat state
  private isActiveChat(user: string): boolean {
    return false; // later tie this to your open chat
  }
}
