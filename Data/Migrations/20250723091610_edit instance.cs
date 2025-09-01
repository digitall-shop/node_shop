using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class editinstance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AssignedXrayPort",
                schema: "Commerce",
                table: "Instance",
                newName: "XrayPort");

            migrationBuilder.RenameColumn(
                name: "AssignedServerPort",
                schema: "Commerce",
                table: "Instance",
                newName: "ServerPort");

            migrationBuilder.RenameColumn(
                name: "AssignedInboundPort",
                schema: "Commerce",
                table: "Instance",
                newName: "InboundPort");

            migrationBuilder.RenameIndex(
                name: "IX_Instance_NodeId_AssignedXrayPort",
                schema: "Commerce",
                table: "Instance",
                newName: "IX_Instance_NodeId_XrayPort");

            migrationBuilder.RenameIndex(
                name: "IX_Instance_NodeId_AssignedServerPort",
                schema: "Commerce",
                table: "Instance",
                newName: "IX_Instance_NodeId_ServerPort");

            migrationBuilder.RenameIndex(
                name: "IX_Instance_NodeId_AssignedInboundPort",
                schema: "Commerce",
                table: "Instance",
                newName: "IX_Instance_NodeId_InboundPort");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "XrayPort",
                schema: "Commerce",
                table: "Instance",
                newName: "AssignedXrayPort");

            migrationBuilder.RenameColumn(
                name: "ServerPort",
                schema: "Commerce",
                table: "Instance",
                newName: "AssignedServerPort");

            migrationBuilder.RenameColumn(
                name: "InboundPort",
                schema: "Commerce",
                table: "Instance",
                newName: "AssignedInboundPort");

            migrationBuilder.RenameIndex(
                name: "IX_Instance_NodeId_XrayPort",
                schema: "Commerce",
                table: "Instance",
                newName: "IX_Instance_NodeId_AssignedXrayPort");

            migrationBuilder.RenameIndex(
                name: "IX_Instance_NodeId_ServerPort",
                schema: "Commerce",
                table: "Instance",
                newName: "IX_Instance_NodeId_AssignedServerPort");

            migrationBuilder.RenameIndex(
                name: "IX_Instance_NodeId_InboundPort",
                schema: "Commerce",
                table: "Instance",
                newName: "IX_Instance_NodeId_AssignedInboundPort");
        }
    }
}
