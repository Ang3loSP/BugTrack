using BugTrack.Models;
using BugTrack.Models.Enums;

namespace BugTrack.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalBugs { get; set; }
        public int OpenBugs { get; set; }
        public int BugsByPriorityLow { get; set; }
        public int BugsByPriorityMedium { get; set; }
        public int BugsByPriorityHigh { get; set; }
        public int BugsByPriorityCritical { get; set; }
        public Dictionary<BugStatus, int> BugsByStatus { get; set; } = new();

        public int TotalTestCases { get; set; }
        public int TestCasesByPriorityLow { get; set; }
        public int TestCasesByPriorityMedium { get; set; }
        public int TestCasesByPriorityHigh { get; set; }

        public Dictionary<TestResult, int> TestRunsByResult { get; set; } = new();
        public int TotalTestRuns { get; set; }
        public double PassRate { get; set; }

        public List<TestRun> RecentTestRuns { get; set; } = new();
        public List<Bug> RecentBugs { get; set; } = new();
    }
}