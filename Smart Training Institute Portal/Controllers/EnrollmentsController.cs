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
using Microsoft.AspNetCore.Identity;

namespace Smart_Training_Institute_Portal.Controllers
{
    [Authorize]
    public class EnrollmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public EnrollmentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Enrollments
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> Index()
        {
            var enrollments = _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.StudentProfile)
                .OrderByDescending(e => e.EnrollmentDate);

            return View(await enrollments.ToListAsync());
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MyCourses()
        {
            var userId = _userManager.GetUserId(User);

            var studentProfile = await _context.StudentProfiles
                .FirstOrDefaultAsync(s => s.ApplicationUserId == userId);

            if (studentProfile == null)
            {
                return NotFound();
            }

            var enrollments = await _context.Enrollments
                .Include(e => e.Course)
                .ThenInclude(c => c.Department)
                .Where(e => e.StudentProfileId == studentProfile.Id)
                .ToListAsync();

            return View(enrollments);
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MyGrades()
        {
            var userId = _userManager.GetUserId(User);

            var studentProfile = await _context.StudentProfiles
                .FirstOrDefaultAsync(s => s.ApplicationUserId == userId);

            if (studentProfile == null)
            {
                return NotFound();
            }

            var enrollments = await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.StudentProfileId == studentProfile.Id)
                .ToListAsync();

            return View(enrollments);
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> PerformanceSummary()
        {
            var userId = _userManager.GetUserId(User);

            var studentProfile = await _context.StudentProfiles
                .FirstOrDefaultAsync(s => s.ApplicationUserId == userId);

            if (studentProfile == null)
            {
                return NotFound();
            }

            var completedEnrollments = await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.StudentProfileId == studentProfile.Id && e.Mark != null)
                .ToListAsync();

            var averageMark = completedEnrollments.Any()
                ? completedEnrollments.Average(e => e.Mark!.Value)
                : 0;

            ViewBag.AverageMark = averageMark;
            ViewBag.TotalCourses = completedEnrollments.Count;

            return View(completedEnrollments);
        }

        // GET: Enrollments/Details/5
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.StudentProfile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // GET: Enrollments/Create
        [Authorize(Roles = "Admin,Instructor")]
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Title");
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "FullName");
            return View();
        }

        // POST: Enrollments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Instructor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EnrollmentDate,Mark,Grade,Status,StudentProfileId,CourseId,Id,CreatedAt,UpdatedAt,DeletedAt,IsDeleted")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(enrollment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Student enrolled successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Code", enrollment.CourseId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "FullName", enrollment.StudentProfileId);
            return View(enrollment);
        }

        // GET: Enrollments/Edit/5
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Code", enrollment.CourseId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "FullName", enrollment.StudentProfileId);
            return View(enrollment);
        }

        // POST: Enrollments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Instructor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EnrollmentDate,Mark,Grade,Status,StudentProfileId,CourseId,Id,CreatedAt,UpdatedAt,DeletedAt,IsDeleted")] Enrollment enrollment)
        {
            if (id != enrollment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var oldEnrollment = await _context.Enrollments
                        .AsNoTracking()
                        .FirstOrDefaultAsync(e => e.Id == id);

                    if (oldEnrollment == null)
                    {
                        return NotFound();
                    }

                    var markChanged = oldEnrollment.Mark != enrollment.Mark;
                    var gradeChanged = oldEnrollment.Grade != enrollment.Grade;

                    if (markChanged || gradeChanged)
                    {
                        var auditLog = new GradeAuditLog
                        {
                            EnrollmentId = enrollment.Id,
                            OldMark = oldEnrollment.Mark,
                            NewMark = enrollment.Mark,
                            OldGrade = oldEnrollment.Grade,
                            NewGrade = enrollment.Grade,
                            ChangedBy = _userManager.GetUserId(User) ?? string.Empty,
                            Notes = "Grade information updated"
                        };

                        _context.GradeAuditLogs.Add(auditLog);
                    }

                    enrollment.UpdatedAt = DateTime.UtcNow;

                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Enrollment and mark updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(enrollment.Id))
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
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Code", enrollment.CourseId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "FullName", enrollment.StudentProfileId);
            return View(enrollment);
        }

        // GET: Enrollments/Delete/5
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.StudentProfile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // POST: Enrollments/Delete/5
        [Authorize(Roles = "Admin,Instructor")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment != null)
            {
                _context.Enrollments.Remove(enrollment);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Enrollment deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool EnrollmentExists(int id)
        {
            return _context.Enrollments.Any(e => e.Id == id);
        }
    }
}
