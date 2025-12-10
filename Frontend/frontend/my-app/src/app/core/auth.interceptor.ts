import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = sessionStorage.getItem('loginToken');
  
  // Nur für API-Requests den Token hinzufügen
  if (token && req.url.includes('/api/')) {
    const clonedRequest = req.clone({
      setHeaders: {
        'Authorization': `Bearer ${token}`,
        'X-Login-Token': token
      }
    });
    return next(clonedRequest);
  }
  
  return next(req);
};
