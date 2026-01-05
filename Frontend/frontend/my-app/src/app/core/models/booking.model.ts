export enum BookingStatus {
  Pending = 'Pending',
  CheckedIn = 'CheckedIn',
  Expired = 'Expired',
  Cancelled = 'Cancelled'
}

export interface Booking {
  id: number;
  roomId: number;
  startTime: string; // ISO 8601 date string
  endTime: string; // ISO 8601 date string
  title: string | null;
  bookingNumber: string; // Buchungsnummer
  status: string; // BookingStatus
  userId: number | null;
  numberOfPersons: number;
}

export interface CreateBookingDto {
  roomId: number;
  startUtc: string; // ISO 8601 date string
  endUtc: string;   // ISO 8601 date string
  title: string;
  userId?: number | null;
  numberOfPersons: number;
}

export interface UpdateBookingStatusDto {
  status: string; // BookingStatus
}
