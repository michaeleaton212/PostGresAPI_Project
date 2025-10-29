import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../core/api.service';
import { ActivatedRoute } from '@angular/router';
import { UserListComponent } from '../../components/user-list/user-list.component';
import { UserFormComponent } from '../../components/user-form/user-form.component';

@Component({
  selector: 'users-page',
  standalone: true,
  imports: [CommonModule, UserListComponent, UserFormComponent],
  template: `
    <div class="page">
      <div style="display:flex;justify-content:space-between;align-items:center;">
        <h2>Users</h2>
        <div>
          <button class="btn primary" (click)="toggleCreate()">Neuen User</button>
          <button class="btn" (click)="load()" [disabled]="loading">
            {{ loading ? 'Lädt...' : 'Aktualisieren' }}
          </button>
        </div>
      </div>

      <div *ngIf="showCreate" style="margin-top:1rem; margin-bottom:1rem;">
        <user-form (createUser)="onCreate($event)"></user-form>
      </div>

      <div *ngIf="loading" class="empty">Lade Daten…</div>

      <div *ngIf="error" class="empty">
        Fehler beim Laden: {{ errorMessage }}
        <div style="margin-top:0.5rem;">
          <button class="btn" (click)="load()">Wiederholen</button>
        </div>
        <pre style="white-space:pre-wrap;margin-top:0.75rem;color:#900">{{ error | json }}</pre>
      </div>

      <user-list
        *ngIf="!loading && !error"
        [users]="users"
        [validationErrors]="validationErrors"
        (update)="onUpdate($event)"
        (deleteUser)="onDelete($event)"
      ></user-list>
    </div>
  `
})
export class UsersPageComponent implements OnInit {
  private api = inject(ApiService);
  private route = inject(ActivatedRoute);

  users: any[] = [];
  loading = false;
  error: any = null;
  validationErrors: Record<number | string, any> | null = null;

  // show/hide create form
  showCreate = false;

  get errorMessage() {
    if (!this.error) return null;
    return (
      this.error?.message ||
      this.error?.statusText ||
      (typeof this.error === 'string' ? this.error : 'Unbekannter Fehler')
    );
  }

  ngOnInit(): void {
    // react to query param 'create' so linking to /users?create=1 shows form
    const q = this.route.snapshot.queryParamMap.get('create');
    this.showCreate = q === '1' || q === 'true';
    this.route.queryParamMap.subscribe((map) => {
      const v = map.get('create');
      this.showCreate = v === '1' || v === 'true';
    });
    this.load();
  }

  toggleCreate() {
    this.showCreate = !this.showCreate;
  }

  load() {
    this.loading = true;
    this.error = null;
    this.validationErrors = null;

    this.api.get<any[]>('users').subscribe({
      next: (d) => {
        this.users = d || [];
        this.loading = false;
      },
      error: (e: any) => {
        console.error('Load users failed', e);
        this.error = e;
        this.loading = false;
      }
    });
  }

  onCreate(payload: { name: string; email: string }) {
    // Anforderungspayload anpassen, wenn der Backend-Server andere Schlüssel erwartet
    const body = { UserName: payload.name, Email: payload.email, Phone: '' };
    this.api.post<any>('users', body).subscribe({
      next: () => {
        this.showCreate = false;
        this.load();
      },
      error: (e: any) => {
        console.error('Create user failed', e);
        this.error = e;
      }
    });
  }

  onUpdate(payload: { id: number; UserName: string; Phone: string; Email: string }) {
    // Frühere Validierungsfehler für diese ID entfernen
    if (this.validationErrors) delete this.validationErrors[payload.id];

    this.api.put<any>(`users/${payload.id}`, payload).subscribe({
      next: () => {
        this.load();
      },
      error: (e: any) => {
        console.error(e);

        if (e?.status === 400 && e?.error) {
          this.validationErrors = this.validationErrors || {};
          this.validationErrors[payload.id] = e.error.errors || e.error;
        } else {
          this.error = e;
        }
      }
    });
  }

  onDelete(id: number) {
    this.api.delete<any>(`users/${id}`).subscribe({
      next: () => {
        this.load();
      },
      error: (e: any) => {
        console.error(e);
        this.error = e;
      }
    });
  }
}
