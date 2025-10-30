namespace Laborator4_AI.Domain.Models.Events
{
    using System;
    using System.Collections.Generic;
    using Laborator4_AI.Domain.Models.ValueObjects;
    using Laborator4_AI.Domain.Models.Entities;

    // Base interface for exam scheduling events
    public interface IExamSchedulingEvent { }

    // Success event - past tense: ExamScheduled
    public sealed record ExamScheduledEvent : IExamSchedulingEvent
    {
        public CourseCode Course { get; }
        public ExamDate Date { get; }
        public RoomNumber Room { get; }
        public DateTime PublishedAt { get; }
        public Capacity RoomCapacity { get; }

        internal ExamScheduledEvent(CourseCode course, ExamDate date, RoomNumber room, DateTime publishedAt, Capacity roomCapacity)
        {
            Course = course;
            Date = date;
            Room = room;
            PublishedAt = publishedAt;
            RoomCapacity = roomCapacity;
        }
    }

    // Failure event
    public sealed record ExamSchedulingFailedEvent : IExamSchedulingEvent
    {
        public string CourseCode { get; }
        public IEnumerable<string> Reasons { get; }

        internal ExamSchedulingFailedEvent(string courseCode, IEnumerable<string> reasons)
        {
            CourseCode = courseCode;
            Reasons = reasons;
        }
    }

    // Extension method to convert IExamScheduling to event
    public static class ExamSchedulingEventExtensions
    {
        public static IExamSchedulingEvent ToEvent(this IExamScheduling scheduling)
        {
            return scheduling switch
            {
                PublishedExamScheduling published => new ExamScheduledEvent(
                    published.Course,
                    published.SelectedDate,
                    published.Room,
                    published.PublishedAt,
                    published.RoomCapacity),
                InvalidExamScheduling invalid => new ExamSchedulingFailedEvent(
                    invalid.CourseCode,
                    invalid.Reasons),
                UnvalidatedExamScheduling unvalidated => new ExamSchedulingFailedEvent(
                    unvalidated.CourseCode,
                    new[] { "Exam scheduling was not completed - remained in unvalidated state" }),
                ValidatedExamScheduling validated => new ExamSchedulingFailedEvent(
                    validated.Course.Value,
                    new[] { "Exam scheduling was not completed - remained in validated state" }),
                RoomAllocatedExamScheduling allocated => new ExamSchedulingFailedEvent(
                    allocated.Course.Value,
                    new[] { "Exam scheduling was not completed - remained in room allocated state" }),
                _ => new ExamSchedulingFailedEvent(
                    "Unknown",
                    new[] { $"Unknown exam scheduling state: {scheduling.GetType().Name}" })
            };
        }
    }
}
