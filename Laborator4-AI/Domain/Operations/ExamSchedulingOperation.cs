namespace Laborator4_AI.Domain.Operations
{
    using Laborator4_AI.Domain.Models.Entities;

    /// <summary>
    /// Base class for ExamScheduling operations using Transform pattern
    /// Each operation uses pattern matching to handle different states
    /// </summary>
    internal abstract class ExamSchedulingOperation
    {
        internal IExamScheduling Transform(IExamScheduling scheduling)
        {
            return scheduling switch
            {
                UnvalidatedExamScheduling unvalidated => OnUnvalidated(unvalidated),
                ValidatedExamScheduling validated => OnValidated(validated),
                RoomAllocatedExamScheduling allocated => OnRoomAllocated(allocated),
                PublishedExamScheduling published => OnPublished(published),
                InvalidExamScheduling invalid => OnInvalid(invalid),
                _ => throw new InvalidOperationException($"Unknown exam scheduling state: {scheduling.GetType().Name}")
            };
        }

        // Virtual methods - override only what is needed
        protected virtual IExamScheduling OnUnvalidated(UnvalidatedExamScheduling scheduling) => scheduling;
        protected virtual IExamScheduling OnValidated(ValidatedExamScheduling scheduling) => scheduling;
        protected virtual IExamScheduling OnRoomAllocated(RoomAllocatedExamScheduling scheduling) => scheduling;
        protected virtual IExamScheduling OnPublished(PublishedExamScheduling scheduling) => scheduling;
        protected virtual IExamScheduling OnInvalid(InvalidExamScheduling scheduling) => scheduling;
    }
}
