using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostGresAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddPricePerNightToBedrooms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "price_per_night",
                table: "rooms",
                type: "numeric(18,2)",
                nullable: true,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "rooms",
                keyColumn: "Id",
                keyValue: 4,
                column: "price_per_night",
                value: 50m);

            migrationBuilder.UpdateData(
                table: "rooms",
                keyColumn: "Id",
                keyValue: 5,
                column: "price_per_night",
                value: 80m);

            migrationBuilder.UpdateData(
                table: "rooms",
                keyColumn: "Id",
                keyValue: 6,
                column: "price_per_night",
                value: 80m);

            migrationBuilder.UpdateData(
                table: "rooms",
                keyColumn: "Id",
                keyValue: 7,
                column: "price_per_night",
                value: 120m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "price_per_night",
                table: "rooms");
        }
    }
}
