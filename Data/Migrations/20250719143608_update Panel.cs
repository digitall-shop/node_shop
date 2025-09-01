using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class updatePanel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PanelIp",
                schema: "Commerce",
                table: "Panels");

            migrationBuilder.AddColumn<string>(
                name: "PanelIpAddress",
                schema: "Commerce",
                table: "Node",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PanelIpAddress",
                schema: "Commerce",
                table: "Node");

            migrationBuilder.AddColumn<string>(
                name: "PanelIp",
                schema: "Commerce",
                table: "Panels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
