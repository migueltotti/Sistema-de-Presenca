using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaPresenca.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserandSubjectpropertiesandrelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Users_ProfessorId",
                table: "Subjects");

            migrationBuilder.DropTable(
                name: "SubjectUser");

            migrationBuilder.DropIndex(
                name: "IX_Users_TagId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "ProfessorId",
                table: "Subjects",
                newName: "DeletedByAdminId");

            migrationBuilder.RenameIndex(
                name: "IX_Subjects_ProfessorId",
                table: "Subjects",
                newName: "IX_Subjects_DeletedByAdminId");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Cpf",
                table: "Users",
                type: "character varying(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(11)",
                oldMaxLength: 11,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByAdminId",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Subjects",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid[]>(
                name: "ProfessorIds",
                table: "Subjects",
                type: "uuid[]",
                nullable: false,
                defaultValue: new Guid[0]);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Sessions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByAdminId",
                table: "Sessions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Majors",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByAdminId",
                table: "Majors",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SubjectProfessors",
                columns: table => new
                {
                    ProfessorsId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectProfessors", x => new { x.ProfessorsId, x.SubjectId });
                    table.ForeignKey(
                        name: "FK_SubjectProfessors_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubjectProfessors_Users_ProfessorsId",
                        column: x => x.ProfessorsId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubjectStudents",
                columns: table => new
                {
                    StudentsId = table.Column<Guid>(type: "uuid", nullable: false),
                    Subject1Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectStudents", x => new { x.StudentsId, x.Subject1Id });
                    table.ForeignKey(
                        name: "FK_SubjectStudents_Subjects_Subject1Id",
                        column: x => x.Subject1Id,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubjectStudents_Users_StudentsId",
                        column: x => x.StudentsId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_DeletedByAdminId",
                table: "Users",
                column: "DeletedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_DeletedByAdminId",
                table: "Sessions",
                column: "DeletedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Majors_DeletedByAdminId",
                table: "Majors",
                column: "DeletedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectProfessors_SubjectId",
                table: "SubjectProfessors",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectStudents_Subject1Id",
                table: "SubjectStudents",
                column: "Subject1Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Majors_Users_DeletedByAdminId",
                table: "Majors",
                column: "DeletedByAdminId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Users_DeletedByAdminId",
                table: "Sessions",
                column: "DeletedByAdminId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Users_DeletedByAdminId",
                table: "Subjects",
                column: "DeletedByAdminId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_DeletedByAdminId",
                table: "Users",
                column: "DeletedByAdminId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Majors_Users_DeletedByAdminId",
                table: "Majors");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Users_DeletedByAdminId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Users_DeletedByAdminId",
                table: "Subjects");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_DeletedByAdminId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "SubjectProfessors");

            migrationBuilder.DropTable(
                name: "SubjectStudents");

            migrationBuilder.DropIndex(
                name: "IX_Users_DeletedByAdminId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_DeletedByAdminId",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Majors_DeletedByAdminId",
                table: "Majors");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DeletedByAdminId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "ProfessorIds",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "DeletedByAdminId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Majors");

            migrationBuilder.DropColumn(
                name: "DeletedByAdminId",
                table: "Majors");

            migrationBuilder.RenameColumn(
                name: "DeletedByAdminId",
                table: "Subjects",
                newName: "ProfessorId");

            migrationBuilder.RenameIndex(
                name: "IX_Subjects_DeletedByAdminId",
                table: "Subjects",
                newName: "IX_Subjects_ProfessorId");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Cpf",
                table: "Users",
                type: "character varying(11)",
                maxLength: 11,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(11)",
                oldMaxLength: 11);

            migrationBuilder.CreateTable(
                name: "SubjectUser",
                columns: table => new
                {
                    StudentsId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectUser", x => new { x.StudentsId, x.SubjectsId });
                    table.ForeignKey(
                        name: "FK_SubjectUser_Subjects_SubjectsId",
                        column: x => x.SubjectsId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubjectUser_Users_StudentsId",
                        column: x => x.StudentsId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_TagId",
                table: "Users",
                column: "TagId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubjectUser_SubjectsId",
                table: "SubjectUser",
                column: "SubjectsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Users_ProfessorId",
                table: "Subjects",
                column: "ProfessorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
