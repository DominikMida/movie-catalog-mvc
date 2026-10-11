using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcPracownicy.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Gatunek",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nazwa = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gatunek", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Studio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nazwa = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Studio", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Film",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Tytul = table.Column<string>(type: "TEXT", nullable: false),
                    Rok = table.Column<int>(type: "INTEGER", nullable: false),
                    StudioId = table.Column<int>(type: "INTEGER", nullable: true),
                    GatunekId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Film", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Film_Gatunek_GatunekId",
                        column: x => x.GatunekId,
                        principalTable: "Gatunek",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Film_Studio_StudioId",
                        column: x => x.StudioId,
                        principalTable: "Studio",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Recenzja",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Autor = table.Column<string>(type: "TEXT", nullable: false),
                    Ocena = table.Column<int>(type: "INTEGER", nullable: false),
                    Komentarz = table.Column<string>(type: "TEXT", nullable: false),
                    FilmId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recenzja", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recenzja_Film_FilmId",
                        column: x => x.FilmId,
                        principalTable: "Film",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Film_GatunekId",
                table: "Film",
                column: "GatunekId");

            migrationBuilder.CreateIndex(
                name: "IX_Film_StudioId",
                table: "Film",
                column: "StudioId");

            migrationBuilder.CreateIndex(
                name: "IX_Recenzja_FilmId",
                table: "Recenzja",
                column: "FilmId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Recenzja");

            migrationBuilder.DropTable(
                name: "Film");

            migrationBuilder.DropTable(
                name: "Gatunek");

            migrationBuilder.DropTable(
                name: "Studio");
        }
    }
}
