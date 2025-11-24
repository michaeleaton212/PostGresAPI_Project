import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FooterComponent } from '../../components/core/footer/footer';


@Component({
  selector: 'login-page',
  standalone: true,
  imports: [CommonModule, FormsModule, FooterComponent],
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.scss']
})
export class LoginPageComponent {
  firstName = '';
  bookingNumber = '';
  bookingInProgress = false;

  get isFormValid(): boolean {
    return this.firstName.trim().length > 0 && this.bookingNumber.trim().length > 0;
  }

  Signin() {
    if (!this.isFormValid || this.bookingInProgress) {
      return;
    }

    this.bookingInProgress = true;

    setTimeout(() => {
      this.bookingInProgress = false;
    }, 1000);
  }
}
