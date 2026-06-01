using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaPresenca.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Updatesessionattendancerelationsihpconfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Sessions_SessionId1",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_SessionId1",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "SessionId1",
                table: "Attendances");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SessionId1",
                table: "Attendances",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_SessionId1",
                table: "Attendances",
                column: "SessionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Sessions_SessionId1",
                table: "Attendances",
                column: "SessionId1",
                principalTable: "Sessions",
                principalColumn: "Id");
        }
    }
}
