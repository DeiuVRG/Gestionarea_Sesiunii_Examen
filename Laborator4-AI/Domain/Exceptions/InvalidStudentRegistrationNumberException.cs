namespace Laborator4_AI.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when a student registration number does not match the required format
    /// Valid format: "LM" followed by exactly 5 digits (e.g., LM12345)
    /// </summary>
    public sealed class InvalidStudentRegistrationNumberException : DomainException
    {
        public string InvalidValue { get; }

        public InvalidStudentRegistrationNumberException(string invalidValue) 
            : base($"Invalid student registration number format: '{invalidValue}'. Expected 'LM' followed by 5 digits (e.g., LM12345).")
        {
            InvalidValue = invalidValue;
        }
    }
}
