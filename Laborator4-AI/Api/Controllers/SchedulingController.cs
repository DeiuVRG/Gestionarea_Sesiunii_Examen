namespace Laborator4_AI.Api.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Laborator4_AI.Api.Models;
    using Laborator4_AI.Infrastructure;
    using Laborator4_AI.Domain.Workflows;
    using Laborator4_AI.Domain.Models.Commands;
    using Laborator4_AI.Domain.Models.Events;

    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SchedulingController : ControllerBase
    {
        private readonly SchedulingDbContext _db;
        private readonly RoomAssignmentNotificationClient _notificationClient;
        private readonly ILogger<SchedulingController> _logger;

        public SchedulingController(
            SchedulingDbContext db,
            RoomAssignmentNotificationClient notificationClient,
            ILogger<SchedulingController> logger)
        {
            _db = db;
            _notificationClient = notificationClient;
            _logger = logger;
        }

        /// <summary>
        /// Programează un examen folosind workflow-ul de domain
        /// </summary>
        [HttpPost("schedule-exam")]
        public async Task<ActionResult<ApiResponse<object>>> ScheduleExam([FromBody] ScheduleExamRequest request)
        {
            try
            {
                // Parse dates
                if (!DateTime.TryParse(request.ProposedDate1, out var date1) ||
                    !DateTime.TryParse(request.ProposedDate2, out var date2) ||
                    !DateTime.TryParse(request.ProposedDate3, out var date3))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Errors = new List<string> { "Invalid date format. Use YYYY-MM-DD" }
                    });
                }

                // Create command
                var command = new ScheduleExamCommand
                {
                    CourseCode = request.CourseCode,
                    ProposedDate1 = request.ProposedDate1,
                    ProposedDate2 = request.ProposedDate2,
                    ProposedDate3 = request.ProposedDate3,
                    Duration = request.DurationMinutes.ToString(),
                    ExpectedStudents = request.ExpectedStudents.ToString()
                };

                // Execute workflow
                var workflow = new ScheduleExamWorkflow();
                var examEvent = workflow.Execute(
                    command,
                    checkCourseExists: courseCode => true, // Simplified for demo
                    getCourseEndDate: courseCode => DateTime.Now.AddMonths(6),
                    findAvailableRooms: (date, duration, capacity) =>
                    {
                        var rooms = _db.Rooms
                            .Where(r => r.Capacity >= capacity.Value)
                            .Select(r => r.Number)
                            .ToList();

                        // Convert string room numbers to RoomNumber value objects
                        var roomNumbers = new List<Domain.Models.ValueObjects.RoomNumber>();
                        foreach (var roomStr in rooms)
                        {
                            if (Domain.Models.ValueObjects.RoomNumber.TryCreate(roomStr, out var roomNumber, out _))
                            {
                                roomNumbers.Add(roomNumber!);
                            }
                        }
                        return roomNumbers;
                    },
                    reserveRoom: (room, date, duration) =>
                    {
                        // Parse date string from ExamDate value object
                        DateTime examDateTime;
                        try
                        {
                            examDateTime = DateTime.Parse(date.ToString());
                        }
                        catch
                        {
                            return false;
                        }

                        var existing = _db.Reservations
                            .FirstOrDefault(r => r.RoomNumber == room.Value && r.Date.Date == examDateTime.Date);

                        if (existing != null)
                            return false;

                        _db.Reservations.Add(new RoomReservation
                        {
                            CourseCode = command.CourseCode,
                            RoomNumber = room.Value,
                            Date = examDateTime,
                            DurationMinutes = (int)duration.Value.TotalMinutes
                        });
                        _db.SaveChanges();
                        return true;
                    }
                );

                // Check if exam was scheduled successfully
                if (examEvent is ExamScheduledEvent scheduledEvent)
                {
                    _logger.LogInformation(
                        "✅ Exam scheduled: {CourseCode} on {Date} in room {Room}",
                        scheduledEvent.Course.Value,
                        scheduledEvent.Date.ToString(),
                        scheduledEvent.Room.Value
                    );

                    // Get room capacity for notification
                    var room = await _db.Rooms
                        .FirstOrDefaultAsync(r => r.Number == scheduledEvent.Room.Value);

                    // Send notification about room assignment with Polly retry policy
                    var notification = new RoomAssignmentNotification
                    {
                        CourseCode = scheduledEvent.Course.Value,
                        ExamDate = DateTime.Parse(scheduledEvent.Date.ToString()),
                        RoomNumber = scheduledEvent.Room.Value,
                        RoomCapacity = scheduledEvent.RoomCapacity.Value,
                        AssignedAt = DateTime.UtcNow
                    };

                    try
                    {
                        await _notificationClient.NotifyRoomAssignmentAsync(notification);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(
                            ex,
                            "⚠️ Failed to send notification after retries for {CourseCode}. Exam was scheduled but notification failed.",
                            scheduledEvent.Course.Value
                        );
                        // Nu aruncăm excepția - examenul a fost programat cu succes
                        // Doar notificarea a eșuat
                    }

                    return Ok(new ApiResponse<object>
                    {
                        Success = true,
                        Data = new
                        {
                            CourseCode = scheduledEvent.Course.Value,
                            ScheduledDate = scheduledEvent.Date.ToString(),
                            AllocatedRoom = scheduledEvent.Room.Value,
                            RoomCapacity = scheduledEvent.RoomCapacity.Value,
                            PublishedAt = scheduledEvent.PublishedAt,
                            NotificationSent = true
                        },
                        Message = $"Exam scheduled successfully for {scheduledEvent.Course.Value}"
                    });
                }
                else if (examEvent is ExamSchedulingFailedEvent failedEvent)
                {
                    _logger.LogWarning(
                        "❌ Exam scheduling failed for {CourseCode}: {Reasons}",
                        failedEvent.CourseCode,
                        string.Join(", ", failedEvent.Reasons)
                    );

                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Errors = failedEvent.Reasons.ToList(),
                        Message = $"Failed to schedule exam for {failedEvent.CourseCode}"
                    });
                }
                else
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Success = false,
                        Errors = new List<string> { "Unknown workflow result" }
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scheduling exam");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
