namespace Laborator4_AI.Domain.Models.Entities
{
    using System;
    using System.Collections.Generic;
    using Laborator4_AI.Domain.Models.ValueObjects;

    // Base interface for ExamScheduling entity
    public interface IExamScheduling { }

    // Unvalidated state - raw input strings
    public sealed record UnvalidatedExamScheduling : IExamScheduling
    {
        public string CourseCode { get; }
        public string ProposedDate1 { get; }
        public string ProposedDate2 { get; }
        public string ProposedDate3 { get; }
        public string Duration { get; }
        public string ExpectedStudents { get; }

        internal UnvalidatedExamScheduling(
            string courseCode,
            string proposedDate1,
            string proposedDate2,
            string proposedDate3,
            string duration,
            string expectedStudents)
        {
            CourseCode = courseCode;
            ProposedDate1 = proposedDate1;
            ProposedDate2 = proposedDate2;
            ProposedDate3 = proposedDate3;
            Duration = duration;
            ExpectedStudents = expectedStudents;
        }
    }

    // Validated state - validated value objects
    public sealed record ValidatedExamScheduling : IExamScheduling
    {
        public CourseCode Course { get; }
        public IReadOnlyList<ExamDate> ProposedDates { get; }
        public Duration Duration { get; }
        public Capacity ExpectedStudents { get; }

        internal ValidatedExamScheduling(
            CourseCode course,
            IReadOnlyList<ExamDate> proposedDates,
            Duration duration,
            Capacity expectedStudents)
        {
            Course = course;
            ProposedDates = proposedDates;
            Duration = duration;
            ExpectedStudents = expectedStudents;
        }
    }

    // Room allocated state - after room allocation
    public sealed record RoomAllocatedExamScheduling : IExamScheduling
    {
        public CourseCode Course { get; }
        public ExamDate SelectedDate { get; }
        public Duration Duration { get; }
        public RoomNumber Room { get; }
        public Capacity RoomCapacity { get; }

        internal RoomAllocatedExamScheduling(
            CourseCode course,
            ExamDate selectedDate,
            Duration duration,
            RoomNumber room,
            Capacity roomCapacity)
        {
            Course = course;
            SelectedDate = selectedDate;
            Duration = duration;
            Room = room;
            RoomCapacity = roomCapacity;
        }
    }

    // Published state - final successful state
    public sealed record PublishedExamScheduling : IExamScheduling
    {
        public CourseCode Course { get; }
        public ExamDate SelectedDate { get; }
        public Duration Duration { get; }
        public RoomNumber Room { get; }
        public Capacity RoomCapacity { get; }
        public Capacity EnrolledStudents { get; }
        public DateTime PublishedAt { get; }

        internal PublishedExamScheduling(
            CourseCode course,
            ExamDate selectedDate,
            Duration duration,
            RoomNumber room,
            Capacity roomCapacity,
            Capacity enrolledStudents,
            DateTime publishedAt)
        {
            Course = course;
            SelectedDate = selectedDate;
            Duration = duration;
            Room = room;
            RoomCapacity = roomCapacity;
            EnrolledStudents = enrolledStudents;
            PublishedAt = publishedAt;
        }
    }

    // Invalid state - when validation or processing fails
    public sealed record InvalidExamScheduling : IExamScheduling
    {
        public string CourseCode { get; }
        public IReadOnlyList<string> Reasons { get; }

        internal InvalidExamScheduling(string courseCode, IEnumerable<string> reasons)
        {
            CourseCode = courseCode;
            Reasons = new List<string>(reasons).AsReadOnly();
        }
    }
}
