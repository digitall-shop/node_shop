using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class cleaning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailablePortsRangeJson",
                schema: "Commerce",
                table: "Node");

            migrationBuilder.DropColumn(
                name: "MarzbanAssignedUsername",
                schema: "Commerce",
                table: "Instance");

            migrationBuilder.DropColumn(
                name: "MarzbanNodeIdInCustomerPanel",
                schema: "Commerce",
                table: "Instance");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvailablePortsRangeJson",
                schema: "Commerce",
                table: "Node",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MarzbanAssignedUsername",
                schema: "Commerce",
                table: "Instance",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MarzbanNodeIdInCustomerPanel",
                schema: "Commerce",
                table: "Instance",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
