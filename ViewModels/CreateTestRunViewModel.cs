using System.ComponentModel.DataAnnotations;
using BugTrack.Models.Enums;

namespace BugTrack.ViewModels
{
    public class CreateTestRunViewModel
    {
        public int TestCaseId { get; set; }
        public string TestCaseTitle { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string ExecutedBy { get; set; } = string.Empty;

        [StringLength(500)]
        public string? ActualResult { get; set; }

        [Required]
        public TestResult Result { get; set; } = TestResult.NotRun;

        [StringLength(500)]
        public string? Notes { get; set; }

        public bool CreateBugFromFailure { get; set; }
        public string? BugTitle { get; set; }
        public string? BugDescription { get; set; }
        public string? StepsToReproduce { get; set; }
        public BugSeverity BugSeverity { get; set; } = BugSeverity.Minor;
        public BugPriority BugPriority { get; set; } = BugPriority.Medium;
    }
}