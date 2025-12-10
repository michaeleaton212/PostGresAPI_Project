# Authentifizierung und Benutzer-spezifische Buchungen

## Änderungen

### Backend (C# .NET)

1. **IBookingRepository** - Neue Methode hinzugefügt:
   - `Task<List<Booking>> GetByName(string name)` - Ruft alle Buchungen für einen bestimmten Namen ab

2. **BookingRepository** - Implementierung:
   - `GetByName()` Methode implementiert, die alle Buchungen mit einem bestimmten Namen (case-insensitive) zurückgibt

3. **IBookingService** - Interface aktualisiert:
   - `Task<List<BookingDto>> GetByName(string name)` hinzugefügt
   - `GetBookingIdByCredentials()` geändert zu `GetBookingIdsByCredentials()` - gibt jetzt eine Liste zurück

4. **BookingService** - Implementierung:
   - `GetByName()` Methode implementiert
   - `GetBookingIdsByCredentials()` aktualisiert, um alle Buchungen für einen Benutzer zu finden

5. **LoginDtos** - Response DTO geändert:
   - `LoginResponseDto` gibt jetzt `List<int> BookingIds` statt `int BookingId` zurück

6. **ITokenService & TokenService** - Token-Handling für mehrere IDs:
   - `Create()` akzeptiert jetzt `List<int> bookingIds`
   - `TryValidate()` gibt jetzt `List<int> bookingIds` zurück
   - Token enthält komma-getrennte Liste von Buchungs-IDs

7. **BookingsController** - Neue Endpoints:
   - `POST /api/bookings/login` - Aktualisiert für mehrere Buchungs-IDs
   - `GET /api/bookings/{bookingId}/secure` - Validiert Token gegen Liste von IDs
   - `GET /api/bookings/by-name/{name}` - Neuer Endpoint zum Abrufen von Buchungen nach Name

### Frontend (Angular)

1. **BookingService** - Interface aktualisiert:
   - `LoginResponseDto` Interface angepasst: `bookingIds: number[]`
   - Neue Methode `getByName(name: string)` hinzugefügt

2. **LoginPageComponent** - Login-Logik erweitert:
   - Speichert jetzt `bookingIds` (als JSON Array), `userName` und `token` im sessionStorage
   - Nach erfolgreichem Login wird der Benutzer zum Dashboard weitergeleitet

3. **DashboardPageComponent** - Benutzer-spezifische Ansicht:
   - Prüft beim Initialisieren, ob Benutzer eingeloggt ist
   - Lädt nur Buchungen für den eingeloggten Benutzer (via `getByName()`)
   - Zeigt Benutzernamen im Header an
   - Logout-Funktion löscht alle Session-Daten

4. **AuthGuard** - Neue Route Guard:
   - Schützt das Dashboard vor unautorisierten Zugriffen
   - Leitet nicht-eingeloggte Benutzer zur Login-Seite

5. **App Routes** - Route-Schutz:
   - Dashboard-Route mit `canActivate: [authGuard]` geschützt

6. **Dashboard UI** - Verbesserungen:
   - Header zeigt "Logged in as: [Email]"
   - "Back" Button geändert zu "Logout"
   - Titel geändert von "Bookings" zu "My Bookings"

## Funktionsweise

1. **Login**: Benutzer gibt E-Mail (Name) und Buchungsnummer ein
2. **Validierung**: Backend prüft, ob Buchungsnummer mit Namen übereinstimmt
3. **Token-Erstellung**: Wenn valide, werden ALLE Buchungen mit diesem Namen gefunden
4. **Token**: Ein signiertes Token wird erstellt mit allen Buchungs-IDs des Benutzers
5. **Dashboard**: Nur die Buchungen des eingeloggten Benutzers werden angezeigt
6. **Mehrere Buchungen**: Wenn ein Benutzer mehrere Buchungen hat, werden alle angezeigt
7. **Sicherheit**: AuthGuard verhindert direkten Zugriff auf Dashboard ohne Login

## Voraussetzungen zum Testen

1. Backend muss laufen (mit PostgreSQL-Datenbank)
2. `Auth:LoginTokenSecret` muss in `appsettings.json` konfiguriert sein
3. Testdaten: Mindestens eine Buchung mit E-Mail als `title` in der Datenbank

## Test-Szenario

1. Erstelle zwei Buchungen mit derselben E-Mail (z.B. "test@example.com")
2. Logge dich mit dieser E-Mail und einer der Buchungsnummern ein
3. Dashboard sollte BEIDE Buchungen anzeigen
4. Logout und versuche direkt auf `/dashboard` zuzugreifen ? Umleitung zu Login
