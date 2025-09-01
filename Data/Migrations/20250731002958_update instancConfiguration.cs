using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class updateinstancConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "SnifferApiPort",
                schema: "Commerce",
                table: "Instance",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApiPort",
                schema: "Commerce",
                table: "Instance",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InboundPort",
                schema: "Commerce",
                table: "Instance",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "XrayPort",
                schema: "Commerce",
                table: "Instance",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiPort",
                schema: "Commerce",
                table: "Instance");

            migrationBuilder.DropColumn(
                name: "InboundPort",
                schema: "Commerce",
                table: "Instance");

            migrationBuilder.DropColumn(
                name: "XrayPort",
                schema: "Commerce",
                table: "Instance");

            migrationBuilder.AlterColumn<int>(
                name: "SnifferApiPort",
                schema: "Commerce",
                table: "Instance",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
