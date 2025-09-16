import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { IsActiveMatchOptions, NavigationEnd, Router, RouterModule } from '@angular/router';
import { NotificationCenterComponent } from "@shared/components/notification-center/notification-center.component";
import { TokenService } from '@core/services/token.service';
import { FormsModule } from '@angular/forms';
import { filter } from 'rxjs';

type Role = 'student' | 'teacher' | 'admin';

@Component({
  selector: 'app-header',
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    NotificationCenterComponent,
  ],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent {
  isMobileMenuOpen = false;
  isUserMenuOpen = false;
  currentRoute = '';
  searchQuery: string = '';

  isSignedIn: boolean;
  role: string;

  constructor(private router: Router, private tokenService: TokenService) {
    // Listen to route changes to update active state
    this.router.events
      .pipe(filter((event) => event instanceof NavigationEnd))
      .subscribe((event: NavigationEnd) => {
        this.currentRoute = event.urlAfterRedirects;
      });
    
    this.isSignedIn = this.tokenService.isAuthenticated();
    this.role = this.tokenService.getUserRole()!.toLocaleLowerCase();

    // Set initial route
    this.currentRoute = this.router.url;
  }

  onSearch() {
    if (this.searchQuery.trim()) {
      this.router.navigate(['/courses'], {
        queryParams: { query: this.searchQuery },
      });
    }
  }

  isActive(path: string): boolean {
    const matchOptions: IsActiveMatchOptions = {
      paths: 'exact',
      queryParams: 'ignored',
      matrixParams: 'ignored',
      fragment: 'ignored',
    };
    
    return this.router.isActive(path, matchOptions);
  }

  toggleMobileMenu(): void {
    this.isMobileMenuOpen = !this.isMobileMenuOpen;
  }

  toggleUserMenu(): void {
    this.isUserMenuOpen = !this.isUserMenuOpen;
  }

  closeMobileMenu(): void {
    this.isMobileMenuOpen = false;
  }

  navigate(path: string): void {
    this.router.navigate([path]);
  }

  signOut(): void {
    this.tokenService.logout();
    this.router.navigate(['/auth/login']);
  }
}
