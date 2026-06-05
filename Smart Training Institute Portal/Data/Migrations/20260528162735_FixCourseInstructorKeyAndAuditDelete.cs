using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Smart_Training_Institute_Portal.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixCourseInstructorKeyAndAuditDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GradeAuditLogs_Enrollments_EnrollmentId",
                table: "GradeAuditLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseInstructors",
                table: "CourseInstructors");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "CourseInstructors",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "CourseInstructors",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "CourseInstructors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CourseInstructors",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "CourseInstructors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseInstructors",
                table: "CourseInstructors",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CourseInstructors_CourseId_InstructorProfileId",
                table: "CourseInstructors",
                columns: new[] { "CourseId", "InstructorProfileId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GradeAuditLogs_Enrollments_EnrollmentId",
                table: "GradeAuditLogs",
                column: "EnrollmentId",
                principalTable: "Enrollments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GradeAuditLogs_Enrollments_EnrollmentId",
                table: "GradeAuditLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseInstructors",
                table: "CourseInstructors");

            migrationBuilder.DropIndex(
                name: "IX_CourseInstructors_CourseId_InstructorProfileId",
                table: "CourseInstructors");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CourseInstructors");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "CourseInstructors");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "CourseInstructors");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CourseInstructors");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "CourseInstructors");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseInstructors",
                table: "CourseInstructors",
                columns: new[] { "CourseId", "InstructorProfileId" });

            migrationBuilder.AddForeignKey(
                name: "FK_GradeAuditLogs_Enrollments_EnrollmentId",
                table: "GradeAuditLogs",
                column: "EnrollmentId",
                principalTable: "Enrollments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
