using BugTrack.Models;
using BugTrack.Models.Enums;

namespace BugTrack.ViewModels
{
    public class TestCaseListViewModel
    {
        public List<TestCase> TestCases { get; set; } = new();
        public string? FilterModule { get; set; }
        public TestPriority? FilterPriority { get; set; }
        public string? SearchTerm { get; set; }

        public List<string> Modules { get; set; } = new();
        public List<TestPriority> Priorities { get; set; } = new();
    }
}