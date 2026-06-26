using Microsoft.EntityFrameworkCore;
using BugTrack.Models;
using BugTrack.Models.Enums;

namespace BugTrack.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.Migrate();

            if (context.TestCases.Any())
            {
                return;
            }

            var testCases = new TestCase[]
            {
                new TestCase
                {
                    Title = "Verify user login with valid credentials",
                    Module = "Authentication",
                    Preconditions = "User is registered in the system",
                    Steps = "1. Navigate to login page\n2. Enter valid username and password\n3. Click Login button",
                    ExpectedResult = "User is successfully logged in and redirected to dashboard",
                    Priority = TestPriority.High,
                    CreatedDate = DateTime.UtcNow
                },
                new TestCase
                {
                    Title = "Verify password reset functionality",
                    Module = "Authentication",
                    Preconditions = "User has access to registered email",
                    Steps = "1. Click Forgot Password link\n2. Enter registered email\n3. Click Send Reset Link\n4. Open email and click reset link\n5. Enter new password and confirm",
                    ExpectedResult = "Password is successfully reset and user can login with new password",
                    Priority = TestPriority.Medium,
                    CreatedDate = DateTime.UtcNow
                }
            };

            context.TestCases.AddRange(testCases);
            context.SaveChanges();

            var bugs = new Bug[]
            {
                new Bug
                {
                    Title = "Login button not working on Chrome browser",
                    Description = "When using Chrome browser version 120+, the login button does not respond to clicks",
                    StepsToReproduce = "1. Open Chrome 120+\n2. Navigate to login page\n3. Enter credentials\n4. Click Login button",
                    Severity = BugSeverity.Major,
                    Priority = BugPriority.High,
                    Status = BugStatus.New,
                    CreatedDate = DateTime.UtcNow,
                    AssignedTo = "Dev Team"
                },
                new Bug
                {
                    Title = "Password reset email not being sent",
                    Description = "When requesting password reset, the email is not being sent to the user's registered email",
                    StepsToReproduce = "1. Navigate to login page\n2. Click Forgot Password\n3. Enter valid registered email\n4. Click Send Reset Link",
                    Severity = BugSeverity.Critical,
                    Priority = BugPriority.Critical,
                    Status = BugStatus.InProgress,
                    CreatedDate = DateTime.UtcNow.AddDays(-2),
                    AssignedTo = "Dev Team"
                }
            };

            context.Bugs.AddRange(bugs);
            context.SaveChanges();

            var testRuns = new TestRun[]
            {
                new TestRun
                {
                    TestCaseId = testCases[0].Id,
                    ExecutedDate = DateTime.UtcNow.AddDays(-1),
                    ExecutedBy = "QA Tester",
                    ActualResult = "User is successfully logged in and redirected to dashboard",
                    Result = TestResult.Pass,
                    Notes = "Test passed on Chrome and Firefox"
                },
                new TestRun
                {
                    TestCaseId = testCases[0].Id,
                    ExecutedDate = DateTime.UtcNow,
                    ExecutedBy = "QA Tester",
                    ActualResult = "Login button is unresponsive",
                    Result = TestResult.Fail,
                    Notes = "Failed on Chrome 120+",
                    LinkedBugId = bugs[0].Id
                }
            };

            context.TestRuns.AddRange(testRuns);
            context.SaveChanges();

            bugs[0].LinkedTestRunId = testRuns[1].Id;
            context.SaveChanges();
        }
    }
}