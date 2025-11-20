namespace Laborator4_AI.Api.Models
{
    public class ExamDto
    {
        public int Id { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public DateTime ExamDate { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public int RoomCapacity { get; set; }
        public int RegisteredStudentsCount { get; set; }
    }

    public class StudentRegistrationDto
    {
        public int Id { get; set; }
        public string StudentRegistrationNumber { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public DateTime ExamDate { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public DateTime RegisteredAt { get; set; }
    }

    public class GradeDto
    {
        public int Id { get; set; }
        public string StudentRegistrationNumber { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public DateTime ExamDate { get; set; }
        public decimal Grade { get; set; }
        public bool IsPassing { get; set; }
        public DateTime PublishedAt { get; set; }
    }

    public class RegisterStudentRequest
    {
        public string StudentRegistrationNumber { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string ExamDate { get; set; } = string.Empty;
    }

    public class ScheduleExamRequest
    {
        public string CourseCode { get; set; } = string.Empty;
        public string ProposedDate1 { get; set; } = string.Empty;
        public string ProposedDate2 { get; set; } = string.Empty;
        public string ProposedDate3 { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public int ExpectedStudents { get; set; }
    }

    public class PublishGradeRequest
    {
        public string StudentRegistrationNumber { get; set; } = string.Empty;
        public decimal Grade { get; set; }
    }

    public class PublishGradesRequest
    {
        public string CourseCode { get; set; } = string.Empty;
        public string ExamDate { get; set; } = string.Empty;
        public List<PublishGradeRequest> Grades { get; set; } = new();
    }

    public class RoomAssignmentNotification
    {
        public string CourseCode { get; set; } = string.Empty;
        public DateTime ExamDate { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public int RoomCapacity { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
