# Unit Tests - Übersicht

## Test-Struktur

Die Unit Tests sind in drei Hauptkategorien unterteilt:

### 1. **Controller Tests** (Controllers/)
Testen die HTTP-Endpunkte und deren Rückgabewerte.

#### UsersControllerTests.cs
- ? GetAll - Alle User abrufen
- ? GetAll - Leere Liste
- ? GetById - Existierender User
- ? GetById - Nicht existierender User (NotFound)
- ? Create - User erstellen
- ? Update - User aktualisieren
- ? Update - Nicht existierender User (NotFound)
- ? Delete - User löschen
- ? Delete - Nicht existierender User (NotFound)

#### BedroomsControllerTests.cs
- ? GetAll - Alle Bedrooms abrufen
- ? GetAll - Leere Liste
- ? GetById - Existierendes Bedroom
- ? GetById - Nicht existierendes Bedroom (NotFound)
- ? Create - Bedroom erstellen
- ? Create - Bedroom ohne Bild erstellen
- ? Update - Bedroom aktualisieren
- ? Update - Nicht existierendes Bedroom (NotFound)
- ? Delete - Bedroom löschen
- ? Delete - Nicht existierendes Bedroom (NotFound)

#### MeetingroomsControllerTests.cs
- ? GetAll - Alle Meetingrooms abrufen
- ? GetAll - Leere Liste
- ? GetById - Existierender Meetingroom
- ? GetById - Nicht existierender Meetingroom (NotFound)
- ? Create - Meetingroom erstellen
- ? Create - Meetingroom ohne Bild erstellen
- ? Update - Meetingroom aktualisieren
- ? Update - Nicht existierender Meetingroom (NotFound)
- ? Delete - Meetingroom löschen
- ? Delete - Nicht existierender Meetingroom (NotFound)

#### RoomsControllerTests.cs (Bereits vorhanden)
- ? GetAll - Filter-Tests
- ? GetById - Tests

#### BookingsControllerTests.cs (Bereits vorhanden)
- ? Create, Login, GetAll, GetById, Delete
- ? Token-Validierung und Authorization

---

### 2. **Service Tests** (Services/)
Testen die Business-Logik.

#### UserServiceTests.cs
- ? GetAll - Alle User abrufen
- ? GetAll - Leere Liste
- ? GetById - Existierender User
- ? GetById - Nicht existierender User
- ? Create - User erstellen
- ? Update - User aktualisieren
- ? Update - Nicht existierender User
- ? Delete - User löschen
- ? Delete - Nicht existierender User

#### BedroomServiceTests.cs
- ? GetAll - Alle Bedrooms abrufen
- ? GetAll - Leere Liste
- ? GetById - Existierendes Bedroom
- ? GetById - Nicht existierendes Bedroom
- ? Create - Bedroom erstellen
- ? Create - Bedroom ohne Bild
- ? Update - Bedroom aktualisieren
- ? Update - Nicht existierendes Bedroom
- ? Delete - Bedroom löschen
- ? Delete - Nicht existierendes Bedroom

#### MeetingroomServiceTests.cs
- ? GetAll - Alle Meetingrooms abrufen
- ? GetAll - Leere Liste
- ? GetById - Existierender Meetingroom
- ? GetById - Nicht existierender Meetingroom
- ? Create - Meetingroom erstellen
- ? Create - Meetingroom ohne Bild
- ? Update - Meetingroom aktualisieren
- ? Update - Nicht existierender Meetingroom
- ? Delete - Meetingroom löschen
- ? Delete - Nicht existierender Meetingroom

#### RoomServiceTests.cs (Bereits vorhanden)
- ? GetAll - Filter-Tests (Bedroom/Meetingroom)
- ? GetById, Create, Update, Delete

#### BookingServiceTests.cs (Bereits vorhanden)
- ? Create, Update, UpdateStatus, Delete
- ? Validierung (Datum, Room-Existenz, Überschneidungen)
- ? Login-Logik, IsActive-Prüfung

---

### 3. **Repository Integration Tests** (Repository/)
Testen die Datenbankzugriffe mit InMemory-Datenbank.

