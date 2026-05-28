using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Smart_Training_Institute_Portal.Models;


namespace Smart_Training_Institute_Portal.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<StudentProfile> StudentProfiles { get; set; }
        public DbSet<InstructorProfile> InstructorProfiles { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<CourseInstructor> CourseInstructors { get; set; }
        public DbSet<CourseTag> CourseTags { get; set; }
        public DbSet<GradeAuditLog> GradeAuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure CourseInstructor many-to-many join entity
            builder.Entity<CourseInstructor>()
                .HasKey(ci => new { ci.CourseId, ci.InstructorProfileId });

            builder.Entity<CourseInstructor>()
                .HasOne(ci => ci.Course)
                .WithMany(c => c.CourseInstructors)
                .HasForeignKey(ci => ci.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CourseInstructor>()
                .HasOne(ci => ci.InstructorProfile)
                .WithMany(ip => ip.CourseInstructors)
                .HasForeignKey(ci => ci.InstructorProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            // Setup Enrollment restrictions
            builder.Entity<Enrollment>()
                .HasIndex(e => new { e.StudentProfileId, e.CourseId })
                .IsUnique();

            builder.Entity<Enrollment>()
                .Property(e => e.Mark)
                .HasColumnType("decimal(5,2)");

            builder.Entity<Enrollment>()
                .HasOne(e => e.Profile)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Decimal configuration for Course price
            builder.Entity<Course>()
                .Property(c => c.Price)
                .HasColumnType("decimal(18,2)");
                
            // Configure GradeAuditLog Delete Behavior
            builder.Entity<GradeAuditLog>()
                .Property(g => g.OldMark)
                .HasColumnType("decimal(5,2)");
                
            builder.Entity<GradeAuditLog>()
                .Property(g => g.NewMark)
                .HasColumnType("decimal(5,2)");

            builder.Entity<GradeAuditLog>()
                .HasOne(g => g.Enrollment)
                .WithMany(e => e.GradeAuditLog)
                .HasForeignKey(g => g.EnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.Entity<GradeAuditLog>()
                .HasOne(g => g.ChangedByUser)
                .WithMany(u => u.GradeAuditLogs)
                .HasForeignKey(g => g.ChangedBy)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Setup Departments
            builder.Entity<Course>()
                .HasOne(c => c.Department)
                .WithMany(d => d.Courses)
                .HasForeignKey(c => c.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }


}
