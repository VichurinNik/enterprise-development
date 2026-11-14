using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Museum.Infrastructure.MySql.Migrations
{
    /// <inheritdoc />
    public partial class SeedMuseum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Excursions",
                columns: new[] { "Id", "Date", "Duration" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 15, 11, 0, 0, 0, DateTimeKind.Unspecified), 54000000000L },
                    { 2, new DateTime(2026, 1, 16, 12, 0, 0, 0, DateTimeKind.Unspecified), 54000000000L },
                    { 3, new DateTime(2026, 1, 17, 13, 0, 0, 0, DateTimeKind.Unspecified), 54000000000L },
                    { 4, new DateTime(2026, 1, 18, 14, 0, 0, 0, DateTimeKind.Unspecified), 54000000000L },
                    { 5, new DateTime(2026, 1, 19, 10, 0, 0, 0, DateTimeKind.Unspecified), 54000000000L },
                    { 6, new DateTime(2026, 1, 20, 11, 0, 0, 0, DateTimeKind.Unspecified), 54000000000L },
                    { 7, new DateTime(2026, 1, 21, 12, 0, 0, 0, DateTimeKind.Unspecified), 54000000000L },
                    { 8, new DateTime(2026, 1, 22, 13, 0, 0, 0, DateTimeKind.Unspecified), 54000000000L },
                    { 9, new DateTime(2026, 1, 23, 14, 0, 0, 0, DateTimeKind.Unspecified), 54000000000L },
                    { 10, new DateTime(2026, 1, 24, 10, 0, 0, 0, DateTimeKind.Unspecified), 54000000000L }
                });

            migrationBuilder.InsertData(
                table: "Exhibitions",
                columns: new[] { "Id", "EndDate", "HallNumber", "Name", "StartDate", "Theme" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Статическая выставка 1", new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Статическая выставка 2", new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 3, new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Статическая выставка 3", new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 4, new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Статическая выставка 4", new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 5, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Статическая выставка 5", new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 6, new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Статическая выставка 6", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 7, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Статическая выставка 7", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 8, new DateTime(2026, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Статическая выставка 8", new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 9, new DateTime(2026, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Статическая выставка 9", new DateTime(2026, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 10, new DateTime(2026, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Статическая выставка 10", new DateTime(2026, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 }
                });

            migrationBuilder.InsertData(
                table: "Visitors",
                columns: new[] { "Id", "BirthDate", "FullName", "Phone" },
                values: new object[,]
                {
                    { 1, new DateTime(1991, 2, 15), "Посетитель Тестовый 1", "+79000000001" },
                    { 2, new DateTime(1992, 3, 15), "Посетитель Тестовый 2", "+79000000002" },
                    { 3, new DateTime(1993, 4, 15), "Посетитель Тестовый 3", "+79000000003" },
                    { 4, new DateTime(1994, 5, 15), "Посетитель Тестовый 4", "+79000000004" },
                    { 5, new DateTime(1995, 6, 15), "Посетитель Тестовый 5", "+79000000005" },
                    { 6, new DateTime(1996, 7, 15), "Посетитель Тестовый 6", "+79000000006" },
                    { 7, new DateTime(1997, 8, 15), "Посетитель Тестовый 7", "+79000000007" },
                    { 8, new DateTime(1998, 9, 15), "Посетитель Тестовый 8", "+79000000008" },
                    { 9, new DateTime(1999, 10, 15), "Посетитель Тестовый 9", "+79000000009" },
                    { 10, new DateTime(2000, 11, 15), "Посетитель Тестовый 10", "+79000000010" }
                });

            migrationBuilder.InsertData(
                table: "ExcursionExhibitions",
                columns: new[] { "ExcursionId", "ExhibitionId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 3 },
                    { 2, 2 },
                    { 2, 4 },
                    { 3, 3 },
                    { 3, 5 },
                    { 4, 4 },
                    { 4, 6 },
                    { 5, 5 },
                    { 5, 7 },
                    { 6, 6 },
                    { 6, 8 },
                    { 7, 7 },
                    { 7, 9 },
                    { 8, 8 },
                    { 8, 10 },
                    { 9, 1 },
                    { 9, 9 },
                    { 10, 2 },
                    { 10, 10 }
                });

            migrationBuilder.InsertData(
                table: "Tickets",
                columns: new[] { "Id", "ExcursionId", "Price", "TicketType", "VisitorId" },
                values: new object[,]
                {
                    { 1, 1, 1000m, 0, 2 },
                    { 2, 1, 500m, 1, 3 },
                    { 3, 1, 1000m, 0, 4 },
                    { 4, 2, 1000m, 0, 3 },
                    { 5, 2, 500m, 1, 4 },
                    { 6, 2, 1000m, 0, 5 },
                    { 7, 3, 1000m, 0, 4 },
                    { 8, 3, 500m, 1, 5 },
                    { 9, 3, 1000m, 0, 6 },
                    { 10, 4, 1000m, 0, 5 },
                    { 11, 4, 500m, 1, 6 },
                    { 12, 4, 1000m, 0, 7 },
                    { 13, 5, 1000m, 0, 6 },
                    { 14, 5, 500m, 1, 7 },
                    { 15, 5, 1000m, 0, 8 },
                    { 16, 6, 1000m, 0, 7 },
                    { 17, 6, 500m, 1, 8 },
                    { 18, 6, 1000m, 0, 9 },
                    { 19, 7, 1000m, 0, 8 },
                    { 20, 7, 500m, 1, 9 },
                    { 21, 7, 1000m, 0, 10 },
                    { 22, 8, 1000m, 0, 9 },
                    { 23, 8, 500m, 1, 10 },
                    { 24, 8, 1000m, 0, 1 },
                    { 25, 9, 1000m, 0, 10 },
                    { 26, 9, 500m, 1, 1 },
                    { 27, 9, 1000m, 0, 2 },
                    { 28, 10, 1000m, 0, 1 },
                    { 29, 10, 500m, 1, 2 },
                    { 30, 10, 1000m, 0, 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ExcursionExhibitions",
                keyColumns: new[] { "ExcursionId", "ExhibitionId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "ExcursionExhibitions",
                keyColumns: new[] { "ExcursionId", "ExhibitionId" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.DeleteData(
                table: "ExcursionExhibitions",
                keyColumns: new[] { "ExcursionId", "ExhibitionId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "ExcursionExhibitions",
                keyColumns: new[] { "ExcursionId", "ExhibitionId" },
                keyValues: new object[] { 2, 4 });

            migrationBuilder.DeleteData(
                table: "ExcursionExhibitions",
                keyColumns: new[] { "ExcursionId", "ExhibitionId" },
                keyValues: new object[] { 3, 3 });

            migrationBuilder.DeleteData(
                table: "ExcursionExhibitions",
                keyColumns: new[] { "ExcursionId", "ExhibitionId" },
                keyValues: new object[] { 3, 5 });

            migrationBuilder.DeleteData(
                table: "ExcursionExhibitions",
                keyColumns: new[] { "ExcursionId", "ExhibitionId" },
                keyValues: new object[] { 4, 4 });

            migrationBuilder.DeleteData(
                table: "ExcursionExhibitions",
                keyColumns: new[] { "ExcursionId", "ExhibitionId" },
                keyValues: new object[] { 4, 6 });

            migrationBuilder.DeleteData(
                table: "ExcursionExhibitions",
                keyColumns: new[] { "ExcursionId", "ExhibitionId" },
                keyValues: new object[] { 5, 5 });

            migrationBuilder.DeleteData(
                table: "ExcursionExhibitions",
                keyColumns: new[] { "ExcursionId", "ExhibitionId" },
                keyValues: new object[] { 5, 7 });

            migrationBuilder.DeleteData(
                table: "ExcursionExhibitions",
                keyColumns: new[] { "ExcursionId", "ExhibitionId" },
                keyValues: new object[] { 6, 6 });

            migrationBuilder.DeleteData(
                table: "ExcursionExhibitions",
                keyColumns: new[] { "ExcursionId", "ExhibitionId" },
                keyValues: new object[] { 6, 8 });

            migrationBuilder.DeleteData(
                table: "ExcursionExhibitions",
                keyColumns: new[] { "ExcursionId", "ExhibitionId" },
                keyValues: new object[] { 7, 7 });

            migrationBuilder.DeleteData(
                table: "ExcursionExhibitions",
                keyColumns: new[] { "ExcursionId", "ExhibitionId" },
                keyValues: new object[] { 7, 9 });

            migrationBuilder.DeleteData(
                table: "ExcursionExhibitions",
                keyColumns: new[] { "ExcursionId", "ExhibitionId" },
                keyValues: new object[] { 8, 8 });

            migrationBuilder.DeleteData(
                table: "ExcursionExhibitions",
                keyColumns: new[] { "ExcursionId", "ExhibitionId" },
                keyValues: new object[] { 8, 10 });

            migrationBuilder.DeleteData(
                table: "ExcursionExhibitions",
                keyColumns: new[] { "ExcursionId", "ExhibitionId" },
                keyValues: new object[] { 9, 1 });

            migrationBuilder.DeleteData(
                table: "ExcursionExhibitions",
                keyColumns: new[] { "ExcursionId", "ExhibitionId" },
                keyValues: new object[] { 9, 9 });

            migrationBuilder.DeleteData(
                table: "ExcursionExhibitions",
                keyColumns: new[] { "ExcursionId", "ExhibitionId" },
                keyValues: new object[] { 10, 2 });

            migrationBuilder.DeleteData(
                table: "ExcursionExhibitions",
                keyColumns: new[] { "ExcursionId", "ExhibitionId" },
                keyValues: new object[] { 10, 10 });

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Excursions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Excursions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Excursions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Excursions",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Excursions",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Excursions",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Excursions",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Excursions",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Excursions",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Excursions",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Exhibitions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Exhibitions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Exhibitions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Exhibitions",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Exhibitions",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Exhibitions",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Exhibitions",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Exhibitions",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Exhibitions",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Exhibitions",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Visitors",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Visitors",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Visitors",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Visitors",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Visitors",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Visitors",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Visitors",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Visitors",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Visitors",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Visitors",
                keyColumn: "Id",
                keyValue: 10);
        }
    }
}
