namespace Smart_Training_Institute_Portal.DTOs
{
    public class CourseDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int CreditHours { get; set; }

        public string? Level { get; set; }

        public decimal? Price { get; set; }

        public string DepartmentName { get; set; } = string.Empty;

        public List<string> Tags { get; set; } = new List<string>();
    }
}