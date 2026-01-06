# Reviews System

Einfaches Bewertungssystem für Hotel Adula.

## Datenbank

### Reviews Tabelle
- **Id** (PK)
- **UserId** (FK zu Users)
- **Title** (Titel der Bewertung)
- **Content** (Bewertungstext)
- **Rating** (1-5 Sterne)
- **CreatedAt** (Erstellungsdatum)
- **UpdatedAt** (Aktualisierungsdatum, optional)

## Migration anwenden

```bash
cd PostGresAPI
dotnet ef migrations add AddReviews
dotnet ef database update
```

Oder in Visual Studio Package Manager Console:
```powershell
Add-Migration AddReviews
Update-Database
```

## API Endpoints

### Reviews

- `GET /api/reviews` - Alle Reviews abrufen
- `GET /api/reviews/{id}` - Review nach ID abrufen
- `GET /api/reviews/user/{userId}` - Alle Reviews eines Users abrufen
- `POST /api/reviews` - Neue Review erstellen (Login erforderlich)
- `PUT /api/reviews/{id}` - Review aktualisieren (nur eigener Besitzer, Login erforderlich)
- `DELETE /api/reviews/{id}` - Review löschen (nur eigener Besitzer, Login erforderlich)

## Request Beispiele

### Review erstellen
```http
POST /api/reviews
Authorization: Bearer {token}
Content-Type: application/json

{
  "title": "Toller Aufenthalt!",
  "content": "Wir hatten eine wunderbare Zeit im Hotel Adula. Das Personal war sehr freundlich.",
  "rating": 5
}
```

### Review aktualisieren
```http
PUT /api/reviews/1
Authorization: Bearer {token}
Content-Type: application/json

{
  "title": "Sehr guter Aufenthalt",
  "content": "Nach einiger Überlegung... Das Hotel war wirklich großartig!",
  "rating": 5
}
```

## Validierung

- **Rating**: Muss zwischen 1 und 5 sein
- **Title**: Erforderlich (max. 200 Zeichen)
- **Content**: Erforderlich
- **Ownership**: User können nur ihre eigenen Reviews bearbeiten/löschen
- **Authentication**: Erstellen/Aktualisieren/Löschen erfordert Login

## Response Format

```json
{
  "id": 1,
  "userId": 123,
  "userName": "Max Mustermann",
  "title": "Toller Aufenthalt!",
  "content": "Wir hatten eine wunderbare Zeit...",
  "rating": 5,
  "createdAt": "2025-01-27T10:00:00Z",
  "updatedAt": null
}
```

## Frontend Integration

Die Comments-Seite existiert bereits unter `/comments`. Du kannst dort die Reviews anzeigen:

1. **Angular Service erstellen** (`review.service.ts`)
2. **Model erstellen** (`review.model.ts`)
3. **Comments Page aktualisieren** um Reviews anzuzeigen und zu erstellen

### Beispiel Angular Service

```typescript
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Review {
  id: number;
  userId: number;
  userName: string;
  title: string;
  content: string;
  rating: number;
  createdAt: Date;
  updatedAt?: Date;
}

@Injectable({
  providedIn: 'root'
})
export class ReviewService {
  private apiUrl = 'http://localhost:5000/api/reviews';

  constructor(private http: HttpClient) {}

  getAll(): Observable<Review[]> {
    return this.http.get<Review[]>(this.apiUrl);
  }

  getById(id: number): Observable<Review> {
    return this.http.get<Review>(`${this.apiUrl}/${id}`);
  }

  create(review: { title: string; content: string; rating: number }): Observable<Review> {
    return this.http.post<Review>(this.apiUrl, review);
  }

  update(id: number, review: { title: string; content: string; rating: number }): Observable<Review> {
    return this.http.put<Review>(`${this.apiUrl}/${id}`, review);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
```
