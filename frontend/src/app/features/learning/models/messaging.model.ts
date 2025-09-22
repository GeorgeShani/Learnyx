export enum ConversationType {
  UserToUser = "UserToUser",
  UserToAssistant = "UserToAssistant"
}

export enum MessageContentType {
  Text = 0,
  Image = 1,
  File = 2,
  System = 3
}

export enum MessageStatus {
  Sent = 0,
  Delivered = 1,
  Read = 2
}

export interface ConversationDto {
  id: number;
  type: ConversationType;
  lastActivityAt: Date;
  isActive: boolean;
  lastMessage?: string;
  unreadCount: number;
  otherUserId: number;
  otherUserName: string;
  otherUserAvatar?: string;
  otherUserRole?: string;
  isAssistantConversation: boolean;
}

export interface MessageContentDto {
  contentType: MessageContentType;
  textContent?: string;
  fileUrl?: string;
  fileName?: string;
  mimeType?: string;
  fileSize?: number;
  width?: number;
  height?: number;
  thumbnailUrl?: string;
  order: number;
}

export interface MessageDto {
  id: number;
  conversationId: number;
  senderId?: number;
  senderName?: string;
  senderAvatar?: string;
  senderRole?: string;
  isFromAssistant: boolean;
  textContent?: string;
  contents: MessageContentDto[];
  createdAt: Date;
  isEdited: boolean;
  editedAt?: Date;
}

export interface CreateConversationRequest {
  type: ConversationType;
  user2Id?: number;
}

export interface SendMessageRequest {
  textContent?: string;
  contents?: MessageContentDto[];
  replyToMessageId?: number;
}

export interface UserPresenceDto {
  userId: number;
  isOnline: boolean;
  lastSeen?: Date;
}