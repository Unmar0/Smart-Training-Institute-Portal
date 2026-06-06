using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Smart_Training_Institute_Portal.Data.Migrations
{
    /// <inheritdoc />
    public partial class fixedmodels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Enrollments_StudentProfileId_CourseId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_CourseInstructors_CourseId_InstructorProfileId",
                table: "CourseInstructors");

            migrationBuilder.AlterColumn<int>(
                name: "EnrollmentId",
                table: "GradeAuditLogs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "Enrollments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "CourseInstructors",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_StudentProfileId_CourseId",
                table: "Enrollments",
                columns: new[] { "StudentProfileId", "CourseId" },
                unique: true,
                filter: "[CourseId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CourseInstructors_CourseId_InstructorProfileId",
                table: "CourseInstructors",
                columns: new[] { "CourseId", "InstructorProfileId" },
                unique: true,
                filter: "[CourseId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Enrollments_StudentProfileId_CourseId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_CourseInstructors_CourseId_InstructorProfileId",
                table: "CourseInstructors");

            migrationBuilder.AlterColumn<int>(
                name: "EnrollmentId",
                table: "GradeAuditLogs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "Enrollments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Courses",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "CourseInstructors",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_StudentProfileId_CourseId",
                table: "Enrollments",
                columns: new[] { "StudentProfileId", "CourseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseInstructors_CourseId_InstructorProfileId",
                table: "CourseInstructors",
                columns: new[] { "CourseId", "InstructorProfileId" },
                unique: true);
        }
    }
}
