using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TiContent.UI.WinUI.Migrations
{
    /// <inheritdoc />
    public partial class HydraFiltersV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(name: "PK_FiltersItems", table: "FiltersItems");

            migrationBuilder.DropColumn(name: "Id", table: "FiltersItems");

            migrationBuilder.RenameTable(name: "FiltersItems", newName: "HydraFiltersItems");

            migrationBuilder.AddPrimaryKey(name: "PK_HydraFiltersItems", table: "HydraFiltersItems", column: "Title");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(name: "PK_HydraFiltersItems", table: "HydraFiltersItems");

            migrationBuilder.RenameTable(name: "HydraFiltersItems", newName: "FiltersItems");

            migrationBuilder.AddColumn<string>(name: "Id", table: "FiltersItems", type: "TEXT", nullable: false, defaultValue: "");

            migrationBuilder.AddPrimaryKey(name: "PK_FiltersItems", table: "FiltersItems", column: "Id");
        }
    }
}
