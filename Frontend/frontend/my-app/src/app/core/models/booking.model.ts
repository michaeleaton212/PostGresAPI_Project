export enum BookingStatus {
  Pending = 'Pending',
  CheckedIn = 'CheckedIn',
  CheckedOut = 'CheckedOut',
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
}

export interface CreateBookingDto {
  roomId: number;
  startUtc: string; // ISO 8601 date string
  endUtc: string;   // ISO 8601 date string
  title: string;
}

export interface UpdateBookingStatusDto {
  status: string; // BookingStatus
}
