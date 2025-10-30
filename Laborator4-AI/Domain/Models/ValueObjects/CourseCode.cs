namespace Laborator4_AI.Domain.Models.ValueObjects
{
    using System;

    /// <summary>
    /// Value object for CourseCode representing the unique identifier of a university course
    /// Validation rules:
    /// - Format: 2-4 uppercase letters optionally followed by a digit
    /// - Examples: "PSSC", "BD", "POO2", "MATH"
    /// - Must not be empty or whitespace
    /// </summary>
    public sealed class CourseCode
    {
        public string Value { get; }
        
        private CourseCode(string value) => Value = value;

        public static bool TryCreate(string? input, out CourseCode? courseCode, out string? error)
        {
            courseCode = null;
            error = null;
            
            if (string.IsNullOrWhiteSpace(input))
            {
                error = "Course code must not be empty";
                return false;
            }
            
            var s = input.Trim().ToUpperInvariant();
            
            if (!System.Text.RegularExpressions.Regex.IsMatch(s, @"^[A-Z]{2,4}\d?$"))
            {
                error = "Invalid course code format. Expected 2-4 uppercase letters optionally followed by a single digit (e.g. PSSC, BD, MATH1)";
                return false;
            }
            
            courseCode = new CourseCode(s);
            return true;
        }

        public override string ToString() => Value;
        
        public override bool Equals(object? obj) => obj is CourseCode other && Value == other.Value;
        
        public override int GetHashCode() => Value.GetHashCode();
    }
}
