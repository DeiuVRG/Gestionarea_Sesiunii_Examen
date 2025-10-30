namespace Laborator4_AI.Domain.Operations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Laborator4_AI.Domain.Models.Entities;
    using Laborator4_AI.Domain.Models.ValueObjects;

    /// <summary>
    /// Operation to allocate a room for the exam
    /// Dependencies:
    /// - findAvailableRooms: Finds rooms that match capacity and date requirements
    /// - reserveRoom: Reserves the selected room
    /// </summary>
    internal sealed class AllocateRoomOperation : ExamSchedulingOperation
    {
        private readonly Func<ExamDate, Duration, Capacity, IEnumerable<RoomNumber>> _findAvailableRooms;
        private readonly Func<RoomNumber, ExamDate, Duration, bool> _reserveRoom;

        public AllocateRoomOperation(
            Func<ExamDate, Duration, Capacity, IEnumerable<RoomNumber>> findAvailableRooms,
            Func<RoomNumber, ExamDate, Duration, bool> reserveRoom)
        {
            _findAvailableRooms = findAvailableRooms;
            _reserveRoom = reserveRoom;
        }

        protected override IExamScheduling OnValidated(ValidatedExamScheduling scheduling)
        {
            var allocationErrors = new List<string>();

            // Try each proposed date
            foreach (var proposedDate in scheduling.ProposedDates)
            {
                // Find available rooms for this date
                var rooms = _findAvailableRooms(proposedDate, scheduling.Duration, scheduling.ExpectedStudents)
                    ?.ToList() ?? new List<RoomNumber>();

                if (!rooms.Any())
                {
                    allocationErrors.Add($"No rooms available for date {proposedDate}");
                    continue;
                }

                // Try to reserve the first available room
                var room = rooms.First();
                var reserved = _reserveRoom(room, proposedDate, scheduling.Duration);

                if (reserved)
                {
                    // Success - room allocated
                    return new RoomAllocatedExamScheduling(
                        scheduling.Course,
                        proposedDate,
                        scheduling.Duration,
                        room,
                        scheduling.ExpectedStudents);
                }
                else
                {
                    allocationErrors.Add($"Failed to reserve room {room} on {proposedDate} (may have been reserved by another process)");
                }
            }

            // No room could be allocated
            var reasons = new List<string> { "No rooms could be allocated for any proposed date" };
            reasons.AddRange(allocationErrors);
            return new InvalidExamScheduling(scheduling.Course.Value, reasons);
        }
    }
}
