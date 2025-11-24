import { Routes } from '@angular/router';
import { HomePageComponent } from './pages/home-page.component/home-page.component';
import { RoomsPageComponent } from './pages/rooms-page.component/rooms-page.component';
import { RoomPreviewPageComponent } from './pages/room-preview-page.component/room-preview-page.component';
import { BedroomPreviewPageComponent } from './pages/bedroom-preview-page.component/bedroom-preview-page.component';
import { MeetingroomPreviewPageComponent } from './pages/meetingroom-preview-page.component/meetingroom-preview-page.component';
import { BookingPageComponent } from './pages/booking-page.component/booking-page.component';
import { LoginPageComponent } from './pages/login-page.component/login-page.component';
import { DashboardPageComponent } from './pages/dashboard-page.component/dashboard-page.component';
import { UsersPageComponent } from './pages/users-page.component/users-page.component';

export const routes: Routes = [
  { path: '', component: HomePageComponent, pathMatch: 'full' },
  { path: 'home', component: HomePageComponent },
  { path: 'rooms', component: RoomsPageComponent },
  { path: 'room-preview/bedroom', component: BedroomPreviewPageComponent },
  { path: 'room-preview/meetingroom', component: MeetingroomPreviewPageComponent },
  { path: 'rooms/preview/:type', component: RoomPreviewPageComponent },
  { path: 'booking', component: BookingPageComponent },
  { path: 'login', component: LoginPageComponent },
  { path: 'dashboard', component: DashboardPageComponent },
  { path: 'users', component: UsersPageComponent }
];

