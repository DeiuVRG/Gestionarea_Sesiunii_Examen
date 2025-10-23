using System;
using System.Collections.Generic;
using System.Linq;
using Laborator4_AI;
using System.Text.Json;

// Mini "Sistem de Gestionare a Sesiunii de Examene" demonstrator
// Implements: professor proposes 3 dates, secretariat validates rooms and assigns, students register (max 2 exams/day), professor grading and publishing, contestation within 48h, and reporting.

// Domain models
record Room(string Id, int Capacity);

enum UserRole { Secretariat, Professor, Student, Administrator }

record User(string Id, string Name, UserRole Role);

class ExamProposal
{
    public string? Id { get; } = Guid.NewGuid().ToString();
    public string? Course { get; set; }
    public string? ProfessorId { get; set; }
    public List<DateTime> ProposedDates { get; set; } = new(); // up to 3
}

class Exam
{
    public string Id { get; } = Guid.NewGuid().ToString();
    public string? Course { get; set; }
    public string? ProfessorId { get; set; }
    public DateTime Date { get; set; }
    public Room? AssignedRoom { get; set; }
    public List<string> RegisteredStudentIds { get; } = new();
    public Dictionary<string, double?> Grades { get; } = new(); // studentId -> grade (null = not entered)
    public DateTime? PublishedAt { get; set; }
}

class Contestation
{
    public string Id { get; } = Guid.NewGuid().ToString();
    public string? ExamId { get; set; }
    public string? StudentId { get; set; }
    public DateTime FiledAt { get; set; }
    public string? Reason { get; set; }
}

// Simple in-memory repositories / services
class SchedulingService
{
    private readonly List<Room> _rooms;
    private readonly List<Exam> _exams;

    public SchedulingService(List<Room> rooms, List<Exam> exams)
    {
        _rooms = rooms;
        _exams = exams;
    }

    // Try to validate a proposed date by finding a room available on that date (no other exam in same room at same datetime)
    public Room? AllocateRoomFor(DateTime date, int expectedStudents)
    {
        // naive check: same date and time conflict
        foreach (var room in _rooms)
        {
            if (room.Capacity < expectedStudents) continue;
            var conflict = _exams.Any(e => e.AssignedRoom != null && e.AssignedRoom.Id == room.Id && e.Date == date);
            if (!conflict) return room;
        }
        return null;
    }
}

class RegistrationService
{
    private readonly List<Exam> _exams;
    private readonly Dictionary<string, User> _users;

    public RegistrationService(List<Exam> exams, Dictionary<string, User> users)
    {
        _exams = exams;
        _users = users;
    }

    // Register a student for an exam with restriction: max 2 exams per calendar day
    public bool RegisterStudent(string studentId, string examId, out string message)
    {
        message = string.Empty;
        if (!_users.TryGetValue(studentId, out var user) || user.Role != UserRole.Student)
        {
            message = "Invalid student";
            return false;
        }

        var exam = _exams.FirstOrDefault(e => e.Id == examId);
        if (exam == null) { message = "Exam not found"; return false; }

        // Count student's other registrations on same day
        var day = exam.Date.Date;
        var countSameDay = _exams.Count(e => e.RegisteredStudentIds.Contains(studentId) && e.Date.Date == day);
        if (countSameDay >= 2)
        {
            message = $"Registration denied: student already registered to {countSameDay} exams on {day:d} (max 2).";
            return false;
        }

        if (exam.RegisteredStudentIds.Contains(studentId))
        {
            message = "Student already registered to this exam"; return false;
        }

        exam.RegisteredStudentIds.Add(studentId);
        exam.Grades[studentId] = null;
        message = "Registered successfully";
        return true;
    }
}

class GradingService
{
    private readonly List<Exam> _exams;

    public GradingService(List<Exam> exams)
    {
        _exams = exams;
    }

