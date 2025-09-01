using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class updatefiledsnameinentites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Instance_ApiPort",
                schema: "Commerce",
                table: "Instance");

            migrationBuilder.DropIndex(
                name: "IX_Instance_InboundPort",
                schema: "Commerce",
                table: "Instance");

            migrationBuilder.DropIndex(
                name: "IX_Instance_XrayPort",
                schema: "Commerce",
                table: "Instance");

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

            migrationBuilder.DropColumn(
                name: "XrayUserUuid",
                schema: "Commerce",
                table: "Instance");

            migrationBuilder.RenameColumn(
                name: "SshCertificateKey",
                schema: "Commerce",
                table: "Panels",
                newName: "CertificateKey");

            migrationBuilder.AlterColumn<int>(
                name: "XrayPort",
                schema: "Commerce",
                table: "Panels",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "InboundPort",
                schema: "Commerce",
                table: "Panels",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ApiPort",
                schema: "Commerce",
                table: "Panels",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SshKeyCertKey",
                schema: "Commerce",
                table: "Node",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SshKeyCertKey",
                schema: "Commerce",
                table: "Node");

            migrationBuilder.RenameColumn(
                name: "CertificateKey",
                schema: "Commerce",
                table: "Panels",
                newName: "SshCertificateKey");

            migrationBuilder.AlterColumn<int>(
                name: "XrayPort",
                schema: "Commerce",
                table: "Panels",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "InboundPort",
                schema: "Commerce",
                table: "Panels",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ApiPort",
                schema: "Commerce",
                table: "Panels",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

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

            migrationBuilder.AddColumn<string>(
                name: "XrayUserUuid",
                schema: "Commerce",
                table: "Instance",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Instance_ApiPort",
                schema: "Commerce",
                table: "Instance",
                column: "ApiPort",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Instance_InboundPort",
                schema: "Commerce",
                table: "Instance",
                column: "InboundPort",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Instance_XrayPort",
                schema: "Commerce",
                table: "Instance",
                column: "XrayPort",
                unique: true);
        }
    }
}
