import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { FooterComponent } from '../../components/core/footer/footer';
import { UserService } from '../../core/user.service';

@Component({
  selector: 'login-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, FooterComponent],
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.scss']
})
export class LoginPageComponent {
  private userService = inject(UserService);
  private router = inject(Router);

  email = '';
  password = '';
  bookingInProgress = false;

  loginResult: boolean | null = null;
  error: string | null = null;

  errorEmail: string | null = null;
  errorPassword: string | null = null;

  emailTouched = false;
  passwordTouched = false;

  get isFormValid(): boolean {
    return this.validateEmail(this.email) && this.password.trim().length >= 6;
  }

  get showEmailError(): boolean {
    return this.emailTouched && (this.email.trim() === '' || !this.validateEmail(this.email));
  }

  get showPasswordError(): boolean {
    return this.passwordTouched && this.password.trim().length < 6;
  }

  validateEmail(email: string): boolean {
    if (!email.trim()) return false;
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

  onPasswordInput(): void {
    this.passwordTouched = true;
    if (this.password.trim() === '') {
      this.errorPassword = 'Bitte Passwort eingeben.';
    } else if (this.password.trim().length < 6) {
      this.errorPassword = 'Passwort muss mindestens 6 Zeichen lang sein.';
    } else {
      this.errorPassword = null;
    }
  }

  validateFields(): boolean {
    this.emailTouched = true;
    this.passwordTouched = true;
    
    this.errorEmail = null;
    this.errorPassword = null;
    let valid = true;
    
    if (!this.email.trim()) {
      this.errorEmail = 'Bitte E-Mail eingeben.';
      valid = false;
    } else if (!this.validateEmail(this.email)) {
      this.errorEmail = 'Bitte gültige E-Mail eingeben.';
      valid = false;
    }
    
    if (!this.password.trim()) {
      this.errorPassword = 'Bitte Passwort eingeben.';
      valid = false;
    } else if (this.password.trim().length < 6) {
      this.errorPassword = 'Passwort muss mindestens 6 Zeichen lang sein.';
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
      userNameOrEmail: this.email.trim(),
      password: this.password
    };

    this.userService.login(loginRequest).subscribe({
      next: (user) => {
        sessionStorage.setItem('userId', user.id.toString());
        sessionStorage.setItem('userName', user.userName);
        sessionStorage.setItem('userEmail', user.email);

        this.loginResult = true;
        this.bookingInProgress = false;

        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        if (err?.status === 401) {
          this.error = 'Ungültige Anmeldedaten. Bitte überprüfen Sie Ihre E-Mail und Ihr Passwort.';
        } else if (err?.status === 404) {
          this.error = 'Benutzer nicht gefunden.';
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
