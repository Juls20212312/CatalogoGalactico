using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CatalogoGalactico.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Personajes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Especie = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Faccion = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Afiliacion = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Estado = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FuerzaSensitivo = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personajes", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PasswordHash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Cartas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PersonajeId = table.Column<int>(type: "int", nullable: false),
                    Poder = table.Column<int>(type: "int", nullable: false),
                    HabilidadEspecial = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Arma = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NivelPeligrosidad = table.Column<int>(type: "int", nullable: false),
                    ImagenUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cartas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cartas_Personajes_PersonajeId",
                        column: x => x.PersonajeId,
                        principalTable: "Personajes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Eventos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Fecha = table.Column<int>(type: "int", nullable: false),
                    Ubicacion = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descripcion = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GanadorId = table.Column<int>(type: "int", nullable: true),
                    Resultado = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Eventos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Eventos_Personajes_GanadorId",
                        column: x => x.GanadorId,
                        principalTable: "Personajes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ParticipacionesEventos",
                columns: table => new
                {
                    EventoId = table.Column<int>(type: "int", nullable: false),
                    PersonajeId = table.Column<int>(type: "int", nullable: false),
                    MurioEnEvento = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipacionesEventos", x => new { x.EventoId, x.PersonajeId });
                    table.ForeignKey(
                        name: "FK_ParticipacionesEventos_Eventos_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParticipacionesEventos_Personajes_PersonajeId",
                        column: x => x.PersonajeId,
                        principalTable: "Personajes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Personajes",
                columns: new[] { "Id", "Afiliacion", "Especie", "Estado", "Faccion", "FuerzaSensitivo", "Nombre" },
                values: new object[,]
                {
                    { 1, "Alianza Rebelde", "Humano", "Vivo", "Rebelde", true, "Luke Skywalker" },
                    { 2, "Alianza Rebelde", "Humana", "Vivo", "Rebelde", true, "Leia Organa" },
                    { 3, "Alianza Rebelde", "Humano", "Vivo", "Rebelde", false, "Han Solo" },
                    { 4, "Imperio Galáctico", "Humano", "Vivo", "Imperio", true, "Darth Vader" },
                    { 5, "Imperio Galáctico", "Humano", "Vivo", "Imperio", true, "Emperador Palpatine" },
                    { 6, "Cazarrecompensas", "Humano", "Vivo", "Neutral", false, "Boba Fett" }
                });

            migrationBuilder.InsertData(
                table: "Cartas",
                columns: new[] { "Id", "Arma", "HabilidadEspecial", "ImagenUrl", "NivelPeligrosidad", "PersonajeId", "Poder" },
                values: new object[,]
                {
                    { 1, "Sable de luz", "Dominio de la Fuerza", "https://example.com/luke.jpg", 8, 1, 100 },
                    { 2, "Bláster", "Liderazgo Rebelde", "https://example.com/leia.jpg", 7, 2, 85 },
                    { 3, "Bláster pesado", "Piloto experto", "https://example.com/han.jpg", 7, 3, 82 },
                    { 4, "Sable de luz rojo", "Poder del lado oscuro", "https://example.com/vader.jpg", 10, 4, 100 },
                    { 5, "Bastón ceremonial", "Rayos de la Fuerza", "https://example.com/palpatine.jpg", 10, 5, 110 },
                    { 6, "Bláster", "Caza de objetivos", "https://example.com/boba.jpg", 8, 6, 75 }
                });

            migrationBuilder.InsertData(
                table: "Eventos",
                columns: new[] { "Id", "Descripcion", "Fecha", "GanadorId", "Nombre", "Resultado", "Ubicacion" },
                values: new object[,]
                {
                    { 1, "Enfrentamiento decisivo de la Alianza Rebelde contra el Imperio.", 0, 1, "Batalla de Yavin", "Victoria de la Alianza Rebelde", "Estrella de la Muerte" },
                    { 2, "Batalla terrestre y espacial contra el Imperio Galáctico.", 4, 1, "Batalla de Endor", "Victoria de la Alianza Rebelde", "Endor" }
                });

            migrationBuilder.InsertData(
                table: "ParticipacionesEventos",
                columns: new[] { "EventoId", "PersonajeId", "MurioEnEvento" },
                values: new object[,]
                {
                    { 1, 1, false },
                    { 1, 2, false },
                    { 1, 3, false },
                    { 1, 4, false },
                    { 2, 1, false },
                    { 2, 2, false },
                    { 2, 3, false },
                    { 2, 4, false },
                    { 2, 5, false }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cartas_PersonajeId",
                table: "Cartas",
                column: "PersonajeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_GanadorId",
                table: "Eventos",
                column: "GanadorId");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipacionesEventos_PersonajeId",
                table: "ParticipacionesEventos",
                column: "PersonajeId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cartas");

            migrationBuilder.DropTable(
                name: "ParticipacionesEventos");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Eventos");

            migrationBuilder.DropTable(
                name: "Personajes");
        }
    }
}
