import { jwtDecode } from 'jwt-decode';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { JwtPayload } from '@core/models/jwt-payload.model';

@Injectable({
  providedIn: 'root',
})
export class TokenService {
  private authState = new BehaviorSubject<boolean>(this.hasToken());
  authState$ = this.authState.asObservable();

  private hasToken(): boolean {
    return !!localStorage.getItem('access_token');
  }

  setToken(token: string): void {
    localStorage.setItem('access_token', token);
    this.authState.next(true);
  }

  logout(): void {
    localStorage.removeItem('access_token');
    this.authState.next(false);
  }

  isAuthenticated(): boolean {
    return this.hasToken();
  }

  getToken(): string | null {
    return localStorage.getItem('access_token');
  }

  isTokenExpired(): boolean {
    const token = this.getToken();
    if (!token) return true;

    const decoded = jwtDecode<JwtPayload>(token);
    const now = Math.floor(Date.now() / 1000);
    return decoded.exp < now;
  }

  getUserRole(): string | null {
    const token = this.getToken();
    if (!token) return null;

    const decoded = jwtDecode<Record<string, string>>(token);
    return decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || null;
  }

  getUserId(): number | null {
    const token = this.getToken();
    if (!token) return null;

    const decoded = jwtDecode<Record<string, number>>(token);
    return decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] || 0;
  }
}
