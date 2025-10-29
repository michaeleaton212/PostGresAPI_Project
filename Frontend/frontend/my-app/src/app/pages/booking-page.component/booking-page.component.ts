import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'booking-page',
  standalone: true,
  imports: [CommonModule],
  template: `
 <div class="page">
 <h2>Booking</h2>
 <p>Welcome to the Booking Pages.</p>
 </div>
 `
})
export class BookingPageComponent { }
