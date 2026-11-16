using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaPresenca.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSessionproperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_RoomBookings_RoomBookingId",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_RoomBookingId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "ProfessorIds",
                table: "Subjects");

            migrationBuilder.AlterColumn<DateTime>(
                name: "InitiedAt",
                table: "Sessions",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<DateOnly>(
                name: "Date",
                table: "Sessions",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "ScheduledEndTime",
                table: "Sessions",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "ScheduledStartTime",
                table: "Sessions",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<Guid>(
                name: "StartedById",
                table: "Sessions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Sessions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "SubjectId",
                table: "RoomBookings",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<bool>(
                name: "AutoStartAttendance",
                table: "RoomBookings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "RoomBookings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_RoomBookingId",
                table: "Sessions",
                column: "RoomBookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_StartedById",
                table: "Sessions",
                column: "StartedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_RoomBookings_RoomBookingId",
                table: "Sessions",
                column: "RoomBookingId",
                principalTable: "RoomBookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Users_StartedById",
                table: "Sessions",
                column: "StartedById",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_RoomBookings_RoomBookingId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Users_StartedById",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_RoomBookingId",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_StartedById",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "ScheduledEndTime",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "ScheduledStartTime",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "StartedById",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "AutoStartAttendance",
                table: "RoomBookings");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "RoomBookings");

            migrationBuilder.AddColumn<Guid[]>(
                name: "ProfessorIds",
                table: "Subjects",
                type: "uuid[]",
                nullable: false,
                defaultValue: new Guid[0]);

            migrationBuilder.AlterColumn<DateTime>(
                name: "InitiedAt",
                table: "Sessions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "SubjectId",
                table: "RoomBookings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

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
    }
}
