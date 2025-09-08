import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { NavigationEnd, Router, RouterModule } from '@angular/router';
import { filter } from 'rxjs';
import { NotificationCenterComponent } from "@shared/components/notification-center/notification-center.component";
import { FormsModule } from '@angular/forms';

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
  isSignedIn = true;
  currentRoute = '';
  role: Role = 'student';
  searchQuery: string = '';

  constructor(private router: Router) {
    // Listen to route changes to update active state
    this.router.events
      .pipe(filter((event) => event instanceof NavigationEnd))
      .subscribe((event: NavigationEnd) => {
        this.currentRoute = event.urlAfterRedirects;
      });

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
    return this.currentRoute === path;
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

  signOut(): void {}
}
