using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class add_node_agent_fields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                schema: "Commerce",
                table: "Node",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "ProvisioningStatus",
                schema: "Commerce",
                table: "Node",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Pending");

            migrationBuilder.AddColumn<string>(
                name: "ProvisioningMessage",
                schema: "Commerce",
                table: "Node",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AgentVersion",
                schema: "Commerce",
                table: "Node",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetAgentVersion",
                schema: "Commerce",
                table: "Node",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSeenUtc",
                schema: "Commerce",
                table: "Node",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstallMethod",
                schema: "Commerce",
                table: "Node",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Docker");

            migrationBuilder.AddColumn<string>(
                name: "MarzbanEndpoint",
                schema: "Commerce",
                table: "Node",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AgentEnrollmentToken",
                schema: "Commerce",
                table: "Node",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AgentEnrollmentTokenExpiresUtc",
                schema: "Commerce",
                table: "Node",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "IsEnabled", schema: "Commerce", table: "Node");
            migrationBuilder.DropColumn(name: "ProvisioningStatus", schema: "Commerce", table: "Node");
            migrationBuilder.DropColumn(name: "ProvisioningMessage", schema: "Commerce", table: "Node");
            migrationBuilder.DropColumn(name: "AgentVersion", schema: "Commerce", table: "Node");
            migrationBuilder.DropColumn(name: "TargetAgentVersion", schema: "Commerce", table: "Node");
            migrationBuilder.DropColumn(name: "LastSeenUtc", schema: "Commerce", table: "Node");
            migrationBuilder.DropColumn(name: "InstallMethod", schema: "Commerce", table: "Node");
            migrationBuilder.DropColumn(name: "MarzbanEndpoint", schema: "Commerce", table: "Node");
            migrationBuilder.DropColumn(name: "AgentEnrollmentToken", schema: "Commerce", table: "Node");
            migrationBuilder.DropColumn(name: "AgentEnrollmentTokenExpiresUtc", schema: "Commerce", table: "Node");
        }
    }
}

