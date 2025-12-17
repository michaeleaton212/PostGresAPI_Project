import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
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
  private router = inject(Router);

  email = '';
  bookingNumber = '';
  bookingInProgress = false;

  loginResult: boolean | null = null;
  error: string | null = null;

  errorEmail: string | null = null;
  errorBookingNumber: string | null = null;

  emailTouched = false;
  bookingNumberTouched = false;

  get isFormValid(): boolean {
    return this.validateEmail(this.email) && this.bookingNumber.trim().length > 0;
  }

  get showEmailError(): boolean {
    return this.emailTouched && (this.email.trim() === '' || !this.validateEmail(this.email));
  }

  get showBookingNumberError(): boolean {
    return this.bookingNumberTouched && this.bookingNumber.trim() === '';
  }

  validateEmail(email: string): boolean {
    if (!email.trim()) return false;
    // Simple email regex
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.trim());
  }

  onEmailInput(): void {
    this.emailTouched = true;
    if (this.email.trim() === '') {
      this.errorEmail = 'Bitte E-Mail eingeben.';
    } else if (!this.validateEmail(this.email)) {
      this.errorEmail = 'Bitte gültige E-Mail eingeben.';
    } else {
      this.errorEmail = null;
    }
  }

  onBookingNumberInput(): void {
    this.bookingNumberTouched = true;
    if (this.bookingNumber.trim() === '') {
      this.errorBookingNumber = 'Bitte Buchungsnummer eingeben.';
    } else {
      this.errorBookingNumber = null;
    }
  }

  validateFields(): boolean {
    this.emailTouched = true;
    this.bookingNumberTouched = true;
    
    this.errorEmail = null;
    this.errorBookingNumber = null;
    let valid = true;
    
    if (!this.email.trim()) {
      this.errorEmail = 'Bitte E-Mail eingeben.';
      valid = false;
    } else if (!this.validateEmail(this.email)) {
      this.errorEmail = 'Bitte gültige E-Mail eingeben.';
      valid = false;
    }
    
    if (!this.bookingNumber.trim()) {
      this.errorBookingNumber = 'Bitte Buchungsnummer eingeben.';
      valid = false;
    }
    
    return valid;
  }

  onLogin(): void {
    if (this.bookingInProgress) return;
    
    this.error = null;
    this.loginResult = null;
    
    if (!this.validateFields()) return;
    
    this.bookingInProgress = true;

    const loginRequest = {
      bookingNumber: this.bookingNumber.trim(),
      name: this.email.trim()
    };

    this.bookingService.login(loginRequest).subscribe({ // call login; store token + booking metadata in sessionStorage; navigate to dashboard

      next: (res) => {
        sessionStorage.setItem('loginToken', res.token);
        sessionStorage.setItem('bookingIds', JSON.stringify(res.bookingIds));
        sessionStorage.setItem('userName', this.email.trim());

        this.loginResult = true;
        this.bookingInProgress = false;

        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        if (err?.status === 401) {
          this.error = 'Ungültige Anmeldedaten. Bitte überprüfen Sie Ihre E-Mail und Buchungsnummer.';
        } else if (err?.status === 404) {
          this.error = 'Buchung nicht gefunden.';
        } else if (err?.status === 0) {
          this.error = 'Verbindung zum Server fehlgeschlagen. Bitte starten Sie das Backend.';
        } else if (err?.error?.error) {
          this.error = err.error.error;
        } else if (err?.error?.message) {
          this.error = err.error.message;
        } else {
          this.error = 'Login fehlgeschlagen. Bitte später erneut versuchen.';
        }
        
        this.bookingInProgress = false;
        this.loginResult = false;
      }
    });
  }

  onKeyDown(event: KeyboardEvent): void {
    if (event.key === 'Enter') {
      event.preventDefault();
      this.onLogin();
    }
  }
}
