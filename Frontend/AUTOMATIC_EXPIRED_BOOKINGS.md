# Automatic Expired Booking Status Update

## Problem
Bookings in the past should automatically have their status set to "Expired", and users should not be able to check-in or cancel expired bookings.

## Solution Implemented

### Backend: Automatic Status Update

**File:** `..\PostGresAPI\Services\BookingService.cs`

#### Added Private Method: `UpdateExpiredBookings()`

```csharp
private async Task UpdateExpiredBookings()
{
    var allBookings = await _bookings.GetAll();
    var now = DateTimeOffset.UtcNow;

    foreach (var booking in allBookings)
    {
        // Only update bookings that are Pending or CheckedIn and have ended
        if ((booking.Status == BookingStatus.Pending || booking.Status == BookingStatus.CheckedIn) 
            && booking.EndTime < now)
        {
            await _bookings.UpdateStatus(booking.Id, BookingStatus.Expired);
        }
    }
}
```

**Logic:**
1. Gets all bookings from database
2. Compares current UTC time with booking end time
3. If booking has ended (`EndTime < now`) AND status is `Pending` or `CheckedIn`
4. Automatically updates status to `Expired`

**Note:** Cancelled bookings are not changed to Expired.

#### Updated Methods to Call `UpdateExpiredBookings()`

All read methods now check for expired bookings before returning data:

```csharp
public async Task<List<BookingDto>> GetAll()
{
    await UpdateExpiredBookings();  // ? Added
    var list = await _bookings.GetAll();
    return list.Select(b => b.ToDto()).ToList();
}

public async Task<BookingDto?> GetById(int id)
{
    await UpdateExpiredBookings();  // ? Added
    var b = await _bookings.GetById(id);
    return b is null ? null : b.ToDto();
}

public async Task<List<BookingDto>> GetByRoomId(int roomId)
{
    await UpdateExpiredBookings();  // ? Added
    var list = await _bookings.GetByRoomId(roomId);
    return list.Select(b => b.ToDto()).ToList();
}

public async Task<List<BookingDto>> GetByName(string name)
{
    await UpdateExpiredBookings();  // ? Added
    var list = await _bookings.GetByName(name);
    return list.Select(b => b.ToDto()).ToList();
}

public async Task<List<BookingDto>> GetByIds(List<int> ids)
{
    await UpdateExpiredBookings();  // ? Added
    var list = await _bookings.GetByIds(ids);
    return list.Select(b => b.ToDto()).ToList();
}
```

### Frontend: Prevent Actions on Expired Bookings

**Already Implemented** (from previous fix):

**File:** `frontend\my-app\src\app\pages\dashboard-page.component\dashboard-page.component.ts`

```typescript
isExpired(booking: BookingDisplay): boolean {
  return booking.status === BookingStatus.Expired;
}

toggleCheckIn(booking: BookingDisplay) {
  // Don't allow toggle for cancelled, expired bookings or already checked-in bookings
  if (this.isCancelled(booking) || this.isExpired(booking) || this.isCheckedIn(booking)) {
    return;
  }
  // ...
}

cancelBooking(bookingNumber: string) {
  const booking = this.bookings.find(b => b.bookingNumber === bookingNumber);
  if (!booking) return;

  // Don't allow cancelling if already cancelled or expired
  if (this.isCancelled(booking) || this.isExpired(booking)) {
    return;
  }
  // ...
}
```

**File:** `frontend\my-app\src\app\pages\dashboard-page.component\dashboard-page.component.html`

```html
<!-- Check-in toggle disabled for expired -->
<div class="toggle" [class.disabled]="isCancelled(b) || isExpired(b) || isCheckedIn(b)">
  <input type="checkbox"
         [disabled]="isCancelled(b) || isExpired(b) || isCheckedIn(b)"
         ... />
</div>

<!-- Cancel button disabled for expired -->
<button class="stornieren"
        [disabled]="isCancelled(b) || isExpired(b)"
        ...>
```

**File:** `frontend\my-app\src\app\pages\dashboard-page.component\dashboard-page.component.scss`

```scss
&.expired-row {
  opacity: 0.5;
  background: rgba(80, 50, 50, 0.3);

  > div:not(.cell-left) {
    color: rgba(200, 180, 180, 0.6);
  }
}
```

## How It Works

### Scenario 1: Booking Expires Naturally

1. **User creates booking** for Dec 1-3, 2025
2. **Dec 3, 2025 23:59** - Booking is still `Pending`
3. **Dec 4, 2025 00:01** - Booking end time has passed
4. **User opens dashboard** (or any API call that reads bookings)
5. **Backend automatically checks** all bookings
6. **Finds booking with** `EndTime < Now` and `Status = Pending`
7. **Updates status** to `Expired`
8. **Frontend receives** updated booking with `status: "Expired"`
9. **Dashboard displays** booking with:
   - Reddish-gray background
   - Status shows "Expired"
   - Check-in toggle disabled
   - Cancel button disabled

### Scenario 2: Checked-In Booking Expires

1. **User checks in** on Dec 1, 2025 ? Status: `CheckedIn`
2. **Dec 3, 2025 ends** ? Booking end time passes
3. **Dec 4, 2025** - Next API call triggers update
4. **Status changes** from `CheckedIn` ? `Expired`
5. **Reason:** Guest should have checked out

### Scenario 3: Cancelled Booking (No Change)

