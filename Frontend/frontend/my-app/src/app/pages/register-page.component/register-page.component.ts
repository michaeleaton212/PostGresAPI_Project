import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { FooterComponent } from '../../components/core/footer/footer';
import { UserService } from '../../core/user.service';

@Component({
  selector: 'register-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, FooterComponent],
  templateUrl: './register-page.component.html',
  styleUrls: ['./register-page.component.scss']
})
export class RegisterPageComponent {
  private userService = inject(UserService);
  private router = inject(Router);

  userName = '';
  email = '';
  phone = '';
  password = '';
  confirmPassword = '';
  registrationInProgress = false;

  registrationResult: boolean | null = null;
  error: string | null = null;

  errorUserName: string | null = null;
  errorEmail: string | null = null;
  errorPhone: string | null = null;
  errorPassword: string | null = null;
  errorConfirmPassword: string | null = null;

  userNameTouched = false;
  emailTouched = false;
  phoneTouched = false;
  passwordTouched = false;
  confirmPasswordTouched = false;

  validateEmail(email: string): boolean {
    if (!email.trim()) return false;
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.trim());
  }

  validatePhone(phone: string): boolean {
    if (!phone.trim()) return true; // Phone is optional
    return /^[\d\s\-\+\(\)]+$/.test(phone.trim());
  }

  onUserNameInput(): void {
    this.userNameTouched = true;
    if (this.userName.trim() === '') {
      this.errorUserName = 'Bitte Benutzernamen eingeben.';
    } else if (this.userName.trim().length < 3) {
      this.errorUserName = 'Benutzername muss mindestens 3 Zeichen lang sein.';
    } else {
      this.errorUserName = null;
    }
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

  onPhoneInput(): void {
    this.phoneTouched = true;
    if (this.phone.trim() !== '' && !this.validatePhone(this.phone)) {
      this.errorPhone = 'Bitte gültige Telefonnummer eingeben.';
    } else {
      this.errorPhone = null;
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
    
    // Also validate confirm password if it's been touched
    if (this.confirmPasswordTouched) {
      this.onConfirmPasswordInput();
    }
  }

  onConfirmPasswordInput(): void {
    this.confirmPasswordTouched = true;
    if (this.confirmPassword.trim() === '') {
      this.errorConfirmPassword = 'Bitte Passwort bestätigen.';
    } else if (this.confirmPassword !== this.password) {
      this.errorConfirmPassword = 'Passwörter stimmen nicht überein.';
    } else {
      this.errorConfirmPassword = null;
    }
  }

  validateFields(): boolean {
    this.userNameTouched = true;
    this.emailTouched = true;
    this.phoneTouched = true;
    this.passwordTouched = true;
    this.confirmPasswordTouched = true;
    
    this.errorUserName = null;
    this.errorEmail = null;
    this.errorPhone = null;
    this.errorPassword = null;
    this.errorConfirmPassword = null;
    
    let valid = true;
    
    if (!this.userName.trim()) {
      this.errorUserName = 'Bitte Benutzernamen eingeben.';
      valid = false;
    } else if (this.userName.trim().length < 3) {
      this.errorUserName = 'Benutzername muss mindestens 3 Zeichen lang sein.';
      valid = false;
    }
    
    if (!this.email.trim()) {
      this.errorEmail = 'Bitte E-Mail eingeben.';
      valid = false;
    } else if (!this.validateEmail(this.email)) {
      this.errorEmail = 'Bitte gültige E-Mail eingeben.';
      valid = false;
    }
    
    if (this.phone.trim() !== '' && !this.validatePhone(this.phone)) {
      this.errorPhone = 'Bitte gültige Telefonnummer eingeben.';
      valid = false;
    }
    
    if (!this.password.trim()) {
      this.errorPassword = 'Bitte Passwort eingeben.';
      valid = false;
    } else if (this.password.trim().length < 6) {
      this.errorPassword = 'Passwort muss mindestens 6 Zeichen lang sein.';
      valid = false;
    }
    
    if (!this.confirmPassword.trim()) {
      this.errorConfirmPassword = 'Bitte Passwort bestätigen.';
      valid = false;
    } else if (this.confirmPassword !== this.password) {
      this.errorConfirmPassword = 'Passwörter stimmen nicht überein.';
      valid = false;
    }
    
    return valid;
  }

  onRegister(): void {
    if (this.registrationInProgress) return;
    
    this.error = null;
    this.registrationResult = null;
    
    if (!this.validateFields()) return;
    
    this.registrationInProgress = true;

    const registerRequest = {
      userName: this.userName.trim(),
      email: this.email.trim(),
      phone: this.phone.trim(),
      password: this.password
    };

    this.userService.register(registerRequest).subscribe({
      next: (user) => {
        console.log('Registration successful:', user);
        this.registrationResult = true;
        this.registrationInProgress = false;
        
        // Automatically log in the user
        sessionStorage.setItem('userId', user.id.toString());
        sessionStorage.setItem('userName', user.userName);
        sessionStorage.setItem('userEmail', user.email);
        
        // Navigate to rooms page
        setTimeout(() => {
          this.router.navigate(['/rooms']);
        }, 1000);
      },
      error: (err) => {
        console.error('Registration error:', err);
        
        if (err?.status === 400) {
          this.error = err.error?.error || 'Registrierung fehlgeschlagen. Bitte überprüfen Sie Ihre Eingaben.';
        } else if (err?.status === 0) {
          this.error = 'Verbindung zum Server fehlgeschlagen. Bitte starten Sie das Backend.';
        } else if (err?.error?.error) {
          this.error = err.error.error;
        } else if (err?.error?.message) {
          this.error = err.error.message;
        } else {
          this.error = 'Registrierung fehlgeschlagen. Bitte später erneut versuchen.';
        }
        
        this.registrationInProgress = false;
        this.registrationResult = false;
      }
    });
  }

  onKeyDown(event: KeyboardEvent): void {
    if (event.key === 'Enter') {
      event.preventDefault();
      this.onRegister();
    }
  }
}
