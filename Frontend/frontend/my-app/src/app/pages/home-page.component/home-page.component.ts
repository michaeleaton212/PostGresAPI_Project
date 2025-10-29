import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
 selector: 'home-page',
 standalone: true,
 imports: [CommonModule],
 template: `
 <div class="page">
 <h2>Home</h2>
 <p>Welcome to the home page.</p>
 </div>
 `
})
export class HomePageComponent {}
