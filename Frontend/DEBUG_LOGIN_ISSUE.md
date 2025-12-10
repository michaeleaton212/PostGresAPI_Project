# Debugging-Anleitung für Login-Problem

## Problem
Dashboard zeigt Buchungen von anderen Benutzern an (z.B. "s", "qw") statt nur die eigenen.

## Diagnose-Schritte

### 1. Backend neu starten
Das Backend muss mit den neuen Änderungen neu gestartet werden:

```bash
# Backend stoppen (Prozess ID 8304 beenden)
# Dann neu starten:
cd ../PostGresAPI
dotnet run
```

### 2. Browser-Konsole überprüfen

Nach dem Login zum Dashboard navigieren und die Browser-Konsole (F12) öffnen.

**Erwartete Logs:**
```
=== DASHBOARD INIT ===
Loading rooms and bookings for user: michael.eaton212@gmail.com
SessionStorage userName: michael.eaton212@gmail.com
SessionStorage bookingIds: [2,4,5]
Calling getByName with: michael.eaton212@gmail.com
Rooms loaded: X
User bookings received: 3
Raw bookings from API: [...]
Bookings titles: ["michael.eaton212@gmail.com", "michael.eaton212@gmail.com", ...]
```

**Wenn falsche Daten geladen werden:**
- Prüfen Sie den API-Call im Network-Tab
- URL sollte sein: `http://localhost:5000/api/bookings/by-name/michael.eaton212%40gmail.com`
- Response sollte nur Buchungen mit passendem `title` enthalten

### 3. API-Endpoint direkt testen

Testen Sie den Endpoint direkt im Browser oder mit curl:

```bash
# Ersetzen Sie EMAIL mit Ihrer E-Mail
curl http://localhost:5000/api/bookings/by-name/michael.eaton212@gmail.com
```

**Erwartete Response:**
```json
[
  {
    "id": 2,
    "title": "michael.eaton212@gmail.com",
    "roomId": 102,
    ...
  },
  {
    "id": 4,
    "title": "michael.eaton212@gmail.com",
    "roomId": 103,
    ...
  }
]
```

### 4. Datenbank prüfen

Überprüfen Sie die Buchungen in der Datenbank:

```sql
SELECT Id, Title, BookingNumber, RoomId, StartTime 
FROM "Bookings" 
ORDER BY Id;
```

**Wichtig:** Der `Title`-Wert muss EXAKT mit der E-Mail übereinstimmen (case-insensitive).

### 5. Häufige Probleme

#### Problem A: Backend nicht neu gestartet
**Symptom:** API-Endpoint `/api/bookings/by-name/{name}` gibt 404
**Lösung:** Backend neu starten

#### Problem B: Falscher userName im SessionStorage
**Symptom:** Console zeigt falschen userName
**Lösung:** 
```javascript
// In Browser-Konsole:
sessionStorage.clear();
// Neu einloggen
```

#### Problem C: Title in DB stimmt nicht mit E-Mail überein
**Symptom:** `getByName` gibt leere Liste zurück
**Lösung:** 
- Buchungen haben möglicherweise andere Werte im `Title`-Feld
- Prüfen Sie die Datenbank: `SELECT DISTINCT Title FROM "Bookings";`
- Beim Erstellen neuer Buchungen wird die E-Mail als `title` gespeichert

#### Problem D: GetByName verwendet falsche Query
**Symptom:** Alle Buchungen werden geladen
**Lösung:**
- Prüfen Sie den Network-Tab: Wird `/api/bookings` statt `/api/bookings/by-name/{name}` aufgerufen?
- Falls ja: Frontend wurde nicht neu kompiliert
- Angular Dev-Server neu starten: `ng serve`

## Lösungsschritte

1. **Backend neu starten:**
   ```bash
   cd ../PostGresAPI
   dotnet run
   ```

2. **Frontend neu kompilieren (falls nötig):**
   ```bash
   cd frontend/my-app
   ng serve
   ```

3. **Browser-Cache leeren:**
   - F12 ? Network-Tab ? "Disable cache" aktivieren
   - Oder: Ctrl+Shift+R (Hard Reload)

4. **Session-Daten löschen:**
   ```javascript
   // In Browser-Konsole (F12):
   sessionStorage.clear();
   ```

5. **Neu einloggen:**
   - Mit E-Mail (z.B. `michael.eaton212@gmail.com`)
   - Mit gültiger Buchungsnummer

6. **Logs überprüfen:**
   - Console sollte zeigen: "User bookings received: X" (nur Ihre Buchungen)
   - Nicht: "All bookings received: X" (alle Buchungen)

## Aktueller Code-Status

? **Backend:**
- `IBookingRepository`: `GetByName()` Methode vorhanden
- `BookingRepository`: Implementierung korrekt (case-insensitive)
- `IBookingService`: `GetByName()` Methode vorhanden
- `BookingService`: Implementierung korrekt
- `BookingsController`: Endpoint `/api/bookings/by-name/{name}` vorhanden

? **Frontend:**
- `BookingService`: `getByName()` Methode vorhanden
- `DashboardPageComponent`: Verwendet `getByName()` statt `getAll()`
- `AuthGuard`: Dashboard geschützt
- Debug-Logs hinzugefügt

## Wenn Problem weiterhin besteht

Senden Sie folgende Informationen:
1. Screenshot der Browser-Console-Logs
2. Screenshot des Network-Tabs (API-Calls)
3. Ausgabe von: `SELECT Id, Title, BookingNumber FROM "Bookings";`
