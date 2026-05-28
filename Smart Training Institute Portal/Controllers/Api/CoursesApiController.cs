using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Training_Institute_Portal.Data;
using Smart_Training_Institute_Portal.DTOs;

namespace Smart_Training_Institute_Portal.Controllers.Api
{
    [Route("api/courses")]
    [ApiController]
    public class CoursesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CoursesApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
        {
            var courses = await _context.Courses
                .Include(c => c.Department)
                .Include(c => c.CourseTags)
                .Where(c => c.IsPublished && c.IsDeleted != true)
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Code = c.Code,
                    Description = c.Description,
                    CreditHours = c.CreditHours,
                    Level = c.Level,
                    Price = c.Price,
                    DepartmentName = c.Department.Name,
                    Tags = c.CourseTags.Select(t => t.Name).ToList()
                })
                .ToListAsync();

            return Ok(courses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDto>> GetCourse(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Department)
                .Include(c => c.CourseTags)
                .Where(c => c.Id == id && c.IsDeleted != true)
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Code = c.Code,
                    Description = c.Description,
                    CreditHours = c.CreditHours,
                    Level = c.Level,
                    Price = c.Price,
                    DepartmentName = c.Department.Name,
                    Tags = c.CourseTags.Select(t => t.Name).ToList()
                })
                .FirstOrDefaultAsync();

            if (course == null)
            {
                return NotFound();
            }

            return Ok(course);
        }
    }
}