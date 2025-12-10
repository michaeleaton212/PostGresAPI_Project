import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';

export const authGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  
  const bookingIds = sessionStorage.getItem('bookingIds');
  const userName = sessionStorage.getItem('userName');
  const token = sessionStorage.getItem('loginToken');
  
  if (bookingIds && userName && token) {
    return true;
  }
  
  console.log('User not authenticated, redirecting to login');
  router.navigate(['/login']);
  return false;
};
