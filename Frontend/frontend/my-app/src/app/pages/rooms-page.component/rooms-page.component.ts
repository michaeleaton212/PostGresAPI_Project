import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { RoomService } from '../../core/room.service';
import { Room } from '../../core/models/room.model';
import { FooterComponent } from '../../components/core/footer/footer'; 




@Component({
  selector: 'rooms-page',
  standalone: true,
  imports: [CommonModule, RouterModule, FooterComponent],
  templateUrl: './rooms-page.component.html',
  styleUrls: ['./rooms-page.component.scss']
})
export class RoomsPageComponent implements OnInit {
  private roomService = inject(RoomService);
  private router = inject(Router);

  rooms: Room[] = [];
  bedrooms: Room[] = [];
  meetingrooms: Room[] = [];
  loading = true;
  error: string | null = null;

  // defines allowed types for view
  view: 'bedroom' | 'meeting' | null = 'bedroom';

  // set view gets called when user clicks on a view button
  setView(v: 'bedroom' | 'meeting') {
    // If same view is clicked again, deselect it
    this.view = this.view === v ? null : v;
  }

  ngOnInit() {
    this.loadRooms();
  }

  loadRooms() {
    this.loading = true;
    this.error = null;

    // Load all rooms
    this.roomService.getAll().subscribe({
      next: (rooms) => {
        console.log('Loaded rooms:', rooms);
        // Sort rooms by ID to maintain consistent order
        this.rooms = rooms.sort((a, b) => a.id - b.id);

        // Case-insensitive filtering
        this.bedrooms = this.rooms.filter(r =>
          r.type.toLowerCase() === 'bedroom'
        );
        this.meetingrooms = this.rooms.filter(r =>
          r.type.toLowerCase() === 'meetingroom' ||
          r.type.toLowerCase() === 'meeting room'
        );

        console.log('Bedrooms:', this.bedrooms);
        console.log('Meetingrooms:', this.meetingrooms);

        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading rooms:', err);
        this.error = 'Fehler beim Laden der Zimmer. Bitte versuchen Sie es später erneut.';
        this.loading = false;
      }
    });
  }

  // Navigation when clicking a room card or image
  openPreview(room: Room) {
    // Navigate to specific room type preview page with room ID as query parameter
    if (room.type.toLowerCase() === 'bedroom') {
      this.router.navigate(['/room-preview/bedroom'], { queryParams: { id: room.id } });
    } else if (room.type.toLowerCase() === 'meetingroom' || room.type.toLowerCase() === 'meeting room') {
      this.router.navigate(['/room-preview/meetingroom'], { queryParams: { id: room.id } });
    } else {
      // Fallback to generic preview if room type is unknown
      this.router.navigate(['/room-preview'], { queryParams: { id: room.id } });
    }
  }

  // Get the first image from comma-separated image string
  getFirstImage(imageString: string | undefined): string {
    if (!imageString) {
      return 'grey.png';
    }
    
    // Split by comma and get first image, trim whitespace
    const firstImage = imageString.split(',')[0].trim();
    return firstImage || 'grey.png';
  }

  onImageError(event: any) {
    event.target.src = 'grey.png';
  }

}
