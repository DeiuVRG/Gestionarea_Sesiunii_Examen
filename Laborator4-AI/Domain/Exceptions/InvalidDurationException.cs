namespace Laborator4_AI.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when an exam duration is invalid
    /// Duration must be positive and in a valid format (minutes as int or TimeSpan)
    /// </summary>
    public sealed class InvalidDurationException : DomainException
    {
        public string InvalidValue { get; }

        public InvalidDurationException(string invalidValue, string reason) 
            : base($"Invalid duration: '{invalidValue}'. Reason: {reason}")
        {
            InvalidValue = invalidValue;
        }

        public InvalidDurationException(string reason) 
            : base($"Invalid duration. Reason: {reason}")
        {
            InvalidValue = string.Empty;
        }
    }
}
