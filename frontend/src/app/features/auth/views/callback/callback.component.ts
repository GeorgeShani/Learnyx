import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TokenService } from '@core/services/token.service';

@Component({
  selector: 'app-callback',
  template: `<p>Finishing Authenticating...</p>`,
})
export class CallbackComponent implements OnInit {
  constructor(
    private route: ActivatedRoute,
    private tokenService: TokenService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      const token = params['token'];

      if (token) {
        this.tokenService.setToken(token);
        this.router.navigate(['/']);
      } else {
        console.error('❌ No token returned');
        this.router.navigate(['/auth/login']);
      }
    });
  }
}
