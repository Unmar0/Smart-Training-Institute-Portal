using System.ComponentModel.DataAnnotations;

namespace Smart_Training_Institute_Portal.DTOs
{
    public class CourseCreateUpdateDto
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Code { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [StringLength(200)]
        public string? ImageUrl { get; set; }

        [Range(1, 100)]
        public int CreditHours { get; set; }

        public decimal? Price { get; set; }

        [Range(1, 100)]
        public int Capacity { get; set; }

        [Required]
        [StringLength(50)]
        public string Level { get; set; } = string.Empty;

        public bool IsPublished { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        public List<int> CourseTagIds { get; set; } = new List<int>();
    }
}