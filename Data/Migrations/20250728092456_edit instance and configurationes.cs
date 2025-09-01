using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class editinstanceandconfigurationes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Instance_NodeId_InboundPort",
                schema: "Commerce",
                table: "Instance");

            migrationBuilder.DropIndex(
                name: "IX_Instance_NodeId_ServerPort",
                schema: "Commerce",
                table: "Instance");

            migrationBuilder.DropIndex(
                name: "IX_Instance_NodeId_XrayPort",
                schema: "Commerce",
                table: "Instance");

            migrationBuilder.RenameColumn(
                name: "ServerPort",
                schema: "Commerce",
                table: "Instance",
                newName: "ApiPort");

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
                name: "IX_Instance_NodeId",
                schema: "Commerce",
                table: "Instance",
                column: "NodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Instance_XrayPort",
                schema: "Commerce",
                table: "Instance",
                column: "XrayPort",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "IX_Instance_NodeId",
                schema: "Commerce",
                table: "Instance");

            migrationBuilder.DropIndex(
                name: "IX_Instance_XrayPort",
                schema: "Commerce",
                table: "Instance");

            migrationBuilder.RenameColumn(
                name: "ApiPort",
                schema: "Commerce",
                table: "Instance",
                newName: "ServerPort");

            migrationBuilder.CreateIndex(
                name: "IX_Instance_NodeId_InboundPort",
                schema: "Commerce",
                table: "Instance",
                columns: new[] { "NodeId", "InboundPort" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Instance_NodeId_ServerPort",
                schema: "Commerce",
                table: "Instance",
                columns: new[] { "NodeId", "ServerPort" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Instance_NodeId_XrayPort",
                schema: "Commerce",
                table: "Instance",
                columns: new[] { "NodeId", "XrayPort" },
                unique: true);
        }
    }
}
