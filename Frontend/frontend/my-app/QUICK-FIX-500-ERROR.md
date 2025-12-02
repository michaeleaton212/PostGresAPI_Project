# 🚨 QUICK FIX - 500 Internal Server Error

## Problem identifiziert
**HTTP 500 Internal Server Error** beim Erstellen einer Buchung.

## Wahrscheinlichste Ursache
Die Datenbank-Tabelle `bookings` hat keine `BookingNumber`-Spalte!

## 🔧 Lösung 1: Migration ausführen (EMPFOHLEN)

### Schritt 1: Backend stoppen
Drücken Sie `Ctrl+C` im Terminal, wo das Backend läuft.

### Schritt 2: Migration erstellen und ausführen
```bash
cd PostGresAPI
dotnet ef migrations add AddBookingNumber
dotnet ef database update
```

### Schritt 3: Backend neu starten
```bash
dotnet run
```

## 🔧 Lösung 2: Manuelle SQL (SCHNELL)

Wenn Migrations nicht funktionieren, führen Sie direkt in PostgreSQL aus:

```sql
-- Verbinden Sie sich mit Ihrer Datenbank
-- psql -U postgres -d your_database_name

ALTER TABLE bookings 
ADD COLUMN IF NOT EXISTS "BookingNumber" VARCHAR(50);

-- Oder mit snake_case:
ALTER TABLE bookings 
ADD COLUMN IF NOT EXISTS booking_number VARCHAR(50);

-- Bestehende Buchungen updaten (falls vorhanden):
UPDATE bookings 
SET "BookingNumber" = UPPER(SUBSTRING(MD5(RANDOM()::text), 1, 8))
WHERE "BookingNumber" IS NULL;

-- Spalte als NOT NULL setzen:
ALTER TABLE bookings 
ALTER COLUMN "BookingNumber" SET NOT NULL;
```

## 🔧 Lösung 3: Datenbank neu erstellen (NUR für Entwicklung!)

**⚠️ WARNUNG: Löscht ALLE Daten!**

```bash
cd PostGresAPI
dotnet ef database drop --force
dotnet ef database update
```

## ✅ Überprüfung

Nach der Lösung:

1. **Backend starten:**
   ```bash
   cd PostGresAPI
   dotnet run
   ```

2. **Test-Script ausführen:**
   ```powershell
   cd frontend/my-app
   .\test-booking-api.ps1
   ```

3. **Frontend testen:**
   - Browser öffnen: http://localhost:4200
   - Buchung versuchen
   - ✓ Sollte jetzt funktionieren!

## 🔍 Datenbank überprüfen

```sql
-- Tabellenstruktur anzeigen
\d bookings

-- Oder:
SELECT column_name, data_type, is_nullable
FROM information_schema.columns
WHERE table_name = 'bookings';
```

**Erwartete Spalten:**
- `Id` (integer)
- `RoomId` (integer)
- `StartTime` (timestamp with time zone)
- `EndTime` (timestamp with time zone)
- `Title` (varchar)
- **`BookingNumber` (varchar)** ← Diese MUSS vorhanden sein!

## 📝 Alternative: ApplicationDbContext anpassen

Falls Sie die Migration nicht ausführen können, können Sie temporär einen Default-Wert hinzufügen:

In `ApplicationDbContext.cs`:

```csharp
booking.Property(b => b.BookingNumber)
    .HasMaxLength(50)
    .IsRequired()
    .HasDefaultValueSql("UPPER(SUBSTRING(MD5(RANDOM()::text), 1, 8))");
```

Dann:
```bash
dotnet ef migrations add FixBookingNumber
dotnet ef database update
```

## 🎯 Nach der Korrektur

Ihr Booking sollte nun funktionieren! Sie sollten in den Backend-Logs sehen:

```
Booking created successfully: Id=1, BookingNumber=A1B2C3D4
```

Und im Frontend:
```
=== BOOKING SUCCESS ===
Booking Number: A1B2C3D4
```
