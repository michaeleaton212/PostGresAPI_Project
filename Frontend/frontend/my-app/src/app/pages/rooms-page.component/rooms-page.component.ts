import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'rooms-page',
  standalone: true,
  imports: [CommonModule],
  template: `
 <div class="page">
 <h2>Rooms - Page</h2>
 <p>Welcome to the Rooms Page.</p>
 </div>
 `
})
export class RoomsPageComponent { }
