using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Trainings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Trainings",
                newName: "StartDate");

            migrationBuilder.AddColumn<int>(
                name: "CashbackPercentage",
                table: "Trainings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Trainings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Trainings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CashbackPercentage",
                table: "Trainings");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Trainings");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Trainings");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Trainings",
                newName: "Date");
        }
    }
}
