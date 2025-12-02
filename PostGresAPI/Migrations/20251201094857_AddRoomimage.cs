using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostGresAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomimage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "image",
                table: "rooms",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "rooms",
                keyColumn: "Id",
                keyValue: 1,
                column: "image",
                value: null);

            migrationBuilder.UpdateData(
                table: "rooms",
                keyColumn: "Id",
                keyValue: 2,
                column: "image",
                value: null);

            migrationBuilder.UpdateData(
                table: "rooms",
                keyColumn: "Id",
                keyValue: 3,
                column: "image",
                value: null);

            migrationBuilder.UpdateData(
                table: "rooms",
                keyColumn: "Id",
                keyValue: 4,
                column: "image",
                value: null);

            migrationBuilder.UpdateData(
                table: "rooms",
                keyColumn: "Id",
                keyValue: 5,
                column: "image",
                value: null);

            migrationBuilder.UpdateData(
                table: "rooms",
                keyColumn: "Id",
                keyValue: 6,
                column: "image",
                value: null);

            migrationBuilder.UpdateData(
                table: "rooms",
                keyColumn: "Id",
                keyValue: 7,
                column: "image",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "image",
                table: "rooms");
        }
    }
}
