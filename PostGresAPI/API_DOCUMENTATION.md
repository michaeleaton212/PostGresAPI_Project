# PostGresAPI - Backend API Dokumentation

## ?? Basis-URL
```
http://localhost:5031
```

---

## ?? API Endpoints

### **1. Rooms (Alle Räume)**

#### GET `/api/rooms`
Gibt alle Räume zurück (Meetingrooms + Bedrooms).

**Query Parameter:**
- `type` (optional): `Meetingroom` oder `Bedroom` zum Filtern

**Beispiel:**
```http
GET /api/rooms
GET /api/rooms?type=Meetingroom
GET /api/rooms?type=Bedroom
```

**Response:**
```json
[
  {
    "id": 1,
    "name": "Conference Room A",
    "type": "Meetingroom"
  }
]
```

#### GET `/api/rooms/{id}`
Gibt einen einzelnen Raum zurück.

**Beispiel:**
```http
GET /api/rooms/1
```

---

### **2. Meetingrooms**

#### GET `/api/meetingrooms`
Gibt alle Meeting-Räume zurück.

**Response:**
```json
[
  {
    "id": 1,
    "name": "Conference Room A",
    "numberOfChairs": 20
  }
]
```

#### GET `/api/meetingrooms/{id}`
Gibt einen Meeting-Raum zurück.

#### POST `/api/meetingrooms`
Erstellt einen neuen Meeting-Raum.

**Request Body:**
```json
{
  "name": "New Conference Room",
  "numberOfChairs": 25
}
```

**Response:** `201 Created`
```json
{
  "id": 8,
  "name": "New Conference Room",
  "numberOfChairs": 25
}
```

#### PUT `/api/meetingrooms/{id}`
Aktualisiert einen Meeting-Raum.

**Request Body:**
```json
{
  "name": "Updated Conference Room",
  "numberOfChairs": 30
}
```

**Response:** `200 OK`

#### DELETE `/api/meetingrooms/{id}`
Löscht einen Meeting-Raum.

**Response:** `204 No Content`

---

### **3. Bedrooms**

#### GET `/api/bedrooms`
Gibt alle Schlafzimmer zurück.

**Response:**
```json
[
  {
    "id": 4,
    "name": "Room 101",
    "numberOfBeds": 1
  }
]
```

#### GET `/api/bedrooms/{id}`
Gibt ein Schlafzimmer zurück.

#### POST `/api/bedrooms`
Erstellt ein neues Schlafzimmer.

**Request Body:**
```json
{
  "name": "Suite 301",
  "numberOfBeds": 3
}
```

#### PUT `/api/bedrooms/{id}`
Aktualisiert ein Schlafzimmer.

**Request Body:**
```json
{
  "name": "Updated Room 101",
  "numberOfBeds": 2
}
```

#### DELETE `/api/bedrooms/{id}`
Löscht ein Schlafzimmer.

---

### **4. Bookings (Buchungen)**

#### GET `/api/bookings`
Gibt alle Buchungen zurück.

**Response:**
```json
[
  {
    "id": 1,
    "roomId": 1,
    "startTime": "2024-11-25T10:00:00Z",
    "endTime": "2024-11-25T11:00:00Z",
    "title": "Team Meeting"
  }
]
```

#### GET `/api/bookings/{id}`
Gibt eine einzelne Buchung zurück.

#### POST `/api/bookings`
Erstellt eine neue Buchung (z.B. Meeting-Raum für 1 Stunde buchen).

**Request Body:**
```json
{
  "roomId": 1,
  "startUtc": "2024-11-25T10:00:00Z",
  "endUtc": "2024-11-25T11:00:00Z",
  "title": "Team Meeting"
}
```

**Validierungen:**
- ? `startUtc` muss vor `endUtc` liegen
- ? Raum muss existieren
- ? Keine Überschneidungen mit anderen Buchungen

**Success Response:** `201 Created`
```json
{
  "id": 1,
  "roomId": 1,
  "startTime": "2024-11-25T10:00:00Z",
  "endTime": "2024-11-25T11:00:00Z",
  "title": "Team Meeting"
}
```

**Error Response:** `400 Bad Request`
```json
{
  "error": "Time range already booked."
}
```

#### PUT `/api/bookings/{id}`
Aktualisiert eine Buchung.

**Request Body:**
```json
{
  "startUtc": "2024-11-25T11:00:00Z",
"endUtc": "2024-11-25T12:00:00Z",
  "title": "Updated Meeting"
}
```

#### DELETE `/api/bookings/{id}`
Löscht eine Buchung.

**Response:** `204 No Content`

---

### **5. Users**

#### GET `/api/users`
Gibt alle Benutzer zurück.

