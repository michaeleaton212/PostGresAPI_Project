# ?? SMTP Email-Konfiguration

## Übersicht
Das System verwendet jetzt **SMTP** statt EmailJS für den Email-Versand. Dies ermöglicht:
- ? Server-zu-Server Email-Versand (kein Browser erforderlich)
- ? Eigene Email-Templates
- ? Professionellere Email-Adressen
- ? Keine Drittanbieter-Abhängigkeit

---

## Konfiguration

### 1. Gmail verwenden (empfohlen für Tests)

#### Schritt 1: App-Passwort erstellen
1. Gehen Sie zu Ihrem Google-Konto: https://myaccount.google.com/
2. Navigieren Sie zu **Sicherheit** ? **2-Schritt-Verifizierung** (muss aktiviert sein!)
3. Scrollen Sie nach unten zu **App-Passwörter**
4. Wählen Sie **Mail** und **Anderes Gerät**
5. Geben Sie einen Namen ein (z.B. "Hotel Management")
6. Kopieren Sie das generierte 16-stellige Passwort

#### Schritt 2: appsettings.json aktualisieren
```json
{
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "UseSsl": true,
    "Username": "ihre-email@gmail.com",
    "Password": "xxxx xxxx xxxx xxxx",  // Das 16-stellige App-Passwort
    "FromEmail": "ihre-email@gmail.com",
    "FromName": "Hotel Management System"
  }
}
```

**Wichtig**: Verwenden Sie das App-Passwort, NICHT Ihr normales Gmail-Passwort!

---

### 2. Andere Email-Anbieter

#### Outlook/Hotmail
```json
{
  "Smtp": {
    "Host": "smtp-mail.outlook.com",
    "Port": 587,
    "UseSsl": true,
    "Username": "ihre-email@outlook.com",
    "Password": "ihr-passwort",
    "FromEmail": "ihre-email@outlook.com",
    "FromName": "Hotel Management"
  }
}
```

#### Yahoo
```json
{
  "Smtp": {
    "Host": "smtp.mail.yahoo.com",
    "Port": 587,
    "UseSsl": true,
    "Username": "ihre-email@yahoo.com",
    "Password": "app-passwort",
    "FromEmail": "ihre-email@yahoo.com",
    "FromName": "Hotel Management"
  }
}
```

#### Custom SMTP Server
```json
{
  "Smtp": {
    "Host": "mail.ihrer-domain.com",
    "Port": 587,
    "UseSsl": true,
    "Username": "noreply@ihrer-domain.com",
    "Password": "passwort",
    "FromEmail": "noreply@ihrer-domain.com",
    "FromName": "Hotel Management"
  }
}
```

---

## Testing

### 1. Anwendung neu starten
```powershell
cd C:\PostGresAPI_Project\PostGresAPI_Project\PostGresAPI
dotnet run
```

### 2. Beim Start prüfen
Sie sollten diese Log-Meldung sehen:
```
info: PostGresAPI.Services.SmtpEmailService[0]
      SmtpEmailService initialized with Host: smtp.gmail.com, Port: 587, Username: ihre-email@gmail.com
```

### 3. Buchung erstellen
1. Im Frontend einloggen
2. Zimmer buchen
3. Terminal-Logs beobachten

### 4. Erwartete Logs
```
=== BOOKING REQUEST RECEIVED ===
UserId from session header (X-User-Id): 4
=== BOOKING CREATION START ===
Booking created with Id: X, BookingNumber: XXXX
User found: Michael Eaton, Email: michael.eaton212@gmail.com
Calling SMTP email service to send confirmation email...
=== EMAIL SENDING START ===
Attempting to send email to: michael.eaton212@gmail.com, Subject: Buchungsbestätigung - XXXX
Sending email via SMTP: smtp.gmail.com:587
? Email sent successfully!
=== EMAIL SENDING SUCCESS ===
Email sending result: SUCCESS
=== BOOKING CREATION END ===
```

---

## Troubleshooting

