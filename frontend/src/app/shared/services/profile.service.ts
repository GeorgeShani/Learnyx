import { Injectable } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { Profile } from '../models/profile.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ProfileService {
  constructor(private apiService: ApiService) {}

  getProfile(): Observable<Profile> {
    return this.apiService.get<Profile>('/api/users/profile');
  }

  updateProfile(profile: Profile): Observable<Profile> {
    return this.apiService.put<Profile>('/api/users/profile', profile);
  }

  changePassword(payload: {
    currentPassword: string;
    newPassword: string;
    confirmNewPassword: string;
  }): Observable<string> {
    return this.apiService.put<string>(`/api/users/profile/password`, payload);
  }

  updateProfilePicture(file: File): Observable<Profile> {
    const formData = new FormData();
    formData.append('profilePicture', file);
    return this.apiService.put<Profile>(`/api/users/profile/picture`, formData);
  }
}
