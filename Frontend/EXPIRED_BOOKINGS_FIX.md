# Expired Bookings - Prevent Check-in and Cancellation

## Problem
Users could still check-in and cancel bookings that were already expired.

## Solution Implemented

### 1. Added `isExpired()` Helper Method

**File:** `frontend\my-app\src\app\pages\dashboard-page.component\dashboard-page.component.ts`

```typescript
isExpired(booking: BookingDisplay): boolean {
  return booking.status === BookingStatus.Expired;
}
```

### 2. Updated `toggleCheckIn()` Method

**Before:**
```typescript
toggleCheckIn(booking: BookingDisplay) {
  // Don't allow toggle for cancelled bookings or already checked-in bookings
  if (this.isCancelled(booking) || this.isCheckedIn(booking)) {
    return;
  }
  // ...
}
```

**After:**
```typescript
toggleCheckIn(booking: BookingDisplay) {
  // Don't allow toggle for cancelled, expired bookings or already checked-in bookings
  if (this.isCancelled(booking) || this.isExpired(booking) || this.isCheckedIn(booking)) {
    return;
  }
  // ...
}
```

### 3. Updated `cancelBooking()` Method

**Before:**
```typescript
cancelBooking(bookingNumber: string) {
  const booking = this.bookings.find(b => b.bookingNumber === bookingNumber);
  if (!booking) return;

  // Don't allow cancelling if already cancelled
  if (this.isCancelled(booking)) {
    return;
  }
  // ...
}
```

**After:**
```typescript
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

### 4. Updated HTML Template

**File:** `frontend\my-app\src\app\pages\dashboard-page.component\dashboard-page.component.html`

**Check-in Toggle:**
```html
<!-- Before -->
<div class="toggle" [class.disabled]="isCancelled(b) || isCheckedIn(b)">
  <input type="checkbox"
         [disabled]="isCancelled(b) || isCheckedIn(b)"
         ... />
</div>

<!-- After -->
<div class="toggle" [class.disabled]="isCancelled(b) || isExpired(b) || isCheckedIn(b)">
  <input type="checkbox"
         [disabled]="isCancelled(b) || isExpired(b) || isCheckedIn(b)"
         ... />
</div>
```

**Cancel Button:**
```html
<!-- Before -->
<button class="stornieren"
        [disabled]="isCancelled(b)"
        ...>

<!-- After -->
<button class="stornieren"
        [disabled]="isCancelled(b) || isExpired(b)"
        ...>
```

**Row Styling:**
```html
<!-- Before -->
<div class="overview-row"
     *ngFor="let b of bookings"
     [class.cancelled-row]="isCancelled(b)">

<!-- After -->
<div class="overview-row"
     *ngFor="let b of bookings"
     [class.cancelled-row]="isCancelled(b)"
     [class.expired-row]="isExpired(b)">
