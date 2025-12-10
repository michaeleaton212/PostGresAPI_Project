# Test-Script für Authentication & Dashboard

## Backend-Status prüfen

### 1. Ist Backend online?
```powershell
curl http://localhost:5000/api/bookings
```
**Erwartung:** Liste aller Buchungen (JSON-Array)

---

## Login testen

### 2. Login mit gültigen Daten
```powershell
# Windows PowerShell
$body = @{
    bookingNumber = "FED62502"
    name = "michael.eaton212@gmail.com"
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri "http://localhost:5000/api/bookings/login" -Method POST -Body $body -ContentType "application/json"

Write-Host "Token: $($response.token)"
Write-Host "Booking IDs: $($response.bookingIds)"
```

**Erwartetes Ergebnis:**
```
Token: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Booking IDs: 2 4 5 7
```

**Wenn Fehler 401:**
- Buchungsnummer oder E-Mail ist falsch
- Überprüfen Sie die Datenbank: `SELECT * FROM "Bookings" WHERE "BookingNumber" = 'FED62502';`

---

## By-IDs Endpoint testen

### 3. Buchungen nach IDs laden
```powershell
# IDs aus Login-Response verwenden
$ids = @(2, 4, 5, 7) | ConvertTo-Json

$bookings = Invoke-RestMethod -Uri "http://localhost:5000/api/bookings/by-ids" -Method POST -Body $ids -ContentType "application/json"

Write-Host "Anzahl Buchungen: $($bookings.Count)"
$bookings | ForEach-Object { Write-Host "ID: $($_.id), Title: $($_.title), Room: $($_.roomId)" }
```

**Erwartetes Ergebnis:**
```
Anzahl Buchungen: 4
ID: 2, Title: michael.eaton212@gmail.com, Room: 102
ID: 4, Title: michael.eaton212@gmail.com, Room: 103
ID: 5, Title: michael.eaton212@gmail.com, Room: 101
ID: 7, Title: michael.eaton212@gmail.com, Room: 201
```

**Wenn Fehler 404:**
- Backend wurde nicht neu gestartet
- Endpoint `/api/bookings/by-ids` existiert nicht

**Wenn Fehler 400:**
- Body ist leer oder ungültiges Format
- Überprüfen Sie: `$ids` sollte JSON-Array sein: `[2,4,5,7]`

---

## Kompletter Flow testen

### 4. Login + Dashboard-Load simulieren
```powershell
# Login
$loginBody = @{
    bookingNumber = "FED62502"
    name = "michael.eaton212@gmail.com"
} | ConvertTo-Json

$loginResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/bookings/login" -Method POST -Body $loginBody -ContentType "application/json"

Write-Host "`n=== LOGIN ERFOLGREICH ==="
Write-Host "Token: $($loginResponse.token.Substring(0, 20))..."
Write-Host "Booking IDs: $($loginResponse.bookingIds -join ', ')"

# Buchungen laden
$idsJson = $loginResponse.bookingIds | ConvertTo-Json

$bookings = Invoke-RestMethod -Uri "http://localhost:5000/api/bookings/by-ids" -Method POST -Body $idsJson -ContentType "application/json"

Write-Host "`n=== BUCHUNGEN GELADEN ==="
Write-Host "Anzahl: $($bookings.Count)"
Write-Host "`nDetails:"
$bookings | ForEach-Object { 
    Write-Host "  - ID: $($_.id), Buchung: $($_.bookingNumber), Name: $($_.title)"
}

# Validierung
$allSameUser = ($bookings | Where-Object { $_.title -ne "michael.eaton212@gmail.com" }).Count -eq 0

if ($allSameUser) {
    Write-Host "`n? ERFOLG: Alle Buchungen gehören zum richtigen User!" -ForegroundColor Green
} else {
    Write-Host "`n? FEHLER: Es wurden Buchungen anderer User geladen!" -ForegroundColor Red
}
```

---

## Datenbank direkt prüfen

### 5. Welche Buchungen hat mein User?
```sql
-- PostgreSQL
SELECT 
    "Id",
    "Title",
    "BookingNumber",
    "RoomId",
    "Status"
FROM "Bookings"
WHERE LOWER("Title") = LOWER('michael.eaton212@gmail.com')
ORDER BY "Id";
```

### 6. Welche Buchungsnummer gehört zu welcher E-Mail?
```sql
SELECT 
    "BookingNumber",
    "Title",
    "Id"
FROM "Bookings"
ORDER BY "Title";
```

---

## Frontend Browser-Test

### 7. Browser Console Log überprüfen

Nach Login auf Dashboard navigieren und Console öffnen (F12):

**Suchen Sie nach:**
```
=== LOADING BOOKINGS ===
Calling POST /api/bookings/by-ids with: [2, 4, 5, 7]
```

**NICHT:**
```
Calling GET /api/bookings
Calling GET /api/bookings/by-name/...
```

### 8. Network Tab überprüfen

Filter auf "by-ids":

**Sollte zu sehen sein:**
```
Request URL: http://localhost:5000/api/bookings/by-ids
Request Method: POST
Status Code: 200
Request Payload: [2,4,5,7]
```

**NICHT zu sehen sein:**
```
Request URL: http://localhost:5000/api/bookings (ohne by-ids)
```

---

## Schnelltest-Checkliste

- [ ] `curl http://localhost:5000/api/bookings` ? 200 OK
- [ ] Login-Endpoint ? Token + bookingIds[] zurück
- [ ] `/api/bookings/by-ids` ? Nur User-Buchungen zurück
- [ ] Browser Console ? "Calling POST /api/bookings/by-ids"
- [ ] Network Tab ? POST zu `/by-ids` sichtbar
- [ ] Dashboard zeigt nur eigene Buchungen

---

## Bei Problemen

### Problem: 404 für `/api/bookings/by-ids`
**Lösung:**
1. Backend-Prozess beenden (Task Manager)
2. `cd C:\PostGresAPI_Project\PostGresAPI_Project\PostGresAPI`
3. `dotnet clean`
4. `dotnet build`
5. `dotnet run`

### Problem: 401 Unauthorized
**Lösung:**
1. SessionStorage löschen: `sessionStorage.clear()`
2. Neu einloggen

### Problem: Falsche Buchungen werden geladen
**Lösung:**
1. Prüfen Sie Network Tab: Wird wirklich `/by-ids` aufgerufen?
2. Wenn nein: Browser Cache leeren (Hard Reload)
3. Wenn ja: Backend-Response prüfen - sind wirklich nur User-IDs drin?

---

## Erfolgs-Kriterium

? **Dashboard zeigt NUR Buchungen mit `title = "michael.eaton212@gmail.com"`**

Keine Buchungen von "qw", "s" oder anderen Users!
