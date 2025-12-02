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

  get isFormValid(): boolean {
    return this.validateEmail(this.email) && this.bookingNumber.trim().length > 0;
  }

  validateEmail(email: string): boolean {
    // Simple email regex
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.trim());
  }

  onLogin(): void {
    if (!this.isFormValid || this.bookingInProgress) return;

    this.error = null;
    this.loginResult = null;
    this.bookingInProgress = true;

    this.bookingService
      .login({
        bookingNumber: this.bookingNumber.trim(),
        name: this.email.trim() // <--- send email as 'name'
      })
      .subscribe({
        next: (res) => {
          // Token safe in sessionStorage
          sessionStorage.setItem('loginToken', res.token);
          sessionStorage.setItem('bookingId', String(res.bookingId));

          // Navigate to dashboard with bookingId as query param
          this.router.navigate(['/dashboard'], {
            queryParams: { bookingId: res.bookingId }
          });
          this.loginResult = true;
        },
        error: (err) => {
          this.error =
            err?.status === 401
              ? 'Invalid Login'
              : 'Login fehlgeschlagen. Bitte später erneut versuchen.';
          this.bookingInProgress = false;
          this.loginResult = false;
        },
        complete: () => {
          this.bookingInProgress = false;
        }
      });
  }
}
