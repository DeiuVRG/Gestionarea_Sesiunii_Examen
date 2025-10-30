namespace Laborator4_AI.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when a capacity value is invalid
    /// Capacity must be a positive integer
    /// </summary>
    public sealed class InvalidCapacityException : DomainException
    {
        public string InvalidValue { get; }

        public InvalidCapacityException(string invalidValue, string reason) 
            : base($"Invalid capacity: '{invalidValue}'. Reason: {reason}")
        {
            InvalidValue = invalidValue;
        }

        public InvalidCapacityException(int invalidValue) 
            : base($"Invalid capacity: {invalidValue}. Capacity must be a positive integer.")
        {
            InvalidValue = invalidValue.ToString();
        }
    }
}
