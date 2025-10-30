namespace Laborator4_AI.Domain.Models.ValueObjects
{
    using System;

    /// <summary>
    /// Value object for Grade representing a student's exam grade
    /// Validation rules:
    /// - Must be between 1.00 and 10.00
    /// - Decimal precision up to 2 places
    /// - Valid examples: "5.00", "10.00", "7.50", "8"
    /// - Invalid examples: "0", "11", "-5", "abc"
    /// </summary>
    public sealed class Grade
    {
        public decimal Value { get; }
        
        private Grade(decimal v) => Value = v;

        public static bool TryCreate(string? input, out Grade? grade, out string? error)
        {
            grade = null;
            error = null;
            
            if (string.IsNullOrWhiteSpace(input))
            {
                error = "Grade must not be empty";
                return false;
            }
            
            if (!decimal.TryParse(input.Trim(), out var v))
            {
                error = "Grade must be a valid number";
                return false;
            }
            
            if (v < 1.00m || v > 10.00m)
            {
                error = "Grade must be between 1.00 and 10.00";
                return false;
            }
            
            // Round to 2 decimal places
            v = Math.Round(v, 2);
            
            grade = new Grade(v);
            return true;
        }

        public static Grade FromDecimal(decimal v)
        {
            if (v < 1.00m || v > 10.00m)
                throw new ArgumentException("Grade must be between 1.00 and 10.00", nameof(v));
            return new Grade(Math.Round(v, 2));
        }

        public bool IsPassing() => Value >= 5.00m;

        public override string ToString() => Value.ToString("F2");
        
        public override bool Equals(object? obj) => obj is Grade other && Value == other.Value;
        
        public override int GetHashCode() => Value.GetHashCode();
    }
}
