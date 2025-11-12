import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { RoomService } from '../../core/room.service';
import { Room } from '../../core/models/room.model';

@Component({
  selector: 'room-preview-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './room-preview-page.component.html',
  styleUrls: ['./room-preview-page.component.scss']
})
export class RoomPreviewPageComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private roomService = inject(RoomService);

  room: Room | null = null;
  loading = true;
  error: string | null = null;

  ngOnInit() {
    // Get room ID from query parameters
    this.route.queryParams.subscribe(params => {
      const roomId = params['id'];
   if (roomId) {
      this.loadRoom(Number(roomId));
      } else {
        this.error = 'Keine Raum-ID angegeben.';
        this.loading = false;
      }
    });
  }

  loadRoom(id: number) {
    this.loading = true;
    this.error = null;

    this.roomService.getById(id).subscribe({
      next: (room) => {
   this.room = room;
      this.loading = false;
      },
      error: (err) => {
        console.error('Error loading room:', err);
   this.error = 'Raum konnte nicht geladen werden.';
        this.loading = false;
      }
    });
  }

  goBack() {
    this.router.navigate(['/rooms']);
  }
}
