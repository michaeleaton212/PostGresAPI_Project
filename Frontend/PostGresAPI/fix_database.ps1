# PowerShell script to fix the database automatically

Write-Host "=== BOOKING DATABASE FIX SCRIPT ===" -ForegroundColor Cyan
Write-Host ""

# Database connection parameters - MODIFY THESE!
$DB_HOST = "localhost"
$DB_PORT = "5432"
$DB_NAME = "your_database_name"  # <-- CHANGE THIS!
$DB_USER = "postgres"        # <-- CHANGE THIS!
$DB_PASSWORD = "your_password"    # <-- CHANGE THIS!

Write-Host "Database Configuration:" -ForegroundColor Yellow
Write-Host "  Host: $DB_HOST" -ForegroundColor White
Write-Host "  Port: $DB_PORT" -ForegroundColor White
Write-Host "  Database: $DB_NAME" -ForegroundColor White
Write-Host "  User: $DB_USER" -ForegroundColor White
Write-Host ""

$confirmation = Read-Host "Is this configuration correct? (y/n)"
if ($confirmation -ne 'y') {
    Write-Host "Please edit this script and update the database parameters." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Step 1: Checking PostgreSQL connection..." -ForegroundColor Yellow

# Set environment variable for password (psql will use it)
$env:PGPASSWORD = $DB_PASSWORD

# Test connection
$testQuery = "SELECT version();"
try {
    $version = psql -h $DB_HOST -p $DB_PORT -U $DB_USER -d $DB_NAME -t -c $testQuery 2>&1
  if ($LASTEXITCODE -eq 0) {
        Write-Host "? Connected to PostgreSQL successfully!" -ForegroundColor Green
    } else {
Write-Host "? Failed to connect to PostgreSQL!" -ForegroundColor Red
        Write-Host "Error: $version" -ForegroundColor Red
      exit 1
    }
} catch {
    Write-Host "? Error connecting to database: $_" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Step 2: Checking if BookingNumber column exists..." -ForegroundColor Yellow

$checkColumn = @"
SELECT EXISTS (
    SELECT 1 
    FROM information_schema.columns 
    WHERE table_name = 'bookings' 
    AND column_name = 'BookingNumber'
);
"@

$columnExists = psql -h $DB_HOST -p $DB_PORT -U $DB_USER -d $DB_NAME -t -c $checkColumn

if ($columnExists -match 't') {
    Write-Host "? BookingNumber column already exists!" -ForegroundColor Green
} else {
    Write-Host "? BookingNumber column does not exist. Adding it now..." -ForegroundColor Yellow
    
    Write-Host ""
    Write-Host "Step 3: Adding BookingNumber column..." -ForegroundColor Yellow
    
    $addColumnSQL = @"
ALTER TABLE bookings 
ADD COLUMN IF NOT EXISTS "BookingNumber" VARCHAR(50);
"@
    
    psql -h $DB_HOST -p $DB_PORT -U $DB_USER -d $DB_NAME -c $addColumnSQL
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "? Column added successfully!" -ForegroundColor Green
    } else {
        Write-Host "? Failed to add column!" -ForegroundColor Red
        exit 1
  }
    
    Write-Host ""
 Write-Host "Step 4: Generating booking numbers for existing records..." -ForegroundColor Yellow
    
    $updateSQL = @"
UPDATE bookings 
SET "BookingNumber" = UPPER(SUBSTRING(MD5(RANDOM()::text || "Id"::text), 1, 8))
WHERE "BookingNumber" IS NULL OR "BookingNumber" = '';
"@
    
    psql -h $DB_HOST -p $DB_PORT -U $DB_USER -d $DB_NAME -c $updateSQL
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "? Booking numbers generated!" -ForegroundColor Green
    } else {
        Write-Host "? Failed to generate booking numbers!" -ForegroundColor Red
        exit 1
    }
    
    Write-Host ""
    Write-Host "Step 5: Setting column to NOT NULL..." -ForegroundColor Yellow
    
    $notNullSQL = @"
ALTER TABLE bookings 
ALTER COLUMN "BookingNumber" SET NOT NULL;
"@
    
    psql -h $DB_HOST -p $DB_PORT -U $DB_USER -d $DB_NAME -c $notNullSQL
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "? Column set to NOT NULL!" -ForegroundColor Green
    } else {
        Write-Host "? Warning: Could not set NOT NULL constraint!" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "Step 6: Verifying database structure..." -ForegroundColor Yellow

$verifySQL = @"
SELECT column_name, data_type, is_nullable
FROM information_schema.columns
WHERE table_name = 'bookings'
ORDER BY ordinal_position;
"@

Write-Host ""
Write-Host "Bookings table structure:" -ForegroundColor Cyan
psql -h $DB_HOST -p $DB_PORT -U $DB_USER -d $DB_NAME -c $verifySQL

Write-Host ""
Write-Host "=== DATABASE FIX COMPLETED ===" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Restart your backend: cd PostGresAPI && dotnet run" -ForegroundColor White
Write-Host "2. Test booking creation: .\test-booking-api.ps1" -ForegroundColor White
Write-Host "3. Try creating a booking in your frontend!" -ForegroundColor White
Write-Host ""

# Clear password from environment
$env:PGPASSWORD = ""
