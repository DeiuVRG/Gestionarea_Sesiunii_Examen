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
    public class GradesController : ControllerBase
    {
        private readonly SchedulingDbContext _db;

        public GradesController(SchedulingDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Obține toate notele publicate
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<GradeDto>>>> GetAllGrades()
        {
            try
            {
                var grades = await _db.ExamGrades
                    .Select(g => new GradeDto
                    {
                        Id = g.Id,
                        StudentRegistrationNumber = g.StudentRegNumber,
                        CourseCode = g.CourseCode,
                        ExamDate = g.ExamDate,
                        Grade = g.Grade,
                        IsPassing = g.Grade >= 5.00m,
                        PublishedAt = g.PublishedAt ?? DateTime.UtcNow
                    })
                    .OrderByDescending(g => g.PublishedAt)
                    .ToListAsync();

                return Ok(new ApiResponse<List<GradeDto>>
                {
                    Success = true,
                    Data = grades,
                    Message = $"Found {grades.Count} grade(s)"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<GradeDto>>
                {
                    Success = false,
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Obține notele unui student
        /// </summary>
        [HttpGet("student/{studentNumber}")]
        public async Task<ActionResult<ApiResponse<List<GradeDto>>>> GetStudentGrades(string studentNumber)
        {
            try
            {
                var grades = await _db.ExamGrades
                    .Where(g => g.StudentRegNumber == studentNumber)
                    .Select(g => new GradeDto
                    {
                        Id = g.Id,
                        StudentRegistrationNumber = g.StudentRegNumber,
                        CourseCode = g.CourseCode,
                        ExamDate = g.ExamDate,
                        Grade = g.Grade,
                        IsPassing = g.Grade >= 5.00m,
                        PublishedAt = g.PublishedAt ?? DateTime.UtcNow
                    })
                    .OrderByDescending(g => g.PublishedAt)
                    .ToListAsync();

                return Ok(new ApiResponse<List<GradeDto>>
                {
                    Success = true,
                    Data = grades,
                    Message = $"Found {grades.Count} grade(s) for student {studentNumber}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<GradeDto>>
                {
                    Success = false,
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Obține notele pentru un examen specific
        /// </summary>
        [HttpGet("exam/{courseCode}/{examDate}")]
        public async Task<ActionResult<ApiResponse<List<GradeDto>>>> GetExamGrades(string courseCode, string examDate)
        {
            try
            {
                if (!DateTime.TryParse(examDate, out var date))
                {
                    return BadRequest(new ApiResponse<List<GradeDto>>
                    {
                        Success = false,
                        Errors = new List<string> { "Invalid date format. Use YYYY-MM-DD" }
                    });
                }

                var grades = await _db.ExamGrades
                    .Where(g => g.CourseCode == courseCode && g.ExamDate.Date == date.Date)
                    .Select(g => new GradeDto
                    {
                        Id = g.Id,
                        StudentRegistrationNumber = g.StudentRegNumber,
                        CourseCode = g.CourseCode,
                        ExamDate = g.ExamDate,
                        Grade = g.Grade,
                        IsPassing = g.Grade >= 5.00m,
                        PublishedAt = g.PublishedAt ?? DateTime.UtcNow
                    })
                    .OrderBy(g => g.StudentRegistrationNumber)
                    .ToListAsync();

                var passingCount = grades.Count(g => g.IsPassing);
                var passingRate = grades.Count > 0 ? (passingCount * 100.0 / grades.Count) : 0;

                return Ok(new ApiResponse<List<GradeDto>>
                {
                    Success = true,
                    Data = grades,
                    Message = $"Found {grades.Count} grade(s). Pass rate: {passingRate:F2}%"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<GradeDto>>
                {
                    Success = false,
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Publică notele pentru un examen
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<object>>> PublishGrades([FromBody] PublishGradesRequest request)
        {
            try
            {
                // Parse exam date
                if (!DateTime.TryParse(request.ExamDate, out var examDate))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Errors = new List<string> { "Invalid exam date format. Use YYYY-MM-DD" }
                    });
                }

                // Verifică dacă examenul există
                var examExists = await _db.Reservations
                    .AnyAsync(r => r.CourseCode == request.CourseCode && r.Date.Date == examDate.Date);

                if (!examExists)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Errors = new List<string> { $"Exam not found for course {request.CourseCode} on {examDate:yyyy-MM-dd}" }
                    });
                }

                var errors = new List<string>();
                var publishedCount = 0;
                var passingCount = 0;

                foreach (var gradeInput in request.Grades)
                {
                    // Validare nota
                    if (gradeInput.Grade < 1.00m || gradeInput.Grade > 10.00m)
                    {
                        errors.Add($"Grade for student {gradeInput.StudentRegistrationNumber}: Grade must be between 1.00 and 10.00");
                        continue;
                    }

                    // Verifică dacă studentul e înregistrat
                    var isRegistered = await _db.StudentRegistrations
                        .AnyAsync(s => s.StudentRegNumber == gradeInput.StudentRegistrationNumber
                                    && s.CourseCode == request.CourseCode
                                    && s.ExamDate.Date == examDate.Date);

                    if (!isRegistered)
                    {
                        errors.Add($"Student {gradeInput.StudentRegistrationNumber} is not registered for this exam");
                        continue;
                    }

                    // Verifică dubluri
                    var existing = await _db.ExamGrades
                        .FirstOrDefaultAsync(g => g.StudentRegNumber == gradeInput.StudentRegistrationNumber
                                                && g.CourseCode == request.CourseCode
                                                && g.ExamDate.Date == examDate.Date);

                    if (existing != null)
                    {
                        // Update existing grade
                        existing.Grade = gradeInput.Grade;
                        existing.PublishedAt = DateTime.UtcNow;
                    }
                    else
                    {
                        // Add new grade
                        var grade = new ExamGradeEntity
                        {
                            StudentRegNumber = gradeInput.StudentRegistrationNumber,
                            CourseCode = request.CourseCode,
                            ExamDate = examDate,
                            Grade = gradeInput.Grade,
                            PublishedAt = DateTime.UtcNow
                        };
                        _db.ExamGrades.Add(grade);
                    }

                    publishedCount++;
                    if (gradeInput.Grade >= 5.00m)
                    {
                        passingCount++;
                    }
                }

                if (publishedCount == 0)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Errors = errors,
                        Message = "No valid grades to publish"
                    });
                }

                await _db.SaveChangesAsync();

                var passingRate = publishedCount > 0 ? (passingCount * 100.0 / publishedCount) : 0;

                return CreatedAtAction(
                    nameof(GetExamGrades),
                    new { courseCode = request.CourseCode, examDate = request.ExamDate },
                    new ApiResponse<object>
                    {
                        Success = true,
                        Data = new
                        {
                            CourseCode = request.CourseCode,
                            ExamDate = request.ExamDate,
                            TotalPublished = publishedCount,
                            PassingStudents = passingCount,
                            PassingRate = $"{passingRate:F2}%"
                        },
                        Message = $"Successfully published {publishedCount} grade(s)",
                        Errors = errors
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
