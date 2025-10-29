import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'user-list',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div *ngIf="!users || users.length === 0" class="empty">Keine Benutzer gefunden.</div>

    <ul *ngIf="users && users.length > 0" style="list-style:none;padding:0">
      <li
        *ngFor="let u of users; trackBy: trackById"
        style="display:flex;justify-content:space-between;align-items:center;padding:0.5rem 0;border-bottom:1px solid #eee"
      >
        <div>
          <div style="font-weight:600">{{ u.UserName || u.userName || u.name || '—' }}</div>
          <div style="font-size:0.9rem;color:#666">
            {{ u.Email || u.email }}
            <span *ngIf="u.Phone || u.phone">• {{ u.Phone || u.phone }}</span>
          </div>

          <div *ngIf="validationErrors && validationErrors[u.id]" style="color:#900;margin-top:0.25rem">
            {{ validationErrors[u.id] | json }}
          </div>
        </div>

        <div style="display:flex;gap:0.5rem">
          <button class="btn" (click)="onEdit(u)">Bearbeiten</button>
          <button class="btn" (click)="deleteUser.emit(u.id)">Löschen</button>
        </div>
      </li>
    </ul>
  `
})
export class UserListComponent {
  @Input() users: any[] = [];
  @Input() validationErrors: Record<number | string, any> | null = null;

  // Falls dein Backend PascalCase erwartet, bleibe bei diesen Keys.
  // Wenn camelCase benötigt wird, passe das Interface und das emit unten an.
  @Output() update = new EventEmitter<{ id: number; UserName: string; Phone: string; Email: string }>();
  @Output() deleteUser = new EventEmitter<number>();

  trackById = (_: number, u: any) => u.id;

  onEdit(u: any) {
    const name = prompt('Name', u.UserName || u.userName || u.name || '');
    if (name === null) return;

    const email = prompt('Email', u.Email || u.email || '');
    if (email === null) return;

    const phone = prompt('Phone', u.Phone || u.phone || '');
    if (phone === null) return;

    this.update.emit({ id: u.id, UserName: name, Phone: phone, Email: email });
    // Falls camelCase nötig:
    // this.update.emit({ id: u.id, userName: name, phone, email });
  }
}
