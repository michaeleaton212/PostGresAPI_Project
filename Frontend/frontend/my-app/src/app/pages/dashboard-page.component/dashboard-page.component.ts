import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'dashboard-page',
  standalone: true,
  imports: [CommonModule],
  template: `
 <div class="page">
 <h2>Dashboard</h2>
 <p>Welcome to the Dashboard Page.</p>
 </div>
 `
})
export class DashboardPageComponent { }
