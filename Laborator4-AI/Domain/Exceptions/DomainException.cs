namespace Laborator4_AI.Domain.Exceptions
{
    using System;

    /// <summary>
    /// Base class for all domain-specific exceptions
    /// Represents violations of domain rules and invariants
    /// </summary>
    public abstract class DomainException : Exception
    {
        protected DomainException(string message) : base(message)
        {
        }

        protected DomainException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
