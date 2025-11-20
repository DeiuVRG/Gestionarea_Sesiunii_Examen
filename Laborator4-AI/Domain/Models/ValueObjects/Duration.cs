namespace Laborator4_AI.Domain.Models.ValueObjects
{
    using System;

    /// <summary>
    /// Value object for Duration representing exam duration
    /// Validation rules:
    /// - Must be positive
    /// - Can be specified as minutes (integer) or TimeSpan format (hh:mm)
    /// - Valid examples: "120", "02:00", "90"
    /// - Invalid examples: "0", "-30", "abc"
    /// </summary>
    public sealed class Duration
    {
        public TimeSpan Value { get; }
        
        private Duration(TimeSpan t) => Value = t;

        public static bool TryCreate(string? input, out Duration? duration, out string? error)
        {
            duration = null;
            error = null;
            
            if (string.IsNullOrWhiteSpace(input))
            {
                error = "Duration must not be empty";
                return false;
            }
            
            var s = input.Trim();
            
            // Try parsing as TimeSpan first
            if (TimeSpan.TryParse(s, out var ts))
            {
                if (ts <= TimeSpan.Zero)
                {
                    error = "Duration must be positive";
                    return false;
                }
                duration = new Duration(ts);
                return true;
            }
            
            // Try parsing as minutes
            if (int.TryParse(s, out var minutes) && minutes > 0)
            {
                duration = new Duration(TimeSpan.FromMinutes(minutes));
                return true;
            }
            
            error = "Invalid duration format. Use 'hh:mm' or minutes as integer";
            return false;
        }

        public override string ToString() => Value.ToString();
        
        public override bool Equals(object? obj) => obj is Duration other && Value == other.Value;
        
        public override int GetHashCode() => Value.GetHashCode();
    }
}
