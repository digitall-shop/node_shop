using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class EditePanelforaddports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApiPort",
                schema: "Commerce",
                table: "Panels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServerPort",
                schema: "Commerce",
                table: "Panels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "XrayPort",
                schema: "Commerce",
                table: "Panels",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiPort",
                schema: "Commerce",
                table: "Panels");

            migrationBuilder.DropColumn(
                name: "ServerPort",
                schema: "Commerce",
                table: "Panels");

            migrationBuilder.DropColumn(
                name: "XrayPort",
                schema: "Commerce",
                table: "Panels");
        }
    }
}
