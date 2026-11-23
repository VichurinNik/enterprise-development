using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Museum.Infrastructure.MySql.Migrations;

/// <inheritdoc />
public partial class InitialSchema : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Excursions",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                Duration = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Excursions", x => x.Id);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Exhibitions",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Theme = table.Column<int>(type: "int", nullable: false),
                HallNumber = table.Column<int>(type: "int", nullable: false),
                StartDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                EndDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Exhibitions", x => x.Id);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Visitors",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                FullName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Phone = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                BirthDate = table.Column<DateTime>(type: "date", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Visitors", x => x.Id);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "ExcursionExhibitions",
            columns: table => new
            {
                ExcursionId = table.Column<int>(type: "int", nullable: false),
                ExhibitionId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ExcursionExhibitions", x => new { x.ExcursionId, x.ExhibitionId });
                table.ForeignKey(
                    name: "FK_ExcursionExhibitions_Excursions_ExcursionId",
                    column: x => x.ExcursionId,
                    principalTable: "Excursions",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_ExcursionExhibitions_Exhibitions_ExhibitionId",
                    column: x => x.ExhibitionId,
                    principalTable: "Exhibitions",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Tickets",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                ExcursionId = table.Column<int>(type: "int", nullable: false),
                VisitorId = table.Column<int>(type: "int", nullable: false),
                TicketType = table.Column<int>(type: "int", nullable: false),
                Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Tickets", x => x.Id);
                table.ForeignKey(
                    name: "FK_Tickets_Excursions_ExcursionId",
                    column: x => x.ExcursionId,
                    principalTable: "Excursions",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Tickets_Visitors_VisitorId",
                    column: x => x.VisitorId,
                    principalTable: "Visitors",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateIndex(
            name: "IX_ExcursionExhibitions_ExhibitionId",
            table: "ExcursionExhibitions",
            column: "ExhibitionId");

        migrationBuilder.CreateIndex(
            name: "IX_Excursions_Date",
            table: "Excursions",
            column: "Date");

        migrationBuilder.CreateIndex(
            name: "IX_Exhibitions_Theme",
            table: "Exhibitions",
            column: "Theme");

        migrationBuilder.CreateIndex(
            name: "IX_Tickets_ExcursionId",
            table: "Tickets",
            column: "ExcursionId");

        migrationBuilder.CreateIndex(
            name: "IX_Tickets_VisitorId",
            table: "Tickets",
            column: "VisitorId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ExcursionExhibitions");

        migrationBuilder.DropTable(
            name: "Tickets");

        migrationBuilder.DropTable(
            name: "Exhibitions");

        migrationBuilder.DropTable(
            name: "Excursions");

        migrationBuilder.DropTable(
            name: "Visitors");
    }
}
