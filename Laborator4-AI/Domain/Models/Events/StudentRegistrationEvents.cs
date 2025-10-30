namespace Laborator4_AI.Domain.Models.Events
{
    using System;
    using System.Collections.Generic;
    using Laborator4_AI.Domain.Models.ValueObjects;
    using Laborator4_AI.Domain.Models.Entities;

    // Base interface for student registration events
    public interface IStudentRegistrationEvent { }

    // Success event - past tense: StudentRegistered
    public sealed record StudentRegisteredEvent : IStudentRegistrationEvent
    {
        public StudentRegistrationNumber Student { get; }
        public CourseCode Course { get; }
        public ExamDate Date { get; }
        public RoomNumber Room { get; }
        public DateTime RegisteredAt { get; }

        internal StudentRegisteredEvent(StudentRegistrationNumber student, CourseCode course, ExamDate date, RoomNumber room, DateTime registeredAt)
        {
            Student = student;
            Course = course;
            Date = date;
            Room = room;
            RegisteredAt = registeredAt;
        }
    }

    // Failure event
    public sealed record StudentRegistrationFailedEvent : IStudentRegistrationEvent
    {
        public string StudentRegistrationNumber { get; }
        public IEnumerable<string> Reasons { get; }

        internal StudentRegistrationFailedEvent(string studentRegistrationNumber, IEnumerable<string> reasons)
        {
            StudentRegistrationNumber = studentRegistrationNumber;
            Reasons = reasons;
        }
    }

    // Extension method to convert IStudentRegistration to event
    public static class StudentRegistrationEventExtensions
    {
        public static IStudentRegistrationEvent ToEvent(this IStudentRegistration registration)
        {
            return registration switch
            {
                RegisteredStudentRegistration registered => new StudentRegisteredEvent(
                    registered.Student,
                    registered.Course,
                    registered.Date,
                    registered.Room,
                    registered.RegisteredAt),
                InvalidStudentRegistration invalid => new StudentRegistrationFailedEvent(
                    invalid.StudentRegistrationNumber,
                    invalid.Reasons),
                UnvalidatedStudentRegistration unvalidated => new StudentRegistrationFailedEvent(
                    unvalidated.StudentRegistrationNumber,
                    new[] { "Student registration was not completed - remained in unvalidated state" }),
                ValidatedStudentRegistration validated => new StudentRegistrationFailedEvent(
                    validated.Student.Value,
                    new[] { "Student registration was not completed - remained in validated state" }),
                CheckedStudentRegistration checkedReg => new StudentRegistrationFailedEvent(
                    checkedReg.Student.Value,
                    new[] { "Student registration was not completed - remained in checked state" }),
                _ => new StudentRegistrationFailedEvent(
                    "Unknown",
                    new[] { $"Unknown student registration state: {registration.GetType().Name}" })
            };
        }
    }
}
