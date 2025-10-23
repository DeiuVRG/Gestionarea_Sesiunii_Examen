namespace Laborator4_AI
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    public sealed class ScheduleExamWorkflow
    {
        // Execute pipeline returns the final IExamScheduling state (Published or Invalid)
        public IExamScheduling Execute(
            ScheduleExamCommand command,
            Func<CourseCode, bool> checkCourseExists,
            Func<CourseCode, DateTime> getCourseEndDate,
            Func<ExamDate, Duration, Capacity, IEnumerable<RoomNumber>> findAvailableRooms,
            Func<RoomNumber, ExamDate, Duration, bool> reserveRoom)
        {
            // 1. Unvalidated
            var unvalidated = new UnvalidatedExamScheduling(command.CourseCode ?? string.Empty, command.ProposedDate1 ?? string.Empty, command.ProposedDate2 ?? string.Empty, command.ProposedDate3 ?? string.Empty, command.Duration ?? string.Empty, command.ExpectedStudents ?? string.Empty);

            // 2. Validate
            var errors = new List<string>();

            if (!CourseCode.TryCreate(unvalidated.CourseCode, out var courseCode, out var courseError))
                errors.Add(courseError ?? "Invalid course code");
            else
            {
                if (!checkCourseExists(courseCode!))
                    errors.Add("Course does not exist in catalog.");
            }

            var proposedDates = new List<ExamDate>();
            foreach (var ds in new[] { unvalidated.ProposedDate1, unvalidated.ProposedDate2, unvalidated.ProposedDate3 }.Where(s => !string.IsNullOrWhiteSpace(s)))
            {
                if (!DateTime.TryParse(ds, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dt))
                {
                    errors.Add($"Proposed date '{ds}' is not a valid date.");
                    continue;
                }
                if (!ExamDate.TryCreate(dt, out var exd, out var dError))
                {
                    errors.Add($"Proposed date '{ds}': {dError}");
                    continue;
                }
                // check course end date constraint
                if (courseCode != null)
                {
                    var courseEnd = getCourseEndDate(courseCode!);
                    if (exd.Date < courseEnd.Date)
                    {
                        errors.Add($"Proposed date '{ds}' is before course end date ({courseEnd:yyyy-MM-dd}).");
                        continue;
                    }
                }
                proposedDates.Add(exd!);
            }

            if (proposedDates.Count == 0)
                errors.Add("At least one valid proposed date is required.");

            if (!Duration.TryCreate(unvalidated.Duration, out var duration, out var durErr))
                errors.Add(durErr ?? "Invalid duration");

            if (!Capacity.TryCreate(unvalidated.ExpectedStudents, out var expectedCapacity, out var capErr))
                errors.Add(capErr ?? "Invalid expected students");

            if (errors.Any())
            {
                return new InvalidExamScheduling(unvalidated.CourseCode, errors);
            }

            var validated = new ValidatedExamScheduling(courseCode!, proposedDates.AsReadOnly(), duration!, expectedCapacity!);

            // 3. Allocate room
            var allocationErrors = new List<string>();
            foreach (var pd in validated.ProposedDates)
            {
                var rooms = findAvailableRooms(pd, validated.Duration, validated.ExpectedStudents) ?? Enumerable.Empty<RoomNumber>();
                var room = rooms.FirstOrDefault();
                if (room != null)
                {
                    var reserved = reserveRoom(room, pd, validated.Duration);
                    if (reserved)
                    {
                        var roomCapacity = validated.ExpectedStudents;
                        var allocated = new RoomAllocatedExamScheduling(validated.Course, pd, validated.Duration, room, roomCapacity);

                        // 4. Publish (enrolled students initially 0)
                        var published = new PublishedExamScheduling(allocated.Course, allocated.SelectedDate, allocated.Duration, allocated.Room, allocated.RoomCapacity, Capacity.FromInt(0), DateTime.Now);

                        return published;
                    }
                    else
                    {
                        allocationErrors.Add($"Failed to reserve room {room} on {pd}.");
                    }
                }
                else
                {
                    allocationErrors.Add($"No rooms available for date {pd}.");
                }
            }

            // If we reach here no successful allocation
            var reasons = new List<string> { "No rooms available for any proposed date." };
            reasons.AddRange(allocationErrors);
            return new InvalidExamScheduling(validated.Course.ToString(), reasons);
        }
    }
}

