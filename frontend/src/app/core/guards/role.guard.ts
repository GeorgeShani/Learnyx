import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { TokenService } from '@core/services/token.service';

export const roleGuard: CanActivateFn = (route, state) => {
  const tokenService = inject(TokenService);
  const router = inject(Router);

  const allowedRoles = route.data?.['roles'] as string[];
  const userRole = tokenService.getUserRole();

  if (userRole && allowedRoles?.includes(userRole)) {
    return true;
  }

  router.navigate(['/']); // redirect if not authorized
  return false;
};
