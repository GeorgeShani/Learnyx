import { Component, type OnInit, ViewChild, type ElementRef, type AfterViewChecked } from "@angular/core";
import { MessagingService, Contact, Message, Conversation } from "../../services/messaging.service";
import { Router } from "@angular/router"
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";

@Component({
  selector: 'app-messaging',
  imports: [CommonModule, FormsModule],
  templateUrl: './messaging.component.html',
  styleUrl: './messaging.component.scss',
})
export class MessagingComponent {
  @ViewChild('messagesContainer') messagesContainer!: ElementRef;

  isAuthenticated = true; // This should come from your auth service
  activeTab: 'instructors' | 'students' | 'ai' = 'instructors';
  searchQuery = '';
  selectedContact: Contact | null = null;
  selectedConversation: Conversation | null = null;
  messages: Message[] = [];
  newMessage = '';

  contacts: Contact[] = [];
  conversations: Conversation[] = [];

  private shouldScrollToBottom = false;

  constructor(
    private router: Router,
    private messagingService: MessagingService
  ) {}

  ngOnInit() {
    this.loadData();
  }

  ngAfterViewChecked() {
    if (this.shouldScrollToBottom) {
      this.scrollToBottom();
      this.shouldScrollToBottom = false;
    }
  }

  loadData() {
    this.contacts = this.messagingService.getContacts();
    this.conversations = this.messagingService.getConversations();
  }

  navigateToLogin() {
    this.router.navigate(['/auth/login']);
  }

  setActiveTab(tab: 'instructors' | 'students' | 'ai') {
    this.activeTab = tab;
    this.selectedContact = null;
    this.selectedConversation = null;
    this.messages = [];
  }

  getFilteredContacts(): Contact[] {
    const roleMap = {
      instructors: 'INSTRUCTOR' as const,
      students: 'STUDENT' as const,
      ai: 'AI' as const,
    };

    return this.contacts
      .filter((contact) => contact.role === roleMap[this.activeTab])
      .filter(
        (contact) =>
          contact.name.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
          (contact.expertise &&
            contact.expertise.some((skill: any) =>
              skill.toLowerCase().includes(this.searchQuery.toLowerCase())
            ))
      );
  }

  selectContact(contact: Contact) {
    this.selectedContact = contact;
    this.selectedConversation = this.getConversationForContact(contact) ?? null;

    if (this.selectedConversation) {
      this.messages = this.messagingService.getMessagesByConversation(
        this.selectedConversation.id
      );
    } else {
      this.messages = [];
    }

    this.shouldScrollToBottom = true;
  }

  getConversationForContact(contact: Contact): Conversation | undefined {
    return this.conversations.find((conv) =>
      conv.participants.includes(contact.id)
    );
  }

  getInitials(name: string): string {
    return name
      .split(' ')
      .map((n) => n[0])
      .join('')
      .toUpperCase();
  }

  getLastSeenText(contact: Contact): string {
    if (contact.isOnline) return 'Online';
    if (contact.lastSeen) {
      const minutes = Math.floor(
        (Date.now() - contact.lastSeen.getTime()) / (1000 * 60)
      );
      if (minutes < 60) return `${minutes}m ago`;
      const hours = Math.floor(minutes / 60);
      if (hours < 24) return `${hours}h ago`;
      return `${Math.floor(hours / 24)}d ago`;
    }
    return 'Offline';
  }

  formatTime(timestamp: Date): string {
    return timestamp.toLocaleTimeString([], {
      hour: '2-digit',
      minute: '2-digit',
    });
  }

  sendMessage() {
    if (!this.newMessage.trim() || !this.selectedContact) return;

    const message: Message = {
      id: `msg-${Date.now()}`,
      senderId: 'current-user',
      receiverId: this.selectedContact.id,
      content: this.newMessage,
      timestamp: new Date(),
      isRead: false,
      type: 'text',
    };

    this.messages.push(message);
    this.newMessage = '';
    this.shouldScrollToBottom = true;

    // Simulate AI response for AI assistant
    if (this.selectedContact.role === 'AI') {
      setTimeout(() => {
        const aiResponse: Message = {
          id: `ai-${Date.now()}`,
          senderId: this.selectedContact!.id,
          receiverId: 'current-user',
          content: this.getAIResponse(message.content),
          timestamp: new Date(),
          isRead: true,
          type: 'text',
        };
        this.messages.push(aiResponse);
        this.shouldScrollToBottom = true;
      }, 1000);
    }
  }

  private getAIResponse(userMessage: string): string {
    const responses = [
      "That's a great question! Based on your learning progress, I'd recommend focusing on practical projects to reinforce your understanding.",
      'I can help you with that! Have you checked out the recommended resources in your course materials?',
      "Excellent progress! To deepen your understanding, try building a small project using what you've learned.",
      'I understand your concern. Many students find this topic challenging. Would you like me to suggest some additional practice exercises?',
      "That's the right mindset for learning! Consistent practice and curiosity will take you far. What specific area would you like to explore next?",
    ];
    return responses[Math.floor(Math.random() * responses.length)];
  }

  private scrollToBottom() {
    if (this.messagesContainer) {
      const element = this.messagesContainer.nativeElement;
      element.scrollTop = element.scrollHeight;
    }
  }
}
