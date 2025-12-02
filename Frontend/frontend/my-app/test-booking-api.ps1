# PowerShell Script zum Testen der Booking API

Write-Host "=== BOOKING API DEBUG SCRIPT ===" -ForegroundColor Cyan
Write-Host ""

# Test 1: Backend erreichbar?
Write-Host "Test 1: Ist das Backend erreichbar?" -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5031/api/bookings" -Method GET -UseBasicParsing
    Write-Host "✓ Backend ist erreichbar (Status: $($response.StatusCode))" -ForegroundColor Green
} catch {
    Write-Host "✗ Backend ist NICHT erreichbar!" -ForegroundColor Red
    Write-Host "Starten Sie das Backend:" -ForegroundColor Yellow
    Write-Host "  cd PostGresAPI" -ForegroundColor White
    Write-Host "  dotnet run" -ForegroundColor White
    exit 1
}
Write-Host ""

# Test 2: Rooms abrufen
Write-Host "Test 2: Verfügbare Rooms abrufen..." -ForegroundColor Yellow
try {
    $rooms = Invoke-RestMethod -Uri "http://localhost:5031/api/rooms" -Method GET
    Write-Host "✓ Rooms abgerufen:" -ForegroundColor Green
    $rooms | ForEach-Object { Write-Host "  - Room ID: $($_.id), Name: $($_.name), Type: $($_.type)" -ForegroundColor White }
} catch {
    Write-Host "✗ Fehler beim Abrufen der Rooms" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
}
Write-Host ""

# Test 3: Booking erstellen
Write-Host "Test 3: Booking erstellen..." -ForegroundColor Yellow
$bookingData = @{
    roomId = 4
    startUtc = "2025-12-05T10:00:00Z"
    endUtc = "2025-12-06T10:00:00Z"
    title = "test@debug.com"
} | ConvertTo-Json

Write-Host "Sende Daten:" -ForegroundColor Cyan
Write-Host $bookingData -ForegroundColor White
Write-Host ""

try {
    $result = Invoke-RestMethod -Uri "http://localhost:5031/api/bookings" `
      -Method POST `
        -Body $bookingData `
     -ContentType "application/json"
    
    Write-Host "✓ Booking erfolgreich erstellt!" -ForegroundColor Green
  Write-Host "Booking Details:" -ForegroundColor Cyan
    Write-Host "  ID: $($result.id)" -ForegroundColor White
    Write-Host "Booking Number: $($result.bookingNumber)" -ForegroundColor White
    Write-Host "  Room ID: $($result.roomId)" -ForegroundColor White
    Write-Host "  Start: $($result.startTime)" -ForegroundColor White
    Write-Host "  End: $($result.endTime)" -ForegroundColor White
    Write-Host "  Title: $($result.title)" -ForegroundColor White
} catch {
    Write-Host "✗ FEHLER bei Booking-Erstellung!" -ForegroundColor Red
    Write-Host "Status Code: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Red
    Write-Host "Fehlermeldung:" -ForegroundColor Red
    
    if ($_.ErrorDetails) {
        $errorObj = $_.ErrorDetails.Message | ConvertFrom-Json
  Write-Host "  Error: $($errorObj.error)" -ForegroundColor Red
    } else {
        Write-Host $_.Exception.Message -ForegroundColor Red
    }
}
Write-Host ""

# Test 4: Alle Bookings abrufen
Write-Host "Test 4: Alle Bookings abrufen..." -ForegroundColor Yellow
try {
    $bookings = Invoke-RestMethod -Uri "http://localhost:5031/api/bookings" -Method GET
    Write-Host "✓ Anzahl Bookings: $($bookings.Count)" -ForegroundColor Green
    if ($bookings.Count -gt 0) {
    Write-Host "Letzte 3 Bookings:" -ForegroundColor Cyan
      $bookings | Select-Object -Last 3 | ForEach-Object {
  Write-Host "  - ID: $($_.id), Number: $($_.bookingNumber), Room: $($_.roomId), Title: $($_.title)" -ForegroundColor White
        }
    }
} catch {
    Write-Host "✗ Fehler beim Abrufen der Bookings" -ForegroundColor Red
}
Write-Host ""

Write-Host "=== TESTS ABGESCHLOSSEN ===" -ForegroundColor Cyan
