export enum ConversationType {
  UserToUser = 0,
  UserToAssistant = 1
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
  user1Id?: number;
  user2Id?: number;
  user1Name?: string;
  user2Name?: string;
  user1Avatar?: string;
  user2Avatar?: string;
  lastActivityAt: Date;
  isActive: boolean;
  lastMessage?: string;
  unreadCount: number;
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