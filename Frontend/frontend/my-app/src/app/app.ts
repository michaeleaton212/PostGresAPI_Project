import { Component, Inject, OnInit } from '@angular/core';
import { DOCUMENT, CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, CommonModule],
  templateUrl: './app.html',
  styleUrls: ['./app.scss']
})
export class AppComponent implements OnInit {
  selectedLocale: 'en-US' | 'de'; // only allow these two locales and one at a time
  isDropdownOpen = false; // track dropdown state


  //for the html template so it knows when to show which language as selected
  constructor(@Inject(DOCUMENT) private doc: Document) {
    this.selectedLocale = this.getLocaleFromPath(this.doc.location.pathname);
  }

  // read the locale from the current path on init
  ngOnInit() {
    this.selectedLocale = this.getLocaleFromPath(this.doc.location.pathname);
  }


  //gets called when the user selects a different language
  onLangChange(locale: string) {
    //when the user selects a language, we normalize(fix format) it to our two supported locales
    const normalized: 'en-US' | 'de' = locale === 'de' ? 'de' : 'en-US';

    //if the selected locale is the same as the current one, do nothing
    if (this.selectedLocale === normalized) {
      this.isDropdownOpen = false; // close dropdown
      return;
    }

    // Update selected locale
    this.selectedLocale = normalized;

    // calculate the new URL based on the selected locale and save in variable
    const url = new URL(this.doc.location.href);

    //current path normalized and saved in variable
    const currentPath = this.normalize(url.pathname);

    //builds new path for the selected locale and saves in variable
    const newPath = this.pathForLocale(currentPath, normalized);

    if (currentPath !== newPath) {
      this.doc.location.assign(newPath + url.search + url.hash);
    }
  }

  toggleDropdown() {
    this.isDropdownOpen = !this.isDropdownOpen;
  }

  //determine language from path
  private getLocaleFromPath(path: string): 'de' | 'en-US' {
    return /^\/de(\/|$)/.test(this.normalize(path)) ? 'de' : 'en-US';
  }

  // normalize paths to a standard format
  private normalize(p: string): string { 
    let x = p.startsWith('/') ? p : '/' + p; //variable with leading slash p stand for path 
    x = x.replace(/\/index\.html?$/i, '');// x stands for the current path
    if (x.length > 1) x = x.replace(/\/+$/, '');// remove all ending slashes except for root
    return x || '/';
  }

  private pathForLocale(p: string, loc: 'en-US' | 'de'): string {
    if (loc === 'de') {
      // Switching to German
      if (p === '/de' || p.startsWith('/de/')) return p;
      return p === '/' ? '/de' : '/de' + p;
    } else {
      // Switching to English
      if (!p.startsWith('/de')) return p;
      if (p === '/de') return '/';
      return p.substring(3); // Remove '/de' prefix
    }
  }
}
