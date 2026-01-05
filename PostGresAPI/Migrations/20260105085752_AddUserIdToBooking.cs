using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostGresAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "bookings",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_booking_user",
                table: "bookings",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_bookings_Users_UserId",
                table: "bookings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bookings_Users_UserId",
                table: "bookings");

            migrationBuilder.DropIndex(
                name: "ix_booking_user",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "bookings");
        }
    }
}
