using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicVilla.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFieldsToVillaDTO : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Occupancy",
                table: "Villas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SquareMeters",
                table: "Villas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Occupancy", "SquareMeters" },
                values: new object[] { 5, 250 });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Occupancy", "SquareMeters" },
                values: new object[] { 3, 120 });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Occupancy", "SquareMeters" },
                values: new object[] { 1, 60 });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Occupancy", "SquareMeters" },
                values: new object[] { 2, 100 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Occupancy",
                table: "Villas");

            migrationBuilder.DropColumn(
                name: "SquareMeters",
                table: "Villas");
        }
    }
}
