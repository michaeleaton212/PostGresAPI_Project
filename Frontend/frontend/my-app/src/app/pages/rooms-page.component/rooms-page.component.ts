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

  setView(v: 'bedroom' | 'meeting') {
    this.view = this.view === v ? null : v;
  }

  ngOnInit() {
    this.loadRooms();
  }

  loadRooms() {
    this.loading = true;
    this.error = null;

    this.roomService.getAll().subscribe({
      next: (rooms) => {
        console.log('Loaded rooms:', rooms);
        // Sort rooms by ID to maintain consistent order
        this.rooms = rooms.sort((a, b) => a.id - b.id);

        // Case-insensitive filtering
        this.bedrooms = this.rooms.filter(r =>
          r.type.toLowerCase() === 'bedroom'
        );
        this.meetingrooms = this.rooms.filter(r => {
          const t = r.type.toLowerCase();
          return t === 'meetingroom' || t === 'meeting room';
        });

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
    const type = room.type.toLowerCase();
    if (type === 'bedroom') {
      this.router.navigate(['/room-preview/bedroom'], { queryParams: { id: room.id } });
    } else if (type === 'meetingroom' || type === 'meeting room') {
      this.router.navigate(['/room-preview/meetingroom'], { queryParams: { id: room.id } });
    } else {
      this.router.navigate(['/room-preview'], { queryParams: { id: room.id } });
    }
  }

  /**
   * Returns the first image from a comma-separated string and normalizes it.
   * - picks first non-empty entry
   * - strips quotes
   * - keeps absolute URLs
   * - replaces leading "/public/" or "public/" with "/"
   * - ensures leading "/" for relative asset paths
   * - falls back to "/grey.png"
   */
  getFirstImage(imageString: string | undefined): string {
    const FALLBACK = '/grey.png';
    if (!imageString) return FALLBACK;

    const firstRaw = imageString
      .split(',')
      .map(s => s.trim())
      .find(s => s.length > 0) || '';

    if (!firstRaw) return FALLBACK;

    let url = firstRaw.replace(/^['"]|['"]$/g, '');

    // absolute URL? leave as-is
    if (/^https?:\/\//i.test(url)) return url;

    // drop leading "/public/" or "public/"
    url = url.replace(/^\/?public\//, '/');

    // ensure leading slash for relative paths
    if (!url.startsWith('/')) url = `/${url}`;

    return url || FALLBACK;
  }

  onImageError(event: any) {
    event.target.src = '/grey.png';
  }
}
