import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LOCALE_ID } from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { AppComponent } from './app';
import { provideRouter } from '@angular/router';

describe('AppComponent Language Switcher', () => {
  let component: AppComponent;
  let fixture: ComponentFixture<AppComponent>;
  let mockDocument: any;

  beforeEach(async () => {
    // Create a more realistic mock document
    mockDocument = {
      location: {
        pathname: '/',
        href: 'http://localhost:4200/',
        search: '',
        hash: '',
        assign: jasmine.createSpy('assign')
      }
    };

    await TestBed.configureTestingModule({
      imports: [AppComponent],
      providers: [
      provideRouter([]),
        { provide: LOCALE_ID, useValue: 'en-US' },
        { provide: DOCUMENT, useValue: mockDocument }
]
    }).compileComponents();

    fixture = TestBed.createComponent(AppComponent);
  component = fixture.componentInstance;
    fixture.detectChanges();
  });

  describe('Language Detection', () => {
    it('should create the app component', () => {
      expect(component).toBeTruthy();
    });

    it('should detect English when on root path', () => {
      mockDocument.location.pathname = '/';
      component.ngOnInit();
      expect(component.selectedLocale).toBe('en-US');
    });

    it('should detect German when on /de path', () => {
      mockDocument.location.pathname = '/de';
      component.ngOnInit();
      expect(component.selectedLocale).toBe('de');
    });

    it('should detect German when on /de/rooms path', () => {
      mockDocument.location.pathname = '/de/rooms';
      component.ngOnInit();
      expect(component.selectedLocale).toBe('de');
    });
  });

  describe('Language Switching', () => {
    it('should not navigate when already on target language', () => {
      mockDocument.location.pathname = '/de';
      component.ngOnInit();
      
      component.onLangChange('de');
      
   // Should not call assign when already on German
      expect(mockDocument.location.assign).not.toHaveBeenCalled();
    });

    it('should navigate when switching from English to German', () => {
      mockDocument.location.pathname = '/';
      mockDocument.location.href = 'http://localhost:4200/';
      component.ngOnInit();
      
      component.onLangChange('de');
   
      expect(mockDocument.location.assign).toHaveBeenCalledWith('/de');
    });

    it('should navigate when switching from German to English', () => {
      mockDocument.location.pathname = '/de';
      mockDocument.location.href = 'http://localhost:4200/de';
      component.ngOnInit();
   
  component.onLangChange('en-US');
      
    expect(mockDocument.location.assign).toHaveBeenCalledWith('/');
    });
  });

  describe('UI Elements', () => {
    it('should render language selector', () => {
      const compiled = fixture.nativeElement as HTMLElement;
      const select = compiled.querySelector('#lang');
      expect(select).toBeTruthy();
    });

    it('should have correct value binding', () => {
      mockDocument.location.pathname = '/de';
      component.ngOnInit();
      fixture.detectChanges();
      
      const compiled = fixture.nativeElement as HTMLElement;
 const select = compiled.querySelector('#lang') as HTMLSelectElement;
      expect(select.value).toBe('de');
    });
  });
});