    public bool EnterGrade(string professorId, string examId, string studentId, double grade, out string message)
    {
        var exam = _exams.FirstOrDefault(e => e.Id == examId);
        if (exam == null) { message = "Exam not found"; return false; }
        if (exam.ProfessorId != professorId) { message = "Only the professor of the exam can enter grades"; return false; }
        if (!exam.RegisteredStudentIds.Contains(studentId)) { message = "Student not registered"; return false; }
        exam.Grades[studentId] = grade;
        message = "Grade entered";
        return true;
    }

    public bool PublishGrades(string professorId, string examId, out string message)
    {
        var exam = _exams.FirstOrDefault(e => e.Id == examId);
        if (exam == null) { message = "Exam not found"; return false; }
        if (exam.ProfessorId != professorId) { message = "Only the professor of the exam can publish grades"; return false; }
        exam.PublishedAt = DateTime.Now;
        message = "Grades published";
        return true;
    }
}

class ContestationService
{
    private readonly List<Contestation> _contestations;
    private readonly List<Exam> _exams;

    public ContestationService(List<Contestation> contestations, List<Exam> exams)
    {
        _contestations = contestations;
        _exams = exams;
    }

    // Students can contest within 48 hours from publication
    public bool FileContestation(string studentId, string examId, string reason, out string message)
    {
        message = string.Empty;
        var exam = _exams.FirstOrDefault(e => e.Id == examId);
        if (exam == null) { message = "Exam not found"; return false; }
        if (!exam.RegisteredStudentIds.Contains(studentId)) { message = "Student did not attend / not registered"; return false; }
        if (exam.PublishedAt == null) { message = "Grades not published yet"; return false; }
        var elapsed = DateTime.Now - exam.PublishedAt.Value;
        if (elapsed > TimeSpan.FromHours(48)) { message = "Contestation window (48h) expired"; return false; }
        var c = new Contestation { ExamId = examId, StudentId = studentId, FiledAt = DateTime.Now, Reason = reason };
        _contestations.Add(c);
        message = "Contestation filed";
        return true;
    }
}

class ReportingService
{
    private readonly List<Exam> _exams;

    public ReportingService(List<Exam> exams)
    {
        _exams = exams;
    }

