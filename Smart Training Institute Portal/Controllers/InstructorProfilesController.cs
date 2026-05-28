using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Smart_Training_Institute_Portal.Data;
using Smart_Training_Institute_Portal.Models;
using Microsoft.AspNetCore.Authorization;

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
            var applicationDbContext = _context.InstructorProfiles.Include(i => i.ApplicationUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: InstructorProfiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructorProfile = await _context.InstructorProfiles
                .Include(i => i.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (instructorProfile == null)
            {
                return NotFound();
            }

            return View(instructorProfile);
        }

        // GET: InstructorProfiles/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: InstructorProfiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InstructorNumber,FullName,Specialization,Bio,ImageUrl,OfficeNumber,ApplicationUserId,Id,CreatedAt,UpdatedAt,DeletedAt,IsDeleted")] InstructorProfile instructorProfile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(instructorProfile);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Instructor profile created successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", instructorProfile.ApplicationUserId);
            return View(instructorProfile);
        }

        // GET: InstructorProfiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructorProfile = await _context.InstructorProfiles.FindAsync(id);
            if (instructorProfile == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", instructorProfile.ApplicationUserId);
            return View(instructorProfile);
        }

        // POST: InstructorProfiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InstructorNumber,FullName,Specialization,Bio,ImageUrl,OfficeNumber,ApplicationUserId,Id,CreatedAt,UpdatedAt,DeletedAt,IsDeleted")] InstructorProfile instructorProfile)
        {
            if (id != instructorProfile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(instructorProfile);
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
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", instructorProfile.ApplicationUserId);
            return View(instructorProfile);
        }

        // GET: InstructorProfiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructorProfile = await _context.InstructorProfiles
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
            var instructorProfile = await _context.InstructorProfiles.FindAsync(id);
            if (instructorProfile != null)
            {
                _context.InstructorProfiles.Remove(instructorProfile);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Instructor profile deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool InstructorProfileExists(int id)
        {
            return _context.InstructorProfiles.Any(e => e.Id == id);
        }
    }
}
