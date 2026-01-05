using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostGresAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddNumberOfPersonsToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfPersons",
                table: "bookings",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfPersons",
                table: "bookings");
        }
    }
}