### ? "SMTP configuration is incomplete"
**Problem**: Konfiguration in appsettings.json fehlt
**Lösung**: Alle Felder in `Smtp` Sektion ausfüllen

### ? "Authentication failed"
**Problem**: Falsches Passwort oder kein App-Passwort
**Lösung**: 
- Gmail: App-Passwort verwenden, nicht normales Passwort
- 2FA muss aktiviert sein

### ? "SMTP server requires a secure connection"
**Problem**: SSL/TLS fehlt
**Lösung**: `"UseSsl": true` in appsettings.json setzen

### ? "Mailbox unavailable"
**Problem**: Absender-Email ist ungültig
**Lösung**: `FromEmail` muss mit `Username` übereinstimmen (bei den meisten Anbietern)

### ? Email wird nicht empfangen
**Mögliche Ursachen**:
1. **Spam-Ordner prüfen**
2. Email-Adresse des Users ist falsch:
   ```sql
   SELECT id, username, email FROM "Users" WHERE id = 4;
   UPDATE "Users" SET email = 'ihre-email@gmail.com' WHERE id = 4;
   ```
3. SMTP-Server blockiert Email (Rate-Limiting)

---

## Email-Template anpassen

Die Email-Templates befinden sich in:
```
PostGresAPI\Services\EmailTemplates.cs
```

Sie können HTML und CSS anpassen:
```csharp
public static string BookingConfirmationHtml(Booking booking)
{
    return $@"
<!doctype html>
<html>
<head>
  <meta charset=""utf-8"">
  <style>
    body {{ font-family: Arial, sans-serif; }}
    .container {{ max-width: 600px; margin: auto; padding: 20px; }}
    .header {{ background: #007bff; color: white; padding: 20px; }}
  </style>
</head>
<body>
  <div class=""container"">
    <div class=""header"">
      <h1>Buchungsbestätigung</h1>
    </div>
    <!-- ... -->
  </div>
</body>
</html>";
}
```

---

## Sicherheitshinweise

### ?? NIEMALS Passwörter in Git committen!

**Lösung 1: User Secrets (empfohlen für Entwicklung)**
```powershell
cd C:\PostGresAPI_Project\PostGresAPI_Project\PostGresAPI
dotnet user-secrets init
dotnet user-secrets set "Smtp:Username" "ihre-email@gmail.com"
dotnet user-secrets set "Smtp:Password" "xxxx xxxx xxxx xxxx"
```

**Lösung 2: Umgebungsvariablen (empfohlen für Produktion)**
```powershell
$env:Smtp__Username = "ihre-email@gmail.com"
$env:Smtp__Password = "xxxx xxxx xxxx xxxx"
```

**Lösung 3: appsettings.Development.json** (nicht in Git)
```json
{
  "Smtp": {
    "Username": "ihre-email@gmail.com",
    "Password": "xxxx xxxx xxxx xxxx"
  }
}
```

Fügen Sie zu `.gitignore` hinzu:
```
appsettings.Development.json
```

---

## Production-Ready Setup

Für Produktion empfehlen wir:

1. **Dedizierte Email-Adresse**: `noreply@ihre-domain.com`
2. **SMTP-Relay-Service**: 
   - SendGrid (12.000 kostenlose Emails/Monat)
   - Mailgun (5.000 kostenlose Emails/Monat)
   - Amazon SES (sehr günstig)
3. **Email-Queue**: Verwenden Sie `OutboxEmailWorker` für asynchrones Senden
4. **Monitoring**: Log alle Email-Fehler
5. **Rate-Limiting**: Begrenzen Sie Emails pro User/Stunde

---

## Nächste Schritte

1. ? SMTP-Konfiguration in `appsettings.json` eintragen
2. ? Anwendung neu starten
3. ? Buchung erstellen und Email erhalten
4. ? Email-Template nach Bedarf anpassen
5. ? Für Produktion: User Secrets oder Umgebungsvariablen verwenden

**Viel Erfolg!** ??
