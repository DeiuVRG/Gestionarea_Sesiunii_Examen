namespace Laborator4_AI.Domain.Workflows
{
    using System;
    using Laborator4_AI.Domain.Models.Commands;
    using Laborator4_AI.Domain.Models.Events;
    using Laborator4_AI.Domain.Operations;
    using Laborator4_AI.Domain.Models.Entities;
    using Laborator4_AI.Domain.Models.ValueObjects;

    /// <summary>
    /// Workflow for filing a grade contestation
    /// Composes operations: Validate → CheckWindow → File
    /// </summary>
    public sealed class FileContestationWorkflow
    {
        public IContestationEvent Execute(
            FileContestationCommand command,
            Func<StudentRegistrationNumber, CourseCode, ExamDate, bool> checkStudentRegistered,
            Func<CourseCode, ExamDate, DateTime?> getGradesPublishedDate,
            Func<StudentRegistrationNumber, CourseCode, ExamDate, string, bool> persistContestation)
        {
            // 1. Create unvalidated state from command
            IContestation contestation = new UnvalidatedContestation(
                command.StudentRegistrationNumber,
                command.CourseCode,
                command.ExamDate,
                command.Reason);

            // 2. Pipeline of operations using Transform
            contestation = new ValidateContestationOperation(checkStudentRegistered, getGradesPublishedDate)
                .Transform(contestation);

            contestation = new CheckContestationWindowOperation(getGradesPublishedDate)
                .Transform(contestation);

            contestation = new FileContestationOperation(persistContestation)
                .Transform(contestation);

            // 3. Convert final state to event
            return contestation.ToEvent();
        }
    }
}
