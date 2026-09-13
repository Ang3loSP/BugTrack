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
            var totalBugs = await _context.Bugs.CountAsync();
            var openBugs = await _context.Bugs.CountAsync(b => b.Status != BugStatus.Closed);

            var bugsByPriority = await _context.Bugs
                .GroupBy(b => b.Priority)
                .Select(g => new { g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.Key, g => g.Count);

            var bugsByStatus = await _context.Bugs
                .GroupBy(b => b.Status)
                .Select(g => new { g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.Key, g => g.Count);

            var totalTestCases = await _context.TestCases.CountAsync();

            var testCasesByPriority = await _context.TestCases
                .GroupBy(tc => tc.Priority)
                .Select(g => new { g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.Key, g => g.Count);

            var totalTestRuns = await _context.TestRuns.CountAsync();

            var runsByResult = await _context.TestRuns
                .GroupBy(tr => tr.Result)
                .Select(g => new { g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.Key, g => g.Count);

            var passCount = runsByResult.GetValueOrDefault(TestResult.Pass);
            var failCount = runsByResult.GetValueOrDefault(TestResult.Fail);
            var executed = passCount + failCount;
            var passRate = executed > 0 ? (double)passCount / executed * 100 : 0;

            var recentTestRuns = await _context.TestRuns
                .Include(tr => tr.TestCase)
                .OrderByDescending(tr => tr.ExecutedDate)
                .Take(10)
                .ToListAsync();

            var recentBugs = await _context.Bugs
                .OrderByDescending(b => b.CreatedDate)
                .Take(10)
                .ToListAsync();

            var dashboard = new DashboardViewModel
            {
                TotalBugs = totalBugs,
                OpenBugs = openBugs,
                BugsByPriorityLow = bugsByPriority.GetValueOrDefault(BugPriority.Low),
                BugsByPriorityMedium = bugsByPriority.GetValueOrDefault(BugPriority.Medium),
                BugsByPriorityHigh = bugsByPriority.GetValueOrDefault(BugPriority.High),
                BugsByPriorityCritical = bugsByPriority.GetValueOrDefault(BugPriority.Critical),
                TotalTestCases = totalTestCases,
                TestCasesByPriorityLow = testCasesByPriority.GetValueOrDefault(TestPriority.Low),
                TestCasesByPriorityMedium = testCasesByPriority.GetValueOrDefault(TestPriority.Medium),
                TestCasesByPriorityHigh = testCasesByPriority.GetValueOrDefault(TestPriority.High),
                TotalTestRuns = totalTestRuns,
                PassRate = passRate,
                RecentTestRuns = recentTestRuns,
                RecentBugs = recentBugs
            };

            foreach (BugStatus status in Enum.GetValues(typeof(BugStatus)))
            {
                dashboard.BugsByStatus[status] = bugsByStatus.GetValueOrDefault(status);
            }

            foreach (TestResult result in Enum.GetValues(typeof(TestResult)))
            {
                dashboard.TestRunsByResult[result] = runsByResult.GetValueOrDefault(result);
            }

            return View(dashboard);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}