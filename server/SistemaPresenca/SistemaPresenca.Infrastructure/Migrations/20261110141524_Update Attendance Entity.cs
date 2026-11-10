using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaPresenca.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAttendanceEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RegisteredAt",
                table: "Attendances",
                newName: "CheckOut");

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckIn",
                table: "Attendances",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckIn",
                table: "Attendances");

            migrationBuilder.RenameColumn(
                name: "CheckOut",
                table: "Attendances",
                newName: "RegisteredAt");
        }
    }
}
