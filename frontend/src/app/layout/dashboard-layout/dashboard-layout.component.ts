import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { FooterComponent, HeaderComponent } from '@shared/components';

@Component({
  selector: 'app-dashboard-layout',
  imports: [HeaderComponent, RouterOutlet, FooterComponent],
  templateUrl: './dashboard-layout.component.html',
  styleUrl: './dashboard-layout.component.scss'
})
export class DashboardLayoutComponent {

}
