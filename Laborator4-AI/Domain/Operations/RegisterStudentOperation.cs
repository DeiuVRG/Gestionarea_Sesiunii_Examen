namespace Laborator4_AI.Domain.Operations
{
    using System;
    using Laborator4_AI.Domain.Models.Entities;
    using Laborator4_AI.Domain.Models.ValueObjects;

    /// <summary>
    /// Operation to register student (final step)
    /// Dependencies:
    /// - persistRegistration: Persists the registration
    /// </summary>
    internal sealed class RegisterStudentOperation : StudentRegistrationOperation
    {
        private readonly Func<StudentRegistrationNumber, CourseCode, ExamDate, RoomNumber, bool> _persistRegistration;

        public RegisterStudentOperation(
            Func<StudentRegistrationNumber, CourseCode, ExamDate, RoomNumber, bool> persistRegistration)
        {
            _persistRegistration = persistRegistration;
        }

        protected override IStudentRegistration OnChecked(CheckedStudentRegistration registration)
        {
            // Persist the registration
            var success = _persistRegistration(
                registration.Student,
                registration.Course,
                registration.Date,
                registration.Room);

            if (!success)
            {
                return new InvalidStudentRegistration(
                    registration.Student.Value,
                    new[] { "Failed to persist student registration" });
            }

            return new RegisteredStudentRegistration(
                registration.Student,
                registration.Course,
                registration.Date,
                registration.Room,
                DateTime.Now);
        }
    }
}
