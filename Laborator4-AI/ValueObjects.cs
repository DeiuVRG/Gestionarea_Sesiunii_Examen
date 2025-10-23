namespace Laborator4_AI
{
    using System;
    using System.Globalization;

    public sealed class CourseCode
    {
        public string Value { get; }
        private CourseCode(string value) => Value = value;

        public static bool TryCreate(string? input, out CourseCode? courseCode, out string? error)
        {
            courseCode = null;
            error = null;
            if (string.IsNullOrWhiteSpace(input)) { error = "Course code must not be empty"; return false; }
            var s = input.Trim();
            if (!System.Text.RegularExpressions.Regex.IsMatch(s, @"^[A-Z]{2,4}\d?$"))
            {
                error = "Invalid course code format. Expected 2-4 uppercase letters optionally followed by a single digit (e.g. PSSC, BD, MATH1).";
                return false;
            }
            courseCode = new CourseCode(s);
            return true;
        }

        public override string ToString() => Value;
    }

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
                error = "Exam date must be a future date.";
                return false;
            }
            bool summer = date >= new DateTime(date.Year, 6, 1) && date <= new DateTime(date.Year, 7, 15);
            bool winter = date >= new DateTime(date.Year, 1, 15) && date <= new DateTime(date.Year, 2, 28);
            if (!(summer || winter))
            {
                error = "Exam date must be within exam sessions: June 1 - July 15 OR January 15 - February 28.";
                return false;
            }
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            {
                error = "Exam date cannot be on a weekend (Saturday or Sunday).";
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
    }

    public sealed class RoomNumber
    {
        public string Value { get; }
        private RoomNumber(string value) => Value = value;

        public static bool TryCreate(string? input, out RoomNumber? room, out string? error)
        {
            room = null;
            error = null;
            if (string.IsNullOrWhiteSpace(input)) { error = "Room number must not be empty"; return false; }
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
    }

    public sealed class Duration
    {
        public TimeSpan Value { get; }
        private Duration(TimeSpan t) => Value = t;

        public static bool TryCreate(string? input, out Duration? duration, out string? error)
        {
            duration = null;
            error = null;
            if (string.IsNullOrWhiteSpace(input)) { error = "Duration must not be empty"; return false; }
            var s = input.Trim();
            if (TimeSpan.TryParse(s, out var ts))
            {
                if (ts <= TimeSpan.Zero) { error = "Duration must be positive"; return false; }
                duration = new Duration(ts);
                return true;
            }
            if (int.TryParse(s, out var minutes) && minutes > 0)
            {
                duration = new Duration(TimeSpan.FromMinutes(minutes));
                return true;
            }
            error = "Invalid duration format. Use 'hh:mm' or minutes as integer.";
            return false;
        }

        public override string ToString() => Value.ToString();
    }

    public sealed class Capacity
    {
        public int Value { get; }
        private Capacity(int v) => Value = v;

        public static bool TryCreate(string? input, out Capacity? capacity, out string? error)
        {
            capacity = null;
            error = null;
            if (string.IsNullOrWhiteSpace(input)) { error = "Capacity must not be empty"; return false; }
            if (!int.TryParse(input.Trim(), out var v) || v <= 0)
            {
                error = "Capacity must be a positive integer."; return false;
            }
            capacity = new Capacity(v);
            return true;
        }

        public static Capacity FromInt(int v) => new Capacity(v);
        public override string ToString() => Value.ToString();
    }
}

