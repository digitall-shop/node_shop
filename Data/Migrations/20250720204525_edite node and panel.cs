using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class editenodeandpanel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PanelIpAddress",
                schema: "Commerce",
                table: "Node");

            migrationBuilder.AddColumn<string>(
                name: "PanelIpAddress",
                schema: "Commerce",
                table: "Panels",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PanelIpAddress",
                schema: "Commerce",
                table: "Panels");

            migrationBuilder.AddColumn<string>(
                name: "PanelIpAddress",
                schema: "Commerce",
                table: "Node",
                type: "nvarchar(100)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
