import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FooterComponent } from '../../components/core/footer/footer';
import { BookingService } from '../../core/booking.service';

@Component({
  selector: 'login-page',
  standalone: true,
  imports: [CommonModule, FormsModule, FooterComponent],
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.scss']
})
export class LoginPageComponent {
  private bookingService = inject(BookingService);

  firstName = '';
  bookingNumber = '';
  bookingInProgress = false;

  loginResult: boolean | null = null;
  error: string | null = null;

  get isFormValid(): boolean {
    return this.firstName.trim().length > 0 && this.bookingNumber.trim().length > 0;
  }

  
}
