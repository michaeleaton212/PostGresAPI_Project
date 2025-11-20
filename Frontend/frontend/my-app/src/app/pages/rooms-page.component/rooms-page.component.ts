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
    // Variante A: als Query-Param (/room-preview?id=123)
    this.router.navigate(['/room-preview'], { queryParams: { id: room.id } });

    // Variante B (falls deine Route /room-preview/:id ist):
    // this.router.navigate(['/room-preview', room.id]);
  }
}
