-- Add BookingNumber column to bookings table if it doesn't exist
DO $$
BEGIN
    IF NOT EXISTS (
      SELECT 1 
        FROM information_schema.columns 
    WHERE table_name = 'bookings' 
    AND column_name = 'BookingNumber'
    ) THEN
        ALTER TABLE bookings 
        ADD COLUMN "BookingNumber" character varying(50) NOT NULL DEFAULT '';
        
        -- Generate booking numbers for existing records
        UPDATE bookings 
   SET "BookingNumber" = UPPER(SUBSTRING(MD5(RANDOM()::TEXT || CLOCK_TIMESTAMP()::TEXT) FROM 1 FOR 8))
  WHERE "BookingNumber" = '' OR "BookingNumber" IS NULL;
    END IF;
END $$;

-- Add migration history record
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251201072639_AddBookingNumberToBooking', '9.0.9')
ON CONFLICT DO NOTHING;
