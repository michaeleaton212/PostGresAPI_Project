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
  userName: string = '';

  readonly BookingStatus = BookingStatus;

  ngOnInit() {
    console.log('=== DASHBOARD INIT ===');
    
    // Check if user is logged in
    const bookingIdsStr = sessionStorage.getItem('bookingIds');
    const userName = sessionStorage.getItem('userName');
    const token = sessionStorage.getItem('loginToken');
    
    console.log('Session check:', { bookingIdsStr, userName, hasToken: !!token });
    
    if (!bookingIdsStr || !userName || !token) {
      console.log('User not logged in, redirecting to login');
      this.router.navigate(['/login']);
      return;
    }
    
    this.userName = userName;
    console.log('User logged in as:', this.userName);
    
    // Parse booking IDs
    try {
      const bookingIds: number[] = JSON.parse(bookingIdsStr);
      console.log('User booking IDs:', bookingIds);
      this.loadBookings(bookingIds);
    } catch (error) {
      console.error('Error parsing bookingIds from sessionStorage:', error);
      this.router.navigate(['/login']);
    }
  }

  loadBookings(bookingIds: number[]) {
    console.log('=== LOADING BOOKINGS ===');
    console.log('User:', this.userName);
    console.log('Booking IDs to load:', bookingIds);
    console.log('Number of IDs:', bookingIds.length);
    
    if (!bookingIds || bookingIds.length === 0) {
      console.warn('No booking IDs found for user');
      this.bookings = [];
      return;
    }
    
    this.roomService.getAll().subscribe({
      next: (rooms) => {
        console.log('Rooms loaded:', rooms.length);
        this.rooms = rooms;
        
        // Get only bookings for the logged-in user by IDs
        console.log('Calling POST /api/bookings/by-ids with:', bookingIds);
        this.bookingService.getByIds(bookingIds).subscribe({
          next: (bookings) => {
            console.log('=== BOOKINGS LOADED SUCCESSFULLY ===');
            console.log('Number of bookings received:', bookings.length);
            console.log('Booking IDs received:', bookings.map(b => b.id));
            console.log('Booking titles:', bookings.map(b => b.title));
            console.log('Raw bookings:', bookings);
            
            this.bookings = bookings.map(b => this.mapBookingToDisplay(b));
            console.log('Mapped bookings for display:', this.bookings.length);
          },
          error: (err) => {
            console.error('=== ERROR LOADING BOOKINGS ===');
            console.error('Error status:', err.status);
            console.error('Error message:', err.message);
            console.error('Error details:', err.error);
            console.error('Full error:', err);
            
            if (err.status === 404) {
              alert('Der Endpoint /api/bookings/by-ids existiert noch nicht. Bitte starten Sie das Backend neu.');
            } else {
              alert('Fehler beim Laden der Buchungen: ' + (err.error?.error || err.message));
            }
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
    // Don't allow toggle for cancelled bookings or already checked-in bookings
    if (this.isCancelled(booking) || this.isCheckedIn(booking)) {
      return;
    }

    // Only allow checking in (not checking out)
    const newStatus = BookingStatus.CheckedIn;

    console.log(`Checking in booking ${booking.id}: ${booking.status} -> ${newStatus}`);

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
    sessionStorage.removeItem('loginToken');
    sessionStorage.removeItem('bookingIds');
    sessionStorage.removeItem('userName');
    this.router.navigate(['/login']);
  }
}
