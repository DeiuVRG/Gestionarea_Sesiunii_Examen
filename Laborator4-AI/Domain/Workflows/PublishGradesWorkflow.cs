namespace Laborator4_AI.Domain.Workflows
{
    using System;
    using System.Linq;
    using Laborator4_AI.Domain.Models.Commands;
    using Laborator4_AI.Domain.Models.Events;
    using Laborator4_AI.Domain.Operations;
    using Laborator4_AI.Domain.Models.Entities;
    using Laborator4_AI.Domain.Models.ValueObjects;

    /// <summary>
    /// Workflow for publishing exam grades
    /// Composes operations: Validate → Publish
    /// </summary>
    public sealed class PublishGradesWorkflow
    {
        public IExamGradingEvent Execute(
            PublishGradesCommand command,
            Func<CourseCode, ExamDate, bool> checkExamExists,
            Func<StudentRegistrationNumber, CourseCode, ExamDate, bool> checkStudentRegistered,
            Func<ValidatedExamGrading, bool> persistGrades)
        {
            // 1. Create unvalidated state from command
            var unvalidatedGrades = command.StudentGrades
                .Select(sg => new UnvalidatedStudentGrade(sg.StudentRegistrationNumber, sg.Grade))
                .ToList()
                .AsReadOnly();

            IExamGrading grading = new UnvalidatedExamGrading(
                command.CourseCode,
                command.ExamDate,
                unvalidatedGrades);

            // 2. Pipeline of operations using Transform
            grading = new ValidateExamGradingOperation(checkExamExists, checkStudentRegistered)
                .Transform(grading);

            grading = new PublishGradesOperation(persistGrades)
                .Transform(grading);

            // 3. Convert final state to event
            return grading.ToEvent();
        }
    }
}
