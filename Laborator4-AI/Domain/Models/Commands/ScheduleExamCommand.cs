namespace Laborator4_AI.Domain.Models.Commands
{
    /// <summary>
    /// Command for scheduling an exam
    /// Contains raw string input that will be validated by the workflow
    /// </summary>
    public sealed class ScheduleExamCommand
    {
        public string CourseCode { get; init; } = string.Empty;
        public string ProposedDate1 { get; init; } = string.Empty;
        public string ProposedDate2 { get; init; } = string.Empty;
        public string ProposedDate3 { get; init; } = string.Empty;
        public string Duration { get; init; } = string.Empty;
        public string ExpectedStudents { get; init; } = string.Empty;
    }
}
