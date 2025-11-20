namespace Laborator4_AI.Api.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Laborator4_AI.Api.Models;

    /// <summary>
    /// Controller pentru primirea notificărilor despre asignările la săli de examen
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class RoomAssignmentNotificationsController : ControllerBase
    {
        private readonly ILogger<RoomAssignmentNotificationsController> _logger;

        public RoomAssignmentNotificationsController(ILogger<RoomAssignmentNotificationsController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Primește notificare despre asignarea unei săli pentru un examen
        /// </summary>
        /// <param name="notification">Detaliile asignării</param>
        [HttpPost]
        public ActionResult<ApiResponse<RoomAssignmentNotification>> NotifyRoomAssignment(
            [FromBody] RoomAssignmentNotification notification)
        {
            try
            {
                // Validare
                if (string.IsNullOrWhiteSpace(notification.CourseCode))
                {
                    return BadRequest(new ApiResponse<RoomAssignmentNotification>
                    {
                        Success = false,
                        Errors = new List<string> { "Course code is required" }
                    });
                }

                if (string.IsNullOrWhiteSpace(notification.RoomNumber))
                {
                    return BadRequest(new ApiResponse<RoomAssignmentNotification>
                    {
                        Success = false,
                        Errors = new List<string> { "Room number is required" }
                    });
                }

                // Log notificarea
                _logger.LogInformation(
                    "📬 Room assignment notification received: Course={CourseCode}, Room={RoomNumber}, Date={ExamDate}, Capacity={Capacity}",
                    notification.CourseCode,
                    notification.RoomNumber,
                    notification.ExamDate,
                    notification.RoomCapacity
                );

                // Aici ar putea fi logica de business: 
                // - salvare în baza de date de audit
                // - trimitere email către responsabili
                // - trigger pentru alte procese
                // - actualizare dashboard real-time

                return Ok(new ApiResponse<RoomAssignmentNotification>
                {
                    Success = true,
                    Data = notification,
                    Message = $"Room assignment notification for {notification.CourseCode} processed successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing room assignment notification");
                return StatusCode(500, new ApiResponse<RoomAssignmentNotification>
                {
                    Success = false,
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Health check endpoint pentru a testa dacă serviciul de notificări este disponibil
        /// </summary>
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }
    }
}
