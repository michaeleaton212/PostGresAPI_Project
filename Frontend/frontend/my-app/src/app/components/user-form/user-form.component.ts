import { Component, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
 selector: 'user-form',
 standalone: true,
 imports: [CommonModule],
 template: `
 <form (submit)="onSubmit($event)" style="max-width:28rem">
 <label style="display:block;margin-bottom:0.5rem">
 Name<br />
 <input name="name" required />
 </label>
 <label style="display:block;margin-bottom:0.5rem">
 Email<br />
 <input name="email" type="email" required />
 </label>
 <div style="margin-top:0.5rem">
 <button class="btn primary" type="submit">Erstellen</button>
 </div>
 </form>
 `
})
export class UserFormComponent {
 @Output() createUser = new EventEmitter<{ name: string; email: string }>();

 onSubmit(e: Event) {
 e.preventDefault();
 const form = e.target as HTMLFormElement;
 const data = new FormData(form);
 const name = (data.get('name') as string) || '';
 const email = (data.get('email') as string) || '';
 this.createUser.emit({ name, email });
 form.reset();
 }
}
