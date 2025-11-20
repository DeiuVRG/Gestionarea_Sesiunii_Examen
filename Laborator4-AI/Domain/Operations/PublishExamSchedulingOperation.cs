namespace Laborator4_AI.Domain.Operations
{
    using System;
    using Laborator4_AI.Domain.Models.Entities;
    using Laborator4_AI.Domain.Models.ValueObjects;

    /// <summary>
    /// Operation to publish the exam scheduling
    /// No external dependencies - just transitions from RoomAllocated to Published
    /// </summary>
    internal sealed class PublishExamSchedulingOperation : ExamSchedulingOperation
    {
        protected override IExamScheduling OnRoomAllocated(RoomAllocatedExamScheduling scheduling)
        {
            // Publish the exam with initially 1 enrolled student (minimum for FromInt validation)
            // In real scenario, this would be tracked properly
            return new PublishedExamScheduling(
                scheduling.Course,
                scheduling.SelectedDate,
                scheduling.Duration,
                scheduling.Room,
                scheduling.RoomCapacity,
                Capacity.FromInt(1),
                DateTime.Now);
        }
    }
}
