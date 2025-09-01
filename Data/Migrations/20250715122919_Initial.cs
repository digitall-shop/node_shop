using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Commerce");

            migrationBuilder.EnsureSchema(
                name: "Account");

            migrationBuilder.CreateTable(
                name: "Node",
                schema: "Commerce",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NodeName = table.Column<string>(type: "varchar(200)", nullable: false),
                    SshPort = table.Column<int>(type: "int", nullable: false, defaultValue: 22),
                    SshHost = table.Column<string>(type: "varchar(100)", nullable: false),
                    SshUsername = table.Column<string>(type: "varchar(200)", nullable: false),
                    SshKeyFilePath = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    SshPassword = table.Column<string>(type: "varchar(100)", nullable: false),
                    Method = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    XrayContainerImage = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    AvailablePortsRangeJson = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Price = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Active"),
                    IsAvailableForShow = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Node", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "Account",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "NO-USERNAME"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsSupperAdmin = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, defaultValue: "NO-NAME"),
                    Balance = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastActivityAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Panels",
                schema: "Commerce",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PanelName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "NO-PANEL-NAME"),
                    PanelUserName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PanelPassword = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUsedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PanelUrl = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Panels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Panels_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Account",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Instance",
                schema: "Commerce",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContainerDockerId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ProvisionedInstanceId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NodeId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    PanelId = table.Column<long>(type: "bigint", nullable: false),
                    AssignedInboundPort = table.Column<int>(type: "int", nullable: false),
                    AssignedXrayPort = table.Column<int>(type: "int", nullable: false),
                    AssignedServerPort = table.Column<int>(type: "int", nullable: false),
                    XrayUserUuid = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MarzbanAssignedUsername = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MarzbanNodeIdInCustomerPanel = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Instance_Node_NodeId",
                        column: x => x.NodeId,
                        principalSchema: "Commerce",
                        principalTable: "Node",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Instance_Panels_PanelId",
                        column: x => x.PanelId,
                        principalSchema: "Commerce",
                        principalTable: "Panels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Instance_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Account",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Instance_NodeId_AssignedInboundPort",
                schema: "Commerce",
                table: "Instance",
                columns: new[] { "NodeId", "AssignedInboundPort" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Instance_NodeId_AssignedServerPort",
                schema: "Commerce",
                table: "Instance",
                columns: new[] { "NodeId", "AssignedServerPort" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Instance_NodeId_AssignedXrayPort",
                schema: "Commerce",
                table: "Instance",
                columns: new[] { "NodeId", "AssignedXrayPort" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Instance_PanelId",
                schema: "Commerce",
                table: "Instance",
                column: "PanelId");

            migrationBuilder.CreateIndex(
                name: "IX_Instance_UserId",
                schema: "Commerce",
                table: "Instance",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Panels_UserId",
                schema: "Commerce",
                table: "Panels",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Instance",
                schema: "Commerce");

            migrationBuilder.DropTable(
                name: "Node",
                schema: "Commerce");

            migrationBuilder.DropTable(
                name: "Panels",
                schema: "Commerce");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "Account");
        }
    }
}
