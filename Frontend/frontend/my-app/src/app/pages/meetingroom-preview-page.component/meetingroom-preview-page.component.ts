import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { RoomService } from '../../core/room.service';
import { Room, Meetingroom } from '../../core/models/room.model';
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
}

@Component({
  selector: 'meetingroom-preview-page',
  standalone: true,
  imports: [CommonModule, FooterComponent],
  templateUrl: './meetingroom-preview-page.component.html',
  styleUrls: ['./meetingroom-preview-page.component.scss']
})
export class MeetingroomPreviewPageComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private roomService = inject(RoomService);

  room: Meetingroom | null = null;
  loading = true;
  error: string | null = null;

  // Calendar properties
  currentMonth: Date = new Date();
  calendarDays: CalendarDay[] = [];

  // Start, end dates for range selection
  rangeStart: Date | null = null;
  rangeEnd: Date | null = null;

  // Error state for missing date selection
  showDateError = false;

monthNames = [
    'January', 'February', 'March', 'April', 'May', 'June',
    'July', 'August', 'September', 'October', 'November', 'December'
  ];
  weekDays = ['S', 'M', 'T', 'W', 'T', 'F', 'S'];

  ngOnInit() {
    this.generateCalendar();

    this.route.queryParams.subscribe(params => {
      const roomId = params['id'];
      if (roomId) {
        this.loadRoom(Number(roomId));
      } else {
     this.error = 'Keine Raum-ID angegeben.';
this.loading = false;
      }
  });
  }

  loadRoom(id: number) {
    this.loading = true;
    this.error = null;

    this.roomService.getById(id).subscribe({
      next: (room) => {
        if (room.type !== 'Meetingroom') {
          this.error = 'Dies ist kein Meetingroom.';
          this.loading = false;
          return;
        }
        this.room = room as Meetingroom;
  console.log('Meetingroom loaded:', this.room);
        this.loading = false;
    },
      error: (err) => {
     console.error('Error loading meetingroom:', err);
this.error = 'Meetingroom konnte nicht geladen werden.';
        this.loading = false;
      }
    });
  }

  goBack() {
    this.router.navigate(['/rooms']);
  }

  openBooking() {
 if (!this.room) return;

    if (!this.rangeStart) {
    this.showDateError = false;
  setTimeout(() => {
        this.showDateError = true;
      }, 0);
    return;
    }

    if (!this.rangeEnd) {
      this.rangeEnd = new Date(this.rangeStart);
      this.rangeEnd.setHours(0, 0, 0, 0);
    }

    this.showDateError = false;

    const queryParams: any = { roomId: this.room.id };
 queryParams.startDate = this.rangeStart.toISOString();
    queryParams.endDate = this.rangeEnd.toISOString();

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
        isRangeEnd: false
      });
    }

const today = new Date();
    today.setHours(0, 0, 0, 0);

  for (let day = 1; day <= daysInMonth; day++) {
      const date = new Date(year, month, day);
date.setHours(0, 0, 0, 0);

      const isToday = date.getTime() === today.getTime();
      const rangeInfo = this.getDateRangeInfo(date);

      this.calendarDays.push({
        day,
        date,
     isPad: false,
        isToday,
        isSelected: rangeInfo.isSelected,
     isInRange: rangeInfo.isInRange,
        isRangeStart: rangeInfo.isRangeStart,
        isRangeEnd: rangeInfo.isRangeEnd
      });
    }
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
    if (calendarDay.isPad) return;

    this.showDateError = false;

 const clickedDate = new Date(calendarDay.date);
    clickedDate.setHours(0, 0, 0, 0);

    if (!this.rangeStart) {
      this.rangeStart = clickedDate;
      this.rangeEnd = null;
    } else if (!this.rangeEnd) {
      const clickedTime = clickedDate.getTime();
      const startTime = this.rangeStart.getTime();

      if (clickedTime === startTime) {
      this.rangeStart = null;
        this.rangeEnd = null;
      } else if (clickedTime < startTime) {
      this.rangeEnd = this.rangeStart;
    this.rangeStart = clickedDate;
      } else {
        this.rangeEnd = clickedDate;
      }
    } else {
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

  today: Date = new Date();

  get monthYearLabel(): string {
    return `${this.monthNames[this.currentMonth.getMonth()]} ${this.currentMonth.getFullYear()}`;
  }

  get selectedDatesCount(): number {
    return this.getSelectedDatesInRange().length;
  }

  get hasRangeSelection(): boolean {
    return this.rangeStart !== null && this.rangeEnd !== null;
  }

  get numberOfChairs(): number | undefined {
    return this.room?.numberOfChairs ?? undefined;
  }
}
