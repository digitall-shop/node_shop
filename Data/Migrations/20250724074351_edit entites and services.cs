using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class editentitesandservices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDelete",
                schema: "Account",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "PanelUrl",
                schema: "Commerce",
                table: "Panels",
                newName: "Url");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "Account",
                table: "Users",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Url",
                schema: "Commerce",
                table: "Panels",
                newName: "PanelUrl");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "Account",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                schema: "Account",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
