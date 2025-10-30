namespace Laborator4_AI.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when a room number does not match the required format
    /// Valid format: Building(A-D) + Floor(0-4) + Room(01-99) (e.g., A301, B205)
    /// </summary>
    public sealed class InvalidRoomNumberException : DomainException
    {
        public string InvalidValue { get; }

        public InvalidRoomNumberException(string invalidValue) 
            : base($"Invalid room number format: '{invalidValue}'. Expected Building(A-D) + Floor(0-4) + Room(01-99), e.g., A301, B205.")
        {
            InvalidValue = invalidValue;
        }
    }
}
