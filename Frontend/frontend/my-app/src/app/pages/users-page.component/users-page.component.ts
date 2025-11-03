import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../core/api.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'users-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './users-page.component.html',
  styleUrls: ['./users-page.component.scss']
})
export class UsersPageComponent implements OnInit {
  private api = inject(ApiService);
  private route = inject(ActivatedRoute);

  users: any[] = [];
  loading = false;
  error: any = null;
  validationErrors: Record<number | string, any> | null = null;

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

  onSubmit(e: Event) {
    e.preventDefault();
    const form = e.target as HTMLFormElement;
    const data = new FormData(form);
    const name = (data.get('name') as string) || '';
    const email = (data.get('email') as string) || '';
    this.onCreate({ name, email });
    form.reset();
  }

  onUpdate(payload: { id: number; UserName: string; Phone: string; Email: string }) {
    if (this.validationErrors) delete this.validationErrors[payload.id];

    this.api.put<any>(`users/${payload.id}`, payload).subscribe({
      next: () => this.load(),
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
      next: () => this.load(),
      error: (e: any) => {
        console.error(e);
        this.error = e;
      }
    });
  }

  trackById = (_: number, u: any) => u.id;

  onEdit(u: any) {
    const name = prompt('Name', u.UserName || u.userName || u.name || '');
    if (name === null) return;

    const email = prompt('Email', u.Email || u.email || '');
    if (email === null) return;

    const phone = prompt('Phone', u.Phone || u.phone || '');
    if (phone === null) return;

    this.onUpdate({ id: u.id, UserName: name, Phone: phone, Email: email });
  }
}
