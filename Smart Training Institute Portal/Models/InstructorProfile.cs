using System.ComponentModel.DataAnnotations;

namespace Smart_Training_Institute_Portal.Models
{
    public class InstructorProfile : BaseEntity
    {
        [StringLength(50)]
        public string InstructorNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string FullName { get; set; }

        [Required]
        [StringLength(100)]
        public string Specialization { get; set; }

        [StringLength(1000)]
        public string? Bio { get; set; }

        [StringLength(200)]
        public string? ImageUrl { get; set; }

        [StringLength(10)]
        public string? OfficeNumber { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public ICollection<CourseInstructor> CourseInstructors { get; set; } = new List<CourseInstructor>();
    }
}
