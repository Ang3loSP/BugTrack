using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BugTrack.Models.Enums;

namespace BugTrack.Models
{
    public class TestRun
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TestCaseId { get; set; }

        [ForeignKey("TestCaseId")]
        public virtual TestCase TestCase { get; set; } = null!;

        [Required]
        public DateTime ExecutedDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(100)]
        public string ExecutedBy { get; set; } = string.Empty;

        [StringLength(500)]
        public string? ActualResult { get; set; }

        [Required]
        public TestResult Result { get; set; } = TestResult.NotRun;

        [StringLength(500)]
        public string? Notes { get; set; }

        public int? LinkedBugId { get; set; }

        [ForeignKey("LinkedBugId")]
        public virtual Bug? LinkedBug { get; set; }
    }
}