**Response:**
```json
[
  {
    "id": 1,
    "userName": "john.doe",
    "email": "john@example.com"
  }
]
```

#### GET `/api/users/{id}`
Gibt einen einzelnen Benutzer zurück.

---

## ?? Frontend Integration - Beispiele

### **JavaScript/TypeScript Fetch**

```javascript
// Alle Meetingrooms laden
const getMeetingrooms = async () => {
  const response = await fetch('http://localhost:5031/api/meetingrooms');
  const data = await response.json();
  return data;
};

// Meetingroom für 1 Stunde buchen
const bookMeetingroom = async (roomId, startTime, endTime, title) => {
  const response = await fetch('http://localhost:5031/api/bookings', {
 method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({
      roomId: roomId,
      startUtc: startTime,  // z.B. "2024-11-25T10:00:00Z"
      endUtc: endTime,      // z.B. "2024-11-25T11:00:00Z"
      title: title
    })
  });
  
  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.error || 'Booking failed');
  }
  
  return await response.json();
};

// Verwendung
try {
  const booking = await bookMeetingroom(
    1,
  "2024-11-25T10:00:00Z",
    "2024-11-25T11:00:00Z",
    "Team Meeting"
  );
  console.log('Booking created:', booking);
} catch (error) {
  console.error('Error:', error.message);
}
```

### **React Beispiel**

```jsx
import { useState, useEffect } from 'react';

function MeetingroomBooking() {
  const [rooms, setRooms] = useState([]);
  const [selectedRoom, setSelectedRoom] = useState(null);
  
  useEffect(() => {
    // Meetingrooms laden
fetch('http://localhost:5031/api/meetingrooms')
    .then(res => res.json())
      .then(data => setRooms(data));
  }, []);
  
  const handleBooking = async (e) => {
    e.preventDefault();
    
    const formData = new FormData(e.target);
    const booking = {
      roomId: parseInt(formData.get('roomId')),
      startUtc: new Date(formData.get('startTime')).toISOString(),
      endUtc: new Date(formData.get('endTime')).toISOString(),
      title: formData.get('title')
 };
    
    try {
      const response = await fetch('http://localhost:5031/api/bookings', {
        method: 'POST',
     headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(booking)
      });
      
      if (!response.ok) {
        const error = await response.json();
     alert(error.error);
return;
  }
      
      alert('Booking successful!');
      e.target.reset();
    } catch (error) {
      alert('Error: ' + error.message);
  }
  };
  
  return (
    <div>
      <h2>Meeting Room Booking</h2>
      <form onSubmit={handleBooking}>
        <select name="roomId" required>
      <option value="">Select Room</option>
     {rooms.map(room => (
  <option key={room.id} value={room.id}>
     {room.name} ({room.numberOfChairs} chairs)
        </option>
          ))}
        </select>
        
        <input type="datetime-local" name="startTime" required />
        <input type="datetime-local" name="endTime" required />
        <input type="text" name="title" placeholder="Meeting Title" />
        
        <button type="submit">Book Now</button>
      </form>
    </div>
  );
}
```

---

## ?? CORS-Konfiguration

Falls Ihr Frontend auf einem anderen Port läuft, ist CORS bereits konfiguriert in `Program.cs`.

---

## ?? Datenbank-Schema

### Tabellen:
- **rooms** - Basis-Tabelle für alle Räume (TPH-Pattern)
  - `Id` (PK)
  - `Name`
  - `room_type` (Discriminator: "Meetingroom" | "Bedroom")
  - `number_of_chairs` (nur Meetingroom)
  - `number_of_beds` (nur Bedroom)

- **bookings** - Zeitbasierte Buchungen
  - `Id` (PK)
  - `RoomId` (FK ? rooms.Id)
  - `StartTime` (DateTimeOffset)
  - `EndTime` (DateTimeOffset)
  - `Title` (optional)

- **Users**
  - `Id` (PK)
  - `UserName`
  - `Email`

---

## ?? Seed-Daten

### Meetingrooms (IDs 1-3):
- Conference Room A (20 Stühle)
- Conference Room B (15 Stühle)
- Board Room (10 Stühle)

### Bedrooms (IDs 4-7):
- Room 101 (1 Bett)
- Room 102 (2 Betten)
- Room 103 (2 Betten)
- Suite 201 (3 Betten)

---

## ?? Backend starten

```bash
cd C:\PostGresAPI_Project\PostGresAPI_Project\PostGresAPI
dotnet run
```

Die API läuft dann auf: **http://localhost:5031**

---

## ?? Tests mit PostGresAPI.http

Öffnen Sie `PostGresAPI.http` in Visual Studio und führen Sie die HTTP-Requests direkt aus.
