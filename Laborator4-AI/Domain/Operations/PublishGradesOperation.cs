namespace Laborator4_AI.Domain.Operations
{
    using System;
    using System.Linq;
    using Laborator4_AI.Domain.Models.Entities;

    /// <summary>
    /// Operation to publish exam grades
    /// Dependencies:
    /// - persistGrades: Persists the grades
    /// </summary>
    internal sealed class PublishGradesOperation : ExamGradingOperation
    {
        private readonly Func<ValidatedExamGrading, bool> _persistGrades;

        public PublishGradesOperation(Func<ValidatedExamGrading, bool> persistGrades)
        {
            _persistGrades = persistGrades;
        }

        protected override IExamGrading OnValidated(ValidatedExamGrading grading)
        {
            // Persist grades
            var success = _persistGrades(grading);

            if (!success)
            {
                return new InvalidExamGrading(
                    grading.Course.Value,
                    new[] { "Failed to persist exam grades" });
            }

            // Calculate statistics
            var totalStudents = grading.StudentGrades.Count;
            var passedStudents = grading.StudentGrades.Count(sg => sg.Grade.IsPassing());

            return new PublishedExamGrading(
                grading.Course,
                grading.Date,
                grading.StudentGrades,
                DateTime.Now,
                totalStudents,
                passedStudents);
        }
    }
}
