using BugTrack.Models.Enums;

namespace BugTrack.Services
{
    public class BugStatusService
    {
        private readonly Dictionary<BugStatus, List<BugStatus>> _allowedTransitions = new()
        {
            { BugStatus.New, new List<BugStatus> { BugStatus.InProgress, BugStatus.Closed } },
            { BugStatus.InProgress, new List<BugStatus> { BugStatus.Fixed, BugStatus.Closed } },
            { BugStatus.Fixed, new List<BugStatus> { BugStatus.Retest, BugStatus.Closed } },
            { BugStatus.Retest, new List<BugStatus> { BugStatus.Closed, BugStatus.Reopened } },
            { BugStatus.Closed, new List<BugStatus> { BugStatus.Reopened } },
            { BugStatus.Reopened, new List<BugStatus> { BugStatus.InProgress, BugStatus.Closed } }
        };

        public bool CanTransition(BugStatus currentStatus, BugStatus newStatus)
        {
            return _allowedTransitions.TryGetValue(currentStatus, out var allowed) &&
                   allowed.Contains(newStatus);
        }

        public List<BugStatus> GetAvailableTransitions(BugStatus currentStatus)
        {
            return _allowedTransitions.TryGetValue(currentStatus, out var allowed)
                ? allowed
                : new List<BugStatus>();
        }

        public string GetStatusDisplayName(BugStatus status)
        {
            return status switch
            {
                BugStatus.New => "New",
                BugStatus.InProgress => "In Progress",
                BugStatus.Fixed => "Fixed",
                BugStatus.Retest => "Retest",
                BugStatus.Closed => "Closed",
                BugStatus.Reopened => "Reopened",
                _ => status.ToString()
            };
        }
    }
}