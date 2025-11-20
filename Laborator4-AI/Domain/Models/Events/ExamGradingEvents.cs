namespace Laborator4_AI.Domain.Models.Events
{
    using System;
    using System.Collections.Generic;
    using Laborator4_AI.Domain.Models.ValueObjects;
    using Laborator4_AI.Domain.Models.Entities;

    // Base interface for exam grading events
    public interface IExamGradingEvent { }

    // Success event - past tense: GradesPublished
    public sealed record GradesPublishedEvent : IExamGradingEvent
    {
        public CourseCode Course { get; }
        public ExamDate Date { get; }
        public DateTime PublishedAt { get; }
        public int TotalStudents { get; }
        public int PassedStudents { get; }
        public double PassRate => TotalStudents > 0 ? Math.Round(100.0 * PassedStudents / TotalStudents, 2) : 0;

        internal GradesPublishedEvent(CourseCode course, ExamDate date, DateTime publishedAt, int totalStudents, int passedStudents)
        {
            Course = course;
            Date = date;
            PublishedAt = publishedAt;
            TotalStudents = totalStudents;
            PassedStudents = passedStudents;
        }
    }

    // Failure event
    public sealed record ExamGradingFailedEvent : IExamGradingEvent
    {
        public string CourseCode { get; }
        public IEnumerable<string> Reasons { get; }

        internal ExamGradingFailedEvent(string courseCode, IEnumerable<string> reasons)
        {
            CourseCode = courseCode;
            Reasons = reasons;
        }
    }

    // Extension method to convert IExamGrading to event
    public static class ExamGradingEventExtensions
    {
        public static IExamGradingEvent ToEvent(this IExamGrading grading)
        {
            return grading switch
            {
                PublishedExamGrading published => new GradesPublishedEvent(
                    published.Course,
                    published.Date,
                    published.PublishedAt,
                    published.TotalStudents,
                    published.PassedStudents),
                InvalidExamGrading invalid => new ExamGradingFailedEvent(
                    invalid.CourseCode,
                    invalid.Reasons),
                UnvalidatedExamGrading unvalidated => new ExamGradingFailedEvent(
                    unvalidated.CourseCode,
                    new[] { "Exam grading was not completed - remained in unvalidated state" }),
                ValidatedExamGrading validated => new ExamGradingFailedEvent(
                    validated.Course.Value,
                    new[] { "Exam grading was not completed - remained in validated state" }),
                _ => new ExamGradingFailedEvent(
                    "Unknown",
                    new[] { $"Unknown exam grading state: {grading.GetType().Name}" })
            };
        }
    }
}
