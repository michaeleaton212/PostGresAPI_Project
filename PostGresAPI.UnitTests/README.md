# Unit Tests für PostGresAPI Backend

Dieses Projekt enthält umfassende Unit Tests für das PostGresAPI Backend.

## ?? Struktur

```
PostGresAPI.UnitTests/
??? Services/
?   ??? BookingServiceTests.cs         (18 Tests)
?   ??? RoomServiceTests.cs            (18 Tests)
??? Controllers/
?   ??? BookingsControllerTests.cs     (13 Tests)
?   ??? RoomsControllerTests.cs        (9 Tests)
??? Repository/
    ??? BookingRepositoryIntegrationTests.cs  (13 Tests)
    ??? RoomRepositoryIntegrationTests.cs     (11 Tests)
```

**Gesamt: 82 Unit Tests**

## ?? Tests ausführen

### Alle Tests ausführen
```bash
dotnet test PostGresAPI.UnitTests/PostGresAPI.UnitTests.csproj
```

### Nur Service Tests
```bash
dotnet test --filter "FullyQualifiedName~Services"
```

### Nur Controller Tests
```bash
dotnet test --filter "FullyQualifiedName~Controllers"
```

### Nur Repository Tests
```bash
dotnet test --filter "FullyQualifiedName~Repository"
```

### Mit Code Coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## ?? Test-Abdeckung

### BookingService (18 Tests)
? Create - Gültige Buchung  
? Create - Start nach Ende (Fehler)  
? Create - Raum existiert nicht (Fehler)  
? Create - Zeitbereich überschneidet sich (Fehler)  
? Update - Gültige Aktualisierung  
? Update - Start nach Ende (Fehler)  
? Update - Buchung nicht gefunden  
? UpdateStatus - Gültiger Status  
? UpdateStatus - Ungültiger Status  
? UpdateStatus - Buchung nicht gefunden  
? Delete - Existierende Buchung  
? Delete - Nicht existierende Buchung  
? GetBookingIdsByCredentials - Gültige Credentials  
? GetBookingIdsByCredentials - Ungültige Buchungsnummer  
? GetBookingIdsByCredentials - Name stimmt nicht überein  
? GetBookingIdsByCredentials - Leere Credentials  
? IsActive - Buchung ist aktiv  
? IsActive - Buchung noch nicht gestartet  
? IsActive - Buchung bereits beendet  

### RoomService (18 Tests)
? GetAll - Ohne Filter (alle Räume)  
? GetAll - Filter nach Bedroom  
? GetAll - Filter nach Meetingroom  
? GetAll - Unbekannter Typ  
? GetAll - Case-insensitive Filter  
? GetById - Existierender Raum  
? GetById - Nicht existierender Raum  
? CreateMeetingroom - Gültige Daten  
? CreateMeetingroom - Ohne Bild  
? CreateBedroom - Gültige Daten  
? UpdateName - Existierender Raum  
? UpdateName - Nicht existierender Raum  
? UpdateImage - Existierender Raum  
? UpdateImage - Nicht existierender Raum  
? Delete - Existierender Raum  
? Delete - Nicht existierender Raum  
? GetAll - Sortiert nach ID  

### BookingsController (13 Tests)
? Create - Gültige Buchung  
? Create - Ungültige Buchung  
? Create - Raum nicht gefunden  
? Login - Gültige Credentials  
? Login - Ungültige Credentials  
? Login - Leere Booking IDs  
? GetByRoomId - Gültige RoomId  
? GetByRoomId - Keine Buchungen  
? GetAll - Mit gültigem Token  
? GetAll - Ohne Token  
? GetById - Mit Token und autorisierter Buchung  
? GetById - Mit Token aber nicht autorisierte Buchung  
? Delete - Mit gültigem Token und autorisierter Buchung  
? Delete - Buchung nicht gefunden  

### RoomsController (9 Tests)
? GetAll - Ohne Filter  
? GetAll - Filter nach Bedroom  
? GetAll - Filter nach Meetingroom  
? GetAll - Unbekannter Filter  
? GetById - Existierender Raum  
? GetById - Nicht existierender Raum  
? GetById - Verschiedene IDs (Theory Test)  
? GetAll - Leere Datenbank  

