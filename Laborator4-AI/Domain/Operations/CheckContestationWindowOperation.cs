namespace Laborator4_AI.Domain.Operations
{
    using System;
    using System.Collections.Generic;
    using Laborator4_AI.Domain.Models.Entities;
    using Laborator4_AI.Domain.Models.ValueObjects;

    /// <summary>
    /// Operation to check 48-hour contestation window
    /// Dependencies:
    /// - getGradesPublishedDate: Gets when grades were published
    /// </summary>
    internal sealed class CheckContestationWindowOperation : ContestationOperation
    {
        private readonly Func<CourseCode, ExamDate, DateTime?> _getGradesPublishedDate;
        private static readonly TimeSpan ContestationWindow = TimeSpan.FromHours(48);

        public CheckContestationWindowOperation(Func<CourseCode, ExamDate, DateTime?> getGradesPublishedDate)
        {
            _getGradesPublishedDate = getGradesPublishedDate;
        }

        protected override IContestation OnValidated(ValidatedContestation contestation)
        {
            var publishedDate = _getGradesPublishedDate(contestation.Course, contestation.Date);

            if (publishedDate == null)
            {
                return new InvalidContestation(
                    contestation.Student.Value,
                    new[] { "Grades have not been published yet for this exam" });
            }

            var timeSincePublication = DateTime.Now - publishedDate.Value;

            if (timeSincePublication > ContestationWindow)
            {
                return new InvalidContestation(
                    contestation.Student.Value,
                    new[] { $"Contestation window has expired. Grades were published {timeSincePublication.TotalHours:F1} hours ago (deadline: 48 hours)" });
            }

            return new CheckedContestation(
                contestation.Student,
                contestation.Course,
                contestation.Date,
                contestation.Reason,
                publishedDate.Value,
                timeSincePublication);
        }
    }
}
