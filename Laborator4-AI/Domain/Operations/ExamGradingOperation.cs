namespace Laborator4_AI.Domain.Operations
{
    using Laborator4_AI.Domain.Models.Entities;

    /// <summary>
    /// Base class for ExamGrading operations using Transform pattern
    /// </summary>
    internal abstract class ExamGradingOperation
    {
        internal IExamGrading Transform(IExamGrading grading)
        {
            return grading switch
            {
                UnvalidatedExamGrading unvalidated => OnUnvalidated(unvalidated),
                ValidatedExamGrading validated => OnValidated(validated),
                PublishedExamGrading published => OnPublished(published),
                InvalidExamGrading invalid => OnInvalid(invalid),
                _ => throw new InvalidOperationException($"Unknown exam grading state: {grading.GetType().Name}")
            };
        }

        protected virtual IExamGrading OnUnvalidated(UnvalidatedExamGrading grading) => grading;
        protected virtual IExamGrading OnValidated(ValidatedExamGrading grading) => grading;
        protected virtual IExamGrading OnPublished(PublishedExamGrading grading) => grading;
        protected virtual IExamGrading OnInvalid(InvalidExamGrading grading) => grading;
    }
}
