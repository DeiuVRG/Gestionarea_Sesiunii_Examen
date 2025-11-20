namespace Laborator4_AI.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when a grade value is outside the valid range
    /// Valid range: 1.00 to 10.00 with up to 2 decimal places
    /// </summary>
    public sealed class InvalidGradeException : DomainException
    {
        public string InvalidValue { get; }

        public InvalidGradeException(string invalidValue, string reason) 
            : base($"Invalid grade: '{invalidValue}'. Reason: {reason}")
        {
            InvalidValue = invalidValue;
        }

        public InvalidGradeException(decimal invalidValue) 
            : base($"Invalid grade: {invalidValue}. Grade must be between 1.00 and 10.00.")
        {
            InvalidValue = invalidValue.ToString();
        }
    }
}
