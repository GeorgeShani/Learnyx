import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  ConversationDto,
  CreateConversationRequest,
  MessageContentDto,
  MessageDto,
  SendMessageRequest,
} from '@features/learning/models/messaging.model';
import { ApiService } from '@core/services/api.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ChatApiService {
  constructor(private apiService: ApiService) {}

  // Conversations
  getConversations(): Observable<ConversationDto[]> {
    return this.apiService.get<ConversationDto[]>(`/conversations`);
  }

  createConversation(
    request: CreateConversationRequest
  ): Observable<ConversationDto> {
    return this.apiService.post<ConversationDto>(`/conversations`, request);
  }

  getConversationInfo(conversationId: number): Observable<ConversationDto> {
    return this.apiService.get<ConversationDto>(
      `/conversations/${conversationId}/info`
    );
  }

  // Messages
  getConversationMessages(
    conversationId: number,
    page: number = 1,
    pageSize: number = 50
  ): Observable<MessageDto[]> {
    return this.apiService.get<MessageDto[]>(
      `/conversations/${conversationId}/messages`,
      new HttpParams()
        .set('page', page.toString())
        .set('pageSize', pageSize.toString())
    );
  }

  sendMessage(
    conversationId: number,
    request: SendMessageRequest
  ): Observable<MessageDto> {
    return this.apiService.post<MessageDto>(
      `/conversations/${conversationId}/messages`,
      request
    );
  }

  editMessage(messageId: number, textContent: string): Observable<MessageDto> {
    return this.apiService.put<MessageDto>(`/messages/${messageId}`, {
      textContent,
    });
  }

  deleteMessage(messageId: number): Observable<void> {
    return this.apiService.delete<void>(`/messages/${messageId}`);
  }

  markMessageAsRead(messageId: number): Observable<void> {
    return this.apiService.put<void>(`/messages/${messageId}/read`, {});
  }

  markAllMessagesAsRead(conversationId: number): Observable<void> {
    return this.apiService.put<void>(
      `/conversations/${conversationId}/messages/read-all`,
      {}
    );
  }

  // File upload
  uploadFile(file: File): Observable<MessageContentDto> {
    const formData = new FormData();
    formData.append('file', file);

    return this.apiService.post<MessageContentDto>(
      `/messages/upload`,
      formData
    );
  }

  // Search
  searchMessages(
    query: string,
    conversationId?: number
  ): Observable<MessageDto[]> {
    const params: HttpParams = new HttpParams();
    params.set('query', query);

    if (conversationId) {
      params.set('conversationId', conversationId.toString());
    }

    return this.apiService.get<MessageDto[]>(`/search`, params);
  }

  // Assistant
  triggerAssistantResponse(conversationId: number): Observable<any> {
    return this.apiService.post(
      `$/conversations/${conversationId}/assistant-message`,
      {}
    );
  }
}
