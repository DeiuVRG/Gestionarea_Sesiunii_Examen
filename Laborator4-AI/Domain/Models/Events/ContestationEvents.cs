namespace Laborator4_AI.Domain.Models.Events
{
    using System;
    using System.Collections.Generic;
    using Laborator4_AI.Domain.Models.ValueObjects;
    using Laborator4_AI.Domain.Models.Entities;

    // Base interface for contestation events
    public interface IContestationEvent { }

    // Success event - past tense: ContestationFiled
    public sealed record ContestationFiledEvent : IContestationEvent
    {
        public StudentRegistrationNumber Student { get; }
        public CourseCode Course { get; }
        public ExamDate Date { get; }
        public string Reason { get; }
        public DateTime FiledAt { get; }

        internal ContestationFiledEvent(StudentRegistrationNumber student, CourseCode course, ExamDate date, string reason, DateTime filedAt)
        {
            Student = student;
            Course = course;
            Date = date;
            Reason = reason;
            FiledAt = filedAt;
        }
    }

    // Failure event
    public sealed record ContestationFailedEvent : IContestationEvent
    {
        public string StudentRegistrationNumber { get; }
        public IEnumerable<string> Reasons { get; }

        internal ContestationFailedEvent(string studentRegistrationNumber, IEnumerable<string> reasons)
        {
            StudentRegistrationNumber = studentRegistrationNumber;
            Reasons = reasons;
        }
    }

    // Extension method to convert IContestation to event
    public static class ContestationEventExtensions
    {
        public static IContestationEvent ToEvent(this IContestation contestation)
        {
            return contestation switch
            {
                FiledContestation filed => new ContestationFiledEvent(
                    filed.Student,
                    filed.Course,
                    filed.Date,
                    filed.Reason,
                    filed.FiledAt),
                InvalidContestation invalid => new ContestationFailedEvent(
                    invalid.StudentRegistrationNumber,
                    invalid.Reasons),
                UnvalidatedContestation unvalidated => new ContestationFailedEvent(
                    unvalidated.StudentRegistrationNumber,
                    new[] { "Contestation was not completed - remained in unvalidated state" }),
                ValidatedContestation validated => new ContestationFailedEvent(
                    validated.Student.Value,
                    new[] { "Contestation was not completed - remained in validated state" }),
                CheckedContestation checkedContest => new ContestationFailedEvent(
                    checkedContest.Student.Value,
                    new[] { "Contestation was not completed - remained in checked state" }),
                _ => new ContestationFailedEvent(
                    "Unknown",
                    new[] { $"Unknown contestation state: {contestation.GetType().Name}" })
            };
        }
    }
}
