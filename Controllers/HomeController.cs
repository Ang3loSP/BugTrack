using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BugTrack.Data;
using BugTrack.ViewModels;
using BugTrack.Models.Enums;

namespace BugTrack.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var bugs = await _context.Bugs.ToListAsync();
            var testCases = await _context.TestCases
                .Include(tc => tc.TestRuns)
                .ToListAsync();
            var testRuns = await _context.TestRuns
                .Include(tr => tr.TestCase)
                .OrderByDescending(tr => tr.ExecutedDate)
                .Take(10)
                .ToListAsync();

            var dashboard = new DashboardViewModel
            {
                TotalBugs = bugs.Count,
                OpenBugs = bugs.Count(b => b.Status != BugStatus.Closed),
                BugsByPriorityLow = bugs.Count(b => b.Priority == BugPriority.Low),
                BugsByPriorityMedium = bugs.Count(b => b.Priority == BugPriority.Medium),
                BugsByPriorityHigh = bugs.Count(b => b.Priority == BugPriority.High),
                BugsByPriorityCritical = bugs.Count(b => b.Priority == BugPriority.Critical),
                TotalTestCases = testCases.Count,
                TestCasesByPriorityLow = testCases.Count(tc => tc.Priority == TestPriority.Low),
                TestCasesByPriorityMedium = testCases.Count(tc => tc.Priority == TestPriority.Medium),
                TestCasesByPriorityHigh = testCases.Count(tc => tc.Priority == TestPriority.High),
                TotalTestRuns = await _context.TestRuns.CountAsync(),
                RecentTestRuns = testRuns,
                RecentBugs = bugs.OrderByDescending(b => b.CreatedDate).Take(10).ToList()
            };

            foreach (BugStatus status in Enum.GetValues(typeof(BugStatus)))
            {
                dashboard.BugsByStatus[status] = bugs.Count(b => b.Status == status);
            }

            foreach (TestResult result in Enum.GetValues(typeof(TestResult)))
            {
                var count = await _context.TestRuns.CountAsync(tr => tr.Result == result);
                dashboard.TestRunsByResult[result] = count;
            }

            var total = dashboard.TotalTestRuns;
            if (total > 0)
            {
                var passed = dashboard.TestRunsByResult.GetValueOrDefault(TestResult.Pass);
                dashboard.PassRate = (double)passed / total * 100;
            }

            return View(dashboard);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}