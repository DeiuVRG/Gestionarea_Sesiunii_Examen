namespace Laborator4_AI.Domain.Models.ValueObjects
{
    using System;

    /// <summary>
    /// Value object for StudentRegistrationNumber representing a student's unique identifier
    /// Validation rules:
    /// - Format: "LM" followed by 5 digits
    /// - Valid examples: "LM12345", "LM00001", "LM99999"
    /// - Invalid examples: "lm12345" (lowercase), "LM123" (too short), "LM123456" (too long), "AB12345" (wrong prefix)
    /// </summary>
    public sealed class StudentRegistrationNumber
    {
        public string Value { get; }
        
        private StudentRegistrationNumber(string value) => Value = value;

        public static bool TryCreate(string? input, out StudentRegistrationNumber? regNumber, out string? error)
        {
            regNumber = null;
            error = null;
            
            if (string.IsNullOrWhiteSpace(input))
            {
                error = "Student registration number must not be empty";
                return false;
            }
            
            var s = input.Trim().ToUpperInvariant();
            
            if (!System.Text.RegularExpressions.Regex.IsMatch(s, @"^LM\d{5}$"))
            {
                error = "Invalid student registration number format. Expected 'LM' followed by 5 digits (e.g. LM12345)";
                return false;
            }
            
            regNumber = new StudentRegistrationNumber(s);
            return true;
        }

        public override string ToString() => Value;
        
        public override bool Equals(object? obj) => obj is StudentRegistrationNumber other && Value == other.Value;
        
        public override int GetHashCode() => Value.GetHashCode();
    }
}
