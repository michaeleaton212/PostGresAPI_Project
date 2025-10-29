import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

const API_BASE = '/api';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private http = inject(HttpClient);

  get<T>(path: string, params?: Record<string, any>): Observable<T> {
    return this.http.get<T>(`${API_BASE}/${path}`, { params });
  }
  post<T>(path: string, body: any): Observable<T> {
    return this.http.post<T>(`${API_BASE}/${path}`, body);
  }
  put<T>(path: string, body: any): Observable<T> {
    return this.http.put<T>(`${API_BASE}/${path}`, body);
  }
  delete<T>(path: string): Observable<T> {
    return this.http.delete<T>(`${API_BASE}/${path}`);
  }

}
