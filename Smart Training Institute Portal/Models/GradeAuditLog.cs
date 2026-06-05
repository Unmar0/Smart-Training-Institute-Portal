using System.ComponentModel.DataAnnotations;

namespace Smart_Training_Institute_Portal.Models
{
    public class GradeAuditLog : BaseEntity
    {

        [Range(0, 100)]
        public decimal? OldMark { get; set; }

        [Range(0, 100)]
        public decimal? NewMark { get; set; }

        [StringLength(20)]
        public string? OldGrade { get; set; }

        [StringLength(20)]
        public string? NewGrade { get; set; }

        [StringLength(50)]
        public string? Notes { get; set; }


        public int? EnrollmentId { get; set; }
        public Enrollment Enrollment { get; set; } = null!;

        public string ChangedBy { get; set; } = string.Empty;
        public ApplicationUser ChangedByUser { get; set; } = null!;
    }
}
