namespace Laborator4_AI.Domain.Models.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Laborator4_AI.Domain.Models.ValueObjects;

    // Base interface for StudentRegistration entity
    public interface IStudentRegistration { }

    // Unvalidated state - raw input strings
    public sealed record UnvalidatedStudentRegistration : IStudentRegistration
    {
        public string StudentRegistrationNumber { get; }
        public string CourseCode { get; }
        public string ExamDate { get; }

        internal UnvalidatedStudentRegistration(
            string studentRegistrationNumber,
            string courseCode,
            string examDate)
        {
            StudentRegistrationNumber = studentRegistrationNumber;
            CourseCode = courseCode;
            ExamDate = examDate;
        }
    }

    // Validated state - validated value objects
    public sealed record ValidatedStudentRegistration : IStudentRegistration
    {
        public StudentRegistrationNumber Student { get; }
        public CourseCode Course { get; }
        public ExamDate Date { get; }

        internal ValidatedStudentRegistration(
            StudentRegistrationNumber student,
            CourseCode course,
            ExamDate date)
        {
            Student = student;
            Course = course;
            Date = date;
        }
    }

    // Checked state - after business rule validation (e.g., max 2 exams per day)
    public sealed record CheckedStudentRegistration : IStudentRegistration
    {
        public StudentRegistrationNumber Student { get; }
        public CourseCode Course { get; }
        public ExamDate Date { get; }
        public RoomNumber Room { get; }
        public int CurrentExamsOnSameDay { get; }

        internal CheckedStudentRegistration(
            StudentRegistrationNumber student,
            CourseCode course,
            ExamDate date,
            RoomNumber room,
            int currentExamsOnSameDay)
        {
            Student = student;
            Course = course;
            Date = date;
            Room = room;
            CurrentExamsOnSameDay = currentExamsOnSameDay;
        }
    }

    // Registered state - final successful state
    public sealed record RegisteredStudentRegistration : IStudentRegistration
    {
        public StudentRegistrationNumber Student { get; }
        public CourseCode Course { get; }
        public ExamDate Date { get; }
        public RoomNumber Room { get; }
        public DateTime RegisteredAt { get; }

        internal RegisteredStudentRegistration(
            StudentRegistrationNumber student,
            CourseCode course,
            ExamDate date,
            RoomNumber room,
            DateTime registeredAt)
        {
            Student = student;
            Course = course;
            Date = date;
            Room = room;
            RegisteredAt = registeredAt;
        }
    }

    // Invalid state - when validation or registration fails
    public sealed record InvalidStudentRegistration : IStudentRegistration
    {
        public string StudentRegistrationNumber { get; }
        public IReadOnlyList<string> Reasons { get; }

        internal InvalidStudentRegistration(string studentRegistrationNumber, IEnumerable<string> reasons)
        {
            StudentRegistrationNumber = studentRegistrationNumber;
            Reasons = new List<string>(reasons).AsReadOnly();
        }
    }
}
