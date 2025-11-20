namespace Laborator4_AI.Domain.Operations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Laborator4_AI.Domain.Models.Entities;
    using Laborator4_AI.Domain.Models.ValueObjects;

    /// <summary>
    /// Operation to check business rules for student registration
    /// Dependencies:
    /// - getStudentExamsOnDate: Gets all exams student is registered for on given date (max 2 per day rule)
    /// - getExamRoom: Gets the room number for the exam
    /// - checkAlreadyRegistered: Checks if student is already registered for this exam
    /// </summary>
    internal sealed class CheckStudentRegistrationOperation : StudentRegistrationOperation
    {
        private readonly Func<StudentRegistrationNumber, ExamDate, int> _getStudentExamsOnDate;
        private readonly Func<CourseCode, ExamDate, RoomNumber?> _getExamRoom;
        private readonly Func<StudentRegistrationNumber, CourseCode, ExamDate, bool> _checkAlreadyRegistered;

        public CheckStudentRegistrationOperation(
            Func<StudentRegistrationNumber, ExamDate, int> getStudentExamsOnDate,
            Func<CourseCode, ExamDate, RoomNumber?> getExamRoom,
            Func<StudentRegistrationNumber, CourseCode, ExamDate, bool> checkAlreadyRegistered)
        {
            _getStudentExamsOnDate = getStudentExamsOnDate;
            _getExamRoom = getExamRoom;
            _checkAlreadyRegistered = checkAlreadyRegistered;
        }

        protected override IStudentRegistration OnValidated(ValidatedStudentRegistration registration)
        {
            var errors = new List<string>();

            // 1. Check if already registered
            if (_checkAlreadyRegistered(registration.Student, registration.Course, registration.Date))
            {
                errors.Add($"Student {registration.Student} is already registered for this exam");
            }

            // 2. Check max 2 exams per day rule
            var examsOnSameDay = _getStudentExamsOnDate(registration.Student, registration.Date);
            if (examsOnSameDay >= 2)
            {
                errors.Add($"Student {registration.Student} already has {examsOnSameDay} exams on {registration.Date} (maximum 2 per day)");
            }

            // 3. Get exam room
            var room = _getExamRoom(registration.Course, registration.Date);
            if (room == null)
            {
                errors.Add($"Exam room not found for course {registration.Course} on {registration.Date}");
            }

            // 4. Return checked or invalid state
            if (errors.Any() || room == null)
            {
                return new InvalidStudentRegistration(registration.Student.Value, errors);
            }

            return new CheckedStudentRegistration(
                registration.Student,
                registration.Course,
                registration.Date,
                room!,
                examsOnSameDay);
        }
    }
}
