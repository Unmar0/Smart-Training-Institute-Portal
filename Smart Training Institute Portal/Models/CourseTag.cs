using System.ComponentModel.DataAnnotations;

namespace Smart_Training_Institute_Portal.Models
{
    public class CourseTag : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
