# User-Based Authentication System

## Übersicht

Das System wurde von Buchungsnummer-basierter Authentifizierung auf User-basierte Authentifizierung umgestellt.

## Datenbank-Migration

Die Migration `AddUserIdToBooking` wurde erfolgreich ausgeführt:
- Fügt `UserId` Spalte zur `Bookings` Tabelle hinzu (nullable)
- Erstellt Foreign Key zu `Users` Tabelle mit `ON DELETE SET NULL`
- Erstellt Index auf `UserId` für Performance

## Backend-Änderungen

### 1. Neue Endpunkte

#### User-Authentifizierung
- `POST /api/userauth/register` - User registrieren
  ```json
  {
    "userName": "testuser",
    "email": "test@example.com",
    "phone": "123456789",
    "password": "test123"
  }
  ```

- `POST /api/userauth/login` - User Login
  ```json
  {
    "userNameOrEmail": "test@example.com",
    "password": "test123"
  }
  ```

#### Booking-Endpunkte (mit User-Authorization)
- `GET /api/bookings` - Gibt nur Buchungen des eingeloggten Users zurück
- `GET /api/bookings/{id}` - Prüft, ob Buchung dem User gehört
- `PUT /api/bookings/{id}` - Update nur eigene Buchungen
- `PATCH /api/bookings/{id}/status` - Status nur eigene Buchungen
- `DELETE /api/bookings/{id}` - Löschen nur eigene Buchungen

### 2. Authorization Header
Alle Booking-Endpunkte erwarten `X-User-Id` Header:
```
X-User-Id: 1
```

## Frontend-Änderungen

### 1. Neue Seiten
- **Registrierungs-Seite** (`/register`)
  - Vollständige Validierung
  - Automatischer Login nach Registrierung
  - Link zur Login-Seite

- **Login-Seite** (`/login`) - Aktualisiert
  - Email und Passwort statt Buchungsnummer
  - Link zur Registrierungs-Seite

### 2. Session Storage
Gespeicherte User-Informationen:
- `userId` - User ID
- `userName` - Benutzername
- `userEmail` - Email-Adresse

### 3. API Service
Fügt automatisch `X-User-Id` Header zu allen Requests hinzu.

### 4. Dashboard
- Zeigt nur Buchungen des eingeloggten Users
- Verwendet `GET /api/bookings` statt `POST /api/bookings/by-ids`
- Logout löscht User-Session

## Test-Workflow

### 1. Backend starten
```bash
cd C:\PostGresAPI_Project\PostGresAPI_Project\PostGresAPI
dotnet run
```

### 2. User registrieren
**Option A: Via Frontend**
1. Öffne http://localhost:4200/register
2. Fülle Formular aus:
   - Benutzername: testuser
   - Email: test@example.com
   - Telefon: 123456789 (optional)
   - Passwort: test123
   - Passwort bestätigen: test123
3. Klicke "Registrieren"
4. Du wirst automatisch eingeloggt und zu `/rooms` weitergeleitet

**Option B: Via API (Postman/HTTP-Client)**
```http
POST http://localhost:5031/api/userauth/register
Content-Type: application/json

{
  "userName": "testuser",
  "email": "test@example.com",
  "phone": "123456789",
  "password": "test123"
}
```

### 3. Login
**Option A: Via Frontend**
1. Öffne http://localhost:4200/login
2. Email: test@example.com
3. Passwort: test123
4. Klicke "Login"
5. Du wirst zu `/dashboard` weitergeleitet

**Option B: Via API**
```http
POST http://localhost:5031/api/userauth/login
Content-Type: application/json

{
  "userNameOrEmail": "test@example.com",
  "password": "test123"
}
```

### 4. Buchung erstellen
**Via Frontend:**
1. Gehe zu "Rooms" (`/rooms`)
2. Wähle einen Raum
3. Wähle Datum/Zeit
4. Gib deine Email ein (wird als Title gespeichert)
5. Die `userId` wird automatisch aus sessionStorage geholt

**Via API:**
```http
POST http://localhost:5031/api/bookings
Content-Type: application/json
X-User-Id: 1

{
  "roomId": 4,
  "startUtc": "2024-01-20T14:00:00Z",
  "endUtc": "2024-01-22T10:00:00Z",
  "title": "test@example.com",
  "userId": 1
}
```

### 5. Buchungen anzeigen
**Via Frontend:**
1. Gehe zu Dashboard (`/dashboard`)
2. Alle deine Buchungen werden angezeigt

**Via API:**
```http
GET http://localhost:5031/api/bookings
X-User-Id: 1
```

### 6. Logout
Klicke auf "Logout" im Dashboard oder navigiere zu `/login`

## Datenbank-Checks

### Überprüfe User-Tabelle
```sql
SELECT * FROM "Users";
```

### Überprüfe Buchungen mit User-Zuordnung
```sql
SELECT 
    b."Id",
    b."BookingNumber",
    b."Title",
    b."UserId",
    u."UserName",
    u."Email"
FROM bookings b
LEFT JOIN "Users" u ON b."UserId" = u."Id"
ORDER BY b."Id" DESC;
```

### Buchungen eines bestimmten Users
```sql
SELECT * FROM bookings WHERE "UserId" = 1;
```

## Wichtige Hinweise

### Bestehende Buchungen
- Buchungen ohne `UserId` (NULL) bleiben erhalten
- Sie sind über die alten Endpunkte mit Buchungsnummer weiterhin zugänglich
- Neue Buchungen erhalten automatisch die `UserId` des eingeloggten Users

### Sicherheit
- Passwörter werden mit SHA256 gehasht
- Keine Klartextspeicherung
- User können nur ihre eigenen Buchungen sehen/bearbeiten

### API-Kompatibilität
- Alte Booking-Login-Endpunkte (`POST /api/bookings/login`) funktionieren weiterhin
- Room-Verfügbarkeit (`GET /api/bookings/room/{id}`) ist weiterhin public

## Troubleshooting

### Backend startet nicht
```bash
# Backend-Prozess beenden
taskkill /F /IM PostGresAPI.exe

# Neu builden
dotnet build

# Starten
dotnet run
```

### Migration nicht angewendet
```bash
dotnet ef database update
```

### Frontend-Fehler
```bash
cd Frontend/frontend/my-app
npm install
ng serve
```

### Datenbank zurücksetzen (optional)
```bash
# Alle Migrations entfernen
dotnet ef database drop

# Neu erstellen
dotnet ef database update
```
