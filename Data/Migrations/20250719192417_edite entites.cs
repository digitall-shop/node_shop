using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class editeentites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SshCertificateKey",
                schema: "Commerce",
                table: "Node");

            migrationBuilder.AddColumn<string>(
                name: "SshCertificateKey",
                schema: "Commerce",
                table: "Panels",
                type: "nvarchar(4000)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SshCertificateKey",
                schema: "Commerce",
                table: "Panels");

            migrationBuilder.AddColumn<string>(
                name: "SshCertificateKey",
                schema: "Commerce",
                table: "Node",
                type: "nvarchar(4000)",
                nullable: false,
                defaultValue: "");
        }
    }
}
