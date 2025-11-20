namespace Laborator4_AI.Domain.Models.Commands
{
    /// <summary>
    /// Command for filing a grade contestation
    /// Contains raw string input that will be validated by the workflow
    /// </summary>
    public sealed class FileContestationCommand
    {
        public string StudentRegistrationNumber { get; init; } = string.Empty;
        public string CourseCode { get; init; } = string.Empty;
        public string ExamDate { get; init; } = string.Empty;
        public string Reason { get; init; } = string.Empty;
    }
}
