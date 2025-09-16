import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { TokenService } from '@core/services/token.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(TokenService);
  const router = inject(Router);

  if (authService.isAuthenticated()) {
    return true; // user is logged in, allow access
  } else {
    router.navigate(['/login'], {
      queryParams: { returnUrl: state.url },
    });
    
    return false; // block access
  }
};
