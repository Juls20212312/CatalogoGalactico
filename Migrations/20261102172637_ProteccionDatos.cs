using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogoGalactico.Migrations
{
    /// <inheritdoc />
    public partial class ProteccionDatos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UbicacionSecretaCifrada",
                table: "Personajes",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Personajes",
                keyColumn: "Id",
                keyValue: 1,
                column: "UbicacionSecretaCifrada",
                value: "");

            migrationBuilder.UpdateData(
                table: "Personajes",
                keyColumn: "Id",
                keyValue: 2,
                column: "UbicacionSecretaCifrada",
                value: "");

            migrationBuilder.UpdateData(
                table: "Personajes",
                keyColumn: "Id",
                keyValue: 3,
                column: "UbicacionSecretaCifrada",
                value: "");

            migrationBuilder.UpdateData(
                table: "Personajes",
                keyColumn: "Id",
                keyValue: 4,
                column: "UbicacionSecretaCifrada",
                value: "");

            migrationBuilder.UpdateData(
                table: "Personajes",
                keyColumn: "Id",
                keyValue: 5,
                column: "UbicacionSecretaCifrada",
                value: "");

            migrationBuilder.UpdateData(
                table: "Personajes",
                keyColumn: "Id",
                keyValue: 6,
                column: "UbicacionSecretaCifrada",
                value: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UbicacionSecretaCifrada",
                table: "Personajes");
        }
    }
}
