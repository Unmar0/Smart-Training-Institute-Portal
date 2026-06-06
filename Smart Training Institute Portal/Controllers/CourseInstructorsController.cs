using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Smart_Training_Institute_Portal.Data;
using Smart_Training_Institute_Portal.Models;

namespace Smart_Training_Institute_Portal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CourseInstructorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CourseInstructorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CourseInstructors
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = ActiveCourseInstructorsQuery()
                .Include(c => c.Course)
                .Include(c => c.InstructorProfile);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: CourseInstructors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseInstructor = await ActiveCourseInstructorsQuery()
                .Include(c => c.Course)
                .Include(c => c.InstructorProfile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (courseInstructor == null)
            {
                return NotFound();
            }

            return View(courseInstructor);
        }

        // GET: CourseInstructors/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses.Where(c => c.IsDeleted != true), "Id", "Title");
            ViewData["InstructorProfileId"] = new SelectList(_context.InstructorProfiles.Where(i => i.IsDeleted != true), "Id", "FullName");
            return View();
        }

        // POST: CourseInstructors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseId,InstructorProfileId")] CourseInstructor courseInstructor)
        {
            await ValidateCourseInstructorSelectionAsync(courseInstructor);

            if (ModelState.IsValid)
            {
                _context.Add(courseInstructor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses.Where(c => c.IsDeleted != true), "Id", "Title", courseInstructor.CourseId);
            ViewData["InstructorProfileId"] = new SelectList(_context.InstructorProfiles.Where(i => i.IsDeleted != true), "Id", "FullName", courseInstructor.InstructorProfileId);
            return View(courseInstructor);
        }

        // GET: CourseInstructors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseInstructor = await ActiveCourseInstructorsQuery().FirstOrDefaultAsync(c => c.Id == id);
            if (courseInstructor == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Courses.Where(c => c.IsDeleted != true), "Id", "Title", courseInstructor.CourseId);
            ViewData["InstructorProfileId"] = new SelectList(_context.InstructorProfiles.Where(i => i.IsDeleted != true), "Id", "FullName", courseInstructor.InstructorProfileId);
            return View(courseInstructor);
        }

        // POST: CourseInstructors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CourseId,InstructorProfileId")] CourseInstructor courseInstructor)
        {
            if (id != courseInstructor.Id)
            {
                return NotFound();
            }

            await ValidateCourseInstructorSelectionAsync(courseInstructor, id);

            if (ModelState.IsValid)
            {
                try
                {
                    var existingCourseInstructor = await ActiveCourseInstructorsQuery()
                        .FirstOrDefaultAsync(c => c.Id == id);

                    if (existingCourseInstructor == null)
                    {
                        return NotFound();
                    }

                    existingCourseInstructor.CourseId = courseInstructor.CourseId;
                    existingCourseInstructor.InstructorProfileId = courseInstructor.InstructorProfileId;
                    existingCourseInstructor.UpdatedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseInstructorExists(courseInstructor.Id))
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
            ViewData["CourseId"] = new SelectList(_context.Courses.Where(c => c.IsDeleted != true), "Id", "Title", courseInstructor.CourseId);
            ViewData["InstructorProfileId"] = new SelectList(_context.InstructorProfiles.Where(i => i.IsDeleted != true), "Id", "FullName", courseInstructor.InstructorProfileId);
            return View(courseInstructor);
        }

        // GET: CourseInstructors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseInstructor = await ActiveCourseInstructorsQuery()
                .Include(c => c.Course)
                .Include(c => c.InstructorProfile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (courseInstructor == null)
            {
                return NotFound();
            }

            return View(courseInstructor);
        }

        // POST: CourseInstructors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var courseInstructor = await ActiveCourseInstructorsQuery()
                .FirstOrDefaultAsync(c => c.Id == id);
            if (courseInstructor == null)
            {
                return RedirectToAction(nameof(Index));
            }

            courseInstructor.IsDeleted = true;
            courseInstructor.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseInstructorExists(int id)
        {
            return ActiveCourseInstructorsQuery().Any(e => e.Id == id);
        }

        private IQueryable<CourseInstructor> ActiveCourseInstructorsQuery()
        {
            return _context.CourseInstructors.Where(c => c.IsDeleted != true);
        }

        private async Task ValidateCourseInstructorSelectionAsync(CourseInstructor courseInstructor, int? currentId = null)
        {
            if (courseInstructor.CourseId == null)
            {
                ModelState.AddModelError(nameof(CourseInstructor.CourseId), "Please select a course.");
                return;
            }

            var courseExists = await _context.Courses
                .AnyAsync(c => c.Id == courseInstructor.CourseId && c.IsDeleted != true);

            if (!courseExists)
            {
                ModelState.AddModelError(nameof(CourseInstructor.CourseId), "Selected course does not exist.");
            }

            var instructorExists = await _context.InstructorProfiles
                .AnyAsync(i => i.Id == courseInstructor.InstructorProfileId && i.IsDeleted != true);

            if (!instructorExists)
            {
                ModelState.AddModelError(nameof(CourseInstructor.InstructorProfileId), "Selected instructor does not exist.");
            }

            if (!ModelState.IsValid)
            {
                return;
            }

            var alreadyAssigned = await ActiveCourseInstructorsQuery()
                .AnyAsync(ci => ci.CourseId == courseInstructor.CourseId
                    && ci.InstructorProfileId == courseInstructor.InstructorProfileId
                    && ci.Id != currentId);

            if (alreadyAssigned)
            {
                ModelState.AddModelError(string.Empty, "This instructor is already assigned to the selected course.");
            }
        }
    }
}
