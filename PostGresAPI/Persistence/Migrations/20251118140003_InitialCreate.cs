using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostGresAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bookings_rooms_RoomId1",
                table: "bookings");

            migrationBuilder.DropIndex(
                name: "IX_bookings_RoomId1",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "RoomId1",
                table: "bookings");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "Users",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "character varying(320)",
                maxLength: 320,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "Users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(320)",
                oldMaxLength: 320);

            migrationBuilder.AddColumn<int>(
                name: "RoomId1",
                table: "bookings",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_bookings_RoomId1",
                table: "bookings",
                column: "RoomId1");

            migrationBuilder.AddForeignKey(
                name: "FK_bookings_rooms_RoomId1",
                table: "bookings",
                column: "RoomId1",
                principalTable: "rooms",
                principalColumn: "Id");
        }
    }
}
