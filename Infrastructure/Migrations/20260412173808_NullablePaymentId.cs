using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NullablePaymentId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TrainingsReservations_PaymentId",
                table: "TrainingsReservations");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentId",
                table: "TrainingsReservations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingsReservations_PaymentId",
                table: "TrainingsReservations",
                column: "PaymentId",
                unique: true,
                filter: "[PaymentId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TrainingsReservations_PaymentId",
                table: "TrainingsReservations");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentId",
                table: "TrainingsReservations",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrainingsReservations_PaymentId",
                table: "TrainingsReservations",
                column: "PaymentId",
                unique: true);
        }
    }
}
