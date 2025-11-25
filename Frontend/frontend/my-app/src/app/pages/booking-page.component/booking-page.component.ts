import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { RoomService } from '../../core/room.service';
import { BookingService } from '../../core/booking.service';
import { Room } from '../../core/models/room.model';
import { CreateBookingDto } from '../../core/models/booking.model';
import { FooterComponent } from '../../components/core/footer/footer';

@Component({
  selector: 'booking-page',
  standalone: true,
  imports: [CommonModule, FormsModule, FooterComponent],
  templateUrl: './booking-page.component.html',
  styleUrls: ['./booking-page.component.scss']
})
export class BookingPageComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private roomService = inject(RoomService);
  private bookingService = inject(BookingService);

  room: Room | null = null;
  startDate: Date | null = null;
  endDate: Date | null = null;
  firstName = '';
  loading = true;
  error: string | null = null;
  bookingSuccess = false;
  bookingInProgress = false;
  currentRoomId = this.route.snapshot.queryParams['roomId'];
  bookingNumber: string | number | null = null; // Property for booking number

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      console.log('=== BOOKING PAGE QUERY PARAMS ===');
      console.log('All params:', params);
      console.log('roomId:', params['roomId']);
      console.log('startDate:', params['startDate']);
      console.log('endDate:', params['endDate']);
      console.log('startTime:', params['startTime']);
      console.log('endTime:', params['endTime']);

      const roomId = params['roomId'];
      const startDateStr = params['startDate'];
      const endDateStr = params['endDate'];
      const startTimeStr = params['startTime'];
      const endTimeStr = params['endTime'];

      if (!roomId) {
        this.error = 'Keine Raum-ID angegeben.';
        this.loading = false;
        return;
      }

      // Parse dates from either startDate/endDate (Bedroom) or startTime/endTime (Meetingroom)
      if (startTimeStr) {
        this.startDate = new Date(startTimeStr);
        console.log('Parsed startTime:', this.startDate);
      } else if (startDateStr) {
        this.startDate = new Date(startDateStr);
        console.log('Parsed startDate:', this.startDate);
      } else {
        console.warn('No startDate or startTime in query params');
      }

      if (endTimeStr) {
        this.endDate = new Date(endTimeStr);
        console.log('Parsed endTime:', this.endDate);
      } else if (endDateStr) {
        this.endDate = new Date(endDateStr);
        console.log('Parsed endDate:', this.endDate);
      } else {
        console.warn('No endDate or endTime in query params');
      }

      // Load room details
      this.loadRoom(Number(roomId));
    });
  }

  loadRoom(id: number) {
    this.loading = true;
    this.error = null;

    this.roomService.getById(id).subscribe({
      next: (room) => {
        this.room = room;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading room:', err);
        this.error = 'Raum konnte nicht geladen werden.';
        this.loading = false;
      }
    });
  }

  confirmBooking() {
    if (!this.room || !this.startDate || !this.endDate) {
      this.error = 'Unvollständige Buchungsinformationen.';
      return;
    }

    // Validate first name
    if (!this.firstName || this.firstName.trim() === '') {
      this.error = 'Bitte geben Sie Ihren Vornamen ein.';
      return;
    }

    this.bookingInProgress = true;
    this.error = null;

    const bookingDto: CreateBookingDto = {
      roomId: this.room.id,
      startUtc: this.startDate.toISOString(),
      endUtc: this.endDate.toISOString(),
      title: this.firstName.trim()
    };

    this.bookingService.create(bookingDto).subscribe({
      next: (booking) => {
        console.log('Booking created:', booking);
        this.bookingNumber = booking.id; // Booking number from  backend
        this.bookingSuccess = true;
        this.bookingInProgress = false;


      },
      error: (err) => {
        console.error('Error creating booking:', err);
        this.error = err.error?.error || 'Buchung konnte nicht erstellt werden.';
        this.bookingInProgress = false;
      }
    });
  }

  goBack() {
    this.router.navigate(['/rooms']);
  }

  goBackPreview(id: number) {
    this.router.navigate(['/room-preview'], { queryParams: { id } });
  }

  get numberOfDays(): number {
    if (!this.startDate) {
      return 0;
    }
    else if (!this.endDate) {
      return 1;
    }
    else {
      const start = new Date(this.startDate);
      const end = new Date(this.endDate);

      start.setHours(0, 0, 0, 0);
      end.setHours(0, 0, 0, 0);

      const diffTime = Math.abs(end.getTime() - start.getTime());

      return Math.floor(diffTime / (1000 * 60 * 60 * 24)) + 1;
    }
  }

  get durationInMinutes(): number {
    if (!this.startDate || !this.endDate) {
      return 0;
    }
    const diffMs = this.endDate.getTime() - this.startDate.getTime();
    return Math.floor(diffMs / (1000 * 60));
  }

  get isFormValid(): boolean {
    return !!(this.startDate && this.endDate && this.firstName.trim());
  }
}