#### UserRepositoryIntegrationTests.cs
- ? GetAll - Alle User aus DB
- ? GetAll - Leere DB
- ? GetAll - Sortierung nach Id
- ? GetById - Existierender User
- ? GetById - Nicht existierender User
- ? Add - User erstellen
- ? Update - User aktualisieren
- ? Update - Nicht existierender User
- ? Delete - User löschen
- ? Delete - Nicht existierender User

#### BedroomRepositoryIntegrationTests.cs
- ? GetAll - Alle Bedrooms aus DB
- ? GetAll - Leere DB
- ? GetAll - Sortierung nach Id
- ? GetById - Existierendes Bedroom
- ? GetById - Nicht existierendes Bedroom
- ? Add - Bedroom erstellen
- ? Update - Bedroom aktualisieren
- ? Update - Nicht existierendes Bedroom
- ? ExistsAsync - Prüfung
- ? Delete - Bedroom löschen
- ? Delete - Nicht existierendes Bedroom

#### MeetingroomRepositoryIntegrationTests.cs
- ? GetAll - Alle Meetingrooms aus DB
- ? GetAll - Leere DB
- ? GetAll - Sortierung nach Id
- ? GetById - Existierender Meetingroom
- ? GetById - Nicht existierender Meetingroom
- ? Add - Meetingroom erstellen
- ? Update - Meetingroom aktualisieren
- ? Update - Nicht existierender Meetingroom
- ? Delete - Meetingroom löschen
- ? Delete - Nicht existierender Meetingroom

#### RoomRepositoryIntegrationTests.cs (Bereits vorhanden)
- ? GetAll, GetBedrooms, GetMeetingrooms
- ? CRUD-Operationen

#### BookingRepositoryIntegrationTests.cs (Bereits vorhanden)
- ? CRUD-Operationen
- ? HasOverlap-Logik
- ? GetByRoomId, GetByName, GetByBookingNumber

---

## Test-Ausführung

### Alle Tests ausführen
```bash
dotnet test
```

### Einzelne Test-Klasse ausführen
```bash
dotnet test --filter FullyQualifiedName~UserServiceTests
```

### Test mit Code Coverage
```bash
dotnet test /p:CollectCoverage=true
```

---

## Test-Coverage Übersicht

| Komponente | Anzahl Tests | Status |
|------------|--------------|--------|
| **Controllers** | 48 Tests | ? Vollständig |
| **Services** | 48 Tests | ? Vollständig |
| **Repositories** | 55 Tests | ? Vollständig |
| **Gesamt** | **151 Tests** | ? Vollständig |

---

## Verwendete Test-Frameworks

- **xUnit** - Test-Framework
- **Moq** - Mocking-Framework
- **FluentAssertions** - Assertion-Library
- **Microsoft.EntityFrameworkCore.InMemory** - InMemory-Datenbank für Integration Tests

---

## Test-Namenskonvention

Alle Tests folgen der Konvention: `MethodName_Scenario_ExpectedBehavior`

Beispiele:
- `GetAll_ReturnsAllUsers` - GetAll-Methode gibt alle User zurück
- `GetById_NonExistingUser_ReturnsNull` - GetById gibt null bei nicht existierendem User
- `Create_ValidUser_ReturnsCreatedUser` - Create erstellt erfolgreich einen User

---

## Wichtige Hinweise

1. **InMemory-Datenbank**: Repository-Tests verwenden eine temporäre InMemory-Datenbank, die nach jedem Test gelöscht wird.

2. **Mocking**: Service- und Controller-Tests verwenden Moq zum Mocken der Dependencies.

3. **Isolation**: Jeder Test ist unabhängig und beeinflusst keine anderen Tests.

4. **Arrange-Act-Assert**: Alle Tests folgen dem AAA-Pattern.

---

## Fehlende Test-Coverage

Die folgenden Komponenten haben noch keine Unit Tests:

- ? TokenService
- ? BookingExpirationService (Background Service)
- ? DTOs Validierung
- ? Extension Methods (Mapping)
- ? Models (Entity Business Logic)

Diese können in zukünftigen Iterationen hinzugefügt werden.
