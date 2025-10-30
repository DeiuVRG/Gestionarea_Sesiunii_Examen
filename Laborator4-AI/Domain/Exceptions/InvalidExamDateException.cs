namespace Laborator4_AI.Domain.Exceptions
{
    using System;

    /// <summary>
    /// Exception thrown when an exam date violates business rules
    /// Rules: Must be in the future, during valid exam sessions, and not on weekends
    /// </summary>
    public sealed class InvalidExamDateException : DomainException
    {
        public DateTime? InvalidDate { get; }
        public string Reason { get; }

        public InvalidExamDateException(DateTime? invalidDate, string reason) 
            : base($"Invalid exam date: {invalidDate?.ToString("yyyy-MM-dd") ?? "null"}. Reason: {reason}")
        {
            InvalidDate = invalidDate;
            Reason = reason;
        }

        public InvalidExamDateException(string reason) 
            : base($"Invalid exam date. Reason: {reason}")
        {
            Reason = reason;
        }
    }
}
