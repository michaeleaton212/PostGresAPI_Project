import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

// Use absolute backend URL during local development if proxy is not active.
const DEV_BACKEND = 'http://localhost:5031/api';
const RELATIVE_API = '/api';
const API_BASE =
  typeof window !== 'undefined' &&
  (window.location.hostname === 'localhost' || window.location.hostname === '127.0.0.1')
    ? DEV_BACKEND
    : RELATIVE_API;

@Injectable({ providedIn: 'root' })
export class ApiService {
  private http = inject(HttpClient);

  private makeUrl(path: string) {
    const p = path.replace(/^\/+/, '');
    // If API_BASE already contains /api and is an absolute URL, avoid doubling
    if (API_BASE.endsWith('/api')) {
      return `${API_BASE}/${p}`;
    }
    return `${API_BASE}/${p}`;
  }

  private jsonHeaders() {
    return { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
  }

  get<T>(path: string, params?: Record<string, any>): Observable<T> {
    const url = this.makeUrl(path);
    console.debug('[ApiService] GET', url, params || '');
    return this.http.get<T>(url, { params });
  }

  post<T>(path: string, body: any): Observable<T> {
    const url = this.makeUrl(path);
    console.debug('[ApiService] POST', url, body || '');
    return this.http.post<T>(url, body, this.jsonHeaders());
  }

  put<T>(path: string, body: any): Observable<T> {
    const url = this.makeUrl(path);
    console.debug('[ApiService] PUT', url, body || '');
    return this.http.put<T>(url, body, this.jsonHeaders());
  }

  delete<T>(path: string): Observable<T> {
    const url = this.makeUrl(path);
    console.debug('[ApiService] DELETE', url);
    return this.http.delete<T>(url);
  }
}
