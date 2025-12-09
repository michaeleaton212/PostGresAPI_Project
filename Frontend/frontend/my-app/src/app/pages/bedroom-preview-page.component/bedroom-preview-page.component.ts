import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { RoomService } from '../../core/room.service';
import { BookingService } from '../../core/booking.service';
import { Room, Bedroom } from '../../core/models/room.model';
import { Booking } from '../../core/models/booking.model';
import { FooterComponent } from '../../components/core/footer/footer';

interface CalendarDay {
  day: number;
  date: Date;
  isPad: boolean;
  isToday: boolean;
  isSelected: boolean;
  isInRange: boolean;
  isRangeStart: boolean;
  isRangeEnd: boolean;
  isBooked: boolean;
}

@Component({
  selector: 'bedroom-preview-page',
  standalone: true,
  imports: [CommonModule, FooterComponent],
  templateUrl: './bedroom-preview-page.component.html',
  styleUrls: ['./bedroom-preview-page.component.scss']
})
export class BedroomPreviewPageComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private roomService = inject(RoomService);
  private bookingService = inject(BookingService);

  room: Bedroom | null = null;
  loading = true;
  error: string | null = null;
  roomBookings: Booking[] = [];

  // Image slider properties
  currentImageIndex = 0;

  // Calendar properties
  currentMonth: Date = new Date();
  calendarDays: CalendarDay[] = [];

  // Range selection
  rangeStart: Date | null = null;
  rangeEnd: Date | null = null;

  // Error state
  showDateError = false;
  dateErrorText = 'Please select a date!';

  monthNames = [
    'January', 'February', 'March', 'April', 'May', 'June',
    'July', 'August', 'September', 'October', 'November', 'December'
  ];
  weekDays = ['S', 'M', 'T', 'W', 'T', 'F', 'S'];

  today: Date = new Date();

  get hasImages(): boolean {
    return !!this.room && !!this.room.images && this.room.images.length > 0;
  }

  get currentImage(): string {
    if (this.hasImages) {
      return this.room!.images![this.currentImageIndex];
    }
    // Fallback: single image or grey.png
    return this.room?.image || '/grey.png';
  }

  ngOnInit() {
    // Versuche zuerst Route-Parameter, dann Query-Parameter
    this.route.paramMap.subscribe(params => {
      const routeId = params.get('id');
      if (routeId) {
        this.loadRoom(Number(routeId));
        return;
      }

      // Fallback auf Query-Parameter
      this.route.queryParams.subscribe(queryParams => {
        const queryId = queryParams['id'];
        if (queryId) {
          this.loadRoom(Number(queryId));
        } else {
          this.error = 'Keine Raum-ID angegeben.';
          this.loading = false;
        }
      });
    });
  }

  loadRoom(id: number) {
    this.loading = true;
    this.error = null;

    this.roomService.getById(id).subscribe({
      next: (room) => {
        if (room.type !== 'Bedroom') {
          this.error = 'Dies ist kein Bedroom.';
          this.loading = false;
          return;
        }
        this.room = room as Bedroom;

        // String aus DB (z. B. "/public/alpenblick.jpg, /public/brainstorm.jpg")
        // in ein Array von Pfaden umwandeln und /public/ entfernen
        if (this.room.image) {
          this.room.images = this.room.image
            .split(',')
            .map(p => p.trim())
            .filter(p => p.length > 0)
            .map(p => p.replace('/public/', '/'));
        }

        // falls kein Array rauskommt, Index zurücksetzen
        this.currentImageIndex = 0;

        // Buchungen für dieses Zimmer laden
        this.loadRoomBookings(id);
      },
      error: (err) => {
        console.error('Error loading bedroom:', err);
        this.error = 'Bedroom konnte nicht geladen werden.';
        this.loading = false;
      }
    });
  }

  loadRoomBookings(roomId: number) {
    this.bookingService.getByRoomId(roomId).subscribe({
      next: (bookings) => {
        this.roomBookings = bookings;
        this.generateCalendar();
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading room bookings:', err);
        // Auch bei Fehler Kalender generieren
        this.generateCalendar();
        this.loading = false;
      }
    });
  }

  goBack() {
    this.router.navigate(['/rooms']);
  }

  onImageError(event: any) {
    console.warn('Image load error:', event.target.src);
    event.target.src = '/grey.png';
  }

  openBooking() {
    if (!this.room) return;

    // Reset error UI
    this.showDateError = false;

    // Both start and end must be chosen
    if (!this.rangeStart || !this.rangeEnd) {
      this.dateErrorText = 'Bitte Start- und Enddatum wählen (mindestens 2 Tage).';
      this.showDateError = true;
      return;
    }

    // Normalize to midnight
    const start = new Date(this.rangeStart);
    const end = new Date(this.rangeEnd);
    start.setHours(0, 0, 0, 0);
    end.setHours(0, 0, 0, 0);

    // Inclusive difference (e.g., 1–2 => 2 Tage)
    const diffDaysInclusive = Math.floor(
      (end.getTime() - start.getTime()) / (1000 * 60 * 60 * 24)
    ) + 1;

    if (diffDaysInclusive < 2) {
      this.dateErrorText = 'Bitte mindestens 2 Tage auswählen.';
      this.showDateError = true;
      return;
    }

    const queryParams: any = {
      roomId: this.room.id,
      startDate: this.rangeStart.toISOString(),
      endDate: this.rangeEnd.toISOString()
    };

    this.router.navigate(['/booking'], { queryParams });
  }

  // Calendar methods
  generateCalendar() {
    this.calendarDays = [];
    const year = this.currentMonth.getFullYear();
    const month = this.currentMonth.getMonth();

    const firstDay = new Date(year, month, 1);
    const firstDayOfWeek = firstDay.getDay();

    const lastDay = new Date(year, month + 1, 0);
    const daysInMonth = lastDay.getDate();

    for (let i = 0; i < firstDayOfWeek; i++) {
      this.calendarDays.push({
        day: 0,
        date: new Date(),
        isPad: true,
        isToday: false,
        isSelected: false,
        isInRange: false,
        isRangeStart: false,
        isRangeEnd: false,
        isBooked: false
      });
    }

    const today = new Date();
    today.setHours(0, 0, 0, 0);

    for (let day = 1; day <= daysInMonth; day++) {
      const date = new Date(year, month, day);
      date.setHours(0, 0, 0, 0);

      const isToday = date.getTime() === today.getTime();
      const rangeInfo = this.getDateRangeInfo(date);
      const isBooked = this.isDateBooked(date);

      this.calendarDays.push({
        day,
        date,
        isPad: false,
        isToday,
        isSelected: rangeInfo.isSelected,
        isInRange: rangeInfo.isInRange,
        isRangeStart: rangeInfo.isRangeStart,
        isRangeEnd: rangeInfo.isRangeEnd,
        isBooked
      });
    }
  }

  isDateBooked(date: Date): boolean {
    const dateTime = date.getTime();
    const nextDay = new Date(date);
    nextDay.setDate(nextDay.getDate() + 1);
    const nextDayTime = nextDay.getTime();

    return this.roomBookings.some(booking => {
      const bookingStart = new Date(booking.startTime);
      const bookingEnd = new Date(booking.endTime);
      bookingStart.setHours(0, 0, 0, 0);
      bookingEnd.setHours(0, 0, 0, 0);

      const startTime = bookingStart.getTime();
      const endTime = bookingEnd.getTime();

      return dateTime >= startTime && dateTime < endTime;
    });
  }

  getDateRangeInfo(
    date: Date
  ): { isSelected: boolean; isInRange: boolean; isRangeStart: boolean; isRangeEnd: boolean } {
    const dateTime = date.getTime();

    if (!this.rangeStart) {
      return { isSelected: false, isInRange: false, isRangeStart: false, isRangeEnd: false };
    }

    const startTime = this.rangeStart.getTime();
    const isRangeStart = dateTime === startTime;

    if (!this.rangeEnd) {
      return {
        isSelected: isRangeStart,
        isInRange: false,
        isRangeStart: isRangeStart,
        isRangeEnd: false
      };
    }

    const endTime = this.rangeEnd.getTime();
    const isRangeEnd = dateTime === endTime;
    const isInRange = dateTime > startTime && dateTime < endTime;
    const isSelected = isRangeStart || isRangeEnd || isInRange;

    return { isSelected, isInRange, isRangeStart, isRangeEnd };
  }

  toggleDateSelection(calendarDay: CalendarDay) {
    if (calendarDay.isPad || calendarDay.isBooked) return;

    this.showDateError = false;

    const clickedDate = new Date(calendarDay.date);
    clickedDate.setHours(0, 0, 0, 0);

    if (!this.rangeStart) {
      // first click -> set start
      this.rangeStart = clickedDate;
      this.rangeEnd = null;
    } else if (!this.rangeEnd) {
      // second click -> define end or reset if same
      const clickedTime = clickedDate.getTime();
      const startTime = this.rangeStart.getTime();

      if (clickedTime === startTime) {
        // same day -> clear selection
        this.rangeStart = null;
        this.rangeEnd = null;
      } else if (clickedTime < startTime) {
        // clicked before start -> swap so start <= end
        this.rangeEnd = this.rangeStart;
        this.rangeStart = clickedDate;
      } else {
        // normal case -> set end
        this.rangeEnd = clickedDate;
      }
    } else {
      // third click -> start a new selection
      this.rangeStart = clickedDate;
      this.rangeEnd = null;
    }

    this.generateCalendar();
  }

  getSelectedDatesInRange(): Date[] {
    if (!this.rangeStart || !this.rangeEnd) return [];

    const dates: Date[] = [];
    const current = new Date(this.rangeStart);
    const end = new Date(this.rangeEnd);

    while (current <= end) {
      dates.push(new Date(current));
      current.setDate(current.getDate() + 1);
    }

    return dates;
  }

  nextMonth() {
    this.currentMonth = new Date(
      this.currentMonth.getFullYear(),
      this.currentMonth.getMonth() + 1,
      1
    );
    this.generateCalendar();
  }

  previousMonth() {
    this.currentMonth = new Date(
      this.currentMonth.getFullYear(),
      this.currentMonth.getMonth() - 1,
      1
    );
    this.generateCalendar();
  }

  clearRange() {
    this.rangeStart = null;
    this.rangeEnd = null;
    this.generateCalendar();
  }

  get monthYearLabel(): string {
    return `${this.monthNames[this.currentMonth.getMonth()]} ${this.currentMonth.getFullYear()}`;
  }

  get selectedDatesCount(): number {
    return this.getSelectedDatesInRange().length;
  }

  get hasRangeSelection(): boolean {
    return this.rangeStart !== null && this.rangeEnd !== null;
  }

  get numberOfBeds(): number | undefined {
    return this.room?.numberOfBeds ?? undefined;
  }

  nextImage(): void {
    if (!this.hasImages) return;
    this.currentImageIndex =
      (this.currentImageIndex + 1) % this.room!.images!.length;
  }

  prevImage(): void {
    if (!this.hasImages) return;
    this.currentImageIndex =
      (this.currentImageIndex - 1 + this.room!.images!.length) %// % Modulo-Operator makes that when first last pictuere it goes back to the last
      this.room!.images!.length;
  }

  selectImage(index: number): void {
    if (!this.hasImages) return;
    this.currentImageIndex = index;
  }
}
