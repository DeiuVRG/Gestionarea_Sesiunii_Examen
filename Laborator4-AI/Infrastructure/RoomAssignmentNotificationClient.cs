namespace Laborator4_AI.Infrastructure
{
    using System.Net.Http.Json;
    using Laborator4_AI.Api.Models;

    /// <summary>
    /// Typed HttpClient pentru trimiterea notificărilor despre asignările la săli
    /// </summary>
    public class RoomAssignmentNotificationClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RoomAssignmentNotificationClient> _logger;

        public RoomAssignmentNotificationClient(
            HttpClient httpClient,
            ILogger<RoomAssignmentNotificationClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <summary>
        /// Trimite notificare despre asignarea unei săli pentru un examen
        /// </summary>
        public async Task<bool> NotifyRoomAssignmentAsync(RoomAssignmentNotification notification)
        {
            try
            {
                _logger.LogInformation(
                    "📤 Sending room assignment notification to external API: Course={CourseCode}, Room={RoomNumber}",
                    notification.CourseCode,
                    notification.RoomNumber
                );

                var response = await _httpClient.PostAsJsonAsync(
                    "api/notifications",
                    notification
                );

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation(
                        "✅ Room assignment notification sent successfully to external API for {CourseCode}",
                        notification.CourseCode
                    );
                    return true;
                }
                else
                {
                    _logger.LogWarning(
                        "⚠️ Room assignment notification failed with status {StatusCode} for {CourseCode}",
                        response.StatusCode,
                        notification.CourseCode
                    );
                    return false;
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(
                    ex,
                    "❌ HTTP request error sending room assignment notification for {CourseCode}",
                    notification.CourseCode
                );
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "❌ Unexpected error sending room assignment notification for {CourseCode}",
                    notification.CourseCode
                );
                throw;
            }
        }

        /// <summary>
        /// Verifică dacă serviciul de notificări este disponibil
        /// </summary>
        public async Task<bool> HealthCheckAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/notifications/health");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
