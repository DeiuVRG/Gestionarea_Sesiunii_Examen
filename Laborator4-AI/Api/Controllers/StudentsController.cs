namespace Laborator4_AI.Api.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Laborator4_AI.Api.Models;
    using Laborator4_AI.Infrastructure;
    using Laborator4_AI.Domain.Models.Entities;

    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class StudentsController : ControllerBase
    {
        private readonly SchedulingDbContext _db;

        public StudentsController(SchedulingDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Obține toate înregistrările de studenți
        /// </summary>
        [HttpGet("registrations")]
        public async Task<ActionResult<ApiResponse<List<StudentRegistrationDto>>>> GetAllRegistrations()
        {
            try
            {
                var registrations = await _db.StudentRegistrations
                    .Select(s => new StudentRegistrationDto
                    {
                        Id = s.Id,
                        StudentRegistrationNumber = s.StudentRegNumber,
                        CourseCode = s.CourseCode,
                        ExamDate = s.ExamDate,
                        RoomNumber = s.RoomNumber,
                        RegisteredAt = s.RegisteredAt
                    })
                    .OrderByDescending(s => s.RegisteredAt)
                    .ToListAsync();

                return Ok(new ApiResponse<List<StudentRegistrationDto>>
                {
                    Success = true,
                    Data = registrations,
                    Message = $"Found {registrations.Count} registration(s)"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<StudentRegistrationDto>>
                {
                    Success = false,
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Obține înregistrările unui student
        /// </summary>
        [HttpGet("{studentNumber}/registrations")]
        public async Task<ActionResult<ApiResponse<List<StudentRegistrationDto>>>> GetStudentRegistrations(string studentNumber)
        {
            try
            {
                var registrations = await _db.StudentRegistrations
                    .Where(s => s.StudentRegNumber == studentNumber)
                    .Select(s => new StudentRegistrationDto
                    {
                        Id = s.Id,
                        StudentRegistrationNumber = s.StudentRegNumber,
                        CourseCode = s.CourseCode,
                        ExamDate = s.ExamDate,
                        RoomNumber = s.RoomNumber,
                        RegisteredAt = s.RegisteredAt
                    })
                    .OrderByDescending(s => s.RegisteredAt)
                    .ToListAsync();

                return Ok(new ApiResponse<List<StudentRegistrationDto>>
                {
                    Success = true,
                    Data = registrations,
                    Message = $"Found {registrations.Count} registration(s) for student {studentNumber}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<StudentRegistrationDto>>
                {
                    Success = false,
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Înregistrează un student la un examen
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<StudentRegistrationDto>>> RegisterStudent([FromBody] RegisterStudentRequest request)
        {
            try
            {
                // Validare student registration number format
                if (string.IsNullOrWhiteSpace(request.StudentRegistrationNumber) || request.StudentRegistrationNumber.Length < 6)
                {
                    return BadRequest(new ApiResponse<StudentRegistrationDto>
                    {
                        Success = false,
                        Errors = new List<string> { "Invalid student registration number format" }
                    });
                }

                // Parse exam date
                if (!DateTime.TryParse(request.ExamDate, out var examDate))
                {
                    return BadRequest(new ApiResponse<StudentRegistrationDto>
                    {
                        Success = false,
                        Errors = new List<string> { "Invalid exam date format. Use YYYY-MM-DD" }
                    });
                }

                // Verifică dacă examenul există
                var exam = await _db.Reservations
                    .FirstOrDefaultAsync(r => r.CourseCode == request.CourseCode && r.Date.Date == examDate.Date);

                if (exam == null)
                {
                    return NotFound(new ApiResponse<StudentRegistrationDto>
                    {
                        Success = false,
                        Errors = new List<string> { $"Exam not found for course {request.CourseCode} on {examDate:yyyy-MM-dd}" }
                    });
                }

                // Verifică dubluri
                var existing = await _db.StudentRegistrations
                    .FirstOrDefaultAsync(s => s.StudentRegNumber == request.StudentRegistrationNumber 
                                           && s.CourseCode == request.CourseCode 
                                           && s.ExamDate.Date == examDate.Date);

                if (existing != null)
                {
                    return BadRequest(new ApiResponse<StudentRegistrationDto>
                    {
                        Success = false,
                        Errors = new List<string> { "Student is already registered for this exam" }
                    });
                }

                // Creează înregistrarea
                var registration = new StudentRegistrationEntity
                {
                    StudentRegNumber = request.StudentRegistrationNumber,
                    CourseCode = request.CourseCode,
                    ExamDate = examDate,
                    RoomNumber = exam.RoomNumber,
                    RegisteredAt = DateTime.UtcNow
                };

                _db.StudentRegistrations.Add(registration);
                await _db.SaveChangesAsync();

                var dto = new StudentRegistrationDto
                {
                    Id = registration.Id,
                    StudentRegistrationNumber = registration.StudentRegNumber,
                    CourseCode = registration.CourseCode,
                    ExamDate = registration.ExamDate,
                    RoomNumber = registration.RoomNumber,
                    RegisteredAt = registration.RegisteredAt
                };

                return CreatedAtAction(
                    nameof(GetStudentRegistrations),
                    new { studentNumber = dto.StudentRegistrationNumber },
                    new ApiResponse<StudentRegistrationDto>
                    {
                        Success = true,
                        Data = dto,
                        Message = $"Student {request.StudentRegistrationNumber} registered successfully"
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<StudentRegistrationDto>
                {
                    Success = false,
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
