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

  Signin(): void {
    if (!this.isFormValid || this.bookingInProgress) {
      return;
    }

    this.bookingInProgress = true;
    this.loginResult = null;
    this.error = null;

    const id = Number(this.bookingNumber);

    if (Number.isNaN(id)) {
      this.loginResult = false;
      this.error = 'Ungültige Buchungsnummer.';
      this.bookingInProgress = false;
      return;
    }

    this.bookingService.getById(id).subscribe({
      next: (booking: any) => {
        const dbTitle = (booking?.title || '').trim().toLowerCase();
        const inputName = this.firstName.trim().toLowerCase();

        if (booking && dbTitle === inputName) {
          this.loginResult = true;   
        } else {
          this.loginResult = false;  
        }

        this.bookingInProgress = false;
      },
      error: (err) => {
        console.error('Error loading booking:', err);
        this.loginResult = false;
        this.error = err?.error?.error || 'Buchung konnte nicht geladen werden.';
        this.bookingInProgress = false;
      }
    });
  }
}
