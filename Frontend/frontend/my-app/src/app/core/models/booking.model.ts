export interface Booking {
id: number;
  roomId: number;
  startTime: string; // ISO 8601 date string
  endTime: string; // ISO 8601 date string
  title: string | null;
}

export interface CreateBookingDto {
  roomId: number;
  startUtc: string; // ISO 8601 date string
  endUtc: string;   // ISO 8601 date string
  title: string | null;
}
