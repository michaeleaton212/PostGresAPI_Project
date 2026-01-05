import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';

export const authGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  
  const userId = sessionStorage.getItem('userId');
  const userName = sessionStorage.getItem('userName');
  const userEmail = sessionStorage.getItem('userEmail');
  
  if (userId && userName && userEmail) {
    return true;
  }
  
  console.log('User not authenticated, redirecting to login');
  router.navigate(['/login']);
  return false;
};
