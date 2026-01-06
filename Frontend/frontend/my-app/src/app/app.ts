import { Component, Inject, OnInit } from '@angular/core';
import { DOCUMENT, CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive, Router } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, CommonModule],
  templateUrl: './app.html',
  styleUrls: ['./app.scss']
})
export class AppComponent implements OnInit {
  selectedLocale: 'en-US' | 'de';
  isDropdownOpen = false;
  isUserDropdownOpen = false;
  
  // User state
  isLoggedIn = false;
  userName = '';
  userEmail = '';

  constructor(
    @Inject(DOCUMENT) private doc: Document,
    private router: Router
  ) {
    this.selectedLocale = this.getLocaleFromPath(this.doc.location.pathname);
  }

  ngOnInit() {
    this.selectedLocale = this.getLocaleFromPath(this.doc.location.pathname);
    this.checkUserSession();
    
    // Listen to route changes to update user state
    this.router.events.subscribe(() => {
      this.checkUserSession();
    });
  }

  checkUserSession() {
    const userId = sessionStorage.getItem('userId');
    const userName = sessionStorage.getItem('userName');
    const userEmail = sessionStorage.getItem('userEmail');
    
    this.isLoggedIn = !!(userId && userName && userEmail);
    this.userName = userName || '';
    this.userEmail = userEmail || '';
  }

  onLangChange(locale: string) {
    const normalized: 'en-US' | 'de' = locale === 'de' ? 'de' : 'en-US';

    if (this.selectedLocale === normalized) {
      this.isDropdownOpen = false;
      return;
    }

    this.selectedLocale = normalized;

    const url = new URL(this.doc.location.href);
    const currentPath = this.normalize(url.pathname);
    const newPath = this.pathForLocale(currentPath, normalized);

    if (currentPath !== newPath) {
      this.doc.location.assign(newPath + url.search + url.hash);
    }
  }

  toggleDropdown() {
    this.isDropdownOpen = !this.isDropdownOpen;
  }

  toggleUserDropdown() {
    this.isUserDropdownOpen = !this.isUserDropdownOpen;
  }

  logout() {
    sessionStorage.removeItem('userId');
    sessionStorage.removeItem('userName');
    sessionStorage.removeItem('userEmail');
    this.isLoggedIn = false;
    this.userName = '';
    this.userEmail = '';
    this.isUserDropdownOpen = false;
    this.router.navigate(['/login']);
  }

  goToLogin() {
    this.router.navigate(['/login']);
  }

  goToDashboard() {
    this.isUserDropdownOpen = false;
    this.router.navigate(['/dashboard']);
  }

  goToWriteReview() {
    this.isUserDropdownOpen = false;
    this.router.navigate(['/reviews'], { queryParams: { write: 'true' } });
  }

  private getLocaleFromPath(path: string): 'de' | 'en-US' {
    return /^\/de(\/|$)/.test(this.normalize(path)) ? 'de' : 'en-US';
  }

  private normalize(p: string): string { 
    let x = p.startsWith('/') ? p : '/' + p;
    x = x.replace(/\/index\.html?$/i, '');
    if (x.length > 1) x = x.replace(/\/+$/, '');
    return x || '/';
  }

  private pathForLocale(p: string, loc: 'en-US' | 'de'): string {
    if (loc === 'de') {
      if (p === '/de' || p.startsWith('/de/')) return p;
      return p === '/' ? '/de' : '/de' + p;
    } else {
      if (!p.startsWith('/de')) return p;
      if (p === '/de') return '/';
      return p.substring(3);
    }
  }
}
