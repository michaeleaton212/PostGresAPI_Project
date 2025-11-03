import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router'; // import RouterModule 

@Component({
  selector: 'home-page',
  standalone: true,
  imports: [CommonModule, RouterModule], // add RouterModule
  templateUrl: './home-page.component.html',
  styleUrls: ['./home-page.component.scss']
})
export class HomePageComponent { }
