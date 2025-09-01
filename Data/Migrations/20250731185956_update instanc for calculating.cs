using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class updateinstancforcalculating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "LastBilledUsage",
                schema: "Commerce",
                table: "Instance",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastBillingTimestamp",
                schema: "Commerce",
                table: "Instance",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastBilledUsage",
                schema: "Commerce",
                table: "Instance");

            migrationBuilder.DropColumn(
                name: "LastBillingTimestamp",
                schema: "Commerce",
                table: "Instance");
        }
    }
}
