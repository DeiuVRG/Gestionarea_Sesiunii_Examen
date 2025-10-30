namespace Laborator4_AI.Domain.Models.ValueObjects
{
    using System;

    /// <summary>
    /// Value object for Capacity representing number of students, room capacity, etc.
    /// Validation rules:
    /// - Must be a positive integer
    /// - Valid examples: "20", "100", "1"
    /// - Invalid examples: "0", "-5", "abc"
    /// </summary>
    public sealed class Capacity
    {
        public int Value { get; }
        
        private Capacity(int v) => Value = v;

        public static bool TryCreate(string? input, out Capacity? capacity, out string? error)
        {
            capacity = null;
            error = null;
            
            if (string.IsNullOrWhiteSpace(input))
            {
                error = "Capacity must not be empty";
                return false;
            }
            
            if (!int.TryParse(input.Trim(), out var v) || v <= 0)
            {
                error = "Capacity must be a positive integer";
                return false;
            }
            
            capacity = new Capacity(v);
            return true;
        }

        public static Capacity FromInt(int v)
        {
            if (v <= 0)
                throw new ArgumentException("Capacity must be positive", nameof(v));
            return new Capacity(v);
        }

        public override string ToString() => Value.ToString();
        
        public override bool Equals(object? obj) => obj is Capacity other && Value == other.Value;
        
        public override int GetHashCode() => Value.GetHashCode();
    }
}
