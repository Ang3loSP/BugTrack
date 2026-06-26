using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BugTrack.Models.Enums;

namespace BugTrack.Models
{
    public class Bug
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public string? StepsToReproduce { get; set; }

        [Required]
        public BugSeverity Severity { get; set; } = BugSeverity.Minor;

        [Required]
        public BugPriority Priority { get; set; } = BugPriority.Medium;

        [Required]
        public BugStatus Status { get; set; } = BugStatus.New;

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }

        [StringLength(100)]
        public string? AssignedTo { get; set; }

        public int? LinkedTestRunId { get; set; }

        [ForeignKey("LinkedTestRunId")]
        public virtual TestRun? LinkedTestRun { get; set; }

        public virtual ICollection<TestRun>? RelatedTestRuns { get; set; }
    }
}