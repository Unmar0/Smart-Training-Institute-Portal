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
            return View(await _context.CourseTags.ToListAsync());
        }

        // GET: CourseTags/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseTag = await _context.CourseTags
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
        public async Task<IActionResult> Create([Bind("Name,Id,CreatedAt,UpdatedAt,DeletedAt,IsDeleted")] CourseTag courseTag)
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

            var courseTag = await _context.CourseTags.FindAsync(id);
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
        public async Task<IActionResult> Edit(int id, [Bind("Name,Id,CreatedAt,UpdatedAt,DeletedAt,IsDeleted")] CourseTag courseTag)
        {
            if (id != courseTag.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(courseTag);
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

            var courseTag = await _context.CourseTags
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
            var courseTag = await _context.CourseTags.FindAsync(id);
            if (courseTag != null)
            {
                _context.CourseTags.Remove(courseTag);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Course tag deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool CourseTagExists(int id)
        {
            return _context.CourseTags.Any(e => e.Id == id);
        }
    }
}
