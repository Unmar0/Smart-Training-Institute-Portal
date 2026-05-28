using Microsoft.AspNetCore.Identity;

namespace Smart_Training_Institute_Portal.Models
{
    public class ApplicationUser : IdentityUser
    {
        public StudentProfile? StudentProfile { get; set; }

        public InstructorProfile? InstructorProfile { get; set; }

        public ICollection<GradeAuditLog> GradeAuditLogs { get; set; } = new List<GradeAuditLog>();

    }
}
