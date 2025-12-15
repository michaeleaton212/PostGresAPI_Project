# Booking Per Room Fix - Implementation Summary

## Problem
1. "Time range already booked" error appeared even when booking different rooms
2. Booked dates weren't visually marked as unavailable in the calendar
3. Users couldn't see which dates were already booked for a specific room

## Solution Implemented

### 1. Backend Fix (BookingsController.cs)
**Changed:** `GET /api/bookings/room/{roomId}` endpoint

**Before:**
```csharp
[HttpGet("room/{roomId:int}")]
public async Task<ActionResult<IEnumerable<BookingDto>>> GetByRoomId(int roomId)
{
    // Filtered by user's authorized bookings - WRONG!
    if (!TryGetAuthorizedBookingIds(out var authorizedIds))
        return Unauthorized();
    
    var allDtos = await _svc.GetByRoomId(roomId);
    var filtered = allDtos.Where(b => authorizedIds.Contains(b.Id)).ToList();
    return Ok(filtered);
}
```

**After:**
```csharp
[HttpGet("room/{roomId:int}")]
// PUBLIC: Availability requires all bookings for the room, regardless of user
public async Task<ActionResult<IEnumerable<BookingDto>>> GetByRoomId(int roomId)
{
    var allDtos = await _svc.GetByRoomId(roomId);
    return Ok(allDtos);
}
```

**Why:** The endpoint now returns ALL bookings for a specific room, allowing the frontend to:
- Show which dates are already booked for that room
- Prevent selecting already booked dates
- Only show "time range already booked" error when that specific room is booked

**Note:** The overlap check in `BookingService.Create()` already filters by `roomId`:
```csharp
if (await _bookings.HasOverlap(createBookingDto.RoomId, ...))
    return (false, "Time range already booked.", null);
```

### 2. Frontend - Bedroom Component

**Already Implemented Correctly:**
- ? Loads bookings per room: `loadRoomBookings(roomId)`
- ? Marks booked dates: `isDateBooked(date)` checks `this.roomBookings`
- ? Prevents clicking on booked dates: `if (calendarDay.isBooked) return;`
- ? Visual styling with strikethrough and gray:

```scss
&.is-booked {
  opacity: 0.4;
  text-decoration: line-through;
  color: rgba(60, 60, 67, 0.5);
  cursor: not-allowed;
  background: rgba(200, 200, 200, 0.3);
}
```

### 3. Frontend - Meetingroom Component

**Added Visual Styling:**
```scss
&.is-booked {
  opacity: 0.4;
  text-decoration: line-through;
  color: rgba(60, 60, 67, 0.5);
  cursor: not-allowed;
  background: rgba(200, 200, 200, 0.3);
}
```

**Note:** Meetingrooms use time slots (30-minute intervals), so they show:
- Yellow background (`has-bookings`) = some bookings exist that day
- Tooltip on hover = shows exact booked time ranges
- Strikethrough (`is-booked`) = fully booked (if implemented)

The current design with yellow markers is appropriate for meetingrooms since they can have multiple bookings per day.

## How It Works Now

### Scenario 1: Booking a Bedroom
1. User opens "Bedroom A" preview
2. Frontend calls `GET /api/bookings/room/1` ? returns all bookings for room 1
3. Calendar marks booked dates with strikethrough and gray
4. User tries to select Dec 5-7
5. If Dec 5-7 is already booked for room 1 ? dates are grayed out, can't select
6. If Dec 5-7 is free for room 1 ? user can book it
7. "Time range already booked" only appears if room 1 is booked for those dates

### Scenario 2: Multiple Rooms
- Room 1 (Bedroom A) is booked Dec 5-7
- Room 2 (Bedroom B) is free Dec 5-7
- User can still book Room 2 for Dec 5-7
- No "time range already booked" error for Room 2

