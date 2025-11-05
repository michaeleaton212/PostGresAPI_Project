import { Component, Inject, LOCALE_ID } from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.html',
  styleUrls: ['./app.scss']
})
export class AppComponent {
  constructor(
    @Inject(LOCALE_ID) public currentLocale: string,
    @Inject(DOCUMENT) private doc: Document
  ) { }

  // Schaltet zwischen '/' (en-US) und '/de/*' (de) um und behält Query/Hash.
  onLangChange(locale: string) {
    const normalized: 'en-US' | 'de' = locale === 'de' ? 'de' : 'en-US';
    const url = new URL(this.doc.location.href);
    const newPath = this.pathForLocale(url.pathname, normalized);
    this.doc.location.href = newPath + url.search + url.hash; // Full reload ist korrekt
  }

  private pathForLocale(pathname: string, locale: 'en-US' | 'de'): string {
    const p = pathname.startsWith('/') ? pathname : '/' + pathname;

    if (locale === 'de') {
      // vorhandenes /de entfernen, dann sicher /de wieder voranstellen
      const core = p.replace(/^\/de(\/|$)/, '/');
      return '/de' + (core === '/' ? '/' : core);
    }

    // en-US: jegliches /de Präfix entfernen → Root-Version
    return p.replace(/^\/de(\/|$)/, '/');
  }
}
