import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { FooterComponent } from '../../components/core/footer/footer';
import { BookingService } from '../../core/booking.service';
import { RoomService } from '../../core/room.service';
import { Booking, BookingStatus } from '../../core/models/booking.model';
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
  status: BookingStatus;
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

  readonly BookingStatus = BookingStatus;

  ngOnInit() {
    console.log('=== DASHBOARD INIT ===');
    this.loadBookings();
  }

  loadBookings() {
    console.log('Loading rooms and bookings...');
    
    this.roomService.getAll().subscribe({
      next: (rooms) => {
        console.log('Rooms loaded:', rooms.length);
        this.rooms = rooms;
        
        this.bookingService.getAll().subscribe({
          next: (bookings) => {
            console.log('All bookings received:', bookings.length);
            console.log('Bookings:', bookings);
            
            // Show ALL bookings including cancelled ones
            this.bookings = bookings.map(b => this.mapBookingToDisplay(b));
            console.log('Mapped bookings:', this.bookings);
          },
          error: (err) => {
            console.error('Error loading bookings:', err);
          }
        });
      },
      error: (err) => {
        console.error('Error loading rooms:', err);
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
      status: booking.status as BookingStatus
    };
  }

  isCheckedIn(booking: BookingDisplay): boolean {
    return booking.status === BookingStatus.CheckedIn;
  }

  isCancelled(booking: BookingDisplay): boolean {
    return booking.status === BookingStatus.Cancelled;
  }

  toggleCheckIn(booking: BookingDisplay) {
    // Don't allow toggle for cancelled bookings
    if (this.isCancelled(booking)) {
      return;
    }

    const newStatus = this.isCheckedIn(booking) 
      ? BookingStatus.CheckedOut 
      : BookingStatus.CheckedIn;

    console.log(`Toggling check-in for booking ${booking.id}: ${booking.status} -> ${newStatus}`);

    this.bookingService.updateStatus(booking.id, { status: newStatus }).subscribe({
      next: (updatedBooking) => {
        console.log('Status updated successfully:', updatedBooking);
        booking.status = updatedBooking.status as BookingStatus;
      },
      error: (err) => {
        console.error('Error updating status:', err);
        alert('Fehler beim Aktualisieren des Check-in Status');
      }
    });
  }

  cancelBooking(bookingNumber: string) {
    const booking = this.bookings.find(b => b.bookingNumber === bookingNumber);
    if (!booking) return;

    // Don't allow cancelling if already cancelled
    if (this.isCancelled(booking)) {
      return;
    }

    if (confirm(`Möchten Sie die Buchung ${bookingNumber} wirklich stornieren?`)) {
      console.log(`Cancelling booking ${booking.id}`);
      
      this.bookingService.updateStatus(booking.id, { status: BookingStatus.Cancelled }).subscribe({
        next: (updatedBooking) => {
          console.log('Booking cancelled successfully');
          // Update the status in the local array instead of removing it
          booking.status = updatedBooking.status as BookingStatus;
        },
        error: (err) => {
          console.error('Error cancelling booking:', err);
          alert('Fehler beim Stornieren der Buchung');
        }
      });
    }
  }

  getStatusText(status: BookingStatus): string {
    switch (status) {
      case BookingStatus.Pending:
        return 'Ausstehend';
      case BookingStatus.CheckedIn:
        return 'Eingecheckt';
      case BookingStatus.CheckedOut:
        return 'Ausgecheckt';
      case BookingStatus.Cancelled:
        return 'Storniert';
      default:
        return 'Unbekannt';
    }
  }

  viewRoom(booking: BookingDisplay) {
    if (booking.roomType.toLowerCase() === 'bedroom') {
      this.router.navigate(['/bedroom-preview', booking.roomId]);
    } else if (booking.roomType.toLowerCase() === 'meetingroom') {
      this.router.navigate(['/meetingroom-preview', booking.roomId]);
    } else {
      console.error('Unknown room type:', booking.roomType);
      alert('Vorschau für diesen Raumtyp nicht verfügbar');
    }
  }

  goBackLogin() {
    this.router.navigate(['/login']);
  }
}
