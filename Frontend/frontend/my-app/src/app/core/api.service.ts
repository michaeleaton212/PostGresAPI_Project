import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, tap, catchError } from 'rxjs';
import { throwError } from 'rxjs';

// Backend API base URL
const DEV_BACKEND = 'http://localhost:5031/api';
const RELATIVE_API = '/api';

// Automatically detect if running locally or in production
const API_BASE =
  typeof window !== 'undefined' &&
  (window.location.hostname === 'localhost' || window.location.hostname === '127.0.0.1')
    ? DEV_BACKEND
    : RELATIVE_API;

@Injectable({ providedIn: 'root' })
export class ApiService {
  public http = inject(HttpClient);

  constructor() {
  console.log('[ApiService] Initialized with API_BASE:', API_BASE);
  }

  /**
   * Build complete API URL
   */
  public makeUrl(path: string): string {
    const cleanPath = path.replace(/^\/+/, '');
    return `${API_BASE}/${cleanPath}`;
  }

  /**
   * Standard JSON headers
   */
  private jsonHeaders() {
    return { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
  }

  /**
   * HTTP GET request
   */
  get<T>(path: string, params?: Record<string, any>): Observable<T> {
    const url = this.makeUrl(path);
    console.debug('[ApiService] GET', url, params || '');
    return this.http.get<T>(url, { params }).pipe(
      tap(response => console.log('[ApiService] GET Response:', response)),
  catchError(error => {
        console.error('[ApiService] GET Error:', error);
return throwError(() => error);
  })
    );
  }

  /**
   * HTTP POST request
   */
  post<T>(path: string, body: any): Observable<T> {
    const url = this.makeUrl(path);
    console.log('[ApiService] POST URL:', url);
  console.log('[ApiService] POST Body:', body);
    console.log('[ApiService] POST Headers:', this.jsonHeaders());
    
    return this.http.post<T>(url, body, this.jsonHeaders()).pipe(
      tap(response => {
        console.log('[ApiService] POST Success Response:', response);
      }),
      catchError(error => {
 console.error('[ApiService] POST Error Details:', {
          status: error.status,
          statusText: error.statusText,
          error: error.error,
          message: error.message,
   url: url,
          body: body
        });
      return throwError(() => error);
      })
    );
  }

  /**
   * HTTP PUT request
   */
  put<T>(path: string, body: any): Observable<T> {
    const url = this.makeUrl(path);
    console.debug('[ApiService] PUT', url, body);
    return this.http.put<T>(url, body, this.jsonHeaders()).pipe(
      tap(response => console.log('[ApiService] PUT Response:', response)),
   catchError(error => {
        console.error('[ApiService] PUT Error:', error);
        return throwError(() => error);
      })
    );
  }

  /**
   * HTTP DELETE request
   */
  delete<T>(path: string): Observable<T> {
    const url = this.makeUrl(path);
    console.debug('[ApiService] DELETE', url);
    return this.http.delete<T>(url).pipe(
      tap(response => console.log('[ApiService] DELETE Response:', response)),
      catchError(error => {
        console.error('[ApiService] DELETE Error:', error);
     return throwError(() => error);
      })
    );
  }
}
