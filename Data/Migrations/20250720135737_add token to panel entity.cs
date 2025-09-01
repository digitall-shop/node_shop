using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class addtokentopanelentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PanelUserName",
                schema: "Commerce",
                table: "Panels",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "PanelPassword",
                schema: "Commerce",
                table: "Panels",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "PanelName",
                schema: "Commerce",
                table: "Panels",
                newName: "Name");

            migrationBuilder.AddColumn<string>(
                name: "Token",
                schema: "Commerce",
                table: "Panels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Token",
                schema: "Commerce",
                table: "Panels");

            migrationBuilder.RenameColumn(
                name: "UserName",
                schema: "Commerce",
                table: "Panels",
                newName: "PanelUserName");

            migrationBuilder.RenameColumn(
                name: "Password",
                schema: "Commerce",
                table: "Panels",
                newName: "PanelPassword");

            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "Commerce",
                table: "Panels",
                newName: "PanelName");
        }
    }
}
