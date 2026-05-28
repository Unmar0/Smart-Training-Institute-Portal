using System.ComponentModel.DataAnnotations;

namespace Smart_Training_Institute_Portal.Models
{
    public class StudentProfile : BaseEntity
    {

        [Required]
        [StringLength(50)]
        public string FullName { get; set; }

        [Required]
        [StringLength(50)]
        public string StudentNumber { get; set; }

        [StringLength(200)]
        public string? ImageUrl { get; set; }

        public DateOnly? DateofBirth { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();


    }
}
