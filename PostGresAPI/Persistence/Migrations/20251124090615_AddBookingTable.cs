using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PostGresAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Idempotentes Seeding für Meetingrooms
            migrationBuilder.Sql(@"
                INSERT INTO rooms (""Id"", ""Name"", number_of_chairs, room_type)
                VALUES (1, 'Conference Room A', 20, 'Meetingroom')
                ON CONFLICT (""Id"") DO NOTHING;

                INSERT INTO rooms (""Id"", ""Name"", number_of_chairs, room_type)
                VALUES (2, 'Conference Room B', 15, 'Meetingroom')
                ON CONFLICT (""Id"") DO NOTHING;

                INSERT INTO rooms (""Id"", ""Name"", number_of_chairs, room_type)
                VALUES (3, 'Board Room', 10, 'Meetingroom')
                ON CONFLICT (""Id"") DO NOTHING;
            ");

            // Idempotentes Seeding für Bedrooms
            migrationBuilder.Sql(@"
                INSERT INTO rooms (""Id"", ""Name"", number_of_beds, room_type)
                VALUES (4, 'Room 101', 1, 'Bedroom')
                ON CONFLICT (""Id"") DO NOTHING;

                INSERT INTO rooms (""Id"", ""Name"", number_of_beds, room_type)
                VALUES (5, 'Room 102', 2, 'Bedroom')
                ON CONFLICT (""Id"") DO NOTHING;

                INSERT INTO rooms (""Id"", ""Name"", number_of_beds, room_type)
                VALUES (6, 'Room 103', 2, 'Bedroom')
                ON CONFLICT (""Id"") DO NOTHING;

                INSERT INTO rooms (""Id"", ""Name"", number_of_beds, room_type)
                VALUES (7, 'Suite 201', 3, 'Bedroom')
                ON CONFLICT (""Id"") DO NOTHING;
            ");

            // Sequenz nachziehen, falls Ids manuell gesetzt wurden
            migrationBuilder.Sql(@"
                DO $$
                DECLARE
                    seq_name text;
                BEGIN
                    -- für Identity/Serial die Sequenz ermitteln und auf MAX(Id) setzen
                    SELECT pg_get_serial_sequence('rooms','Id') INTO seq_name;
                    IF seq_name IS NOT NULL THEN
                        EXECUTE format('SELECT setval(''%s'', (SELECT COALESCE(MAX(""Id""), 1) FROM rooms))', seq_name);
                    END IF;
                END$$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Beim Rollback die Seed-Datensätze entfernen (falls vorhanden)
            migrationBuilder.Sql(@"DELETE FROM rooms WHERE ""Id"" IN (1,2,3,4,5,6,7);");
        }
    }
}
