using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Smart_Training_Institute_Portal.Data;
using Smart_Training_Institute_Portal.Models;

namespace Smart_Training_Institute_Portal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class InstructorProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InstructorProfilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: InstructorProfiles
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = ActiveInstructorProfilesQuery()
                .Include(i => i.ApplicationUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: InstructorProfiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructorProfile = await ActiveInstructorProfilesQuery()
                .Include(i => i.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (instructorProfile == null)
            {
                return NotFound();
            }

            return View(instructorProfile);
        }

        // GET: InstructorProfiles/Create
        public async Task<IActionResult> Create()
        {
            await PopulateApplicationUsersAsync();
            return View();
        }

        // POST: InstructorProfiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InstructorNumber,FullName,Specialization,Bio,ImageUrl,OfficeNumber,ApplicationUserId,Id")] InstructorProfile instructorProfile)
        {
            await ValidateApplicationUserSelectionAsync(instructorProfile.ApplicationUserId);

            if (ModelState.IsValid)
            {
                _context.Add(instructorProfile);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Instructor profile created successfully.";
                return RedirectToAction(nameof(Index));
            }

            await PopulateApplicationUsersAsync(instructorProfile.ApplicationUserId);
            return View(instructorProfile);
        }

        // GET: InstructorProfiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructorProfile = await ActiveInstructorProfilesQuery().FirstOrDefaultAsync(i => i.Id == id);
            if (instructorProfile == null)
            {
                return NotFound();
            }
            await PopulateApplicationUsersAsync(instructorProfile.ApplicationUserId);
            return View(instructorProfile);
        }

        // POST: InstructorProfiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InstructorNumber,FullName,Specialization,Bio,ImageUrl,OfficeNumber,ApplicationUserId,Id")] InstructorProfile instructorProfile)
        {
            if (id != instructorProfile.Id)
            {
                return NotFound();
            }

            await ValidateApplicationUserSelectionAsync(instructorProfile.ApplicationUserId, instructorProfile.Id);

            if (ModelState.IsValid)
            {
                try
                {
                    var existingInstructorProfile = await ActiveInstructorProfilesQuery()
                        .FirstOrDefaultAsync(i => i.Id == id);

                    if (existingInstructorProfile == null)
                    {
                        return NotFound();
                    }

                    existingInstructorProfile.InstructorNumber = instructorProfile.InstructorNumber;
                    existingInstructorProfile.FullName = instructorProfile.FullName;
                    existingInstructorProfile.Specialization = instructorProfile.Specialization;
                    existingInstructorProfile.Bio = instructorProfile.Bio;
                    existingInstructorProfile.ImageUrl = instructorProfile.ImageUrl;
                    existingInstructorProfile.OfficeNumber = instructorProfile.OfficeNumber;
                    existingInstructorProfile.ApplicationUserId = instructorProfile.ApplicationUserId;
                    existingInstructorProfile.UpdatedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Instructor profile updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstructorProfileExists(instructorProfile.Id))
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
            await PopulateApplicationUsersAsync(instructorProfile.ApplicationUserId);
            return View(instructorProfile);
        }

        // GET: InstructorProfiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructorProfile = await ActiveInstructorProfilesQuery()
                .Include(i => i.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (instructorProfile == null)
            {
                return NotFound();
            }

            return View(instructorProfile);
        }

        // POST: InstructorProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instructorProfile = await ActiveInstructorProfilesQuery()
                .FirstOrDefaultAsync(i => i.Id == id);
            if (instructorProfile == null)
            {
                return RedirectToAction(nameof(Index));
            }

            instructorProfile.IsDeleted = true;
            instructorProfile.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Instructor profile deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool InstructorProfileExists(int id)
        {
            return ActiveInstructorProfilesQuery().Any(e => e.Id == id);
        }

        private IQueryable<InstructorProfile> ActiveInstructorProfilesQuery()
        {
            return _context.InstructorProfiles
                .Where(i => i.IsDeleted != true);
        }

        private async Task PopulateApplicationUsersAsync(string? selectedUserId = null)
        {
            var availableUsers = await _context.Users
                .Where(u => u.InstructorProfile == null || u.Id == selectedUserId)
                .OrderBy(u => u.Email)
                .Select(u => new
                {
                    u.Id,
                    DisplayName = string.IsNullOrWhiteSpace(u.Email) ? u.Id : u.Email
                })
                .ToListAsync();

            ViewData["ApplicationUserId"] = new SelectList(availableUsers, "Id", "DisplayName", selectedUserId);
        }

        private async Task ValidateApplicationUserSelectionAsync(string? applicationUserId, int? currentInstructorProfileId = null)
        {
            if (string.IsNullOrWhiteSpace(applicationUserId))
            {
                ModelState.AddModelError(nameof(InstructorProfile.ApplicationUserId), "Please select a user account.");
                return;
            }

            var userExists = await _context.Users.AnyAsync(u => u.Id == applicationUserId);
            if (!userExists)
            {
                ModelState.AddModelError(nameof(InstructorProfile.ApplicationUserId), "Selected user account does not exist.");
                return;
            }

            var alreadyAssigned = await _context.InstructorProfiles
                .AnyAsync(i => i.ApplicationUserId == applicationUserId
                    && i.Id != currentInstructorProfileId
                    && i.IsDeleted != true);

            if (alreadyAssigned)
            {
                ModelState.AddModelError(nameof(InstructorProfile.ApplicationUserId), "This user already has an instructor profile.");
            }
        }
    }
}
