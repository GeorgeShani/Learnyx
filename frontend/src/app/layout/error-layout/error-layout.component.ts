import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { HeaderComponent, FooterComponent } from "@shared/components";

@Component({
  selector: 'app-error-layout',
  imports: [HeaderComponent, FooterComponent],
  templateUrl: './error-layout.component.html',
  styleUrl: './error-layout.component.scss',
})
export class ErrorLayoutComponent {
  constructor(private router: Router) {}

  navigateToHome(): void {
    this.router.navigate(['/']);
  }

  navigateToCourses(): void {
    this.router.navigate(['/courses']);
  }

  navigateToTeachers(): void {
    this.router.navigate(['/teachers']);
  }
}
