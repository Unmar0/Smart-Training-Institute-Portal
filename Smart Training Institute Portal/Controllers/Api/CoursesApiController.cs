using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Training_Institute_Portal.Data;
using Smart_Training_Institute_Portal.DTOs;
using Smart_Training_Institute_Portal.Models;

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
            var courses = await PublishedActiveCoursesQuery()
                .Include(c => c.Department)
                .Include(c => c.CourseTags)
                .Include(c => c.Enrollments)
                .Include(c => c.CourseInstructors)
                .Select(ProjectToCourseDto())
                .ToListAsync();

            return Ok(courses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDto>> GetCourse(int id)
        {
            var course = await ActiveCoursesQuery()
                .Include(c => c.Department)
                .Include(c => c.CourseTags)
                .Where(c => c.Id == id)
                .Select(ProjectToCourseDto())
                .FirstOrDefaultAsync();

            if (course == null)
            {
                return NotFound();
            }

            return Ok(course);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CourseDto>> CreateCourse(CourseCreateUpdateDto dto)
        {
            var departmentExists = await _context.Departments
                .AnyAsync(d => d.Id == dto.DepartmentId && d.IsDeleted != true);

            if (!departmentExists)
            {
                return BadRequest("Invalid DepartmentId.");
            }

            var codeExists = await _context.Courses
                .AnyAsync(c => c.Code == dto.Code && c.IsDeleted != true);

            if (codeExists)
            {
                return BadRequest("Course code already exists.");
            }

            var course = new Course
            {
                Title = dto.Title,
                Code = dto.Code,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                CreditHours = dto.CreditHours,
                Price = dto.Price,
                Capacity = dto.Capacity,
                Level = dto.Level,
                IsPublished = dto.IsPublished,
                DepartmentId = dto.DepartmentId
            };

            if (dto.CourseTagIds.Any())
            {
                var tags = await _context.CourseTags
                    .Where(t => dto.CourseTagIds.Contains(t.Id) && t.IsDeleted != true)
                    .ToListAsync();

                course.CourseTags = tags;
            }

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            var result = await BuildCourseDto(course.Id);

            if (result == null)
            {
                return NotFound();
            }

            return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCourse(int id, CourseCreateUpdateDto dto)
        {
            var course = await ActiveCoursesQuery()
                .Include(c => c.CourseTags)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            var departmentExists = await _context.Departments
                .AnyAsync(d => d.Id == dto.DepartmentId && d.IsDeleted != true);

            if (!departmentExists)
            {
                return BadRequest("Invalid DepartmentId.");
            }

            var codeExists = await _context.Courses
                .AnyAsync(c => c.Code == dto.Code && c.Id != id && c.IsDeleted != true);

            if (codeExists)
            {
                return BadRequest("Course code already exists.");
            }

            course.Title = dto.Title;
            course.Code = dto.Code;
            course.Description = dto.Description;
            course.ImageUrl = dto.ImageUrl;
            course.CreditHours = dto.CreditHours;
            course.Price = dto.Price;
            course.Capacity = dto.Capacity;
            course.Level = dto.Level;
            course.IsPublished = dto.IsPublished;
            course.DepartmentId = dto.DepartmentId;
            course.UpdatedAt = DateTime.UtcNow;

            course.CourseTags.Clear();

            if (dto.CourseTagIds.Any())
            {
                var tags = await _context.CourseTags
                    .Where(t => dto.CourseTagIds.Contains(t.Id) && t.IsDeleted != true)
                    .ToListAsync();

                foreach (var tag in tags)
                {
                    course.CourseTags.Add(tag);
                }
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await ActiveCoursesQuery()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            course.IsDeleted = true;
            course.DeletedAt = DateTime.UtcNow;
            course.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<CourseDto?> BuildCourseDto(int id)
        {
            return await ActiveCoursesQuery()
                .Include(c => c.Department)
                .Include(c => c.CourseTags)
                .Where(c => c.Id == id)
                .Select(ProjectToCourseDto())
                .FirstOrDefaultAsync();
        }

        private IQueryable<Course> ActiveCoursesQuery()
        {
            return _context.Courses.Where(c => c.IsDeleted != true);
        }

        private IQueryable<Course> PublishedActiveCoursesQuery()
        {
            return ActiveCoursesQuery().Where(c => c.IsPublished);
        }

        private static System.Linq.Expressions.Expression<Func<Course, CourseDto>> ProjectToCourseDto()
        {
            return c => new CourseDto
            {
                Id = c.Id,
                Title = c.Title,
                Code = c.Code,
                Description = c.Description ?? string.Empty,
                CreditHours = c.CreditHours,
                Level = c.Level,
                Price = c.Price,
                DepartmentName = c.Department.Name,
                Tags = c.CourseTags.Select(t => t.Name).ToList()
            };
        }
    }
}
