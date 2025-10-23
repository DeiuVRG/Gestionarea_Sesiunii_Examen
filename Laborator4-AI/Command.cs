namespace Laborator4_AI
{
    // Command
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