### BookingRepository Integration Tests (13 Tests)
? HasOverlap - Mit überschneidender Buchung  
? HasOverlap - Ohne überschneidende Buchung  
? HasOverlap - Unterschiedlicher Raum  
? HasOverlap - Mit ExcludeBookingId  
? GetByRoomId - Nur Buchungen für diesen Raum  
? GetByName - Buchungen mit übereinstimmendem Namen  
? GetByBookingNumber - Korrekte Buchung  
? Add - Erstellt neue Buchung  
? Update - Aktualisiert existierende Buchung  
? UpdateStatus - Aktualisiert Buchungsstatus  
? Delete - Entfernt Buchung  
? Delete - Nicht existierende Buchung  
? GetAll - Sortiert nach StartTime  

### RoomRepository Integration Tests (11 Tests)
? GetAll - Gibt alle Räume zurück  
? GetBedrooms - Nur Bedrooms  
? GetMeetingrooms - Nur Meetingrooms  
? GetById - Existierender Raum  
? GetById - Nicht existierender Raum  
? Add - Erstellt Bedroom  
? Add - Erstellt Meetingroom  
? UpdateName - Aktualisiert Raumnamen  
? UpdateName - Nicht existierender Raum  
? UpdateImage - Aktualisiert Raumbild  
? UpdateImage - Nicht existierender Raum  
? Delete - Entfernt Raum  
? Delete - Nicht existierender Raum  
? Exists - Existierender Raum  
? Exists - Nicht existierender Raum  
? GetAll - Sortiert nach ID  

## ??? Verwendete Technologien

- **xUnit** - Test Framework
- **Moq** - Mocking Framework für Unit Tests
- **FluentAssertions** - Für lesbare Assertions
- **Microsoft.EntityFrameworkCore.InMemory** - In-Memory Datenbank für Integration Tests

## ?? Test-Patterns

### Unit Tests mit Mocks
```csharp
[Fact]
public async Task Create_ValidBooking_ReturnsSuccess()
{
    // Arrange
    var dto = new CreateBookingDto { ... };
    _mockRepo.Setup(r => r.Add(It.IsAny<CreateBookingDto>()))
        .ReturnsAsync(new Booking(...));

    // Act
    var result = await _service.Create(dto);

    // Assert
    Assert.True(result.Ok);
    Assert.NotNull(result.Result);
}
```

### Integration Tests mit InMemory DB
```csharp
[Fact]
public async Task Add_CreatesNewBooking()
{
    // Arrange
    var dto = new CreateBookingDto { ... };

    // Act
    var result = await _repository.Add(dto);

    // Assert
    Assert.NotNull(result);
    var savedBooking = await _context.Bookings.FindAsync(result.Id);
    Assert.NotNull(savedBooking);
}
```

## ?? Best Practices

1. **AAA-Pattern**: Arrange, Act, Assert in jedem Test
2. **Isolation**: Jeder Test ist unabhängig
3. **Mocking**: Services mocken Repositories für schnelle Tests
4. **Integration Tests**: Repository-Tests verwenden InMemory DB
5. **Cleanup**: Dispose() räumt nach Integration Tests auf
6. **Descriptive Names**: Test-Namen beschreiben das Szenario

## ?? Continuous Integration

Diese Tests können in CI/CD Pipelines integriert werden:

```yaml
# Beispiel für GitHub Actions
- name: Run Tests
  run: dotnet test --no-build --verbosity normal
```

## ?? Debugging

### Test im Debug-Modus ausführen
```bash
dotnet test --logger "console;verbosity=detailed"
```

### Nur fehlgeschlagene Tests
```bash
dotnet test --filter "TestCategory=Failed"
```

## ?? Nächste Schritte

Mögliche Erweiterungen:
- [ ] Token-Service Tests hinzufügen
- [ ] Validierungs-Tests für DTOs
- [ ] Performance-Tests für große Datenmengen
- [ ] End-to-End Tests mit TestServer
- [ ] Mutation Testing für Code Coverage Verbesserung

## ?? Tipps

1. **Vor jedem Commit**: Tests ausführen
2. **Bei Refactoring**: Tests bleiben grün
3. **Neue Features**: Zuerst Tests schreiben (TDD)
4. **Bug Fixes**: Test reproduziert Bug, dann fixen

---

**Erstellt:** 2025
**Framework:** .NET 10.0 / .NET 8.0  
**Test Runner:** xUnit  
