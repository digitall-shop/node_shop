using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class editeallentites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                schema: "Account",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "Account",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "Commerce",
                table: "Panels",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateAt",
                schema: "Commerce",
                table: "Node",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "Commerce",
                table: "Node",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateAt",
                schema: "Commerce",
                table: "Node",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "Commerce",
                table: "Instance",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDelete",
                schema: "Account",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "Account",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "Commerce",
                table: "Panels");

            migrationBuilder.DropColumn(
                name: "CreateAt",
                schema: "Commerce",
                table: "Node");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "Commerce",
                table: "Node");

            migrationBuilder.DropColumn(
                name: "UpdateAt",
                schema: "Commerce",
                table: "Node");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "Commerce",
                table: "Instance");
        }
    }
}
