using System.ComponentModel.DataAnnotations;

namespace Smart_Training_Institute_Portal.Models
{
    public class Course : BaseEntity
    {

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(20)]
        public string Code { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        [Range(1, 100)]
        public int CreditHours { get; set; }

        public decimal? Price { get; set; }

        [Range(1, 100)]
        public int Capacity { get; set; }

        [Required]
        [StringLength(50)]
        public string Level { get; set; }

        public bool IsPublished { get; set; } = false;


        public int DepartmentId { get; set; }
        public Department? Department { get; set; }
        public ICollection<CourseInstructor> CourseInstructors { get; set; } = new List<CourseInstructor>();

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

        public ICollection<CourseTag> CourseTags { get; set; } = new List<CourseTag>();




    }
}
