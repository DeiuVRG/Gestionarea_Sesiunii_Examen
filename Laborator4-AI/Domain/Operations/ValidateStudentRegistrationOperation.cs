namespace Laborator4_AI.Domain.Operations
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Laborator4_AI.Domain.Models.Entities;
    using Laborator4_AI.Domain.Models.ValueObjects;

    /// <summary>
    /// Operation to validate student registration
    /// Dependencies:
    /// - checkStudentExists: Verifies student exists
    /// - checkExamExists: Verifies exam was scheduled for this course and date
    /// </summary>
    internal sealed class ValidateStudentRegistrationOperation : StudentRegistrationOperation
    {
        private readonly Func<StudentRegistrationNumber, bool> _checkStudentExists;
        private readonly Func<CourseCode, ExamDate, (bool exists, RoomNumber? room)> _checkExamExists;

        public ValidateStudentRegistrationOperation(
            Func<StudentRegistrationNumber, bool> checkStudentExists,
            Func<CourseCode, ExamDate, (bool exists, RoomNumber? room)> checkExamExists)
        {
            _checkStudentExists = checkStudentExists;
            _checkExamExists = checkExamExists;
        }

        protected override IStudentRegistration OnUnvalidated(UnvalidatedStudentRegistration registration)
        {
            var errors = new List<string>();

            // 1. Parse and validate student registration number
            if (!StudentRegistrationNumber.TryCreate(registration.StudentRegistrationNumber, out var studentRegNum, out var studentError))
            {
                errors.Add(studentError ?? "Invalid student registration number");
            }
            else
            {
                // 2. Check if student exists
                if (!_checkStudentExists(studentRegNum!))
                {
                    errors.Add($"Student '{studentRegNum!.Value}' does not exist");
                }
            }

            // 3. Parse and validate course code
            if (!CourseCode.TryCreate(registration.CourseCode, out var courseCode, out var courseError))
            {
                errors.Add(courseError ?? "Invalid course code");
            }

            // 4. Parse and validate exam date
            ExamDate? examDate = null;
            if (!DateTime.TryParse(registration.ExamDate, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dt))
            {
                errors.Add($"Exam date '{registration.ExamDate}' is not a valid date");
            }
            else if (!ExamDate.TryCreate(dt, out examDate, out var dateError))
            {
                errors.Add($"Exam date: {dateError}");
            }

            // 5. Check if exam exists for this course and date
            RoomNumber? room = null;
            if (courseCode != null && examDate != null)
            {
                var (exists, examRoom) = _checkExamExists(courseCode!, examDate!);
                if (!exists)
                {
                    errors.Add($"No exam scheduled for course '{courseCode!.Value}' on {examDate}");
                }
                room = examRoom;
            }

            // 6. Return validated or invalid state
            if (errors.Any() || room == null)
            {
                if (room == null && !errors.Any())
                {
                    errors.Add("Exam room not found");
                }
                return new InvalidStudentRegistration(registration.StudentRegistrationNumber, errors);
            }

            return new ValidatedStudentRegistration(studentRegNum!, courseCode!, examDate!);
        }
    }
}
