namespace Laborator4_AI.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when a course code does not match the required format
    /// Valid format: 2-4 uppercase letters optionally followed by a single digit (e.g., PSSC, BD, MATH1)
    /// </summary>
    public sealed class InvalidCourseCodeException : DomainException
    {
        public string InvalidValue { get; }

        public InvalidCourseCodeException(string invalidValue) 
            : base($"Invalid course code format: '{invalidValue}'. Expected 2-4 uppercase letters optionally followed by a single digit (e.g., PSSC, BD, MATH1).")
        {
            InvalidValue = invalidValue;
        }
    }
}
