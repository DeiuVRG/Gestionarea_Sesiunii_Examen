namespace Laborator4_AI.Domain.Models.Entities
{
    using System;
    using System.Collections.Generic;
    using Laborator4_AI.Domain.Models.ValueObjects;

    // Base interface for Contestation entity
    public interface IContestation { }

    // Unvalidated state - raw input strings
    public sealed record UnvalidatedContestation : IContestation
    {
        public string StudentRegistrationNumber { get; }
        public string CourseCode { get; }
        public string ExamDate { get; }
        public string Reason { get; }

        internal UnvalidatedContestation(
            string studentRegistrationNumber,
            string courseCode,
            string examDate,
            string reason)
        {
            StudentRegistrationNumber = studentRegistrationNumber;
            CourseCode = courseCode;
            ExamDate = examDate;
            Reason = reason;
        }
    }

    // Validated state - validated value objects
    public sealed record ValidatedContestation : IContestation
    {
        public StudentRegistrationNumber Student { get; }
        public CourseCode Course { get; }
        public ExamDate Date { get; }
        public string Reason { get; }

        internal ValidatedContestation(
            StudentRegistrationNumber student,
            CourseCode course,
            ExamDate date,
            string reason)
        {
            Student = student;
            Course = course;
            Date = date;
            Reason = reason;
        }
    }

    // Checked state - after checking contestation window (48h)
    public sealed record CheckedContestation : IContestation
    {
        public StudentRegistrationNumber Student { get; }
        public CourseCode Course { get; }
        public ExamDate Date { get; }
        public string Reason { get; }
        public DateTime GradesPublishedAt { get; }
        public TimeSpan TimeSincePublication { get; }

        internal CheckedContestation(
            StudentRegistrationNumber student,
            CourseCode course,
            ExamDate date,
            string reason,
            DateTime gradesPublishedAt,
            TimeSpan timeSincePublication)
        {
            Student = student;
            Course = course;
            Date = date;
            Reason = reason;
            GradesPublishedAt = gradesPublishedAt;
            TimeSincePublication = timeSincePublication;
        }
    }

    // Filed state - final successful state
    public sealed record FiledContestation : IContestation
    {
        public StudentRegistrationNumber Student { get; }
        public CourseCode Course { get; }
        public ExamDate Date { get; }
        public string Reason { get; }
        public DateTime FiledAt { get; }

        internal FiledContestation(
            StudentRegistrationNumber student,
            CourseCode course,
            ExamDate date,
            string reason,
            DateTime filedAt)
        {
            Student = student;
            Course = course;
            Date = date;
            Reason = reason;
            FiledAt = filedAt;
        }
    }

    // Invalid state - when validation or filing fails
    public sealed record InvalidContestation : IContestation
    {
        public string StudentRegistrationNumber { get; }
        public IReadOnlyList<string> Reasons { get; }

        internal InvalidContestation(string studentRegistrationNumber, IEnumerable<string> reasons)
        {
            StudentRegistrationNumber = studentRegistrationNumber;
            Reasons = new List<string>(reasons).AsReadOnly();
        }
    }
}
