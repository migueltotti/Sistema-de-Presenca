using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaPresenca.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelsConfigurations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RoomId",
                table: "Sessions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    MicrocontrollerId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByAdminId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedByAdminId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rooms_Users_CreatedByAdminId",
                        column: x => x.CreatedByAdminId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Rooms_Users_DeletedByAdminId",
                        column: x => x.DeletedByAdminId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RoomAccessPermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoomId = table.Column<Guid>(type: "uuid", nullable: false),
                    GrantedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByAdminId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedByAdminId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomAccessPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomAccessPermissions_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoomAccessPermissions_Users_CreatedByAdminId",
                        column: x => x.CreatedByAdminId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RoomAccessPermissions_Users_DeletedByAdminId",
                        column: x => x.DeletedByAdminId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RoomAccessPermissions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoomBookings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoomId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProfessorId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartsAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndsAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByAdminId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedByAdminId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomBookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomBookings_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoomBookings_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoomBookings_Users_CreatedByAdminId",
                        column: x => x.CreatedByAdminId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RoomBookings_Users_DeletedByAdminId",
                        column: x => x.DeletedByAdminId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RoomBookings_Users_ProfessorId",
                        column: x => x.ProfessorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_RoomId",
                table: "Sessions",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomAccessPermissions_CreatedByAdminId",
                table: "RoomAccessPermissions",
                column: "CreatedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomAccessPermissions_DeletedByAdminId",
                table: "RoomAccessPermissions",
                column: "DeletedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomAccessPermissions_RoomId",
                table: "RoomAccessPermissions",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomAccessPermissions_UserId_RoomId",
                table: "RoomAccessPermissions",
                columns: new[] { "UserId", "RoomId" },
                unique: true,
                filter: "\"DeletedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RoomBookings_CreatedByAdminId",
                table: "RoomBookings",
                column: "CreatedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomBookings_DeletedByAdminId",
                table: "RoomBookings",
                column: "DeletedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomBookings_ProfessorId",
                table: "RoomBookings",
                column: "ProfessorId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomBookings_RoomId_StartsAt_EndsAt",
                table: "RoomBookings",
                columns: new[] { "RoomId", "StartsAt", "EndsAt" });

            migrationBuilder.CreateIndex(
                name: "IX_RoomBookings_SubjectId",
                table: "RoomBookings",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_CreatedByAdminId",
                table: "Rooms",
                column: "CreatedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_DeletedByAdminId",
                table: "Rooms",
                column: "DeletedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_MicrocontrollerId",
                table: "Rooms",
                column: "MicrocontrollerId",
                unique: true,
                filter: "\"DeletedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_Name",
                table: "Rooms",
                column: "Name",
                unique: true,
                filter: "\"DeletedAt\" IS NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Rooms_RoomId",
                table: "Sessions",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Rooms_RoomId",
                table: "Sessions");

            migrationBuilder.DropTable(
                name: "RoomAccessPermissions");

            migrationBuilder.DropTable(
                name: "RoomBookings");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_RoomId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "Sessions");
        }
    }
}
