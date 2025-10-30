namespace Laborator4_AI.Domain.Operations
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Laborator4_AI.Domain.Models.Entities;
    using Laborator4_AI.Domain.Models.ValueObjects;

    /// <summary>
    /// Operation to validate contestation and check 48h window
    /// Dependencies:
    /// - checkStudentRegistered: Verifies student is registered for exam
    /// - getGradesPublishedDate: Gets when grades were published (null if not published)
    /// </summary>
    internal sealed class ValidateContestationOperation : ContestationOperation
    {
        private readonly Func<StudentRegistrationNumber, CourseCode, ExamDate, bool> _checkStudentRegistered;
        private readonly Func<CourseCode, ExamDate, DateTime?> _getGradesPublishedDate;

        public ValidateContestationOperation(
            Func<StudentRegistrationNumber, CourseCode, ExamDate, bool> checkStudentRegistered,
            Func<CourseCode, ExamDate, DateTime?> getGradesPublishedDate)
        {
            _checkStudentRegistered = checkStudentRegistered;
            _getGradesPublishedDate = getGradesPublishedDate;
        }

        protected override IContestation OnUnvalidated(UnvalidatedContestation contestation)
        {
            var errors = new List<string>();

            // 1. Parse and validate student registration number
            if (!StudentRegistrationNumber.TryCreate(contestation.StudentRegistrationNumber, out var studentRegNum, out var studentError))
            {
                errors.Add(studentError ?? "Invalid student registration number");
            }

            // 2. Parse and validate course code
            if (!CourseCode.TryCreate(contestation.CourseCode, out var courseCode, out var courseError))
            {
                errors.Add(courseError ?? "Invalid course code");
            }

            // 3. Parse and validate exam date
            ExamDate? examDate = null;
            if (!DateTime.TryParse(contestation.ExamDate, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dt))
            {
                errors.Add($"Exam date '{contestation.ExamDate}' is not a valid date");
            }
            else if (!ExamDate.TryCreate(dt, out examDate, out var dateError))
            {
                errors.Add($"Exam date: {dateError}");
            }

            // 4. Validate reason
            if (string.IsNullOrWhiteSpace(contestation.Reason))
            {
                errors.Add("Contestation reason must not be empty");
            }

            // 5. Check if student is registered for this exam
            if (studentRegNum != null && courseCode != null && examDate != null)
            {
                if (!_checkStudentRegistered(studentRegNum!, courseCode!, examDate!))
                {
                    errors.Add($"Student {studentRegNum!.Value} is not registered for exam {courseCode!.Value} on {examDate}");
                }
            }

            // 6. Return validated or invalid state
            if (errors.Any())
            {
                return new InvalidContestation(contestation.StudentRegistrationNumber, errors);
            }

            return new ValidatedContestation(studentRegNum!, courseCode!, examDate!, contestation.Reason);
        }
    }
}
