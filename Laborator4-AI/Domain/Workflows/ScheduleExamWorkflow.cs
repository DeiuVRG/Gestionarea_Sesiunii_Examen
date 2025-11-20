namespace Laborator4_AI.Domain.Workflows
{
    using System;
    using System.Collections.Generic;
    using Laborator4_AI.Domain.Models.Commands;
    using Laborator4_AI.Domain.Models.Events;
    using Laborator4_AI.Domain.Operations;
    using Laborator4_AI.Domain.Models.Entities;
    using Laborator4_AI.Domain.Models.ValueObjects;

    /// <summary>
    /// Workflow for scheduling an exam
    /// Composes operations: Validate → AllocateRoom → Publish
    /// </summary>
    public sealed class ScheduleExamWorkflow
    {
        public IExamSchedulingEvent Execute(
            ScheduleExamCommand command,
            Func<CourseCode, bool> checkCourseExists,
            Func<CourseCode, DateTime> getCourseEndDate,
            Func<ExamDate, Duration, Capacity, IEnumerable<RoomNumber>> findAvailableRooms,
            Func<RoomNumber, ExamDate, Duration, bool> reserveRoom)
        {
            // 1. Create unvalidated state from command
            IExamScheduling scheduling = new UnvalidatedExamScheduling(
                command.CourseCode,
                command.ProposedDate1,
                command.ProposedDate2,
                command.ProposedDate3,
                command.Duration,
                command.ExpectedStudents);

            // 2. Pipeline of operations using Transform
            scheduling = new ValidateExamSchedulingOperation(checkCourseExists, getCourseEndDate)
                .Transform(scheduling);

            scheduling = new AllocateRoomOperation(findAvailableRooms, reserveRoom)
                .Transform(scheduling);

            scheduling = new PublishExamSchedulingOperation()
                .Transform(scheduling);

            // 3. Convert final state to event
            return scheduling.ToEvent();
        }
    }
}
