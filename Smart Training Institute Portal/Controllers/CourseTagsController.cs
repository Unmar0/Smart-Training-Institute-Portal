using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Training_Institute_Portal.Data;
using Smart_Training_Institute_Portal.Models;

namespace Smart_Training_Institute_Portal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CourseTagsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CourseTagsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CourseTags
        public async Task<IActionResult> Index()
        {
            return View(await ActiveCourseTagsQuery().ToListAsync());
        }

        // GET: CourseTags/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseTag = await ActiveCourseTagsQuery()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (courseTag == null)
            {
                return NotFound();
            }

            return View(courseTag);
        }

        // GET: CourseTags/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CourseTags/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Id")] CourseTag courseTag)
        {
            if (ModelState.IsValid)
            {
                _context.Add(courseTag);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Course tag created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(courseTag);
        }

        // GET: CourseTags/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseTag = await ActiveCourseTagsQuery().FirstOrDefaultAsync(t => t.Id == id);
            if (courseTag == null)
            {
                return NotFound();
            }
            return View(courseTag);
        }

        // POST: CourseTags/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Id")] CourseTag courseTag)
        {
            if (id != courseTag.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingCourseTag = await ActiveCourseTagsQuery()
                        .FirstOrDefaultAsync(t => t.Id == id);

                    if (existingCourseTag == null)
                    {
                        return NotFound();
                    }

                    existingCourseTag.Name = courseTag.Name;
                    existingCourseTag.UpdatedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Course tag updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseTagExists(courseTag.Id))
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
            return View(courseTag);
        }

        // GET: CourseTags/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseTag = await ActiveCourseTagsQuery()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (courseTag == null)
            {
                return NotFound();
            }

            return View(courseTag);
        }

        // POST: CourseTags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var courseTag = await ActiveCourseTagsQuery()
                .FirstOrDefaultAsync(t => t.Id == id);
            if (courseTag == null)
            {
                return RedirectToAction(nameof(Index));
            }

            courseTag.IsDeleted = true;
            courseTag.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Course tag deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool CourseTagExists(int id)
        {
            return ActiveCourseTagsQuery().Any(e => e.Id == id);
        }

        private IQueryable<CourseTag> ActiveCourseTagsQuery()
        {
            return _context.CourseTags
                .Where(t => t.IsDeleted != true)
                .OrderBy(t => t.Name);
        }
    }
}
