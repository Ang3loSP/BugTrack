using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BugTrack.Data;
using BugTrack.Models;
using BugTrack.Models.Enums;
using BugTrack.ViewModels;

namespace BugTrack.Controllers
{
    public class TestCasesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TestCasesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(
            string? filterModule,
            TestPriority? filterPriority,
            string? searchTerm)
        {
            var query = _context.TestCases
                .Include(tc => tc.TestRuns)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filterModule))
                query = query.Where(tc => tc.Module != null && tc.Module.Contains(filterModule));

            if (filterPriority.HasValue)
                query = query.Where(tc => tc.Priority == filterPriority.Value);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(tc => tc.Title.Contains(searchTerm) ||
                                         tc.Steps.Contains(searchTerm) ||
                                         tc.ExpectedResult.Contains(searchTerm));
            }

            var testCases = await query.OrderByDescending(tc => tc.CreatedDate).ToListAsync();

            var modules = await _context.TestCases
                .Where(tc => tc.Module != null)
                .Select(tc => tc.Module!)
                .Distinct()
                .ToListAsync();

            var model = new TestCaseListViewModel
            {
                TestCases = testCases,
                FilterModule = filterModule,
                FilterPriority = filterPriority,
                SearchTerm = searchTerm,
                Modules = modules,
                Priorities = Enum.GetValues(typeof(TestPriority)).Cast<TestPriority>().ToList()
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var testCase = await _context.TestCases
                .Include(tc => tc.TestRuns)
                .ThenInclude(tr => tr.LinkedBug)
                .FirstOrDefaultAsync(tc => tc.Id == id);

            if (testCase == null)
                return NotFound();

            return View(testCase);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Module,Preconditions,Steps,ExpectedResult,Priority")] TestCase testCase)
        {
            if (ModelState.IsValid)
            {
                testCase.CreatedDate = DateTime.UtcNow;
                _context.Add(testCase);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Test case created successfully!";
                return RedirectToAction(nameof(Details), new { id = testCase.Id });
            }
            return View(testCase);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var testCase = await _context.TestCases.FindAsync(id);
            if (testCase == null)
                return NotFound();

            return View(testCase);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Module,Preconditions,Steps,ExpectedResult,Priority")] TestCase testCase)
        {
            if (id != testCase.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.TestCases.AsNoTracking().FirstOrDefaultAsync(tc => tc.Id == id);
                    if (existing == null)
                        return NotFound();

                    testCase.CreatedDate = existing.CreatedDate;
                    testCase.UpdatedDate = DateTime.UtcNow;

                    _context.Update(testCase);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Test case updated successfully!";
                    return RedirectToAction(nameof(Details), new { id = testCase.Id });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TestCaseExists(testCase.Id))
                        return NotFound();
                    else
                        throw;
                }
            }
            return View(testCase);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var testCase = await _context.TestCases
                .Include(tc => tc.TestRuns)
                .FirstOrDefaultAsync(tc => tc.Id == id);

            if (testCase == null)
                return NotFound();

            ViewBag.TestRunCount = testCase.TestRuns.Count;

            return View(testCase);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var testCase = await _context.TestCases.FindAsync(id);
            if (testCase != null)
            {
                var runCount = await _context.TestRuns.CountAsync(tr => tr.TestCaseId == id);
                if (runCount > 0)
                {
                    TempData["Error"] = "This test case has execution history and cannot be deleted.";
                    return RedirectToAction(nameof(Index));
                }

                try
                {
                    _context.TestCases.Remove(testCase);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Test case deleted successfully!";
                }
                catch (DbUpdateException)
                {
                    TempData["Error"] = "This test case is referenced by other records and could not be deleted.";
                    return RedirectToAction(nameof(Index));
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TestCaseExists(int id)
        {
            return _context.TestCases.Any(e => e.Id == id);
        }
    }
}