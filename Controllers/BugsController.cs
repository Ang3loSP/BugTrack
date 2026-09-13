using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BugTrack.Data;
using BugTrack.Models;
using BugTrack.Models.Enums;
using BugTrack.Services;
using BugTrack.ViewModels;

namespace BugTrack.Controllers
{
    public class BugsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BugStatusService _statusService;

        public BugsController(ApplicationDbContext context, BugStatusService statusService)
        {
            _context = context;
            _statusService = statusService;
        }

        public async Task<IActionResult> Index(
            BugStatus? statusFilter,
            BugPriority? priorityFilter,
            BugSeverity? severityFilter,
            string? searchTerm)
        {
            var query = _context.Bugs.AsQueryable();

            if (statusFilter.HasValue)
                query = query.Where(b => b.Status == statusFilter.Value);

            if (priorityFilter.HasValue)
                query = query.Where(b => b.Priority == priorityFilter.Value);

            if (severityFilter.HasValue)
                query = query.Where(b => b.Severity == severityFilter.Value);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(b => b.Title.Contains(searchTerm) ||
                                         b.Description.Contains(searchTerm));
            }

            var bugs = await query.OrderByDescending(b => b.CreatedDate).ToListAsync();

            var model = new BugListViewModel
            {
                Bugs = bugs,
                FilterStatus = statusFilter,
                FilterPriority = priorityFilter,
                FilterSeverity = severityFilter,
                SearchTerm = searchTerm,
                Statuses = Enum.GetValues(typeof(BugStatus)).Cast<BugStatus>().ToList(),
                Priorities = Enum.GetValues(typeof(BugPriority)).Cast<BugPriority>().ToList(),
                Severities = Enum.GetValues(typeof(BugSeverity)).Cast<BugSeverity>().ToList()
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var bug = await _context.Bugs
                .Include(b => b.LinkedTestRun)
                .ThenInclude(tr => tr!.TestCase)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bug == null)
                return NotFound();

            ViewBag.AvailableTransitions = _statusService.GetAvailableTransitions(bug.Status);

            return View(bug);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, BugStatus newStatus)
        {
            var bug = await _context.Bugs.FindAsync(id);
            if (bug == null)
                return NotFound();

            if (!_statusService.CanTransition(bug.Status, newStatus))
            {
                TempData["Error"] = $"Cannot change status from {bug.Status} to {newStatus}";
                return RedirectToAction(nameof(Details), new { id });
            }

            bug.Status = newStatus;
            bug.UpdatedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Bug status updated to {newStatus}";
            return RedirectToAction(nameof(Details), new { id });
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,StepsToReproduce,Severity,Priority,AssignedTo")] Bug bug)
        {
            if (ModelState.IsValid)
            {
                bug.CreatedDate = DateTime.UtcNow;
                bug.Status = BugStatus.New;

                _context.Add(bug);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Bug created successfully!";
                return RedirectToAction(nameof(Details), new { id = bug.Id });
            }
            return View(bug);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var bug = await _context.Bugs.FindAsync(id);
            if (bug == null)
                return NotFound();

            return View(bug);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,StepsToReproduce,Severity,Priority,AssignedTo")] Bug bug)
        {
            if (id != bug.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.Bugs.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
                    if (existing == null)
                        return NotFound();

                    bug.Status = existing.Status;
                    bug.CreatedDate = existing.CreatedDate;
                    bug.LinkedTestRunId = existing.LinkedTestRunId;
                    bug.UpdatedDate = DateTime.UtcNow;

                    _context.Update(bug);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Bug updated successfully!";
                    return RedirectToAction(nameof(Details), new { id = bug.Id });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BugExists(bug.Id))
                        return NotFound();
                    else
                        throw;
                }
            }
            return View(bug);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var bug = await _context.Bugs
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bug == null)
                return NotFound();

            ViewBag.LinkedRunCount = await _context.TestRuns.CountAsync(tr => tr.LinkedBugId == id.Value);

            return View(bug);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bug = await _context.Bugs.FindAsync(id);
            if (bug != null)
            {
                var linkedRunCount = await _context.TestRuns.CountAsync(tr => tr.LinkedBugId == id);
                if (linkedRunCount > 0)
                {
                    TempData["Error"] = "This bug is linked to test run(s) and cannot be deleted.";
                    return RedirectToAction(nameof(Index));
                }

                try
                {
                    _context.Bugs.Remove(bug);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Bug deleted successfully!";
                }
                catch (DbUpdateException)
                {
                    TempData["Error"] = "This bug is referenced by other records and could not be deleted.";
                    return RedirectToAction(nameof(Index));
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BugExists(int id)
        {
            return _context.Bugs.Any(e => e.Id == id);
        }
    }
}