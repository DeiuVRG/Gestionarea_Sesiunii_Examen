namespace Laborator4_AI.Domain.Operations
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Laborator4_AI.Domain.Models.Entities;
    using Laborator4_AI.Domain.Models.ValueObjects;

    /// <summary>
    /// Operation to validate exam scheduling from unvalidated to validated state
    /// Dependencies:
    /// - checkCourseExists: Verifies course exists in catalog
    /// - getCourseEndDate: Gets when course ends for date validation
    /// </summary>
    internal sealed class ValidateExamSchedulingOperation : ExamSchedulingOperation
    {
        private readonly Func<CourseCode, bool> _checkCourseExists;
        private readonly Func<CourseCode, DateTime> _getCourseEndDate;

        public ValidateExamSchedulingOperation(
            Func<CourseCode, bool> checkCourseExists,
            Func<CourseCode, DateTime> getCourseEndDate)
        {
            _checkCourseExists = checkCourseExists;
            _getCourseEndDate = getCourseEndDate;
        }

        protected override IExamScheduling OnUnvalidated(UnvalidatedExamScheduling scheduling)
        {
            var errors = new List<string>();

            // 1. Parse and validate course code
            if (!CourseCode.TryCreate(scheduling.CourseCode, out var courseCode, out var courseError))
            {
                errors.Add(courseError ?? "Invalid course code");
            }
            else
            {
                // 2. Check if course exists
                if (!_checkCourseExists(courseCode!))
                {
                    errors.Add($"Course '{courseCode!.Value}' does not exist in catalog");
                }
            }

            // 3. Parse and validate proposed dates
            var proposedDates = new List<ExamDate>();
            var dateStrings = new[] { scheduling.ProposedDate1, scheduling.ProposedDate2, scheduling.ProposedDate3 }
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();

            foreach (var ds in dateStrings)
            {
                if (!DateTime.TryParse(ds, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dt))
                {
                    errors.Add($"Proposed date '{ds}' is not a valid date");
                    continue;
                }

                if (!ExamDate.TryCreate(dt, out var exd, out var dError))
                {
                    errors.Add($"Proposed date '{ds}': {dError}");
                    continue;
                }

                // 4. Check date is at least 7 days after course end
                if (courseCode != null)
                {
                    var courseEnd = _getCourseEndDate(courseCode!);
                    if (exd!.Date < courseEnd.AddDays(7))
                    {
                        errors.Add($"Proposed date '{ds}' must be at least 7 days after course end date ({courseEnd:yyyy-MM-dd})");
                        continue;
                    }
                }

                proposedDates.Add(exd!);
            }

            if (proposedDates.Count == 0)
            {
                errors.Add("At least one valid proposed date is required");
            }

            // 5. Parse and validate duration
            if (!Duration.TryCreate(scheduling.Duration, out var duration, out var durErr))
            {
                errors.Add(durErr ?? "Invalid duration");
            }

            // 6. Parse and validate expected students capacity
            if (!Capacity.TryCreate(scheduling.ExpectedStudents, out var expectedCapacity, out var capErr))
            {
                errors.Add(capErr ?? "Invalid expected students capacity");
            }

            // 7. Return validated or invalid state
            if (errors.Any())
            {
                return new InvalidExamScheduling(scheduling.CourseCode, errors);
            }

            return new ValidatedExamScheduling(courseCode!, proposedDates.AsReadOnly(), duration!, expectedCapacity!);
        }
    }
}
