namespace Laborator4_AI.Api.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Laborator4_AI.Api.Models;
    using Laborator4_AI.Infrastructure;

    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ExamsController : ControllerBase
    {
        private readonly SchedulingDbContext _db;

        public ExamsController(SchedulingDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Obține toate examenele programate
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<ExamDto>>>> GetAllExams()
        {
            try
            {
                var exams = await _db.Reservations
                    .Select(r => new ExamDto
                    {
                        Id = r.Id,
                        CourseCode = r.CourseCode,
                        ExamDate = r.Date,
                        RoomNumber = r.RoomNumber,
                        RegisteredStudentsCount = _db.StudentRegistrations
                            .Count(s => s.CourseCode == r.CourseCode && s.ExamDate == r.Date)
                    })
                    .ToListAsync();

                foreach (var exam in exams)
                {
                    var room = await _db.Rooms.FirstOrDefaultAsync(r => r.Number == exam.RoomNumber);
                    exam.RoomCapacity = room?.Capacity ?? 0;
                }

                return Ok(new ApiResponse<List<ExamDto>>
                {
                    Success = true,
                    Data = exams,
                    Message = $"Found {exams.Count} exam(s)"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<ExamDto>>
                {
                    Success = false,
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Obține un examen specific
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ExamDto>>> GetExam(int id)
        {
            try
            {
                var reservation = await _db.Reservations.FindAsync(id);
                if (reservation == null)
                {
                    return NotFound(new ApiResponse<ExamDto>
                    {
                        Success = false,
                        Errors = new List<string> { $"Exam with ID {id} not found" }
                    });
                }

                var room = await _db.Rooms.FirstOrDefaultAsync(r => r.Number == reservation.RoomNumber);
                var registeredCount = await _db.StudentRegistrations
                    .CountAsync(s => s.CourseCode == reservation.CourseCode && s.ExamDate == reservation.Date);

                var exam = new ExamDto
                {
                    Id = reservation.Id,
                    CourseCode = reservation.CourseCode,
                    ExamDate = reservation.Date,
                    RoomNumber = reservation.RoomNumber,
                    RoomCapacity = room?.Capacity ?? 0,
                    RegisteredStudentsCount = registeredCount
                };

                return Ok(new ApiResponse<ExamDto>
                {
                    Success = true,
                    Data = exam
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<ExamDto>
                {
                    Success = false,
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Obține toate sălile disponibile
        /// </summary>
        [HttpGet("rooms")]
        public async Task<ActionResult<ApiResponse<List<object>>>> GetRooms()
        {
            try
            {
                var rooms = await _db.Rooms
                    .Select(r => new { r.Number, r.Capacity })
                    .OrderBy(r => r.Number)
                    .ToListAsync();

                return Ok(new ApiResponse<List<object>>
                {
                    Success = true,
                    Data = rooms.Cast<object>().ToList(),
                    Message = $"Found {rooms.Count} room(s)"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<object>>
                {
                    Success = false,
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
