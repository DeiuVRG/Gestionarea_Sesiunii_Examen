namespace Laborator4_AI.Domain.Operations
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Laborator4_AI.Domain.Models.Entities;
    using Laborator4_AI.Domain.Models.ValueObjects;

    /// <summary>
    /// Operation to validate exam grading
    /// Dependencies:
    /// - checkExamExists: Verifies exam exists
    /// - checkStudentRegistered: Verifies each student is registered for the exam
    /// </summary>
    internal sealed class ValidateExamGradingOperation : ExamGradingOperation
    {
        private readonly Func<CourseCode, ExamDate, bool> _checkExamExists;
        private readonly Func<StudentRegistrationNumber, CourseCode, ExamDate, bool> _checkStudentRegistered;

        public ValidateExamGradingOperation(
            Func<CourseCode, ExamDate, bool> checkExamExists,
            Func<StudentRegistrationNumber, CourseCode, ExamDate, bool> checkStudentRegistered)
        {
            _checkExamExists = checkExamExists;
            _checkStudentRegistered = checkStudentRegistered;
        }

        protected override IExamGrading OnUnvalidated(UnvalidatedExamGrading grading)
        {
            var errors = new List<string>();

            // 1. Parse and validate course code
            if (!CourseCode.TryCreate(grading.CourseCode, out var courseCode, out var courseError))
            {
                errors.Add(courseError ?? "Invalid course code");
            }

            // 2. Parse and validate exam date
            ExamDate? examDate = null;
            if (!DateTime.TryParse(grading.ExamDate, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dt))
            {
                errors.Add($"Exam date '{grading.ExamDate}' is not a valid date");
            }
            else if (!ExamDate.TryCreate(dt, out examDate, out var dateError))
            {
                errors.Add($"Exam date: {dateError}");
            }

            // 3. Check if exam exists
            if (courseCode != null && examDate != null)
            {
                if (!_checkExamExists(courseCode!, examDate!))
                {
                    errors.Add($"No exam found for course '{courseCode!.Value}' on {examDate}");
                }
            }

            // 4. Parse and validate student grades
            var validatedGrades = new List<StudentGrade>();
            foreach (var sg in grading.StudentGrades)
            {
                if (!StudentRegistrationNumber.TryCreate(sg.StudentRegistrationNumber, out var studentRegNum, out var studentError))
                {
                    errors.Add($"Student: {studentError}");
                    continue;
                }

                if (!Grade.TryCreate(sg.Grade, out var grade, out var gradeError))
                {
                    errors.Add($"Grade for student {sg.StudentRegistrationNumber}: {gradeError}");
                    continue;
                }

                // Check if student is registered for this exam
                if (courseCode != null && examDate != null)
                {
                    if (!_checkStudentRegistered(studentRegNum!, courseCode!, examDate!))
                    {
                        errors.Add($"Student {studentRegNum!.Value} is not registered for this exam");
                        continue;
                    }
                }

                validatedGrades.Add(new StudentGrade(studentRegNum!, grade!));
            }

            if (validatedGrades.Count == 0 && grading.StudentGrades.Count > 0)
            {
                errors.Add("No valid student grades provided");
            }

            // 5. Return validated or invalid state
            if (errors.Any())
            {
                return new InvalidExamGrading(grading.CourseCode, errors);
            }

            return new ValidatedExamGrading(courseCode!, examDate!, validatedGrades.AsReadOnly());
        }
    }
}
