import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Booking, CreateBookingDto } from './models/booking.model';

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
   * Create a new booking
   */
  create(booking: CreateBookingDto): Observable<Booking> {
    return this.api.post<Booking>('bookings', booking);
  }

  /**
   * Delete a booking
   */
  delete(id: number): Observable<void> {
    return this.api.delete<void>(`bookings/${id}`);
  }

  /**
   * Find booking by email (title) and booking number (id)
   */
  findByEmailAndBookingNumber(
    email: string,
    bookingNumber: string
  ): Observable<Booking> {
    return this.api.get<Booking>('bookings/find', {
      title: email,
      id: bookingNumber,
    });
  }
}