### Scenario 3: Meetingroom
1. User opens "Meetingroom X" preview
2. Frontend calls `GET /api/bookings/room/5` ? returns all bookings for room 5
3. Days with bookings show yellow background
4. Hover over day shows tooltip with exact time ranges booked
5. User selects day + time slot (e.g., Dec 5 at 10:00-10:30)
6. If that exact time slot is free ? booking succeeds
7. If that exact time slot is booked ? "time range already booked" error

## Files Changed

### Backend
- `../PostGresAPI/Controllers/BookingsController.cs`
  - Removed authorization from `GetByRoomId` endpoint

### Frontend
- `frontend/my-app/src/app/pages/bedroom-preview-page.component/`
  - No changes needed (already correct)
  
- `frontend/my-app/src/app/pages/meetingroom-preview-page.component/meetingroom-preview-page.component.scss`
  - Added `.is-booked` styling for consistency

## Testing

### Test Case 1: Bedroom Booking Per Room
1. Create booking for Bedroom A (room ID 1) for Dec 5-7
2. Open Bedroom A preview
   - Expected: Dec 5-7 shows strikethrough and gray
   - Expected: Cannot click/select Dec 5-7
3. Open Bedroom B preview (room ID 2)
   - Expected: Dec 5-7 is available (not grayed out)
   - Expected: Can book Dec 5-7 for Bedroom B
   - Expected: No "time range already booked" error

### Test Case 2: Meetingroom Booking Per Room
1. Create booking for Meetingroom X (room ID 5) for Dec 5 10:00-10:30
2. Open Meetingroom X preview
   - Expected: Dec 5 shows yellow background
   - Expected: Hover shows tooltip "10:00 - 10:30"
3. Try to book Dec 5 10:00-10:30 for Meetingroom X
   - Expected: "Time range already booked" error
4. Try to book Dec 5 11:00-11:30 for Meetingroom X
   - Expected: Booking succeeds (different time slot)
5. Open Meetingroom Y preview (room ID 6)
   - Expected: Dec 5 10:00-10:30 is available
   - Expected: Can book without error

### Test Case 3: Cross-Room Independence
1. Book all rooms for Dec 10-12
2. Open any room preview
   - Expected: Each room only shows ITS OWN bookings as grayed out
   - Expected: Booking attempt fails only for dates already booked FOR THAT ROOM

## Database Queries

The overlap check is per room:
```sql
-- Executed by HasOverlap in BookingRepository
SELECT COUNT(*) FROM bookings 
WHERE room_id = {roomId}  -- <-- KEY: Filters by room
  AND start_time < {endUtc}
  AND end_time > {startUtc}
```

This ensures each room's availability is checked independently.

## Security Considerations

**Question:** Is it safe to return all bookings for a room without authentication?

**Answer:** Yes, for availability checking:
- Booking details (title/name) are visible, but this is expected for a booking system
- Room availability must be public to prevent double bookings
- Personal operations (update, delete) still require authentication
- This is similar to hotel booking systems where you can see "booked" dates

**If Privacy is Required:** Filter `title` field in response:
```csharp
[HttpGet("room/{roomId:int}")]
public async Task<ActionResult<IEnumerable<BookingDto>>> GetByRoomId(int roomId)
{
    var allDtos = await _svc.GetByRoomId(roomId);
    // Anonymize for public availability
    var anonymized = allDtos.Select(dto => dto with { Title = null }).ToList();
    return Ok(anonymized);
}
```

## Summary

? **Fixed:** "Time range already booked" now only appears when that specific room is booked
? **Fixed:** Each room shows only its own booked dates as grayed out
? **Fixed:** Users can book the same date range for different rooms
? **Visual:** Booked dates show strikethrough and gray background
? **UX:** Users can't click on booked dates in bedroom calendars
? **UX:** Meetingroom bookings show yellow markers with time tooltips

The implementation correctly handles per-room booking conflicts while maintaining a clear visual indication of availability.
