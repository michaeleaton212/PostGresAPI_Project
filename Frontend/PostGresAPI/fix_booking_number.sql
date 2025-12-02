-- SQL Script to add BookingNumber column to bookings table
-- Run this in your PostgreSQL database

-- Step 1: Add the BookingNumber column (allow NULL initially)
ALTER TABLE bookings 
ADD COLUMN IF NOT EXISTS "BookingNumber" VARCHAR(50);

-- Step 2: Generate booking numbers for existing rows
UPDATE bookings 
SET "BookingNumber" = UPPER(SUBSTRING(MD5(RANDOM()::text || id::text), 1, 8))
WHERE "BookingNumber" IS NULL OR "BookingNumber" = '';

-- Step 3: Make the column NOT NULL
ALTER TABLE bookings 
ALTER COLUMN "BookingNumber" SET NOT NULL;

-- Step 4: Add index for faster lookups (optional but recommended)
CREATE INDEX IF NOT EXISTS idx_bookings_booking_number 
ON bookings("BookingNumber");

-- Verify the change
SELECT column_name, data_type, is_nullable, character_maximum_length
FROM information_schema.columns
WHERE table_name = 'bookings'
ORDER BY ordinal_position;

-- Test: Show first few bookings with their booking numbers
SELECT "Id", "RoomId", "BookingNumber", "Title", "StartTime", "EndTime"
FROM bookings
ORDER BY "Id" DESC
LIMIT 5;
