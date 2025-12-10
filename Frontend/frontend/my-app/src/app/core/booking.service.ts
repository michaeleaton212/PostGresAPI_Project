// src/app/core/booking.service.ts
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Booking, CreateBookingDto, UpdateBookingStatusDto } from './models/booking.model';

// Login-DTOs passend zum Backend
export interface LoginRequestDto {
  bookingNumber: string;
  name: string;
}
export interface LoginResponseDto {
  bookingIds: number[];
  token: string;
}

@Injectable({ providedIn: 'root' })
export class BookingService {
  private api = inject(ApiService);

  /**
   * Get all bookings
   */
  getAll(): Observable<Booking[]> {
    return this.api.get<Booking[]>('bookings');
  }

  /**
   * Get a single booking by ID
   */
  getById(id: number): Observable<Booking> {
    return this.api.get<Booking>(`bookings/${id}`);
  }

  /**
   * Get bookings by name
   */
  getByName(name: string): Observable<Booking[]> {
    return this.api.get<Booking[]>(`bookings/by-name/${encodeURIComponent(name)}`);
  }

  /**
   * Get bookings by room ID
   */
  getByRoomId(roomId: number): Observable<Booking[]> {
    return this.api.get<Booking[]>(`bookings/room/${roomId}`);
  }

  /**
   * Create a new booking
   */
  create(booking: CreateBookingDto): Observable<Booking> {
    return this.api.post<Booking>('bookings', booking);
  }

  /**
   * Update booking status
   */
  updateStatus(id: number, dto: UpdateBookingStatusDto): Observable<Booking> {
    return this.api.patch<Booking>(`bookings/${id}/status`, dto);
  }

  /**
   * Delete a booking
   */
  delete(id: number): Observable<void> {
    return this.api.delete<void>(`bookings/${id}`);
  }

  /**
   * Login with booking number and name
   */
  login(req: LoginRequestDto): Observable<LoginResponseDto> {
    return this.api.post<LoginResponseDto>('bookings/login', req);
  }

  /**
   * Get booking with secure token
   */
  getBookingSecure(bookingId: number, token: string): Observable<Booking> {
    const url = this.api.makeUrl(`bookings/${bookingId}/secure`);
    return this.api.http.get<Booking>(url, { 
      headers: { 'X-Login-Token': token } 
    });
  }
}
