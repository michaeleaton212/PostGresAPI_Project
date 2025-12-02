# Booking Error Debugging Guide

## Problem
Die Fehlermeldung "Buchung konnte nicht erstellt werden" erscheint, aber wir müssen herausfinden, warum.

## Debugging-Schritte

### 1. Browser-Konsole überprüfen
Öffnen Sie die Browser-Entwicklertools (F12) und gehen Sie zur Konsole:

**Nach dem Klicken auf "Confirm Booking" sollten Sie folgende Logs sehen:**

```
=== CONFIRM BOOKING STARTED ===
Room: {...}
Start Date: ...
End Date: ...
First Name: ...
=== BOOKING DTO ===
DTO Object: {...}
DTO as JSON: {...}
[ApiService] POST URL: http://localhost:5031/api/bookings
[ApiService] POST Body: {...}
```

**Bei Erfolg:**
```
[ApiService] POST Success Response: {...}
=== BOOKING SUCCESS ===
```

**Bei Fehler:**
```
[ApiService] POST Error Details: {...}
=== BOOKING ERROR ===
```

### 2. Netzwerk-Tab überprüfen
Gehen Sie zum Netzwerk-Tab (Network) in den Entwicklertools:

1. Filtern Sie nach "bookings"
2. Klicken Sie auf "Confirm Booking"
3. Schauen Sie sich die Anfrage an:
   - **Headers**: Überprüfen Sie URL, Methode (POST), Content-Type
   - **Payload**: Überprüfen Sie die gesendeten Daten
   - **Response**: Schauen Sie sich die Antwort an

### 3. Mögliche Fehlerursachen

#### A) Backend läuft nicht
**Symptom:** Status 0 oder "Failed to fetch"
**Lösung:** Starten Sie das Backend
```bash
cd PostGresAPI
dotnet run
```

#### B) CORS-Fehler
**Symptom:** CORS policy error in Konsole
**Lösung:** Überprüfen Sie Program.cs - CORS muss aktiviert sein

#### C) Datenbankverbindung
**Symptom:** Status 500, "Database error"
**Lösung:** Überprüfen Sie PostgreSQL-Verbindung

#### D) Validierungsfehler
**Symptom:** Status 400, "Start must be before End" oder "Room not found"
**Lösung:** 
- Überprüfen Sie Datum-Format
- Stellen Sie sicher, dass RoomId existiert

#### E) Zeitzone-Problem
**Symptom:** Status 400, "Time range already booked"
**Lösung:** Daten werden als UTC gesendet, Backend muss UTC verarbeiten

### 4. Backend-Logs überprüfen

Wenn das Backend läuft, sollten Sie in der Konsole sehen:
```
=== BOOKING CONTROLLER: CREATE BOOKING ===
Received DTO: RoomId=..., StartUtc=..., EndUtc=..., Title=...
```

### 5. Häufige Probleme

#### Problem: "Room not found"
- Überprüfen Sie, ob die RoomId gültig ist
- Query: `SELECT * FROM rooms WHERE id = [roomId]`

#### Problem: "Time range already booked"
- Überprüfen Sie bestehende Buchungen
- Query: `SELECT * FROM bookings WHERE room_id = [roomId]`

#### Problem: "Start must be before End"
- Überprüfen Sie Datum-Parsing
- Stellen Sie sicher, dass startDate < endDate

### 6. Quick Test mit cURL

Test die API direkt:
```bash
curl -X POST http://localhost:5031/api/bookings \
  -H "Content-Type: application/json" \
  -d '{
    "roomId": 4,
    "startUtc": "2025-12-05T00:00:00Z",
    "endUtc": "2025-12-06T00:00:00Z",
 "title": "test@example.com"
  }'
```

### 7. Checkliste

- [ ] Backend läuft (http://localhost:5031)
- [ ] PostgreSQL läuft
- [ ] Frontend läuft (http://localhost:4200)
- [ ] Browser-Konsole zeigt detaillierte Logs
- [ ] Netzwerk-Tab zeigt POST-Anfrage
- [ ] Keine CORS-Fehler
- [ ] RoomId existiert in Datenbank
- [ ] Kein Zeitüberschneidung mit anderen Buchungen

## Nächste Schritte

1. **Aktivieren Sie alle Logs** (bereits gemacht)
2. **Versuchen Sie eine Buchung**
3. **Kopieren Sie die Konsolenausgabe** (sowohl Browser als auch Backend)
4. **Teilen Sie die Logs** um den genauen Fehler zu identifizieren

## Beispiel für vollständige Debug-Ausgabe

### Browser-Konsole:
```
=== CONFIRM BOOKING STARTED ===
Room: {id: 4, name: "test", type: "Bedroom", ...}
Start Date: Thu Dec 05 2025 00:00:00 GMT+0100
End Date: Fri Dec 06 2025 00:00:00 GMT+0100
First Name: michael.eaton212@gmail.com
=== BOOKING DTO ===
DTO Object: {roomId: 4, startUtc: "2025-12-04T23:00:00.000Z", endUtc: "2025-12-05T23:00:00.000Z", title: "michael.eaton212@gmail.com"}
[ApiService] POST URL: http://localhost:5031/api/bookings
[ApiService] POST Body: {roomId: 4, startUtc: "2025-12-04T23:00:00.000Z", endUtc: "2025-12-05T23:00:00.000Z", title: "michael.eaton212@gmail.com"}
```

### Backend-Konsole:
```
=== BOOKING CONTROLLER: CREATE BOOKING ===
Received DTO: RoomId=4, StartUtc=12/04/2025 11:00:00 PM +00:00, EndUtc=12/05/2025 11:00:00 PM +00:00, Title=michael.eaton212@gmail.com
Booking created successfully: Id=123, BookingNumber=A1B2C3D4
```
