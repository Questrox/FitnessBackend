using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DateAndAdminId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminId",
                table: "Payments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "Memberships",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Memberships",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "AdminId",
                table: "CancellationNotifications",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_AdminId",
                table: "Payments",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_CancellationNotifications_AdminId",
                table: "CancellationNotifications",
                column: "AdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_CancellationNotifications_AspNetUsers_AdminId",
                table: "CancellationNotifications",
                column: "AdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_AspNetUsers_AdminId",
                table: "Payments",
                column: "AdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CancellationNotifications_AspNetUsers_AdminId",
                table: "CancellationNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_AspNetUsers_AdminId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_AdminId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_CancellationNotifications_AdminId",
                table: "CancellationNotifications");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "CancellationNotifications");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "Memberships",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Memberships",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");
        }
    }
}
