# Token-Based Authorization Implementation

## Zusammenfassung
Die Anwendung wurde so angepasst, dass Benutzer **nur ihre eigenen Buchungen** sehen können. Die Autorisierung basiert auf dem Login-Token, der die Buchungs-IDs des Benutzers enthält.

## Änderungen im Backend

### `BookingsController.cs` - Token-Validierung für alle Endpunkte

Ich habe folgende Änderungen vorgenommen:

1. **Helper-Methode `TryGetAuthorizedBookingIds`** hinzugefügt:
   - Extrahiert den Token aus dem `X-Login-Token` oder `Authorization` Header
   - Validiert den Token mit dem `ITokenService`
   - Gibt die autorisierten Buchungs-IDs zurück

2. **Geschützte Endpunkte mit Token-Validierung**:

   - **GET /api/bookings** - Gibt nur die Buchungen des Benutzers zurück
   - **GET /api/bookings/{id}** - Prüft, ob die Buchung dem Benutzer gehört
   - **GET /api/bookings/room/{roomId}** - Filtert nach autorisierten Buchungen
   - **PUT /api/bookings/{id}** - Nur eigene Buchungen bearbeitbar
   - **PATCH /api/bookings/{id}/status** - Nur eigene Buchungen aktualisierbar
   - **DELETE /api/bookings/{id}** - Nur eigene Buchungen löschbar
   - **GET /api/bookings/by-name/{name}** - Filtert nach autorisierten Buchungen
   - **POST /api/bookings/by-ids** - Gibt nur autorisierte Buchungen zurück

3. **Ungeschützte Endpunkte** (kein Token erforderlich):
   - **POST /api/bookings/login** - Login-Endpunkt
   - **POST /api/bookings** - Neue Buchungen erstellen

## Wie es funktioniert

### Login-Flow:
1. Benutzer loggt sich mit E-Mail und Buchungsnummer ein
2. Backend erstellt einen Token mit allen Buchungs-IDs des Benutzers
3. Frontend speichert Token und Buchungs-IDs in `sessionStorage`

### Autorisierung:
1. Frontend sendet Token automatisch bei allen API-Anfragen (via `auth.interceptor.ts`)
2. Backend validiert den Token bei geschützten Endpunkten
3. Backend prüft, ob die angeforderten Buchungs-IDs im Token enthalten sind
4. Nur autorisierte Daten werden zurückgegeben

### Beispiel-Szenario:

```
Benutzer A: email=user1@test.de, Buchungen=[1, 2, 3]
Benutzer B: email=user2@test.de, Buchungen=[4, 5]

Benutzer A versucht, GET /api/bookings/4 aufzurufen:
? Token enthält [1, 2, 3]
? Anfrage für ID 4
? 403 Forbidden (nicht autorisiert)

Benutzer A ruft GET /api/bookings auf:
? Token enthält [1, 2, 3]
? Gibt nur Buchungen 1, 2, 3 zurück
```

## Frontend (keine Änderungen erforderlich)

Das Frontend ist bereits korrekt konfiguriert:
- `auth.interceptor.ts` sendet den Token automatisch bei allen API-Anfragen
- `dashboard-page.component.ts` lädt Buchungen mit `getByIds()`
- Der Token wird beim Login in `sessionStorage` gespeichert

## Sicherheitsverbesserungen

? **Datenschutz**: Benutzer können nur ihre eigenen Buchungen sehen
? **Autorisierung**: Alle Buchungsoperationen sind geschützt
? **Token-Validierung**: Token wird auf Gültigkeit und Ablaufzeit geprüft
? **ID-Filterung**: Anfragen werden auf autorisierte IDs beschränkt

## Nächste Schritte

1. **Backend neu starten**, damit die Änderungen aktiv werden
2. **Testen**:
   - Mit verschiedenen Benutzern einloggen
   - Prüfen, dass jeder nur seine eigenen Buchungen sieht
   - Versuchen, auf fremde Buchungen zuzugreifen (sollte 403 zurückgeben)

## Technische Details

### Token-Struktur:
Der Token enthält:
- Liste der Buchungs-IDs
- Ablaufzeit (30 Minuten)
- HMAC-SHA256 Signatur

### HTTP-Status-Codes:
- `401 Unauthorized` - Token fehlt oder ist ungültig
- `403 Forbidden` - Token gültig, aber keine Berechtigung für diese Ressource
- `200 OK` - Erfolgreiche autorisierte Anfrage
