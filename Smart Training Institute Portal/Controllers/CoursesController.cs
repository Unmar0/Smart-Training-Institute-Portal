using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Smart_Training_Institute_Portal.Data;
using Smart_Training_Institute_Portal.Models;

namespace Smart_Training_Institute_Portal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CoursesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Catalog()
        {
            var courses = await _context.Courses
                .Include(c => c.Department)
                .Include(c => c.CourseTags)
                .Where(c => c.IsPublished && c.IsDeleted != true)
                .ToListAsync();

            return View(courses);
        }

        [AllowAnonymous]
        public IActionResult LiveCatalog()
        {
            return View();
        }

        // GET: Courses
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = ActiveCoursesQuery()
                .Include(c => c.Department);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await ActiveCoursesQuery()
                .Include(c => c.Department)
                .Include(c => c.CourseTags)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Courses/Create
        public async Task<IActionResult> Create()
        {
            await PopulateCourseFormSelectionsAsync();
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Code,Description,ImageUrl,CreditHours,Price,Capacity,Level,IsPublished,DepartmentId")] Course course, int? selectedTagId)
        {
            if (ModelState.IsValid)
            {
                await ApplySelectedTagAsync(course, selectedTagId);
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            await PopulateCourseFormSelectionsAsync(course.DepartmentId, selectedTagId);
            return View(course);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await ActiveCoursesQuery()
                .Include(c => c.CourseTags)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (course == null)
            {
                return NotFound();
            }
            await PopulateCourseFormSelectionsAsync(course.DepartmentId, course.CourseTags.Select(t => t.Id).FirstOrDefault());
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Title,Code,Description,ImageUrl,CreditHours,Price,Capacity,Level,IsPublished,DepartmentId,Id")] Course course, int? selectedTagId)
        {
            if (id != course.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingCourse = await ActiveCoursesQuery()
                        .Include(c => c.CourseTags)
                        .FirstOrDefaultAsync(c => c.Id == id);

                    if (existingCourse == null)
                    {
                        return NotFound();
                    }

                    existingCourse.Title = course.Title;
                    existingCourse.Code = course.Code;
                    existingCourse.Description = course.Description;
                    existingCourse.ImageUrl = course.ImageUrl;
                    existingCourse.CreditHours = course.CreditHours;
                    existingCourse.Price = course.Price;
                    existingCourse.Capacity = course.Capacity;
                    existingCourse.Level = course.Level;
                    existingCourse.IsPublished = course.IsPublished;
                    existingCourse.DepartmentId = course.DepartmentId;
                    existingCourse.UpdatedAt = DateTime.UtcNow;

                    await ApplySelectedTagAsync(existingCourse, selectedTagId);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            await PopulateCourseFormSelectionsAsync(course.DepartmentId, selectedTagId);
            return View(course);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await ActiveCoursesQuery()
                .Include(c => c.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await ActiveCoursesQuery()
                .FirstOrDefaultAsync(c => c.Id == id);
            if (course == null)
            {
                return RedirectToAction(nameof(Index));
            }

            course.IsDeleted = true;
            course.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return ActiveCoursesQuery().Any(e => e.Id == id);
        }

        private IQueryable<Course> ActiveCoursesQuery()
        {
            return _context.Courses.Where(c => c.IsDeleted != true);
        }

        private async Task PopulateCourseFormSelectionsAsync(int? selectedDepartmentId = null, int? selectedTagId = null)
        {
            ViewData["DepartmentId"] = new SelectList(
                await _context.Departments
                    .Where(d => d.IsDeleted != true)
                    .OrderBy(d => d.Name)
                    .ToListAsync(),
                "Id",
                "Name",
                selectedDepartmentId);

            ViewData["CourseTags"] = new SelectList(
                await _context.CourseTags
                    .Where(t => t.IsDeleted != true)
                    .OrderBy(t => t.Name)
                    .ToListAsync(),
                "Id",
                "Name",
                selectedTagId);
        }

        private async Task ApplySelectedTagAsync(Course course, int? selectedTagId)
        {
            course.CourseTags.Clear();

            if (selectedTagId == null)
            {
                return;
            }

            var tag = await _context.CourseTags
                .FirstOrDefaultAsync(t => t.Id == selectedTagId && t.IsDeleted != true);

            if (tag != null)
            {
                course.CourseTags.Add(tag);
            }
        }
    }
}
