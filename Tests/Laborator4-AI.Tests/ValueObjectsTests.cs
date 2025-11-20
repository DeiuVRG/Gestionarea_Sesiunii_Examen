using System;
using Xunit;
using Laborator4_AI;

namespace Laborator4_AI.Tests
{
    public class ValueObjectsTests
    {
        [Fact]
        public void CourseCode_ValidAndInvalidCases()
        {
            Assert.True(CourseCode.TryCreate("PSSC", out var cc1, out var e1));
            Assert.Null(e1);
            Assert.Equal("PSSC", cc1!.Value);

            Assert.True(CourseCode.TryCreate("BD", out var cc2, out var e2));
            Assert.Null(e2);
            Assert.Equal("BD", cc2!.Value);

            Assert.False(CourseCode.TryCreate("pssc", out var cc3, out var e3));
            Assert.NotNull(e3);

            Assert.False(CourseCode.TryCreate("P", out var cc4, out var e4));
            Assert.NotNull(e4);

            Assert.False(CourseCode.TryCreate("ABCDEFG", out var cc5, out var e5));
            Assert.NotNull(e5);
        }

        [Fact]
        public void RoomNumber_ValidAndInvalidCases()
        {
            Assert.True(RoomNumber.TryCreate("A301", out var r1, out var e1));
            Assert.Equal("A301", r1!.Value);

            Assert.True(RoomNumber.TryCreate("B205", out var r2, out var e2));
            Assert.Equal("B205", r2!.Value);

            Assert.False(RoomNumber.TryCreate("Z301", out var r3, out var e3));
            Assert.NotNull(e3);

            Assert.False(RoomNumber.TryCreate("A5", out var r4, out var e4));
            Assert.NotNull(e4);

            Assert.False(RoomNumber.TryCreate("A801", out var r5, out var e5));
            Assert.NotNull(e5);
        }

        [Fact]
        public void ExamDate_ValidAndInvalidCases()
        {
            // find a valid future date within next year's June session
            var year = DateTime.Now.Year + 1;
            var validDate = new DateTime(year, 6, 15);
            // ensure it's in the future relative to now
            if (validDate <= DateTime.Now.Date) validDate = new DateTime(year + 1, 6, 15);

            Assert.True(ExamDate.TryCreate(validDate, out var ed1, out var e1));
            Assert.Null(e1);
            Assert.Equal(validDate.Date, ed1!.Date);

            // invalid: not in session (e.g., December next year)
            var invalidDate = new DateTime(year, 12, 15);
            Assert.False(ExamDate.TryCreate(invalidDate, out var ed2, out var e2));
            Assert.NotNull(e2);

            // find a weekend (Saturday) in June1-July15 of that year
            DateTime? weekend = null;
            for (var d = new DateTime(year, 6, 1); d <= new DateTime(year, 7, 15); d = d.AddDays(1))
            {
                if (d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday)
                {
                    if (d > DateTime.Now.Date) { weekend = d; break; }
                }
            }
            if (weekend.HasValue)
            {
                Assert.False(ExamDate.TryCreate(weekend.Value, out var ed3, out var e3));
                Assert.NotNull(e3);
            }
            else
            {
                // If no weekend found (unlikely), skip
            }
        }
    }
}

