import { HomePageComponent } from './pages/home-page.component/home-page.component';
import { RoomsPageComponent } from './pages/rooms-page.component/rooms-page.component';
import { RoomPreviewPageComponent } from './pages/room-preview-page.component/room-preview-page.component';
import { BookingPageComponent } from './pages/booking-page.component/booking-page.component';
import { LoginPageComponent } from './pages/login-page.component/login-page.component';
import { DashboardPageComponent } from './pages/dashboard-page.component/dashboard-page.component';
import { UsersPageComponent } from './pages/users-page.component/users-page.component';

export const routes: any[] = [
  { path: '', redirectTo: 'users', pathMatch: 'full' },
  { path: 'home', component: HomePageComponent },
  { path: 'rooms', component: RoomsPageComponent },
  { path: 'room-preview', component: RoomPreviewPageComponent },
  { path: 'booking', component: BookingPageComponent },
  { path: 'login', component: LoginPageComponent },
  { path: 'dashboard', component: DashboardPageComponent },
  { path: 'users', component: UsersPageComponent },
];
