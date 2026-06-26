using BugTrack.Models;
using BugTrack.Models.Enums;

namespace BugTrack.ViewModels
{
    public class BugListViewModel
    {
        public List<Bug> Bugs { get; set; } = new();
        public BugStatus? FilterStatus { get; set; }
        public BugPriority? FilterPriority { get; set; }
        public BugSeverity? FilterSeverity { get; set; }
        public string? SearchTerm { get; set; }

        public List<BugStatus> Statuses { get; set; } = new();
        public List<BugPriority> Priorities { get; set; } = new();
        public List<BugSeverity> Severities { get; set; } = new();
    }
}