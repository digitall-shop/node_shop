using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class editedomaineventes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "Ticket",
                table: "SupportTickets");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                schema: "Account",
                table: "Users",
                newName: "IsDelete");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "Account",
                table: "Users",
                newName: "ModifiedDate");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                schema: "Payment",
                table: "Transactions",
                newName: "IsDelete");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                schema: "Ticket",
                table: "SupportTickets",
                newName: "IsDelete");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "Ticket",
                table: "SupportTickets",
                newName: "CreateDate");

            migrationBuilder.RenameIndex(
                name: "IX_SupportTickets_CreatedAt",
                schema: "Ticket",
                table: "SupportTickets",
                newName: "IX_SupportTickets_CreateDate");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                schema: "Support",
                table: "SupportMessages",
                newName: "IsDelete");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "Support",
                table: "SupportMessages",
                newName: "CreateDate");

            migrationBuilder.RenameIndex(
                name: "IX_SupportMessages_TicketId_CreatedAt",
                schema: "Support",
                table: "SupportMessages",
                newName: "IX_SupportMessages_TicketId_CreateDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                schema: "Payment",
                table: "PaymentRequests",
                newName: "ModifiedDate");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                schema: "Payment",
                table: "PaymentRequests",
                newName: "IsDelete");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "Payment",
                table: "PaymentRequests",
                newName: "CreateDate");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                schema: "Commerce",
                table: "Panels",
                newName: "IsDelete");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "Commerce",
                table: "Panels",
                newName: "ModifiedDate");

            migrationBuilder.RenameColumn(
                name: "UpdateAt",
                schema: "Commerce",
                table: "Node",
                newName: "ModifiedDate");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                schema: "Commerce",
                table: "Node",
                newName: "IsDelete");

            migrationBuilder.RenameColumn(
                name: "CreateAt",
                schema: "Commerce",
                table: "Node",
                newName: "CreateDate");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                schema: "System",
                table: "Logs",
                newName: "IsDelete");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                schema: "Commerce",
                table: "Instance",
                newName: "IsDelete");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "Commerce",
                table: "Instance",
                newName: "ModifiedDate");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                schema: "Account",
                table: "BankAccount",
                newName: "IsDelete");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "Account",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "Payment",
                table: "Transactions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                schema: "Payment",
                table: "Transactions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                schema: "Ticket",
                table: "SupportTickets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                schema: "Support",
                table: "SupportMessages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "Commerce",
                table: "Panels",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "System",
                table: "Logs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                schema: "System",
                table: "Logs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "Commerce",
                table: "Instance",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                schema: "Setting",
                table: "CurrencyRates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "Account",
                table: "BankAccount",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                schema: "Account",
                table: "BankAccount",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "Account",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "Payment",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                schema: "Payment",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                schema: "Ticket",
                table: "SupportTickets");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                schema: "Support",
                table: "SupportMessages");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "Commerce",
                table: "Panels");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "System",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                schema: "System",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "Commerce",
                table: "Instance");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                schema: "Setting",
                table: "CurrencyRates");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "Account",
                table: "BankAccount");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                schema: "Account",
                table: "BankAccount");

            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                schema: "Account",
                table: "Users",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "IsDelete",
                schema: "Account",
                table: "Users",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "IsDelete",
                schema: "Payment",
                table: "Transactions",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "IsDelete",
                schema: "Ticket",
                table: "SupportTickets",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "CreateDate",
                schema: "Ticket",
                table: "SupportTickets",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_SupportTickets_CreateDate",
                schema: "Ticket",
                table: "SupportTickets",
                newName: "IX_SupportTickets_CreatedAt");

            migrationBuilder.RenameColumn(
                name: "IsDelete",
                schema: "Support",
                table: "SupportMessages",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "CreateDate",
                schema: "Support",
                table: "SupportMessages",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_SupportMessages_TicketId_CreateDate",
                schema: "Support",
                table: "SupportMessages",
                newName: "IX_SupportMessages_TicketId_CreatedAt");

            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                schema: "Payment",
                table: "PaymentRequests",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "IsDelete",
                schema: "Payment",
                table: "PaymentRequests",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "CreateDate",
                schema: "Payment",
                table: "PaymentRequests",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                schema: "Commerce",
                table: "Panels",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "IsDelete",
                schema: "Commerce",
                table: "Panels",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                schema: "Commerce",
                table: "Node",
                newName: "UpdateAt");

            migrationBuilder.RenameColumn(
                name: "IsDelete",
                schema: "Commerce",
                table: "Node",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "CreateDate",
                schema: "Commerce",
                table: "Node",
                newName: "CreateAt");

            migrationBuilder.RenameColumn(
                name: "IsDelete",
                schema: "System",
                table: "Logs",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                schema: "Commerce",
                table: "Instance",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "IsDelete",
                schema: "Commerce",
                table: "Instance",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "IsDelete",
                schema: "Account",
                table: "BankAccount",
                newName: "IsDeleted");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "Ticket",
                table: "SupportTickets",
                type: "datetime2",
                nullable: true);
        }
    }
}
