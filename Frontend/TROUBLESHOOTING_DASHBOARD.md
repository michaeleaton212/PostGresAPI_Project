# SCHRITT-FÜR-SCHRITT: Dashboard zeigt alle Buchungen statt nur eigene

## Problem
Dashboard zeigt alle Buchungen (z.B. "qw", "s", "michael.eaton212@gmail.com") statt nur die des eingeloggten Users.

## Ursache
Das Backend wurde noch nicht neu gestartet und der neue Endpoint `POST /api/bookings/by-ids` ist nicht verfügbar.

---

## ? LÖSUNG (GENAU DIESE SCHRITTE BEFOLGEN)

### Schritt 1: Backend STOPPEN
```powershell
# Task Manager öffnen (Ctrl+Shift+Esc)
# Suchen Sie nach "PostGresAPI" oder "dotnet"
# Rechtsklick ? Task beenden
```

### Schritt 2: Backend NEU STARTEN
```powershell
cd C:\PostGresAPI_Project\PostGresAPI_Project\PostGresAPI
dotnet run
```

**Wichtig:** Warten Sie bis Sie diese Nachricht sehen:
```
Now listening on: http://localhost:5000
Application started. Press Ctrl+C to shut down.
```

### Schritt 3: Browser Cache KOMPLETT leeren
```
1. Öffnen Sie Chrome/Edge DevTools (F12)
2. Rechtsklick auf den Reload-Button
3. Wählen Sie "Empty Cache and Hard Reload"
```

### Schritt 4: Session-Daten löschen
```javascript
// Im Browser Console (F12 ? Console):
sessionStorage.clear();
localStorage.clear();
```

### Schritt 5: Zur Login-Seite navigieren
```
http://localhost:4200/login
```

### Schritt 6: Einloggen mit korrekten Daten
```
E-Mail: michael.eaton212@gmail.com
Buchungsnummer: FED62502  (oder eine andere gültige Nummer)
```

---

## ?? WAS IN DER CONSOLE ERSCHEINEN SOLLTE

### Bei erfolgreichem Login:
```
=== DASHBOARD INIT ===
Session check: {
  bookingIdsStr: "[2,4,5,7]",
  userName: "michael.eaton212@gmail.com",
  hasToken: true
}
User logged in as: michael.eaton212@gmail.com
User booking IDs: [2, 4, 5, 7]
```

### Beim Laden der Buchungen:
```
=== LOADING BOOKINGS ===
User: michael.eaton212@gmail.com
Booking IDs to load: [2, 4, 5, 7]
Number of IDs: 4
Calling POST /api/bookings/by-ids with: [2, 4, 5, 7]
=== BOOKINGS LOADED SUCCESSFULLY ===
Number of bookings received: 4
Booking IDs received: [2, 4, 5, 7]
Booking titles: ["michael.eaton212@gmail.com", "michael.eaton212@gmail.com", ...]
```

### ? WENN FEHLER AUFTRETEN:

**Fehler 1: "404 Not Found" für `/api/bookings/by-ids`**
```
=== ERROR LOADING BOOKINGS ===
Error status: 404
```
**Lösung:** Backend wurde noch nicht neu gestartet ? Zurück zu Schritt 1

**Fehler 2: "401 Unauthorized"**
```
Error status: 401
```
**Lösung:** Token ist abgelaufen ? Session löschen (Schritt 4) und neu einloggen

**Fehler 3: Alle Buchungen werden geladen**
```
Number of bookings received: 10
Booking titles: ["qw", "s", "michael.eaton212@gmail.com", ...]
```
**Lösung:** Backend verwendet noch alten Code ? Backend-Prozess komplett beenden und neu starten

---

## ?? NETWORK-TAB ÜBERPRÜFEN

Öffnen Sie DevTools (F12) ? Network Tab

### Nach Login sollte zu sehen sein:
```
POST /api/bookings/login
Status: 200
Response: {
  "bookingIds": [2, 4, 5, 7],
  "token": "eyJhbGci..."
}
```

### Nach Dashboard-Load sollte zu sehen sein:
```
POST /api/bookings/by-ids
Status: 200
Request Payload: [2, 4, 5, 7]
Response: [
  { "id": 2, "title": "michael.eaton212@gmail.com", ... },
  { "id": 4, "title": "michael.eaton212@gmail.com", ... },
  ...
]
```

### ? NICHT zu sehen sein sollte:
```
GET /api/bookings          ? FALSCH! Dieser Endpoint lädt ALLE Buchungen
GET /api/bookings/by-name/... ? FALSCH! Alter Fallback
```

---

## ?? BACKEND TESTEN (Optional)

### Test 1: Login-Endpoint
```powershell
curl -X POST http://localhost:5000/api/bookings/login `
  -H "Content-Type: application/json" `
  -d '{"bookingNumber":"FED62502","name":"michael.eaton212@gmail.com"}'
```

**Erwartete Response:**
```json
{
  "bookingIds": [2, 4, 5, 7],
  "token": "base64encodedtoken..."
}
```

### Test 2: By-IDs Endpoint
```powershell
curl -X POST http://localhost:5000/api/bookings/by-ids `
  -H "Content-Type: application/json" `
  -d '[2,4,5,7]'
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
  ...
]
```

---

## ? CHECKLISTE

- [ ] Backend wurde KOMPLETT beendet (Task Manager)
- [ ] Backend wurde NEU gestartet (`dotnet run`)
- [ ] Backend zeigt "Now listening on: http://localhost:5000"
- [ ] Browser Cache wurde geleert (Hard Reload)
- [ ] SessionStorage wurde gelöscht (`sessionStorage.clear()`)
- [ ] Neu eingeloggt mit korrekter E-Mail + Buchungsnummer
- [ ] Console zeigt: "Calling POST /api/bookings/by-ids"
- [ ] Network Tab zeigt: POST zu `/api/bookings/by-ids`
- [ ] Nur eigene Buchungen werden angezeigt

---

## ?? WENN ES IMMER NOCH NICHT FUNKTIONIERT

1. **Screenshot der Browser Console** (alle Logs)
2. **Screenshot des Network Tabs** (alle API-Calls)
3. **Backend Console Output** kopieren
4. Senden Sie diese 3 Dinge zur weiteren Analyse

---

## ?? ZUSAMMENFASSUNG

Das Problem ist definitiv, dass:
1. ? Backend läuft mit alter Version (ohne `/api/bookings/by-ids` Endpoint)
2. ? Frontend cached die alte Version
3. ? Session enthält alte/ungültige Daten

Die Lösung ist:
1. ? Backend NEU STARTEN
2. ? Browser HART neu laden
3. ? Session LÖSCHEN
4. ? NEU EINLOGGEN
