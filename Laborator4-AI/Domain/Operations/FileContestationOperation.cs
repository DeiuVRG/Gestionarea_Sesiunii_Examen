namespace Laborator4_AI.Domain.Operations
{
    using System;
    using Laborator4_AI.Domain.Models.Entities;
    using Laborator4_AI.Domain.Models.ValueObjects;

    /// <summary>
    /// Operation to file contestation
    /// Dependencies:
    /// - persistContestation: Persists the contestation
    /// </summary>
    internal sealed class FileContestationOperation : ContestationOperation
    {
        private readonly Func<StudentRegistrationNumber, CourseCode, ExamDate, string, bool> _persistContestation;

        public FileContestationOperation(
            Func<StudentRegistrationNumber, CourseCode, ExamDate, string, bool> persistContestation)
        {
            _persistContestation = persistContestation;
        }

        protected override IContestation OnChecked(CheckedContestation contestation)
        {
            var success = _persistContestation(
                contestation.Student,
                contestation.Course,
                contestation.Date,
                contestation.Reason);

            if (!success)
            {
                return new InvalidContestation(
                    contestation.Student.Value,
                    new[] { "Failed to persist contestation" });
            }

            return new FiledContestation(
                contestation.Student,
                contestation.Course,
                contestation.Date,
                contestation.Reason,
                DateTime.Now);
        }
    }
}
