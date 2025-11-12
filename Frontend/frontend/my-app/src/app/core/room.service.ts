import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Room } from './models/room.model';

@Injectable({ providedIn: 'root' })
export class RoomService {
  private api = inject(ApiService);

  /**
   * Get all rooms or filter by type
   * @param type Optional filter: 'Bedroom' or 'Meetingroom'
   */
  getAll(type?: string): Observable<Room[]> {
    const params = type ? { type } : undefined;
    return this.api.get<Room[]>('rooms', params);
  }

  /**
   * Get a single room by ID
   */
  getById(id: number): Observable<Room> {
    return this.api.get<Room>(`rooms/${id}`);
  }
}
