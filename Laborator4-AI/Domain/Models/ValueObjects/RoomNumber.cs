namespace Laborator4_AI.Domain.Models.ValueObjects
{
    using System;

    /// <summary>
    /// Value object for RoomNumber representing a university room identifier
    /// Validation rules:
    /// - Format: Building(A-D) + Floor(0-4) + Room number(01-99)
    /// - Valid examples: "A301", "B205", "C110"
    /// - Invalid examples: "Z301" (invalid building), "A5" (too short), "A801" (invalid floor)
    /// </summary>
    public sealed class RoomNumber
    {
        public string Value { get; }
        
        private RoomNumber(string value) => Value = value;

        public static bool TryCreate(string? input, out RoomNumber? room, out string? error)
        {
            room = null;
            error = null;
            
            if (string.IsNullOrWhiteSpace(input))
            {
                error = "Room number must not be empty";
                return false;
            }
            
            var s = input.Trim().ToUpperInvariant();
            
            if (!System.Text.RegularExpressions.Regex.IsMatch(s, @"^[A-D][0-4](0[1-9]|[1-9][0-9])$"))
            {
                error = "Invalid room number. Expected format Building(A-D)+Floor(0-4)+Room(01-99) e.g. A301";
                return false;
            }
            
            room = new RoomNumber(s);
            return true;
        }

        public override string ToString() => Value;
        
        public override bool Equals(object? obj) => obj is RoomNumber other && Value == other.Value;
        
        public override int GetHashCode() => Value.GetHashCode();
    }
}
