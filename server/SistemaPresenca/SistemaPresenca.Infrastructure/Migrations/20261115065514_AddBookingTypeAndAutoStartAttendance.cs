using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaPresenca.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingTypeAndAutoStartAttendance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "RoomId",
                table: "Sessions",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "RoomBookingId",
                table: "Sessions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_RoomBookingId",
                table: "Sessions",
                column: "RoomBookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_RoomBookings_RoomBookingId",
                table: "Sessions",
                column: "RoomBookingId",
                principalTable: "RoomBookings",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_RoomBookings_RoomBookingId",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_RoomBookingId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "RoomBookingId",
                table: "Sessions");

            migrationBuilder.AlterColumn<Guid>(
                name: "RoomId",
                table: "Sessions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
