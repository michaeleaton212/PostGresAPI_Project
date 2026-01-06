# ?? EMAIL DEBUGGING GUIDE

## Problem
Keine Email wird beim Buchen eines Zimmers empfangen.

## Debugging-Schritte

### 1. Stoppen und Neu starten der Anwendung

```powershell
# Strg+C im Terminal drücken um zu stoppen
cd C:\PostGresAPI_Project\PostGresAPI_Project\PostGresAPI
dotnet run
```

### 2. Logs im Terminal beobachten

Nach dem Neustart der Anwendung sollten Sie folgende Ausgabe sehen:

```
info: PostGresAPI.Services.EmailJsService[0]
      EmailJsService initialized with ServiceId: service_6rczoex, TemplateId: template_wgafe4j, PublicKey: PqPY...
```

? **Wenn diese Ausgabe erscheint**: EmailJS ist korrekt konfiguriert
? **Wenn "MISSING" erscheint**: appsettings.json prüfen

### 3. Test-Buchung erstellen

Im Frontend:
1. Als User einloggen (z.B. testuser / Test123!)
2. Ein Zimmer auswählen
3. Buchung durchführen

**WICHTIG**: Im Browser DevTools (F12) ? Network ? Achten Sie auf den Request Header `X-User-Id`

### 4. Terminal-Logs analysieren

Nach der Buchung sollten Sie folgende Log-Sequenz sehen:

```
=== BOOKING REQUEST RECEIVED ===
UserId from session header (X-User-Id): 1
=== BOOKING CREATION START ===
CreateBookingDto - RoomId: 1, UserId: 1, StartUtc: ...
Booking created with Id: X, BookingNumber: XXXX, UserId: 1
Fetching user #1 to get email address...
User found: testuser, Email: test@example.com
Calling EmailJS service to send confirmation email...
=== EMAIL SENDING START ===
Attempting to send booking confirmation email to: test@example.com
Email payload prepared: { ... }
Sending POST request to EmailJS API...
? Email sent successfully! Response: OK
=== EMAIL SENDING SUCCESS ===
```

## Häufige Fehler und Lösungen

### ? "No UserId in session header"
**Problem**: Frontend sendet kein `X-User-Id` Header
**Lösung**: 
- Prüfen Sie `api.service.ts` ? `getHeaders()` Methode
- Prüfen Sie ob User eingeloggt ist

### ? "User #X not found in database"
**Problem**: User existiert nicht in der DB
**Lösung**:
```sql
SELECT * FROM users WHERE id = 1;
```

### ? "No recipient email address"
**Problem**: User hat keine Email-Adresse
**Lösung**:
```sql
UPDATE users SET email = 'test@example.com' WHERE id = 1;
```

### ? "EmailJS configuration is incomplete"
**Problem**: appsettings.json fehlen Werte
**Lösung**: Prüfen Sie `PostGresAPI\appsettings.json`:
```json
"EmailJs": {
  "ServiceId": "service_6rczoex",
  "TemplateId": "template_wgafe4j",
  "PublicKey": "PqPY56nEQgsXhzvaJ"
}
```

### ? "Email sending failed. Status: 400"
**Probleme**:
1. EmailJS Template existiert nicht
2. Template-Parameter stimmen nicht überein
3. EmailJS Account ist nicht aktiv

**Lösung**:
1. Gehen Sie zu https://dashboard.emailjs.com
2. Prüfen Sie ob Service und Template existieren
3. Prüfen Sie Template-Parameter

### ? "Email sending failed. Status: 403"
**Problem**: PublicKey ist falsch oder Account ist gesperrt
**Lösung**: PublicKey in EmailJS Dashboard prüfen

### ? Email wird gesendet aber kommt nicht an
**Mögliche Ursachen**:
1. Spam-Ordner prüfen
2. Email-Adresse in EmailJS Template prüfen
3. EmailJS Template verwendet `{{to_email}}` Variable?

## Manueller Test mit REST Client

Öffnen Sie `PostGresAPI\TEST_EMAIL_DEBUG.http` in Visual Studio und führen Sie die Requests aus:

1. User Login
2. Buchung erstellen mit X-User-Id Header
3. Logs im Terminal beobachten

## EmailJS Template Prüfen

Ihr EmailJS Template sollte folgende Variablen enthalten:

```
{{to_email}}          - Empfänger Email
{{booking_number}}    - Buchungsnummer
{{room_name}}         - Zimmername
{{room_type}}         - Zimmertyp
{{check_in}}          - Check-in Datum
{{check_out}}         - Check-out Datum
{{number_of_persons}} - Anzahl Personen
{{duration}}          - Aufenthaltsdauer
{{total_price}}       - Gesamtpreis
{{guest_name}}        - Gästename
```

## Datenbank-Queries zum Debuggen

```sql
-- User mit Email prüfen
SELECT id, username, email FROM users;

-- Letzte Buchung mit User prüfen
SELECT b.id, b.booking_number, b.user_id, u.email 
FROM bookings b 
LEFT JOIN users u ON b.user_id = u.id 
ORDER BY b.id DESC 
LIMIT 1;
```

## Kontakt

Falls das Problem weiterhin besteht:
1. Kopieren Sie die kompletten Terminal-Logs
2. Prüfen Sie den Browser Network Tab (F12)
3. Prüfen Sie die PostgreSQL User-Tabelle

**Wichtig**: Achten Sie besonders auf die Log-Zeilen zwischen:
- `=== BOOKING REQUEST RECEIVED ===`
- `=== EMAIL SENDING SUCCESS ===` oder `=== EMAIL SENDING FAILED ===`
