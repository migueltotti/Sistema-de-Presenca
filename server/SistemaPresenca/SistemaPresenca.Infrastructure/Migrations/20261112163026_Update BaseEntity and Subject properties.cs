using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaPresenca.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBaseEntityandSubjectproperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Majors_Users_CreatedByAdminId",
                table: "Majors");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomAccessPermissions_Users_CreatedByAdminId",
                table: "RoomAccessPermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomBookings_Users_CreatedByAdminId",
                table: "RoomBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Users_CreatedByAdminId",
                table: "Rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Users_CreatedByAdminId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Users_ProfessorId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Users_CreatedByAdminId",
                table: "Subjects");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_CreatedByAdminId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_ProfessorId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "ProfessorId",
                table: "Sessions");

            migrationBuilder.RenameColumn(
                name: "CreatedByAdminId",
                table: "Users",
                newName: "CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Users_CreatedByAdminId",
                table: "Users",
                newName: "IX_Users_CreatedById");

            migrationBuilder.RenameColumn(
                name: "CreatedByAdminId",
                table: "Subjects",
                newName: "CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Subjects_CreatedByAdminId",
                table: "Subjects",
                newName: "IX_Subjects_CreatedById");

            migrationBuilder.RenameColumn(
                name: "CreatedByAdminId",
                table: "Sessions",
                newName: "CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Sessions_CreatedByAdminId",
                table: "Sessions",
                newName: "IX_Sessions_CreatedById");

            migrationBuilder.RenameColumn(
                name: "CreatedByAdminId",
                table: "Rooms",
                newName: "CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Rooms_CreatedByAdminId",
                table: "Rooms",
                newName: "IX_Rooms_CreatedById");

            migrationBuilder.RenameColumn(
                name: "CreatedByAdminId",
                table: "RoomBookings",
                newName: "CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_RoomBookings_CreatedByAdminId",
                table: "RoomBookings",
                newName: "IX_RoomBookings_CreatedById");

            migrationBuilder.RenameColumn(
                name: "CreatedByAdminId",
                table: "RoomAccessPermissions",
                newName: "CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_RoomAccessPermissions_CreatedByAdminId",
                table: "RoomAccessPermissions",
                newName: "IX_RoomAccessPermissions_CreatedById");

            migrationBuilder.RenameColumn(
                name: "CreatedByAdminId",
                table: "Majors",
                newName: "CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Majors_CreatedByAdminId",
                table: "Majors",
                newName: "IX_Majors_CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Majors_Users_CreatedById",
                table: "Majors",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomAccessPermissions_Users_CreatedById",
                table: "RoomAccessPermissions",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomBookings_Users_CreatedById",
                table: "RoomBookings",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Users_CreatedById",
                table: "Rooms",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Users_CreatedById",
                table: "Sessions",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Users_CreatedById",
                table: "Subjects",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_CreatedById",
                table: "Users",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Majors_Users_CreatedById",
                table: "Majors");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomAccessPermissions_Users_CreatedById",
                table: "RoomAccessPermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomBookings_Users_CreatedById",
                table: "RoomBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Users_CreatedById",
                table: "Rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Users_CreatedById",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Users_CreatedById",
                table: "Subjects");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_CreatedById",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "Users",
                newName: "CreatedByAdminId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_CreatedById",
                table: "Users",
                newName: "IX_Users_CreatedByAdminId");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "Subjects",
                newName: "CreatedByAdminId");

            migrationBuilder.RenameIndex(
                name: "IX_Subjects_CreatedById",
                table: "Subjects",
                newName: "IX_Subjects_CreatedByAdminId");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "Sessions",
                newName: "CreatedByAdminId");

            migrationBuilder.RenameIndex(
                name: "IX_Sessions_CreatedById",
                table: "Sessions",
                newName: "IX_Sessions_CreatedByAdminId");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "Rooms",
                newName: "CreatedByAdminId");

            migrationBuilder.RenameIndex(
                name: "IX_Rooms_CreatedById",
                table: "Rooms",
                newName: "IX_Rooms_CreatedByAdminId");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "RoomBookings",
                newName: "CreatedByAdminId");

            migrationBuilder.RenameIndex(
                name: "IX_RoomBookings_CreatedById",
                table: "RoomBookings",
                newName: "IX_RoomBookings_CreatedByAdminId");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "RoomAccessPermissions",
                newName: "CreatedByAdminId");

            migrationBuilder.RenameIndex(
                name: "IX_RoomAccessPermissions_CreatedById",
                table: "RoomAccessPermissions",
                newName: "IX_RoomAccessPermissions_CreatedByAdminId");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "Majors",
                newName: "CreatedByAdminId");

            migrationBuilder.RenameIndex(
                name: "IX_Majors_CreatedById",
                table: "Majors",
                newName: "IX_Majors_CreatedByAdminId");

            migrationBuilder.AddColumn<Guid>(
                name: "ProfessorId",
                table: "Sessions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_ProfessorId",
                table: "Sessions",
                column: "ProfessorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Majors_Users_CreatedByAdminId",
                table: "Majors",
                column: "CreatedByAdminId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomAccessPermissions_Users_CreatedByAdminId",
                table: "RoomAccessPermissions",
                column: "CreatedByAdminId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomBookings_Users_CreatedByAdminId",
                table: "RoomBookings",
                column: "CreatedByAdminId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Users_CreatedByAdminId",
                table: "Rooms",
                column: "CreatedByAdminId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Users_CreatedByAdminId",
                table: "Sessions",
                column: "CreatedByAdminId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Users_ProfessorId",
                table: "Sessions",
                column: "ProfessorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Users_CreatedByAdminId",
                table: "Subjects",
                column: "CreatedByAdminId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_CreatedByAdminId",
                table: "Users",
                column: "CreatedByAdminId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
