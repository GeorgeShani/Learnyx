import { Injectable } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { CreateUserRequest, EmailCheckResponse, UpdateUserRequest, User, UserRole } from '@shared/models/user.model';
import { BehaviorSubject, map, Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class UserApiService {
  private readonly endpoint = '/api/users';
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private apiService: ApiService) {}

  // 🔹 Get all users
  getAllUsers(): Observable<User[]> {
    return this.apiService.get<User[]>(this.endpoint);
  }

  // 🔹 Get user by ID
  getUserById(id: number): Observable<User> {
    return this.apiService.get<User>(`${this.endpoint}/${id}`);
  }

  // 🔹 Get user by email
  getUserByEmail(email: string): Observable<User> {
    return this.apiService.get<User>(
      `${this.endpoint}/email/${encodeURIComponent(email)}`
    );
  }

  // 🔹 Get users by role
  getUsersByRole(role: UserRole): Observable<User[]> {
    return this.apiService.get<User[]>(`${this.endpoint}/role/${role}`);
  }

  // 🔹 Create new user
  createUser(user: CreateUserRequest): Observable<User> {
    return this.apiService.post<User>(this.endpoint, user);
  }

  // 🔹 Update user
  updateUser(id: number, user: UpdateUserRequest): Observable<User> {
    return this.apiService.put<User>(`${this.endpoint}/${id}`, user);
  }

  // 🔹 Delete user
  deleteUser(id: number): Observable<void> {
    return this.apiService.delete<void>(`${this.endpoint}/${id}`);
  }

  // 🔹 Check if email exists
  checkEmailExists(email: string): Observable<boolean> {
    return this.apiService
      .get<EmailCheckResponse>(
        `${this.endpoint}/check-email/${encodeURIComponent(email)}`
      )
      .pipe(map((response) => response.exists));
  }

  // 🔹 OAuth specific methods
  getUserByGoogleId(googleId: string): Observable<User> {
    return this.apiService.get<User>(`${this.endpoint}/google/${googleId}`);
  }

  getUserByFacebookId(facebookId: string): Observable<User> {
    return this.apiService.get<User>(`${this.endpoint}/facebook/${facebookId}`);
  }

  // 🔹 Current user management
  setCurrentUser(user: User | null): void {
    this.currentUserSubject.next(user);
    if (user) {
      localStorage.setItem('currentUser', JSON.stringify(user));
    } else {
      localStorage.removeItem('currentUser');
    }
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  loadCurrentUserFromStorage(): void {
    const stored = localStorage.getItem('currentUser');
    if (stored) {
      try {
        const user = JSON.parse(stored) as User;
        this.currentUserSubject.next(user);
      } catch (error) {
        console.error('Error parsing stored user:', error);
        localStorage.removeItem('currentUser');
      }
    }
  }

  // 🔹 Helper methods
  getUserFullName(user: User): string {
    return `${user.firstName} ${user.lastName}`.trim();
  }

  getUserRoleName(role: UserRole): string {
    switch (role) {
      case UserRole.Admin:
        return 'Administrator';
      case UserRole.Teacher:
        return 'Teacher';
      case UserRole.Student:
        return 'Student';
      default:
        return 'Unknown';
    }
  }

  isAdmin(user: User): boolean {
    return user.role === UserRole.Admin;
  }

  isTeacher(user: User): boolean {
    return user.role === UserRole.Teacher;
  }

  isStudent(user: User): boolean {
    return user.role === UserRole.Student;
  }

  // 🔹 Profile management methods
  updateCurrentUserProfile(
    updates: Partial<UpdateUserRequest>
  ): Observable<User> {
    const currentUser = this.getCurrentUser();
    if (!currentUser) {
      throw new Error('No current user found');
    }

    const updateRequest: UpdateUserRequest = {
      firstName: updates.firstName || currentUser.firstName,
      lastName: updates.lastName || currentUser.lastName,
      email: updates.email || currentUser.email,
      role: updates.role || currentUser.role,
      avatar:
        updates.avatar !== undefined ? updates.avatar : currentUser.avatar,
      authProvider: updates.authProvider || currentUser.authProvider,
      googleId:
        updates.googleId !== undefined
          ? updates.googleId
          : currentUser.googleId,
      facebookId:
        updates.facebookId !== undefined
          ? updates.facebookId
          : currentUser.facebookId,
    };

    if (updates.password) {
      updateRequest.password = updates.password;
    }

    return this.updateUser(currentUser.id, updateRequest).pipe(
      tap((updatedUser) => this.setCurrentUser(updatedUser))
    );
  }

  uploadAvatar(id: number, avatarFile: File): Observable<string> {
    const formData = new FormData();
    formData.append('avatar', avatarFile);

    return this.apiService
      .post<{ avatarUrl: string }>(`${this.endpoint}/${id}/avatar`, formData)
      .pipe(
        map((response) => response.avatarUrl)
      );
  }
}