```

### 5. Added Visual Styling for Expired Bookings

**File:** `frontend\my-app\src\app\pages\dashboard-page.component\dashboard-page.component.scss`

```scss
.overview-row {
  // ...existing code...

  &.cancelled-row {
    opacity: 0.5;
    background: rgba(50, 50, 50, 0.3);

    > div:not(.cell-left) {
      color: rgba(200, 200, 200, 0.6);
    }
  }

  // NEW: Expired row styling
  &.expired-row {
    opacity: 0.5;
    background: rgba(80, 50, 50, 0.3);

    > div:not(.cell-left) {
      color: rgba(200, 180, 180, 0.6);
    }
  }
}
```

## How It Works Now

### Status: Expired

**Visual Appearance:**
- Row has reddish-gray background (`rgba(80, 50, 50, 0.3)`)
- Text is dimmed (`rgba(200, 180, 180, 0.6)`)
- Opacity reduced to 50%

**Disabled Actions:**
- ? Check-in toggle is disabled and grayed out
- ? Cancel button is disabled and grayed out
- ? "View room" button still works

**Status Button:**
- Shows "Expired" status with red/orange styling (`.status-not-checked`)

### Status: Cancelled

**Visual Appearance:**
- Row has dark gray background (`rgba(50, 50, 50, 0.3)`)
- Text is dimmed (`rgba(200, 200, 200, 0.6)`)
- Opacity reduced to 50%

**Disabled Actions:**
- ? Check-in toggle is disabled and grayed out
- ? Cancel button is disabled and shows "Cancelled"
- ? "View room" button still works

**Status Button:**
- Shows "Cancelled" status with gray styling and strikethrough (`.status-cancelled`)

### Status: Pending

**Visual Appearance:**
- Normal row appearance

**Available Actions:**
- ? Can check-in
- ? Can cancel
- ? Can view room

### Status: CheckedIn

**Visual Appearance:**
- Normal row appearance

**Available Actions:**
- ? Check-in toggle is checked and disabled (already checked in)
- ? Can still cancel (if needed)
- ? Can view room

## Business Logic

### Expired Bookings
A booking becomes "Expired" when:
- The end date/time has passed
- The booking was never checked in
- Status automatically changes from `Pending` to `Expired`

**Why disable actions?**
- **Check-in:** Cannot check into a booking that has already ended
- **Cancel:** No point canceling a booking that has already expired
- **View room:** Still allowed to see room details for reference

### Cancelled Bookings
A booking is "Cancelled" when:
- User manually cancelled the booking
- Status changed from `Pending` or `CheckedIn` to `Cancelled`

**Why disable actions?**
- **Check-in:** Cannot check into a cancelled booking
- **Cancel:** Already cancelled, button shows "Cancelled" text
- **View room:** Still allowed to see room details

## Testing

### Test Case 1: Expired Booking
1. Create a booking with end date in the past
2. Backend should mark it as `Expired`
3. Login to dashboard
4. Expected:
   - Row has reddish-gray background
   - Status shows "Expired" in red
   - Check-in toggle is disabled (grayed out)
   - Cancel button is disabled (grayed out)
   - "View room" button still works

### Test Case 2: Try to Check-in Expired Booking
1. Have an expired booking
2. Try to click the check-in toggle
3. Expected: Nothing happens (disabled)

### Test Case 3: Try to Cancel Expired Booking
1. Have an expired booking
2. Try to click the "Cancel" button
3. Expected: Nothing happens (disabled)

### Test Case 4: Pending Booking
1. Have a pending booking (future dates)
2. Expected:
   - Row has normal appearance
   - Check-in toggle is enabled
   - Cancel button is enabled and shows "Cancel"

### Test Case 5: Checked-in Booking
1. Check in a booking
2. Expected:
   - Row has normal appearance
   - Check-in toggle is checked and disabled
   - Cancel button is still enabled (in case of early checkout)

## Files Changed

### Frontend
- `frontend\my-app\src\app\pages\dashboard-page.component\dashboard-page.component.ts`
  - Added `isExpired()` helper method
  - Updated `toggleCheckIn()` to check for expired status
  - Updated `cancelBooking()` to check for expired status

- `frontend\my-app\src\app\pages\dashboard-page.component\dashboard-page.component.html`
  - Added `isExpired(b)` check to toggle disabled condition
  - Added `isExpired(b)` check to cancel button disabled condition
  - Added `[class.expired-row]="isExpired(b)"` to row styling

- `frontend\my-app\src\app\pages\dashboard-page.component\dashboard-page.component.scss`
  - Added `.expired-row` styling with reddish-gray background

## Summary

? **Fixed:** Expired bookings can no longer be checked in
? **Fixed:** Expired bookings can no longer be cancelled
? **Visual:** Expired bookings show distinct reddish-gray background
? **UX:** Disabled buttons provide clear visual feedback
? **Consistent:** Expired and cancelled bookings have similar disabled behavior

The implementation prevents users from performing invalid actions on expired bookings while maintaining the ability to view room details for reference.
