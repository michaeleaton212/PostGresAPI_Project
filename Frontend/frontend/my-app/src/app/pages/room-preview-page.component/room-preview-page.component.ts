import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { RoomService } from '../../core/room.service';
import { Room, Bedroom, Meetingroom } from '../../core/models/room.model';

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
  selector: 'room-preview-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './room-preview-page.component.html',
  styleUrls: ['./room-preview-page.component.scss']
})
export class RoomPreviewPageComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private roomService = inject(RoomService);

  room: Room | Bedroom | Meetingroom | null = null;
  loading = true;
  error: string | null = null;

  // Calendar properties
  currentMonth: Date = new Date();
  calendarDays: CalendarDay[] = [];

  // Start, end dates for range selection
  rangeStart: Date | null = null;
  rangeEnd: Date | null = null;
  
  monthNames = ['January', 'February', 'March', 'April', 'May', 'June', 
      'July', 'August', 'September', 'October', 'November', 'December'];
  weekDays = ['S', 'M', 'T', 'W', 'T', 'F', 'S'];

  ngOnInit() {
    // Initialize calendar
    this.generateCalendar();

    // Get room ID from query parameters
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
        this.room = room;
        console.log('=== ROOM LOADED ===');
console.log('Full room object:', room);
        console.log('Room type:', room.type);
     console.log('numberOfBeds (direct):', room.numberOfBeds);
        console.log('numberOfChairs (direct):', room.numberOfChairs);
        console.log('All properties:', Object.keys(room));
        console.log('numberOfBeds via getter:', this.numberOfBeds);
        console.log('numberOfChairs via getter:', this.numberOfChairs);
     console.log('isBedroom:', this.isBedroom);
        console.log('isMeetingroom:', this.isMeetingroom);
        console.log('===================');
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading room:', err);
this.error = 'Raum konnte nicht geladen werden.';
        this.loading = false;
  }
    });
  }

  goBack() {
    this.router.navigate(['/rooms']);
  }

  openBooking() {
    if (!this.room) return;
    
    // Navigate to booking page with room ID and selected dates
    const queryParams: any = { roomId: this.room.id };
    
    if (this.rangeStart) {
      queryParams.startDate = this.rangeStart.toISOString();
    }
  
    if (this.rangeEnd) {
      queryParams.endDate = this.rangeEnd.toISOString();
    }
    
    this.router.navigate(['/booking'], { queryParams });
  }

// Calendar methods
  generateCalendar() {
    this.calendarDays = [];
    const year = this.currentMonth.getFullYear();
    const month = this.currentMonth.getMonth();
    
    // First day of the month
    const firstDay = new Date(year, month, 1);
    const firstDayOfWeek = firstDay.getDay();

    // Last day of the month
    const lastDay = new Date(year, month + 1, 0);
    const daysInMonth = lastDay.getDate();
    
    // Add padding days
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
    
    // Add actual days
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

  //  Checks if a date is in the range
  getDateRangeInfo(date: Date): { isSelected: boolean; isInRange: boolean; isRangeStart: boolean; isRangeEnd: boolean } {
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
    
    const clickedDate = new Date(calendarDay.date);
    clickedDate.setHours(0, 0, 0, 0);
    
    // Case 1: No start date set -> Set start
    if (!this.rangeStart) {
      this.rangeStart = clickedDate;
  this.rangeEnd = null;
      console.log('Range Start:', this.rangeStart.toLocaleDateString('de-DE'));
    }
    // Case 2: Start set, no end -> Set end (or new start if same)
    else if (!this.rangeEnd) {
      const clickedTime = clickedDate.getTime();
   const startTime = this.rangeStart.getTime();
      
      if (clickedTime === startTime) {
        // Same day clicked -> Reset
     this.rangeStart = null;
        this.rangeEnd = null;
        console.log('Range reset');
      } else if (clickedTime < startTime) {
        // Earlier date -> Swap
        this.rangeEnd = this.rangeStart;
  this.rangeStart = clickedDate;
        console.log('Range:', this.rangeStart.toLocaleDateString('de-DE'), '->', this.rangeEnd.toLocaleDateString('de-DE'));
      } else {
        // Later date -> Normal
        this.rangeEnd = clickedDate;
        console.log('Range:', this.rangeStart.toLocaleDateString('de-DE'), '->', this.rangeEnd.toLocaleDateString('de-DE'));
      }
    }
    // Case 3: Both set -> Reset and new start
    else {
      this.rangeStart = clickedDate;
   this.rangeEnd = null;
      console.log('New Range Start:', this.rangeStart.toLocaleDateString('de-DE'));
    }
    
    // Regenerate calendar to show range
    this.generateCalendar();
    
  this.logRangeInfo();
  }

  
  logRangeInfo() {
    if (!this.rangeStart) {
      console.log('No date range selected');
      return;
    }
    
    if (!this.rangeEnd) {
  console.log('Start date:', this.rangeStart.toLocaleDateString('de-DE'));
      console.log('Select end date');
      return;
    }
    
    const days = this.getSelectedDatesInRange();
    console.log('Selected time period:');
    console.log('  From:', this.rangeStart.toLocaleDateString('de-DE'));
 console.log('  To:', this.rangeEnd.toLocaleDateString('de-DE'));
    console.log('  Number of days:', days.length);
 console.log('  All dates:', days.map(d => d.toLocaleDateString('de-DE')));
  }

  // Get all dates in the selected range
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
    console.log('Month changed to:', this.monthYearLabel);
  }

  previousMonth() {
    this.currentMonth = new Date(
      this.currentMonth.getFullYear(),
      this.currentMonth.getMonth() - 1,
      1
    );
    this.generateCalendar();
    console.log('Month changed to:', this.monthYearLabel);
  }

  // Delete range
  clearRange() {
 this.rangeStart = null;
    this.rangeEnd = null;
    this.generateCalendar();
    console.log('Date range deleted');
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

  get isBedroom(): boolean {
    return this.room?.type === 'Bedroom';
  }

  get isMeetingroom(): boolean {
    return this.room?.type === 'Meetingroom';
  }

  get numberOfBeds(): number | undefined {
    if (!this.room || this.room.type !== 'Bedroom') {
      return undefined;
    }
    return this.room.numberOfBeds ?? undefined;
  }

  get numberOfChairs(): number | undefined {
    if (!this.room || this.room.type !== 'Meetingroom') {
      return undefined;
    }
    return this.room.numberOfChairs ?? undefined;
  }
}
