using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class addSshCertificateKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SshKeyFilePath",
                schema: "Commerce",
                table: "Node");

            migrationBuilder.AddColumn<string>(
                name: "SshCertificateKey",
                schema: "Commerce",
                table: "Node",
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
                table: "Node");

            migrationBuilder.AddColumn<string>(
                name: "SshKeyFilePath",
                schema: "Commerce",
                table: "Node",
                type: "nvarchar(500)",
                nullable: false,
                defaultValue: "");
        }
    }
}
