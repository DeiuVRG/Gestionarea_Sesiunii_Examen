namespace Laborator4_AI.Domain.Operations
{
    using Laborator4_AI.Domain.Models.Entities;

    /// <summary>
    /// Base class for Contestation operations using Transform pattern
    /// </summary>
    internal abstract class ContestationOperation
    {
        internal IContestation Transform(IContestation contestation)
        {
            return contestation switch
            {
                UnvalidatedContestation unvalidated => OnUnvalidated(unvalidated),
                ValidatedContestation validated => OnValidated(validated),
                CheckedContestation checkedContest => OnChecked(checkedContest),
                FiledContestation filed => OnFiled(filed),
                InvalidContestation invalid => OnInvalid(invalid),
                _ => throw new InvalidOperationException($"Unknown contestation state: {contestation.GetType().Name}")
            };
        }

        protected virtual IContestation OnUnvalidated(UnvalidatedContestation contestation) => contestation;
        protected virtual IContestation OnValidated(ValidatedContestation contestation) => contestation;
        protected virtual IContestation OnChecked(CheckedContestation contestation) => contestation;
        protected virtual IContestation OnFiled(FiledContestation contestation) => contestation;
        protected virtual IContestation OnInvalid(InvalidContestation contestation) => contestation;
    }
}
