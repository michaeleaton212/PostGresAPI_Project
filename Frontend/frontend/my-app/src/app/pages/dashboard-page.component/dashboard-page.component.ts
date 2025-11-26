import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FooterComponent } from '../../components/core/footer/footer';



@Component({
  selector: 'dashboard-page',
  standalone: true,
  imports: [CommonModule, FooterComponent],
  templateUrl: './dashboard-page.component.html',
  styleUrls: ['./dashboard-page.component.scss']
})


export class DashboardPageComponent {

  goBackLogin() {
  }
}
