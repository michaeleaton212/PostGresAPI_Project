import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { FooterComponent } from '../../components/core/footer/footer';
import { BookingService } from '../../core/booking.service';
import { RoomService } from '../../core/room.service';
import { Booking } from '../../core/models/booking.model';
import { Room } from '../../core/models/room.model';

interface BookingDisplay {
  id: number;
  name: string;
  roomName: string;
  roomId: number;
  roomType: string;
  startDate: Date;
  endDate: Date;
  bookingNumber: string;
  isCheckedIn: boolean;
}

@Component({
  selector: 'dashboard-page',
  standalone: true,
  imports: [CommonModule, FooterComponent, FormsModule],
  templateUrl: './dashboard-page.component.html',
  styleUrls: ['./dashboard-page.component.scss']
})
export class DashboardPageComponent implements OnInit {
  private bookingService = inject(BookingService);
  private roomService = inject(RoomService);
  private router = inject(Router);

  bookings: BookingDisplay[] = [];
  rooms: Room[] = [];

  ngOnInit() {
  this.loadBookings();
  }

  loadBookings() {
    // Zuerst alle Räume laden
    this.roomService.getAll().subscribe({
      next: (rooms) => {
     this.rooms = rooms;
        // Dann alle Buchungen laden
        this.bookingService.getAll().subscribe({
    next: (bookings) => {
            this.bookings = bookings.map(b => this.mapBookingToDisplay(b));
          },
    error: (err) => {
            console.error('Fehler beim Laden der Buchungen:', err);
          }
    });
      },
      error: (err) => {
        console.error('Fehler beim Laden der Räume:', err);
}
    });
  }

  mapBookingToDisplay(booking: Booking): BookingDisplay {
    const room = this.rooms.find(r => r.id === booking.roomId);
    return {
 id: booking.id,
      name: booking.title || 'Keine Angabe',
   roomName: room ? room.name : `Raum ${booking.roomId}`,
      roomId: booking.roomId,
      roomType: room ? room.type : 'unknown',
   startDate: new Date(booking.startTime),
    endDate: new Date(booking.endTime),
      bookingNumber: booking.bookingNumber,
      isCheckedIn: false // Standardwert, kann später erweitert werden
    };
  }

  cancelBooking(bookingNumber: string) {
  const booking = this.bookings.find(b => b.bookingNumber === bookingNumber);
    if (!booking) return;

    if (confirm(`Möchten Sie die Buchung ${bookingNumber} wirklich stornieren?`)) {
      this.bookingService.delete(booking.id).subscribe({
  next: () => {
      console.log('Buchung erfolgreich storniert');
       this.loadBookings(); // Neu laden
    },
        error: (err) => {
          console.error('Fehler beim Stornieren:', err);
          alert('Fehler beim Stornieren der Buchung');
        }
      });
    }
  }

  viewRoom(booking: BookingDisplay) {
    if (booking.roomType.toLowerCase() === 'bedroom') {
 this.router.navigate(['/bedroom-preview', booking.roomId]);
    } else if (booking.roomType.toLowerCase() === 'meetingroom') {
      this.router.navigate(['/meetingroom-preview', booking.roomId]);
    } else {
    console.error('Unbekannter Raumtyp:', booking.roomType);
      alert('Vorschau für diesen Raumtyp nicht verfügbar');
 }
  }

  goBackLogin() {
    this.router.navigate(['/login']);
  }
}
