namespace Laborator4_AI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IExamScheduling
    {
        IExamScheduledEvent? ToEvent();
    }

    public interface IExamScheduledEvent
    {
        CourseCode Course { get; }
        ExamDate Date { get; }
        RoomNumber Room { get; }
        DateTime PublishedAt { get; }
    }

    public sealed class ExamScheduledEvent : IExamScheduledEvent
    {
        public required CourseCode Course { get; init; }
        public required ExamDate Date { get; init; }
        public required RoomNumber Room { get; init; }
        public required DateTime PublishedAt { get; init; }
    }

    public sealed class UnvalidatedExamScheduling : IExamScheduling
    {
        internal UnvalidatedExamScheduling(string courseCode, string proposedDate1, string proposedDate2, string proposedDate3, string duration, string expectedStudents)
        {
            CourseCode = courseCode;
            ProposedDate1 = proposedDate1;
            ProposedDate2 = proposedDate2;
            ProposedDate3 = proposedDate3;
            Duration = duration;
            ExpectedStudents = expectedStudents;
        }
        public string CourseCode { get; }
        public string ProposedDate1 { get; }
        public string ProposedDate2 { get; }
        public string ProposedDate3 { get; }
        public string Duration { get; }
        public string ExpectedStudents { get; }
        public IExamScheduledEvent? ToEvent() => null;
    }

    public sealed class ValidatedExamScheduling : IExamScheduling
    {
        internal ValidatedExamScheduling(CourseCode course, IReadOnlyList<ExamDate> proposedDates, Duration duration, Capacity expectedStudents)
        {
            Course = course;
            ProposedDates = proposedDates;
            Duration = duration;
            ExpectedStudents = expectedStudents;
        }
        public CourseCode Course { get; }
        public IReadOnlyList<ExamDate> ProposedDates { get; }
        public Duration Duration { get; }
        public Capacity ExpectedStudents { get; }
        public IExamScheduledEvent? ToEvent() => null;
    }

    public sealed class RoomAllocatedExamScheduling : IExamScheduling
    {
        internal RoomAllocatedExamScheduling(CourseCode course, ExamDate selectedDate, Duration duration, RoomNumber room, Capacity roomCapacity)
        {
            Course = course;
            SelectedDate = selectedDate;
            Duration = duration;
            Room = room;
            RoomCapacity = roomCapacity;
        }
        public CourseCode Course { get; }
        public ExamDate SelectedDate { get; }
        public Duration Duration { get; }
        public RoomNumber Room { get; }
        public Capacity RoomCapacity { get; }
        public IExamScheduledEvent? ToEvent() => null;
    }

    public sealed class PublishedExamScheduling : IExamScheduling
    {
        internal PublishedExamScheduling(CourseCode course, ExamDate selectedDate, Duration duration, RoomNumber room, Capacity roomCapacity, Capacity enrolledStudents, DateTime publishedAt)
        {
            Course = course;
            SelectedDate = selectedDate;
            Duration = duration;
            Room = room;
            RoomCapacity = roomCapacity;
            EnrolledStudents = enrolledStudents;
            PublishedAt = publishedAt;
        }
        public CourseCode Course { get; }
        public ExamDate SelectedDate { get; }
        public Duration Duration { get; }
        public RoomNumber Room { get; }
        public Capacity RoomCapacity { get; }
        public Capacity EnrolledStudents { get; }
        public DateTime PublishedAt { get; }

        public IExamScheduledEvent? ToEvent()
        {
            return new ExamScheduledEvent
            {
                Course = Course,
                Date = SelectedDate,
                Room = Room,
                PublishedAt = PublishedAt
            };
        }
    }

    public sealed class ClosedExamScheduling : IExamScheduling
    {
        internal ClosedExamScheduling(CourseCode course, ExamDate selectedDate, RoomNumber room, Capacity enrolledStudents, Capacity attendedStudents, DateTime closedAt)
        {
            Course = course;
            SelectedDate = selectedDate;
            Room = room;
            EnrolledStudents = enrolledStudents;
            AttendedStudents = attendedStudents;
            ClosedAt = closedAt;
        }
        public CourseCode Course { get; }
        public ExamDate SelectedDate { get; }
        public RoomNumber Room { get; }
        public Capacity EnrolledStudents { get; }
        public Capacity AttendedStudents { get; }
        public DateTime ClosedAt { get; }
        public IExamScheduledEvent? ToEvent() => null;
    }

    public sealed class InvalidExamScheduling : IExamScheduling
    {
        internal InvalidExamScheduling(string courseCode, IEnumerable<string> reasons)
        {
            CourseCode = courseCode;
            Reasons = reasons.ToList().AsReadOnly();
        }
        public string CourseCode { get; }
        public IReadOnlyList<string> Reasons { get; }
        public IExamScheduledEvent? ToEvent() => null;
    }
}

