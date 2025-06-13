using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TiContent.UI.WinUI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FiltersItems",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    FilterType = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FiltersItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HydraLinksItems",
                columns: table => new
                {
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Owner = table.Column<string>(type: "TEXT", nullable: false),
                    CleanTitle = table.Column<string>(type: "TEXT", nullable: false),
                    FileSize = table.Column<double>(type: "REAL", nullable: false),
                    UploadDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Link = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HydraLinksItems", x => x.Title);
                });

            migrationBuilder.CreateTable(
                name: "ImageItems",
                columns: table => new
                {
                    Url = table.Column<string>(type: "TEXT", nullable: false),
                    Data = table.Column<byte[]>(type: "BLOB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageItems", x => x.Url);
                });

            migrationBuilder.CreateTable(
                name: "QueryHistoryItems",
                columns: table => new
                {
                    Query = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueryHistoryItems", x => x.Query);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FiltersItems");

            migrationBuilder.DropTable(
                name: "HydraLinksItems");

            migrationBuilder.DropTable(
                name: "ImageItems");

            migrationBuilder.DropTable(
                name: "QueryHistoryItems");
        }
    }
}
