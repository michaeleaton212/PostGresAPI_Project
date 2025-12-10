# Sofort-Checkliste: Dashboard zeigt falsche Buchungen

## ? WICHTIG: Diese Schritte MÜSSEN durchgeführt werden

### Schritt 1: Backend neu starten
```bash
# 1. Stoppen Sie das laufende Backend (Task Manager ? PostGresAPI.exe beenden)
# 2. Neu starten:
cd C:\PostGresAPI_Project\PostGresAPI_Project\PostGresAPI
dotnet run
```

**Warum?** Das Backend läuft noch mit der alten Version ohne den `/api/bookings/by-name/{name}` Endpoint.

### Schritt 2: Browser-Cache leeren
```
1. Öffnen Sie die Anwendung
2. Drücken Sie F12 (Developer Tools)
3. Rechtsklick auf Refresh-Button
4. Wählen Sie "Empty Cache and Hard Reload"
```

### Schritt 3: Session löschen und neu einloggen
```javascript
// In Browser Console (F12):
sessionStorage.clear();
```

Dann zur Login-Seite und neu einloggen mit:
- **E-Mail:** Die E-Mail, die Sie beim Erstellen der Buchung verwendet haben
- **Buchungsnummer:** Eine gültige Buchungsnummer für diese E-Mail

### Schritt 4: Console überprüfen

Nach dem Login sollten Sie im Dashboard folgendes sehen:

**? RICHTIG:**
```
=== DASHBOARD INIT ===
Session check: {bookingIdsStr: "[1,2,3]", userName: "michael.eaton212@gmail.com", hasToken: true}
User logged in as: michael.eaton212@gmail.com
Loading rooms and bookings for user: michael.eaton212@gmail.com
Calling getByName with: michael.eaton212@gmail.com
User bookings received: 3
```

**? FALSCH:**
```
All bookings received: 10
```

## Wenn es immer noch nicht funktioniert

### Test 1: API-Endpoint direkt testen

Öffnen Sie im Browser:
```
http://localhost:5000/api/bookings/by-name/michael.eaton212@gmail.com
```

Ersetzen Sie die E-Mail mit Ihrer tatsächlichen E-Mail.

**Erwartetes Ergebnis:** JSON-Array nur mit Ihren Buchungen

**Wenn 404 Fehler:** Backend wurde nicht neu gestartet oder kompiliert nicht

### Test 2: Alle Buchungen prüfen

```
http://localhost:5000/api/bookings
```

Prüfen Sie das `title`-Feld jeder Buchung. Es sollte die E-Mail enthalten.

**Wenn `title` leer oder anders:** 
- Das ist das Problem!
- Buchungen wurden mit anderem Namen erstellt
- Lösung: Neue Test-Buchung erstellen mit korrekter E-Mail

### Test 3: Network-Tab prüfen

1. F12 öffnen
2. Network-Tab öffnen
3. Zum Dashboard navigieren
4. Prüfen Sie welche API-Calls gemacht werden

**? Sollte sein:**
```
GET /api/bookings/by-name/michael.eaton212@gmail.com
```

**? Nicht:**
```
GET /api/bookings
```

## Datenbank-Test (Optional)

Falls Sie direkten Zugriff auf die PostgreSQL-Datenbank haben:

```sql
-- Zeige alle Buchungen mit ihrem Title
SELECT 
    "Id",
    "Title",
    "BookingNumber",
    "RoomId",
    "StartTime"
FROM "Bookings"
ORDER BY "Id";

-- Zeige nur Buchungen für eine bestimmte E-Mail
SELECT * 
FROM "Bookings" 
WHERE LOWER("Title") = LOWER('michael.eaton212@gmail.com');
```

## Quick-Fix: Manuell filtern (Temporär)

Falls Backend-Neustart nicht funktioniert, können Sie temporär im Frontend filtern:

**NICHT EMPFOHLEN** (nur für Test):

```typescript
// In dashboard-page.component.ts, loadBookings():
this.bookingService.getAll().subscribe({
  next: (bookings) => {
    // Manuell nach userName filtern
    const userBookings = bookings.filter(b => 
      b.title?.toLowerCase() === this.userName.toLowerCase()
    );
    console.log('Filtered bookings:', userBookings.length);
    this.bookings = userBookings.map(b => this.mapBookingToDisplay(b));
  }
});
```

## Zusammenfassung

**Das Problem ist höchstwahrscheinlich:**
1. ? Backend läuft noch mit alter Version ? Neu starten!
2. ? Browser-Cache zeigt alte Version ? Hard Reload!
3. ? Session enthält alte Daten ? SessionStorage leeren!

**Die Lösung:**
1. ? Backend neu starten
2. ? Browser Cache leeren (Hard Reload)
3. ? SessionStorage leeren
4. ? Neu einloggen
5. ? Console-Logs überprüfen
