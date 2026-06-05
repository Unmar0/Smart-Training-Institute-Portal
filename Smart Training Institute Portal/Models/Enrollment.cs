using System.ComponentModel.DataAnnotations;

namespace Smart_Training_Institute_Portal.Models
{
    public class Enrollment : BaseEntity
    {
        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;

        [Range(0, 100)]
        public decimal? Mark { get; set; }

        [StringLength(20)]
        public string? Grade { get; set; }

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "Enrolled";

        [Required]
        public int? StudentProfileId { get; set; }
        public StudentProfile StudentProfile { get; set; } = null!;

        public int? CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public ICollection<GradeAuditLog> GradeAuditLogs { get; set; } = new List<GradeAuditLog>();

    }
}
