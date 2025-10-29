import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'login-page',
  standalone: true,
  imports: [CommonModule],
  template: `
 <div class="page">
 <h2>Login</h2>
 <p>Welcome to the Login Page.</p>
 </div>
 `
})
export class LoginPageComponent { }
