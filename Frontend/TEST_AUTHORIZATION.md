# Test-Anleitung: Token-basierte Autorisierung

## Vorbereitung

1. **Backend stoppen** (falls läuft)
2. **Backend neu starten**:
   ```bash
   cd PostGresAPI
   dotnet run
   ```

## Test-Szenarien

### Test 1: Erfolgreicher Login und Anzeige eigener Buchungen

1. Öffne die Angular-App: `http://localhost:4200/login`
2. Logge dich mit gültigen Credentials ein:
   - E-Mail: `user1@test.com` (oder eine E-Mail aus deiner Datenbank)
   - Buchungsnummer: `BK-001` (oder eine gültige Nummer)
3. Du wirst zum Dashboard weitergeleitet
4. **Erwartetes Ergebnis**: 
   - Nur die Buchungen für `user1@test.com` werden angezeigt
   - Keine Buchungen anderer Benutzer sind sichtbar

### Test 2: Zweiter Benutzer sieht nur seine Buchungen

1. **Logout** (oder öffne Inkognito-Tab)
2. Logge dich mit anderem Benutzer ein:
   - E-Mail: `user2@test.com`
   - Buchungsnummer: `BK-002`
3. **Erwartetes Ergebnis**:
   - Nur Buchungen von `user2@test.com` sind sichtbar
   - Keine Überschneidung mit Buchungen von `user1@test.com`

### Test 3: Token-Validierung im Browser

1. Logge dich ein
2. Öffne Browser DevTools (F12)
3. Gehe zu Application/Storage ? Session Storage
4. Prüfe folgende Einträge:
   - `loginToken`: JWT-ähnlicher Token
   - `bookingIds`: JSON-Array mit IDs (z.B. `[1,2,3]`)
   - `userName`: Deine E-Mail

5. Gehe zu Network Tab
6. Lade das Dashboard neu
7. Prüfe die API-Anfrage zu `/api/bookings/by-ids`
8. **Erwartetes Ergebnis**:
   - Request Headers enthalten: `X-Login-Token: <token>`
   - Request Body: Die IDs aus `bookingIds`
   - Response: Nur Buchungen mit diesen IDs

### Test 4: Unbefugter Zugriff (Negativtest)

**Option A: Manueller API-Test mit Postman/curl**

```bash
# Versuche, eine fremde Buchung abzurufen (ohne Token)
curl http://localhost:5000/api/bookings/999
# Erwartung: 401 Unauthorized

# Mit gültigem Token, aber fremder Buchungs-ID
# 1. Logge dich ein und kopiere den Token
# 2. Versuche, auf eine fremde Buchung zuzugreifen
curl -H "X-Login-Token: YOUR_TOKEN" http://localhost:5000/api/bookings/999
# Erwartung: 403 Forbidden (wenn ID 999 nicht im Token ist)
```

**Option B: Browser DevTools Console**

```javascript
// Im Dashboard, öffne die Console (F12)
// Versuche, eine fremde Buchung abzurufen
fetch('/api/bookings/999')
  .then(r => r.json())
  .then(console.log)
  .catch(console.error);

// Erwartung: 403 Forbidden oder 401 Unauthorized
```

### Test 5: Token-Ablauf

1. Logge dich ein
2. Warte 30+ Minuten (oder ändere Ablaufzeit im Code auf 1 Minute zum Testen)
3. Versuche, eine Aktion durchzuführen (z.B. Check-in)
4. **Erwartetes Ergebnis**:
   - `401 Unauthorized` Error
   - Benutzer sollte zum Login zurückgeleitet werden

## Überprüfung der Logs

### Backend-Logs prüfen

Schaue im Terminal, wo das Backend läuft:
- Bei erfolgreicher Anfrage: `200 OK`
- Bei fehlgeschlaffenem Token: `401 Unauthorized`
- Bei fehlenden Berechtigungen: `403 Forbidden`

### Frontend-Logs prüfen

Öffne Browser Console (F12):
```
=== DASHBOARD INIT ===
User logged in as: user1@test.com
User booking IDs: [1, 2, 3]
Calling POST /api/bookings/by-ids with: [1, 2, 3]
=== BOOKINGS LOADED SUCCESSFULLY ===
Number of bookings received: 3
```

## Datenbank-Validierung

Prüfe, ob verschiedene Benutzer in der DB existieren:

```sql
-- Alle Buchungen anzeigen mit E-Mail (Title)
SELECT id, "BookingNumber", "Title" as Email, "RoomId" 
FROM "Bookings" 
ORDER BY "Title";

-- Erwartung: Verschiedene E-Mail-Adressen mit jeweils eigenen Buchungen
```

## Häufige Probleme

### Problem: "Ungültiger oder fehlender Token"
**Lösung**: 
- Stelle sicher, dass Backend läuft
- Prüfe, dass `auth.interceptor.ts` korrekt konfiguriert ist
- Lösche Session Storage und logge dich neu ein

### Problem: Alle Buchungen werden angezeigt
**Lösung**:
- Backend wurde noch nicht neu gestartet
- Starte Backend neu: `dotnet run`

### Problem: Keine Buchungen werden angezeigt
**Lösung**:
- Prüfe Browser Console auf Fehler
- Prüfe Network Tab auf 401/403 Fehler
- Stelle sicher, dass der Token richtig gespeichert wurde

## Erfolgreiche Implementierung

? Nur eigene Buchungen werden angezeigt
? Verschiedene Benutzer sehen unterschiedliche Daten
? Unbefugter Zugriff wird blockiert (403)
? Fehlende Tokens werden abgelehnt (401)
? Token wird automatisch bei allen Anfragen gesendet
