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

  // Du nutzt das Feld aktuell als E-Mail / title
  firstName = '';

  loading = true;
  error: string | null = null;
  bookingSuccess = false;
  bookingInProgress = false;
  currentRoomId = this.route.snapshot.queryParams['roomId'];
  bookingNumber: string | number | null = null;

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      console.log('=== BOOKING PAGE QUERY PARAMS ===');
      console.log('All params:', params);

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

      if (startTimeStr) {
        this.startDate = new Date(startTimeStr);
      } else if (startDateStr) {
        this.startDate = new Date(startDateStr);
      }

      if (endTimeStr) {
        this.endDate = new Date(endTimeStr);
      } else if (endDateStr) {
        this.endDate = new Date(endDateStr);
      }

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

  // Login-Status aus sessionStorage (du nutzt userId bereits im DTO)
  get isLoggedIn(): boolean {
    const userIdStr = sessionStorage.getItem('userId');
    const userId = userIdStr ? parseInt(userIdStr, 10) : NaN;
    return Number.isFinite(userId) && userId > 0;
  }

  confirmBooking() {
    console.log('=== CONFIRM BOOKING STARTED ===');

    // Nur eingeloggt darf buchen
    if (!this.isLoggedIn) {
      this.error = 'Bitte einloggen, um zu buchen.';
      return;
    }

    if (!this.room || !this.startDate || !this.endDate) {
      this.error = 'Unvollständige Buchungsinformationen.';
      return;
    }

    this.bookingInProgress = true;
    this.error = null;

    // Get userId from sessionStorage
    const userIdStr = sessionStorage.getItem('userId');
    const userId = userIdStr ? parseInt(userIdStr, 10) : null;

    // Falls dein Backend "title" braucht: nie leer lassen
    const title = this.firstName.trim() || 'Booking';

    const bookingDto: CreateBookingDto = {
      roomId: this.room.id,
      startUtc: this.startDate.toISOString(),
      endUtc: this.endDate.toISOString(),
      title: title,
      userId: userId
    };

    console.log('=== BOOKING DTO ===');
    console.log('DTO Object:', bookingDto);

    this.bookingService.create(bookingDto).subscribe({
      next: (booking) => {
        console.log('=== BOOKING SUCCESS ===');
        console.log('Booking Response:', booking);
        this.bookingNumber = booking.bookingNumber;
        this.bookingSuccess = true;
        this.bookingInProgress = false;
      },
      error: (err) => {
        console.error('=== BOOKING ERROR ===');
        console.error('Full Error Object:', err);

        let errorMessage = 'Buchung konnte nicht erstellt werden.';
        if (err.error?.error) {
          errorMessage = err.error.error;
        } else if (err.error?.message) {
          errorMessage = err.error.message;
        } else if (err.message) {
          errorMessage = err.message;
        }

        this.error = errorMessage;
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
    } else if (!this.endDate) {
      return 1;
    } else {
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

  // Form-Validierung entfernt: nur noch Datum prüfen
  get isFormValid(): boolean {
    return !!this.startDate && !!this.endDate;
  }

  get totalPrice(): number {
    if (!this.room || this.room.type !== 'Bedroom' || !this.room.pricePerNight) {
      return 0;
    }
    return this.room.pricePerNight * this.numberOfDays;
  }
}
