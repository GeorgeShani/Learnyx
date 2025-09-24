import { Injectable } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { Observable } from 'rxjs';

export interface SendCodeRequest {
  email: string;
}

export interface VerifyCodeRequest {
  code: string;
}

export interface ResetPasswordRequest {
  resetToken: string;
  password: string;
  confirmPassword: string;
}

export interface ApiResponse {
  message: string;
  resetToken?: string;
}

@Injectable({
  providedIn: 'root'
})
export class ForgotPasswordService {
  constructor(private apiService: ApiService) { }
  
  sendVerificationCode(request: SendCodeRequest): Observable<ApiResponse> {
    return this.apiService.post<ApiResponse>('/api/auth/forgot-password/send-code', request);
  }

  verifyCode(request: VerifyCodeRequest): Observable<ApiResponse> { 
    return this.apiService.post<ApiResponse>('/api/auth/forgot-password/verify-code', request);
  }

  resetPassword(request: ResetPasswordRequest): Observable<ApiResponse> {
    return this.apiService.post<ApiResponse>('/api/auth/forgot-password/reset-password', request);
  }
}