1. **User cancels booking** ? Status: `Cancelled`
2. **Booking end time passes**
3. **Status remains** `Cancelled` (not changed to Expired)
4. **Reason:** Cancelled bookings stay cancelled

## Status Transition Flow

```
Pending ???????
              ???> Expired (if EndTime < Now)
CheckedIn ?????

Cancelled ????> Cancelled (never changes to Expired)
```

## Timing Details

**When does a booking expire?**
- As soon as `EndTime < DateTimeOffset.UtcNow`
- For example:
  - Booking ends: `2025-12-03T23:00:00Z` (11 PM UTC)
  - Current time: `2025-12-04T00:01:00Z` (12:01 AM UTC)
  - Result: Booking is expired

**When is the status updated?**
- Every time any read operation is performed:
  - `GET /api/bookings`
  - `GET /api/bookings/{id}`
  - `GET /api/bookings/room/{roomId}`
  - `POST /api/bookings/by-ids`
  - `GET /api/bookings/by-name/{name}`

**Why this approach?**
- **No background job needed** - Updates happen on-demand
- **Always current** - Status is checked on every read
- **Simple** - No scheduling or timers required
- **Efficient** - Only processes bookings that need updating

## Performance Considerations

**Concern:** Calling `UpdateExpiredBookings()` on every read might be slow.

**Solution:**
- Method only updates bookings that are actually expired
- Database query is fast (indexed on `EndTime` and `Status`)
- Updates are minimal (only Pending/CheckedIn bookings past their end time)

**Optimization (if needed):**
```csharp
private async Task UpdateExpiredBookings()
{
    var now = DateTimeOffset.UtcNow;
    
    // Only get bookings that might need updating
    var allBookings = await _bookings.GetAll();
    var expiredBookings = allBookings
        .Where(b => (b.Status == BookingStatus.Pending || b.Status == BookingStatus.CheckedIn) 
                    && b.EndTime < now)
        .ToList();

    // Batch update (if repository supports it)
    foreach (var booking in expiredBookings)
    {
        await _bookings.UpdateStatus(booking.Id, BookingStatus.Expired);
    }
}
```

## Database Impact

**Before:**
```sql
SELECT * FROM bookings WHERE id = 1;
-- Status: Pending, EndTime: 2025-12-03 23:00:00+00
```

**After first API call on Dec 4, 2025:**
```sql
-- Automatic update triggered
UPDATE bookings SET status = 'Expired' WHERE id = 1;

SELECT * FROM bookings WHERE id = 1;
-- Status: Expired, EndTime: 2025-12-03 23:00:00+00
```

## Testing

### Test Case 1: Pending Booking Expires
1. Create booking: Start: Dec 1, End: Dec 3
2. Change system time to Dec 4 (or wait)
3. Call `GET /api/bookings`
4. Expected:
   - Status automatically changes to `Expired`
   - Frontend disables check-in and cancel buttons

### Test Case 2: Checked-In Booking Expires
1. Create booking and check in
2. Wait for end time to pass
3. Load dashboard
4. Expected:
   - Status changes from `CheckedIn` to `Expired`
   - Check-in toggle disabled, Cancel button disabled

### Test Case 3: Cancelled Booking Stays Cancelled
1. Create booking and cancel it
2. Wait for end time to pass
3. Load dashboard
4. Expected:
   - Status remains `Cancelled` (not Expired)

### Test Case 4: Multiple Bookings
1. Create 3 bookings: Past, Current, Future
2. Load dashboard
3. Expected:
   - Past booking ? `Expired`
   - Current booking ? `Pending` or `CheckedIn`
   - Future booking ? `Pending`

## Files Changed

### Backend
- `..\PostGresAPI\Services\BookingService.cs`
  - Added `UpdateExpiredBookings()` method
  - Updated all read methods to call `UpdateExpiredBookings()`

### Frontend
- No changes needed (already implemented in previous fix)

## Alternative Approaches (Not Used)

### 1. Background Job
```csharp
// Run every hour to update expired bookings
public class BookingExpirationJob : IHostedService
{
    public async Task ExecuteAsync()
    {
        // Update all expired bookings
    }
}
```
**Pros:** Predictable timing
**Cons:** Requires hosting infrastructure, more complex

### 2. Database Trigger
```sql
CREATE TRIGGER update_expired_bookings
BEFORE SELECT ON bookings
FOR EACH ROW
BEGIN
    IF NEW.end_time < NOW() AND NEW.status IN ('Pending', 'CheckedIn') THEN
        NEW.status = 'Expired';
    END IF;
END;
```
**Pros:** Automatic, no application code
**Cons:** Database-dependent, harder to test/debug

### 3. Client-Side Check
```typescript
// Frontend checks if booking is expired
isExpired(booking: BookingDisplay): boolean {
  return new Date(booking.endDate) < new Date();
}
```
**Pros:** No backend changes
**Cons:** Status in database doesn't match, inconsistent data

## Summary

? **Backend:** Automatically updates expired bookings on every read operation
? **Frontend:** Disables check-in and cancel buttons for expired bookings
? **Visual:** Expired bookings show reddish-gray background
? **Logic:** Only Pending or CheckedIn bookings can become Expired
? **Preserved:** Cancelled bookings remain Cancelled (never become Expired)
? **Performance:** Efficient on-demand updates, no background jobs needed
? **Simple:** No complex scheduling or timers required

Users can no longer check-in or cancel bookings that have passed their end time, and the status is automatically updated to "Expired" whenever bookings are loaded.
