#!/bin/bash

# Booking Debug Script
# Dieses Script testet die Booking API direkt

echo "=== BOOKING API DEBUG SCRIPT ==="
echo ""

# Test 1: Backend erreichbar?
echo "Test 1: Ist das Backend erreichbar?"
curl -s -o /dev/null -w "%{http_code}" http://localhost:5031/api/bookings
if [ $? -eq 0 ]; then
    echo "✓ Backend ist erreichbar"
else
    echo "✗ Backend ist NICHT erreichbar! Starten Sie das Backend:"
    echo "  cd PostGresAPI && dotnet run"
    exit 1
fi
echo ""

# Test 2: Rooms abrufen
echo "Test 2: Rooms abrufen..."
curl -s http://localhost:5031/api/rooms | json_pp
echo ""

# Test 3: Booking erstellen
echo "Test 3: Booking erstellen..."
RESPONSE=$(curl -s -X POST http://localhost:5031/api/bookings \
  -H "Content-Type: application/json" \
  -d '{
    "roomId": 4,
    "startUtc": "2025-12-05T10:00:00Z",
    "endUtc": "2025-12-06T10:00:00Z",
    "title": "test@debug.com"
  }')

echo "Response:"
echo $RESPONSE | json_pp
echo ""

# Test 4: Prüfe auf Fehler
if echo $RESPONSE | grep -q "error"; then
    echo "✗ FEHLER bei Booking-Erstellung!"
    echo "Fehlermeldung:"
    echo $RESPONSE | json_pp
else
    echo "✓ Booking erfolgreich erstellt!"
fi
