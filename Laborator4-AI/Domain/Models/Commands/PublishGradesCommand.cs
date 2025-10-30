namespace Laborator4_AI.Domain.Models.Commands
{
    using System.Collections.Generic;

    /// <summary>
    /// Command for publishing exam grades
    /// Contains raw string inputs that will be validated by the workflow
    /// </summary>
    public sealed class PublishGradesCommand
    {
        public string CourseCode { get; init; } = string.Empty;
        public string ExamDate { get; init; } = string.Empty;
        public IReadOnlyList<StudentGradeInput> StudentGrades { get; init; } = new List<StudentGradeInput>();
    }

    public sealed class StudentGradeInput
    {
        public string StudentRegistrationNumber { get; init; } = string.Empty;
        public string Grade { get; init; } = string.Empty;
    }
}