    // Generate a simple pass/fail report for a given exam (threshold configurable)
    public (int total, int passed, double passRate) GetPassReport(string examId, double passThreshold = 5.0)
    {
        var exam = _exams.FirstOrDefault(e => e.Id == examId);
        if (exam == null) return (0, 0, 0);
        var grades = exam.Grades.Values.Where(g => g.HasValue).Select(g => g!.Value).ToList();
        var total = grades.Count;
        var passed = grades.Count(g => g >= passThreshold);
        var rate = total == 0 ? 0 : Math.Round(100.0 * passed / total, 2);
        return (total, passed, rate);
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Demo scenario to exercise requirements
        Console.WriteLine("Sistem de gestionare a sesiunii de examene - demo\n");

        // Setup users
        var users = new Dictionary<string, User>();
        var professor = new User("prof1", "Dr. Popescu", UserRole.Professor);
        var secretariat = new User("sec1", "Secretariat", UserRole.Secretariat);
        var admin = new User("admin1", "Admin", UserRole.Administrator);
        var student1 = new User("s1", "Ioana", UserRole.Student);
        var student2 = new User("s2", "Andrei", UserRole.Student);
        var student3 = new User("s3", "Maria", UserRole.Student);

        users[professor.Id] = professor;
        users[secretariat.Id] = secretariat;
        users[admin.Id] = admin;
        users[student1.Id] = student1;
        users[student2.Id] = student2;
        users[student3.Id] = student3;

        // Rooms
        var rooms = new List<Room>
        {
            new Room("R101", 30),
            new Room("R102", 15),
        };

        // storage
        var exams = new List<Exam>();
        var contestations = new List<Contestation>();

        var scheduling = new SchedulingService(rooms, exams);
        var registration = new RegistrationService(exams, users);
        var grading = new GradingService(exams);
        var contestService = new ContestationService(contestations, exams);
        var reporting = new ReportingService(exams);

        // 1) Professor proposes 3 dates for an exam
        var proposal = new ExamProposal
        {
            Course = "Algoritmi si Structuri de Date",
            ProfessorId = professor.Id,
            ProposedDates = new List<DateTime>
            {
                DateTime.Today.AddDays(7).AddHours(9),
                DateTime.Today.AddDays(8).AddHours(14),
                DateTime.Today.AddDays(9).AddHours(11),
            }
        };
        Console.WriteLine($"Profesorul {professor.Name} a propus 3 date pentru cursul '{proposal.Course}':");
        foreach (var d in proposal.ProposedDates) Console.WriteLine(" - " + d);

        // 2) Secretariat validates availability and selects one
        DateTime? selectedDate = null;
        Room? assignedRoom = null;
        int expectedStudents = 3; // estimate
        foreach (var date in proposal.ProposedDates)
        {
            var room = scheduling.AllocateRoomFor(date, expectedStudents);
            if (room != null)
            {
                selectedDate = date;
                assignedRoom = room;
                break;
            }
        }

        if (selectedDate == null || assignedRoom == null)
        {
            Console.WriteLine("Secretariat: Nu exista sali disponibile pentru datele propuse.");
            return;
        }

        var exam = new Exam
        {
            Course = proposal.Course,
            ProfessorId = proposal.ProfessorId,
            Date = selectedDate.Value,
            AssignedRoom = assignedRoom
        };
        exams.Add(exam);
        Console.WriteLine($"Secretariatul a validat data {exam.Date} si a alocat sala {exam.AssignedRoom!.Id} (cap {exam.AssignedRoom.Capacity}).\n");

        // 3) Student registrations (respect max 2 exams/day)
        Console.WriteLine("Inregistrare studenti:\n");
        // Create two other exams on same day to test daily limit
        var exam2 = new Exam { Course = "Bazele Datelor", ProfessorId = "prof2", Date = exam.Date, AssignedRoom = rooms[0] };
        var exam3 = new Exam { Course = "Tehnici de Programare", ProfessorId = "prof3", Date = exam.Date.AddHours(2), AssignedRoom = rooms[1] };
        exams.Add(exam2);
        exams.Add(exam3);

        void TryRegister(string studentId, Exam e)
        {
            if (registration.RegisterStudent(studentId, e.Id, out var msg))
                Console.WriteLine($"{users[studentId].Name} -> inscris la '{e.Course}' ({e.Date})");
            else
                Console.WriteLine($"{users[studentId].Name} -> inscriere esuata la '{e.Course}': {msg}");
        }

        // Register student1 to exam, exam2 (same day) -> should allow both (2)
        TryRegister(student1.Id, exam);
        TryRegister(student1.Id, exam2);
        // Try to register student1 to third exam same day -> should be denied
        TryRegister(student1.Id, exam3);

        // Register student2 and student3 for main exam
        TryRegister(student2.Id, exam);
        TryRegister(student3.Id, exam);

        Console.WriteLine();

        // 4) Professor enters grades and publishes
        Console.WriteLine("Profesor introduce notele:\n");
        // Enter some grades
        grading.EnterGrade(professor.Id, exam.Id, student1.Id, 9.5, out var m1);
        grading.EnterGrade(professor.Id, exam.Id, student2.Id, 4.0, out var m2);
        grading.EnterGrade(professor.Id, exam.Id, student3.Id, 6.0, out var m3);
        Console.WriteLine(m1);
        Console.WriteLine(m2);
        Console.WriteLine(m3);

        // Publish grades (set PublishedAt to now - 1 day to simulate time passing within 48h)
        exam.PublishedAt = DateTime.Now.AddHours(-24);
        Console.WriteLine("Notele au fost publicate acum ~24h (simulare).\n");

        // 5) Student contests within 48h
        Console.WriteLine("Contestate:\n");
        if (contestService.FileContestation(student2.Id, exam.Id, "Consider ca nota e incorecta.", out var cm))
            Console.WriteLine($"{users[student2.Id].Name} a depus contestatie: {cm}");
        else
            Console.WriteLine($"{users[student2.Id].Name} nu a putut depune contestatie: {cm}");

        // Simulate a published time older than 48h for exam2
        exam2.PublishedAt = DateTime.Now.AddHours(-72);

        if (contestService.FileContestation(student1.Id, exam2.Id, "Verificare partiale", out var cm2))
            Console.WriteLine($"{users[student1.Id].Name} a depus contestatie la '{exam2.Course}': {cm2}");
        else
            Console.WriteLine($"{users[student1.Id].Name} nu a putut depune contestatie la '{exam2.Course}': {cm2}");

        Console.WriteLine();

        // 6) Reporting
        Console.WriteLine("Rapoarte:\n");
        var (total, passed, rate) = reporting.GetPassReport(exam.Id);
        Console.WriteLine($"Exam: {exam.Course} - Total note inregistrate: {total}, Promovati: {passed}, Rata promovabilitate: {rate}%\n");

        Console.WriteLine("Lista contestatii depuse:");
        foreach (var c in contestations)
        {
            Console.WriteLine($" - Contestatie {c.Id}: student={c.StudentId}, examen={c.ExamId}, filedAt={c.FiledAt}, reason={c.Reason}");
        }

        Console.WriteLine("Demo complet.");

        // Small demo for ScheduleExamWorkflow (EF-backed, accepts optional JSON file as first arg)
        {
            Console.WriteLine("\n--- ScheduleExamWorkflow (EF-backed) demo ---\n");

            var dbPath = "scheduling.db";
            using var db = new SchedulingDbContext(dbPath);
            db.EnsureSeeded();

            // read command from first CLI arg (JSON) if provided
            ScheduleExamCommand cmd;
            if (args.Length > 0 && System.IO.File.Exists(args[0]))
            {
                try
                {
                    var json = System.IO.File.ReadAllText(args[0]);
                    cmd = JsonSerializer.Deserialize<ScheduleExamCommand>(json) ?? new ScheduleExamCommand { CourseCode = "PSSC", ProposedDate1 = "2026-01-20", Duration = "120", ExpectedStudents = "25" };
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to parse JSON input: {ex.Message}. Using default command.");
                    cmd = new ScheduleExamCommand { CourseCode = "PSSC", ProposedDate1 = "2026-01-20", Duration = "120", ExpectedStudents = "25" };
                }
            }
            else
            {
                cmd = new ScheduleExamCommand { CourseCode = "PSSC", ProposedDate1 = "2026-01-20", ProposedDate2 = "2026-06-10", ProposedDate3 = "2026-06-15", Duration = "120", ExpectedStudents = "25" };
            }

            var workflow = new ScheduleExamWorkflow();

            bool CheckCourseExists(CourseCode c) => c.Value == "PSSC" || c.Value == "BD" || c.Value == "MATH1";
            DateTime GetCourseEndDate(CourseCode c) => DateTime.Today.AddDays(-30); // assume course ended 30 days ago for demo

            IEnumerable<RoomNumber> FindAvailableRooms(ExamDate d, Duration dur, Capacity cap) => EfPersistence.FindAvailableRooms(db, d, dur, cap);

            bool ReserveRoom(RoomNumber r, ExamDate d, Duration dur) => EfPersistence.ReserveRoom(db, r, d, dur);

            var result = workflow.Execute(cmd, CheckCourseExists, GetCourseEndDate, FindAvailableRooms, ReserveRoom);

            if (result is PublishedExamScheduling published)
            {
                Console.WriteLine($"Exam scheduled and published:\n - Course: {published.Course}\n - Date: {published.SelectedDate}\n - Room: {published.Room}\n - RoomCap: {published.RoomCapacity}\n - PublishedAt: {published.PublishedAt}\n");
            }
            else if (result is InvalidExamScheduling invalid)
            {
                Console.WriteLine($"Scheduling failed for course '{invalid.CourseCode}'. Reasons:");
                foreach (var r in invalid.Reasons) Console.WriteLine(" - " + r);
            }
            else
            {
                Console.WriteLine("Scheduling resulted in an unexpected state.");
            }
        }
    }
}
