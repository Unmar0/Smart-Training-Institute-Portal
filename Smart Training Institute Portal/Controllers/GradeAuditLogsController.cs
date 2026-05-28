using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Training_Institute_Portal.Data;

namespace Smart_Training_Institute_Portal.Controllers
{
    [Authorize(Roles = "Admin,Instructor")]
    public class GradeAuditLogsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GradeAuditLogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var logs = await _context.GradeAuditLogs
                .Include(g => g.Enrollment)
                    .ThenInclude(e => e.StudentProfile)
                .Include(g => g.Enrollment)
                    .ThenInclude(e => e.Course)
                .Include(g => g.ChangedByUser)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();

            return View(logs);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var log = await _context.GradeAuditLogs
                .Include(g => g.Enrollment)
                    .ThenInclude(e => e.StudentProfile)
                .Include(g => g.Enrollment)
                    .ThenInclude(e => e.Course)
                .Include(g => g.ChangedByUser)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (log == null)
            {
                return NotFound();
            }

            return View(log);
        }
    }
}