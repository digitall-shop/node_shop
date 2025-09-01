using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class deleteaportfrominstanc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SnifferApiPort",
                schema: "Commerce",
                table: "Instance");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SnifferApiPort",
                schema: "Commerce",
                table: "Instance",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
