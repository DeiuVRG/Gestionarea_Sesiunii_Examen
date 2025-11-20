namespace NotificationsAPI.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class RoomAssignmentNotification
    {
        public string CourseCode { get; set; } = string.Empty;
        public DateTime ExamDate { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public int RoomCapacity { get; set; }
        public DateTime AssignedAt { get; set; }
    }

    [ApiController]
    [Route("api/notifications")]
    [Produces("application/json")]
    public class NotificationsController : ControllerBase
    {
        private readonly ILogger<NotificationsController> _logger;
        private static readonly List<RoomAssignmentNotification> _receivedNotifications = new();

        public NotificationsController(ILogger<NotificationsController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Primește notificare despre asignarea unei săli (endpoint pentru Laborator4-AI)
        /// </summary>
        [HttpPost]
        public IActionResult ReceiveNotification([FromBody] RoomAssignmentNotification notification)
        {
            try
            {
                _logger.LogInformation(
                    "📬 [NOTIFICARE PRIMITĂ] Course: {Course}, Room: {Room}, Date: {Date}, Capacity: {Capacity}",
                    notification.CourseCode,
                    notification.RoomNumber,
                    notification.ExamDate.ToString("yyyy-MM-dd HH:mm"),
                    notification.RoomCapacity
                );

                // Salvează în memorie (în producție ar merge în DB)
                _receivedNotifications.Add(notification);

                Console.WriteLine();
                Console.WriteLine("═══════════════════════════════════════════════════════════");
                Console.WriteLine($"📬 NOTIFICARE NOUĂ PRIMITĂ!");
                Console.WriteLine($"   Curs: {notification.CourseCode}");
                Console.WriteLine($"   Sală: {notification.RoomNumber} (Capacitate: {notification.RoomCapacity})");
                Console.WriteLine($"   Data examen: {notification.ExamDate:yyyy-MM-dd HH:mm}");
                Console.WriteLine($"   Asignat la: {notification.AssignedAt:yyyy-MM-dd HH:mm:ss}");
                Console.WriteLine("═══════════════════════════════════════════════════════════");
                Console.WriteLine();

                return Ok(new
                {
                    success = true,
                    message = $"Notification received for {notification.CourseCode}",
                    data = notification,
                    totalNotificationsReceived = _receivedNotifications.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing notification");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Obține toate notificările primite
        /// </summary>
        [HttpGet]
        public IActionResult GetAllNotifications()
        {
            return Ok(new
            {
                success = true,
                total = _receivedNotifications.Count,
                notifications = _receivedNotifications.OrderByDescending(n => n.AssignedAt).ToList()
            });
        }

        /// <summary>
        /// Health check
        /// </summary>
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                notificationsReceived = _receivedNotifications.Count
            });
        }

        /// <summary>
        /// Șterge toate notificările (pentru testing)
        /// </summary>
        [HttpDelete]
        public IActionResult ClearNotifications()
        {
            var count = _receivedNotifications.Count;
            _receivedNotifications.Clear();
            _logger.LogInformation("🗑️ Cleared {Count} notifications", count);
            return Ok(new { success = true, message = $"Cleared {count} notifications" });
        }
    }
}
