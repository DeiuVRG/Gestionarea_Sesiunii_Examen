namespace Laborator4_AI.Domain.Models.Commands
{
    /// <summary>
    /// Command for registering a student to an exam
    /// Contains raw string input that will be validated by the workflow
    /// </summary>
    public sealed class RegisterStudentCommand
    {
        public string StudentRegistrationNumber { get; init; } = string.Empty;
        public string CourseCode { get; init; } = string.Empty;
        public string ExamDate { get; init; } = string.Empty;
    }
}
