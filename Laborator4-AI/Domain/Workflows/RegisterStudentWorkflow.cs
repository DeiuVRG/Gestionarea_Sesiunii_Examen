namespace Laborator4_AI.Domain.Workflows
{
    using System;
    using Laborator4_AI.Domain.Models.Commands;
    using Laborator4_AI.Domain.Models.Events;
    using Laborator4_AI.Domain.Operations;
    using Laborator4_AI.Domain.Models.Entities;
    using Laborator4_AI.Domain.Models.ValueObjects;

    /// <summary>
    /// Workflow for registering a student to an exam
    /// Composes operations: Validate → Check → Register
    /// </summary>
    public sealed class RegisterStudentWorkflow
    {
        public IStudentRegistrationEvent Execute(
            RegisterStudentCommand command,
            Func<StudentRegistrationNumber, bool> checkStudentExists,
            Func<CourseCode, ExamDate, (bool exists, RoomNumber? room)> checkExamExists,
            Func<StudentRegistrationNumber, ExamDate, int> getStudentExamsOnDate,
            Func<CourseCode, ExamDate, RoomNumber?> getExamRoom,
            Func<StudentRegistrationNumber, CourseCode, ExamDate, bool> checkAlreadyRegistered,
            Func<StudentRegistrationNumber, CourseCode, ExamDate, RoomNumber, bool> persistRegistration)
        {
            // 1. Create unvalidated state from command
            IStudentRegistration registration = new UnvalidatedStudentRegistration(
                command.StudentRegistrationNumber,
                command.CourseCode,
                command.ExamDate);

            // 2. Pipeline of operations using Transform
            registration = new ValidateStudentRegistrationOperation(checkStudentExists, checkExamExists)
                .Transform(registration);

            registration = new CheckStudentRegistrationOperation(
                    getStudentExamsOnDate,
                    getExamRoom,
                    checkAlreadyRegistered)
                .Transform(registration);

            registration = new RegisterStudentOperation(persistRegistration)
                .Transform(registration);

            // 3. Convert final state to event
            return registration.ToEvent();
        }
    }
}
