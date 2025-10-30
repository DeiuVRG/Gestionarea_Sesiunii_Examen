namespace Laborator4_AI.Domain.Models.Entities
{
    using System;
    using System.Collections.Generic;
    using Laborator4_AI.Domain.Models.ValueObjects;

    // Base interface for ExamGrading entity
    public interface IExamGrading { }

    // Individual student grade (used in collections)
    public sealed record StudentGrade
    {
        public StudentRegistrationNumber Student { get; }
        public Grade Grade { get; }

        internal StudentGrade(StudentRegistrationNumber student, Grade grade)
        {
            Student = student;
            Grade = grade;
        }
    }

    // Unvalidated individual grade input (raw strings)
    public sealed record UnvalidatedStudentGrade
    {
        public string StudentRegistrationNumber { get; }
        public string Grade { get; }

        internal UnvalidatedStudentGrade(string studentRegistrationNumber, string grade)
        {
            StudentRegistrationNumber = studentRegistrationNumber;
            Grade = grade;
        }
    }

    // Unvalidated state - raw input strings
    public sealed record UnvalidatedExamGrading : IExamGrading
    {
        public string CourseCode { get; }
        public string ExamDate { get; }
        public IReadOnlyList<UnvalidatedStudentGrade> StudentGrades { get; }

        internal UnvalidatedExamGrading(
            string courseCode,
            string examDate,
            IReadOnlyList<UnvalidatedStudentGrade> studentGrades)
        {
            CourseCode = courseCode;
            ExamDate = examDate;
            StudentGrades = studentGrades;
        }
    }

    // Validated state - validated value objects
    public sealed record ValidatedExamGrading : IExamGrading
    {
        public CourseCode Course { get; }
        public ExamDate Date { get; }
        public IReadOnlyList<StudentGrade> StudentGrades { get; }

        internal ValidatedExamGrading(
            CourseCode course,
            ExamDate date,
            IReadOnlyList<StudentGrade> studentGrades)
        {
            Course = course;
            Date = date;
            StudentGrades = studentGrades;
        }
    }

    // Published state - final successful state
    public sealed record PublishedExamGrading : IExamGrading
    {
        public CourseCode Course { get; }
        public ExamDate Date { get; }
        public IReadOnlyList<StudentGrade> StudentGrades { get; }
        public DateTime PublishedAt { get; }
        public int TotalStudents { get; }
        public int PassedStudents { get; }

        internal PublishedExamGrading(
            CourseCode course,
            ExamDate date,
            IReadOnlyList<StudentGrade> studentGrades,
            DateTime publishedAt,
            int totalStudents,
            int passedStudents)
        {
            Course = course;
            Date = date;
            StudentGrades = studentGrades;
            PublishedAt = publishedAt;
            TotalStudents = totalStudents;
            PassedStudents = passedStudents;
        }
    }

    // Invalid state - when validation or publishing fails
    public sealed record InvalidExamGrading : IExamGrading
    {
        public string CourseCode { get; }
        public IReadOnlyList<string> Reasons { get; }

        internal InvalidExamGrading(string courseCode, IEnumerable<string> reasons)
        {
            CourseCode = courseCode;
            Reasons = new List<string>(reasons).AsReadOnly();
        }
    }
}
