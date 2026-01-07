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
  pricePerNight?: number;
  totalPrice?: number;
  numberOfNights?: number;
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

  // Popup state
  showCancelPopup = false;
  pendingCancelBookingNumber: string = '';

  readonly BookingStatus = BookingStatus;

  // Status texts for i18n
  statusPending = $localize`:@@booking.status.pending:Pending`;
  statusCheckedIn = $localize`:@@booking.status.checkedIn:Checked In`;
  statusExpired = $localize`:@@booking.status.expired:Expired`;
  statusCancelled = $localize`:@@booking.status.cancelled:Cancelled`;
  statusUnknown = $localize`:@@booking.status.unknown:Unknown`;

  // === Pagination ===
  pageSize = 10;
  currentPage = 1;

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.bookings.length / this.pageSize));
  }

  get pagedBookings(): BookingDisplay[] {
    const start = (this.currentPage - 1) * this.pageSize;
    return this.bookings.slice(start, start + this.pageSize);
  }

  nextPage() {
    if (this.currentPage < this.totalPages) this.currentPage++;
  }

  prevPage() {
    if (this.currentPage > 1) this.currentPage--;
  }

  // Wenn sich bookings neu laden, Seite korrigieren
  private clampPage() {
    this.currentPage = Math.min(this.currentPage, this.totalPages);
    if (this.currentPage < 1) this.currentPage = 1;
  }
  // === /Pagination ===

  ngOnInit() {
    console.log('=== DASHBOARD INIT ===');

    const userId = sessionStorage.getItem('userId');
    const userName = sessionStorage.getItem('userName');
    const userEmail = sessionStorage.getItem('userEmail');

    console.log('Session check:', { userId, userName, userEmail });

    if (!userId || !userName || !userEmail) {
      console.log('User not logged in, redirecting to login');
      this.router.navigate(['/login']);
      return;
    }

    this.userName = userName;
    console.log('User logged in as:', this.userName);

    this.loadBookings();
  }

  loadBookings() {
    console.log('=== LOADING BOOKINGS ===');
    console.log('User:', this.userName);

    this.roomService.getAll().subscribe({
      next: (rooms) => {
        console.log('Rooms loaded:', rooms.length);
        this.rooms = rooms;

        console.log('Calling GET /api/bookings (filtered by user)');
        this.bookingService.getAll().subscribe({
          next: (bookings) => {
            console.log('=== BOOKINGS LOADED SUCCESSFULLY ===');
            console.log('Number of bookings received:', bookings.length);
            console.log('Raw bookings:', bookings);

            this.bookings = bookings.map(b => this.mapBookingToDisplay(b));
            console.log('Mapped bookings for display:', this.bookings.length);

            // Pagination nach dem Laden justieren
            this.clampPage();
            // Optional: bei jedem Reload wieder auf Seite 1 springen:
            // this.currentPage = 1;
          },
          error: (err) => {
            console.error('=== ERROR LOADING BOOKINGS ===');
            console.error('Error details:', err);

            if (err.status === 401) {
              alert('Sitzung abgelaufen. Bitte melden Sie sich erneut an.');
              this.router.navigate(['/login']);
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
    const startDate = new Date(booking.startTime);
    const endDate = new Date(booking.endTime);

    const diffTime = Math.abs(endDate.getTime() - startDate.getTime());
    const numberOfNights = Math.ceil(diffTime / (1000 * 60 * 60 * 24));

    let pricePerNight: number | undefined;
    let totalPrice: number | undefined;

    if (room && room.type === 'Bedroom' && room.pricePerNight) {
      pricePerNight = room.pricePerNight;
      totalPrice = pricePerNight * numberOfNights;
    }

    return {
      id: booking.id,
      name: booking.title || 'Keine Angabe',
      roomName: room ? room.name : `Raum ${booking.roomId}`,
      roomId: booking.roomId,
      roomType: room ? room.type : 'unknown',
      startDate,
      endDate,
      bookingNumber: booking.bookingNumber,
      status: booking.status as BookingStatus,
      pricePerNight,
      totalPrice,
      numberOfNights
    };
  }

  calculateNumberOfNights(startDate: Date, endDate: Date): number {
    const start = new Date(startDate);
    const end = new Date(endDate);
    const timeDiff = Math.abs(end.getTime() - start.getTime());
    const diffDays = Math.ceil(timeDiff / (1000 * 3600 * 24));
    return diffDays;
  }

  isCheckedIn(booking: BookingDisplay): boolean {
    return booking.status === BookingStatus.CheckedIn;
  }

  isCancelled(booking: BookingDisplay): boolean {
    return booking.status === BookingStatus.Cancelled;
  }

  isExpired(booking: BookingDisplay): boolean {
    return booking.status === BookingStatus.Expired;
  }

  toggleCheckIn(booking: BookingDisplay) {
    if (this.isCancelled(booking) || this.isExpired(booking) || this.isCheckedIn(booking)) {
      return;
    }

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

    if (this.isCancelled(booking) || this.isExpired(booking)) {
      return;
    }

    this.pendingCancelBookingNumber = bookingNumber;
    this.showCancelPopup = true;
  }

  closeCancelPopup() {
    this.showCancelPopup = false;
    this.pendingCancelBookingNumber = '';
  }

  confirmCancelBooking() {
    const booking = this.bookings.find(b => b.bookingNumber === this.pendingCancelBookingNumber);
    if (!booking) {
      this.closeCancelPopup();
      return;
    }

    console.log(`Cancelling booking ${booking.id}`);

    this.bookingService.updateStatus(booking.id, { status: BookingStatus.Cancelled }).subscribe({
      next: (updatedBooking) => {
        console.log('Booking cancelled successfully');
        booking.status = updatedBooking.status as BookingStatus;
        this.closeCancelPopup();
      },
      error: (err) => {
        console.error('Error cancelling booking:', err);
        alert('Fehler beim Stornieren der Buchung');
        this.closeCancelPopup();
      }
    });
  }

  getStatusText(status: BookingStatus): string {
    switch (status) {
      case BookingStatus.Pending:
        return this.statusPending;
      case BookingStatus.CheckedIn:
        return this.statusCheckedIn;
      case BookingStatus.Expired:
        return this.statusExpired;
      case BookingStatus.Cancelled:
        return this.statusCancelled;
      default:
        return this.statusUnknown;
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
    sessionStorage.removeItem('userId');
    sessionStorage.removeItem('userName');
    sessionStorage.removeItem('userEmail');
    this.router.navigate(['/login']);
  }
}
