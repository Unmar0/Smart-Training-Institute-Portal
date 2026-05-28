using System.ComponentModel.DataAnnotations;

namespace Smart_Training_Institute_Portal.Models
{
    public class CourseInstructor : BaseEntity
    {
        public int CourseId { get; set; }
        public Course Course { get; set; }

        public int InstructorProfileId { get; set; }
        public InstructorProfile InstructorProfile { get; set; }
    }
}
