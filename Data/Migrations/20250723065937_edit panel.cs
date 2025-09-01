using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class editpanel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PanelIpAddress",
                schema: "Commerce",
                table: "Panels");

            migrationBuilder.AddColumn<bool>(
                name: "SSL",
                schema: "Commerce",
                table: "Panels",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SSL",
                schema: "Commerce",
                table: "Panels");

            migrationBuilder.AddColumn<string>(
                name: "PanelIpAddress",
                schema: "Commerce",
                table: "Panels",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
