using System.ComponentModel.DataAnnotations;
using BugTrack.Models.Enums;

namespace BugTrack.Models
{
    public class TestCase
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Module { get; set; }

        public string? Preconditions { get; set; }

        [Required]
        public string Steps { get; set; } = string.Empty;

        [Required]
        public string ExpectedResult { get; set; } = string.Empty;

        [Required]
        public TestPriority Priority { get; set; } = TestPriority.Medium;

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }

        public virtual ICollection<TestRun> TestRuns { get; set; } = new List<TestRun>();
    }
}