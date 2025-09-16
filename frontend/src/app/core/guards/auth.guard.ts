import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { TokenService } from '@core/services/token.service';

export const authGuard: CanActivateFn = (route, state) => {
  const tokenService = inject(TokenService);
  const router = inject(Router);

  if (tokenService.isAuthenticated() && !tokenService.isTokenExpired()) {
    return true; // user is logged in, allow access
  }

  router.navigate(['/auth/login'], {
    queryParams: { returnUrl: state.url },
  });
    
  return false; // block access
};
