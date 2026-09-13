using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BugTrack.Data;
using BugTrack.Models;
using BugTrack.Models.Enums;
using BugTrack.ViewModels;

namespace BugTrack.Controllers
{
    public class TestRunsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TestRunsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Create(int testCaseId)
        {
            var testCase = await _context.TestCases.FindAsync(testCaseId);
            if (testCase == null)
                return NotFound();

            var model = new CreateTestRunViewModel
            {
                TestCaseId = testCaseId,
                TestCaseTitle = testCase.Title
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTestRunViewModel model)
        {
            if (ModelState.IsValid)
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

                var testRun = new TestRun
                {
                    TestCaseId = model.TestCaseId,
                    ExecutedDate = DateTime.UtcNow,
                    ExecutedBy = model.ExecutedBy,
                    ActualResult = model.ActualResult,
                    Result = model.Result,
                    Notes = model.Notes
                };

                if (model.Result == TestResult.Fail && model.CreateBugFromFailure)
                {
                    var bug = new Bug
                    {
                        Title = model.BugTitle ?? $"Bug from failed test: {model.TestCaseTitle}",
                        Description = model.BugDescription ?? "Bug found during test execution.",
                        StepsToReproduce = model.StepsToReproduce ?? "",
                        Severity = model.BugSeverity,
                        Priority = model.BugPriority,
                        Status = BugStatus.New,
                        CreatedDate = DateTime.UtcNow,
                        LinkedTestRunId = null
                    };

                    _context.Bugs.Add(bug);
                    await _context.SaveChangesAsync();

                    testRun.LinkedBugId = bug.Id;
                }

                _context.TestRuns.Add(testRun);
                await _context.SaveChangesAsync();

                if (testRun.LinkedBugId.HasValue)
                {
                    var bug = await _context.Bugs.FindAsync(testRun.LinkedBugId.Value);
                    if (bug != null)
                    {
                        bug.LinkedTestRunId = testRun.Id;
                        await _context.SaveChangesAsync();
                    }
                }

                await transaction.CommitAsync();

                TempData["Success"] = "Test execution recorded successfully!";
                return RedirectToAction("Details", "TestCases", new { id = model.TestCaseId });
            }

            var testCase = await _context.TestCases.FindAsync(model.TestCaseId);
            if (testCase != null)
                model.TestCaseTitle = testCase.Title;

            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var testRun = await _context.TestRuns
                .Include(tr => tr.TestCase)
                .Include(tr => tr.LinkedBug)
                .FirstOrDefaultAsync(tr => tr.Id == id);

            if (testRun == null)
                return NotFound();

            return View(testRun);
        }
    }
}