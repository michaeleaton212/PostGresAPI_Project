import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'room-preview-page',
  standalone: true,
  imports: [CommonModule],
  template: `
 <div class="page">
 <h2>Room Preview</h2>
 <p>Welcome to the Room Preview Page.</p>
 </div>
 `
})
export class RoomPreviewPageComponent { }
