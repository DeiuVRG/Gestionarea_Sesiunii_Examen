namespace Laborator4_AI.Domain.Models.ValueObjects
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Value object for ExamDate representing a valid exam date
    /// Validation rules:
    /// - Must be a future date
    /// - Must be within exam sessions: June 1 - July 15 OR January 15 - February 28
    /// - Cannot be on a weekend (Saturday or Sunday)
    /// </summary>
    public sealed class ExamDate
    {
        public DateTime Date { get; }
        
        private ExamDate(DateTime date) => Date = date.Date;

        public static bool TryCreate(DateTime inputDate, out ExamDate? examDate, out string? error)
        {
            examDate = null;
            error = null;
            
            var date = inputDate.Date;
            var now = DateTime.Now.Date;
            
            if (date <= now)
            {
                error = "Exam date must be a future date";
                return false;
            }
            
            bool summer = date >= new DateTime(date.Year, 6, 1) && date <= new DateTime(date.Year, 7, 15);
            bool winter = date >= new DateTime(date.Year, 1, 15) && date <= new DateTime(date.Year, 2, 28);
            
            if (!(summer || winter))
            {
                error = "Exam date must be within exam sessions: June 1 - July 15 OR January 15 - February 28";
                return false;
            }
            
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            {
                error = "Exam date cannot be on a weekend (Saturday or Sunday)";
                return false;
            }
            
            examDate = new ExamDate(date);
            return true;
        }

        public bool IsInSameWeek(ExamDate other)
        {
            var cal = CultureInfo.InvariantCulture.Calendar;
            var weekRule = CultureInfo.InvariantCulture.DateTimeFormat.CalendarWeekRule;
            var d1 = cal.GetWeekOfYear(this.Date, weekRule, DayOfWeek.Monday);
            var d2 = cal.GetWeekOfYear(other.Date, weekRule, DayOfWeek.Monday);
            return this.Date.Year == other.Date.Year && d1 == d2;
        }

        public override string ToString() => Date.ToString("yyyy-MM-dd");
        
        public override bool Equals(object? obj) => obj is ExamDate other && Date == other.Date;
        
        public override int GetHashCode() => Date.GetHashCode();
    }
}
