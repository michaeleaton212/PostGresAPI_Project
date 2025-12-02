# 🔍 Booking Error - Debugging Anleitung

## Problem
Die Fehlermeldung **"Buchung konnte nicht erstellt werden"** erscheint im Frontend.

## ✅ Sofort-Checkliste

### 1. Backend prüfen
```bash
# Im PostGresAPI Ordner:
cd PostGresAPI
dotnet run
```

**Erwartete Ausgabe:**
```
Now listening on: http://localhost:5031
```

### 2. PostgreSQL prüfen
```bash
# PostgreSQL Status prüfen
# Windows (Services):
services.msc -> PostgreSQL

# Oder mit psql:
psql -U postgres -d your_database -c "SELECT * FROM rooms LIMIT 5;"
```

### 3. Frontend prüfen
```bash
cd frontend/my-app
ng serve
# Oder:
npm start
```

## 🧪 Schnell-Test mit PowerShell

Führen Sie das Test-Script aus:
```powershell
cd frontend/my-app
.\test-booking-api.ps1
```

Das Script testet:
1. ✓ Backend erreichbar?
2. ✓ Rooms abrufbar?
3. ✓ Booking erstellbar?
4. ✓ Bookings abrufbar?

## 🔬 Detailliertes Debugging

### Schritt 1: Browser-Konsole öffnen
1. Drücken Sie `F12` in Chrome/Edge
2. Gehen Sie zum Tab "Console"
3. Versuchen Sie eine Buchung zu erstellen

### Schritt 2: Logs analysieren

**Im Browser sollten Sie sehen:**
```javascript
=== CONFIRM BOOKING STARTED ===
Room: {id: 4, name: "test", type: "Bedroom", ...}
Start Date: Thu Dec 05 2025 00:00:00 GMT+0100
End Date: Fri Dec 06 2025 00:00:00 GMT+0100
First Name: michael.eaton212@gmail.com

=== BOOKING DTO ===
DTO Object: {
  roomId: 4,
  startUtc: "2025-12-04T23:00:00.000Z",
  endUtc: "2025-12-05T23:00:00.000Z",
  title: "michael.eaton212@gmail.com"
}

[ApiService] POST URL: http://localhost:5031/api/bookings
[ApiService] POST Body: {...}
```

### Schritt 3: Fehlermeldungen interpretieren

| Fehlermeldung | Ursache | Lösung |
|---------------|---------|--------|
| `Failed to fetch` | Backend läuft nicht | Backend starten: `dotnet run` |
| `CORS policy error` | CORS nicht konfiguriert | Program.cs prüfen |
| `Room {id} not found` | RoomId existiert nicht | Datenbank prüfen |
| `Time range already booked` | Überschneidung | Andere Zeit wählen |
| `Start must be before End` | Datum-Fehler | Datumslogik prüfen |
| `500 Internal Server Error` | Backend-Fehler | Backend-Logs prüfen |

## 🔍 Häufige Probleme

### Problem 1: Backend läuft nicht
**Symptom:** `Failed to fetch` oder `ERR_CONNECTION_REFUSED`

**Lösung:**
```bash
cd PostGresAPI
dotnet run
```

### Problem 2: CORS-Fehler
**Symptom:** 
```
Access to fetch at 'http://localhost:5031/api/bookings' from origin 'http://localhost:4200' has been blocked by CORS policy
```

**Lösung:** In `Program.cs` prüfen:
```csharp
app.UseCors("NgDev"); // Muss VOR MapControllers() sein
```

### Problem 3: Datenbank-Fehler
**Symptom:** `500 Internal Server Error`

**Lösung:** Datenbank-Verbindung prüfen:
```bash
# In PostgreSQL:
SELECT * FROM rooms WHERE id = 4;
SELECT * FROM bookings WHERE room_id = 4;
```

### Problem 4: Zeitzone-Problem
**Symptom:** Datum wird falsch gespeichert

**Prüfen Sie:**
```javascript
// Browser-Konsole:
console.log(new Date('2025-12-05').toISOString());
// Sollte: 2025-12-05T00:00:00.000Z (oder ähnlich) ausgeben
```

## 📋 Debugging-Checkliste

Gehen Sie diese Punkte durch:

- [ ] Backend läuft auf http://localhost:5031
- [ ] PostgreSQL läuft und ist erreichbar
- [ ] Frontend läuft auf http://localhost:4200
- [ ] Browser-Konsole zeigt detaillierte Logs
- [ ] Netzwerk-Tab zeigt POST-Request an `/api/bookings`
- [ ] Keine CORS-Fehler in der Konsole
- [ ] RoomId existiert in der Datenbank
- [ ] Start-Datum liegt vor End-Datum
- [ ] Keine Überschneidung mit bestehenden Bookings

## 🛠️ Backend-Logs aktivieren

Im Backend (Program.cs) können Sie zusätzliche Logs aktivieren:

```csharp
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
```

## 📊 Datenbank direkt prüfen

```sql
-- Alle Räume anzeigen
SELECT * FROM rooms;

-- Alle Bookings anzeigen
SELECT * FROM bookings ORDER BY id DESC LIMIT 10;

-- Prüfen ob Raum 4 existiert
SELECT * FROM rooms WHERE id = 4;

-- Überschneidungen prüfen
SELECT * FROM bookings 
WHERE room_id = 4 
  AND start_time < '2025-12-06T00:00:00Z' 
  AND end_time > '2025-12-05T00:00:00Z';
```

## 🚀 Nächste Schritte

1. **Führen Sie das PowerShell-Test-Script aus:**
   ```powershell
   .\test-booking-api.ps1
   ```

2. **Wenn der Test funktioniert, aber das Frontend nicht:**
   - Browser-Cache leeren (Ctrl+Shift+Delete)
   - Hard Reload (Ctrl+Shift+R)
   - Inkognito-Modus testen

3. **Wenn der Test NICHT funktioniert:**
   - Kopieren Sie die Fehlermeldung
   - Prüfen Sie Backend-Logs
 - Prüfen Sie Datenbank

## 📞 Support

Wenn Sie immer noch Probleme haben:

1. Führen Sie `.\test-booking-api.ps1` aus
2. Kopieren Sie die VOLLSTÄNDIGE Ausgabe
3. Kopieren Sie die Browser-Konsolen-Logs
4. Kopieren Sie die Backend-Konsolen-Logs
5. Teilen Sie alle drei Ausgaben

## 🔗 Verwandte Dateien

- `test-booking-api.ps1` - PowerShell Test-Script
- `DEBUGGING-BOOKING.md` - Detaillierte Debugging-Anleitung
- `frontend/my-app/src/app/core/api.service.ts` - Mit Logging versehen
- `frontend/my-app/src/app/pages/booking-page.component/booking-page.component.ts` - Mit Logging versehen

---

**Wichtig:** Alle Dateien wurden mit zusätzlichem Logging versehen. 
Bitte versuchen Sie eine Buchung und schauen Sie sich die Logs an!
