using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Smart_Training_Institute_Portal.Data;
using Smart_Training_Institute_Portal.Models;

namespace Smart_Training_Institute_Portal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StudentProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentProfilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: StudentProfiles
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = ActiveStudentProfilesQuery()
                .Include(s => s.ApplicationUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: StudentProfiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentProfile = await ActiveStudentProfilesQuery()
                .Include(s => s.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentProfile == null)
            {
                return NotFound();
            }

            return View(studentProfile);
        }

        // GET: StudentProfiles/Create
        public async Task<IActionResult> Create()
        {
            await PopulateApplicationUsersAsync();
            return View();
        }

        // POST: StudentProfiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FullName,StudentNumber,ImageUrl,DateofBirth,ApplicationUserId,Id")] StudentProfile studentProfile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(studentProfile);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Student profile created successfully.";
                return RedirectToAction(nameof(Index));
            }
            await PopulateApplicationUsersAsync(studentProfile.ApplicationUserId);
            return View(studentProfile);
        }

        // GET: StudentProfiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentProfile = await ActiveStudentProfilesQuery().FirstOrDefaultAsync(s => s.Id == id);
            if (studentProfile == null)
            {
                return NotFound();
            }
            await PopulateApplicationUsersAsync(studentProfile.ApplicationUserId);
            return View(studentProfile);
        }

        // POST: StudentProfiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FullName,StudentNumber,ImageUrl,DateofBirth,ApplicationUserId,Id")] StudentProfile studentProfile)
        {
            if (id != studentProfile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingStudentProfile = await ActiveStudentProfilesQuery()
                        .FirstOrDefaultAsync(s => s.Id == id);

                    if (existingStudentProfile == null)
                    {
                        return NotFound();
                    }

                    existingStudentProfile.FullName = studentProfile.FullName;
                    existingStudentProfile.StudentNumber = studentProfile.StudentNumber;
                    existingStudentProfile.ImageUrl = studentProfile.ImageUrl;
                    existingStudentProfile.DateofBirth = studentProfile.DateofBirth;
                    existingStudentProfile.ApplicationUserId = studentProfile.ApplicationUserId;
                    existingStudentProfile.UpdatedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Student profile updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentProfileExists(studentProfile.Id))
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
            await PopulateApplicationUsersAsync(studentProfile.ApplicationUserId);
            return View(studentProfile);
        }

        // GET: StudentProfiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentProfile = await ActiveStudentProfilesQuery()
                .Include(s => s.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentProfile == null)
            {
                return NotFound();
            }

            return View(studentProfile);
        }

        // POST: StudentProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studentProfile = await ActiveStudentProfilesQuery()
                .FirstOrDefaultAsync(s => s.Id == id);
            if (studentProfile == null)
            {
                return RedirectToAction(nameof(Index));
            }

            studentProfile.IsDeleted = true;
            studentProfile.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Student profile deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool StudentProfileExists(int id)
        {
            return ActiveStudentProfilesQuery().Any(e => e.Id == id);
        }

        private IQueryable<StudentProfile> ActiveStudentProfilesQuery()
        {
            return _context.StudentProfiles
                .Where(s => s.IsDeleted != true);
        }

        private async Task PopulateApplicationUsersAsync(string? selectedUserId = null)
        {
            var availableUsers = await _context.Users
                .Where(u => u.StudentProfile == null || u.Id == selectedUserId)
                .OrderBy(u => u.Email)
                .Select(u => new
                {
                    u.Id,
                    DisplayName = string.IsNullOrWhiteSpace(u.Email) ? u.Id : u.Email
                })
                .ToListAsync();

            ViewData["ApplicationUserId"] = new SelectList(availableUsers, "Id", "DisplayName", selectedUserId);
        }
    }
}
