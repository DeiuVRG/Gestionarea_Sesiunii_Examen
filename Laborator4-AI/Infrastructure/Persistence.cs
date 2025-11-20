namespace Laborator4_AI.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Laborator4_AI.Domain.Models.ValueObjects;
    using Laborator4_AI.Domain.Models.Entities;

    /// <summary>
    /// Entity Framework DbContext for exam scheduling system
    /// Manages rooms, reservations, registrations, grades, and contestations
    /// </summary>
    public class SchedulingDbContext : DbContext
    {
        public DbSet<RoomEntity> Rooms { get; set; } = null!;
        public DbSet<RoomReservation> Reservations { get; set; } = null!;
        public DbSet<StudentRegistrationEntity> StudentRegistrations { get; set; } = null!;
        public DbSet<ExamGradeEntity> ExamGrades { get; set; } = null!;
        public DbSet<ContestationEntity> Contestations { get; set; } = null!;

        private readonly string? _connectionString;

        // Constructor for DI (ASP.NET Core Web API)
        public SchedulingDbContext(DbContextOptions<SchedulingDbContext> options) : base(options)
        {
        }

        // Constructor for direct instantiation (Console app)
        public SchedulingDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured && !string.IsNullOrEmpty(_connectionString))
            {
                optionsBuilder.UseNpgsql(_connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure DateTime as timestamp without time zone (pentru compatibilitate cross-platform)
            modelBuilder.Entity<RoomReservation>()
                .Property(r => r.Date)
                .HasColumnType("timestamp without time zone");
            
            modelBuilder.Entity<StudentRegistrationEntity>()
                .Property(s => s.ExamDate)
                .HasColumnType("timestamp without time zone");
            
            modelBuilder.Entity<StudentRegistrationEntity>()
                .Property(s => s.RegisteredAt)
                .HasColumnType("timestamp without time zone");
            
            modelBuilder.Entity<ExamGradeEntity>()
                .Property(g => g.ExamDate)
                .HasColumnType("timestamp without time zone");
            
            modelBuilder.Entity<ExamGradeEntity>()
                .Property(g => g.PublishedAt)
                .HasColumnType("timestamp without time zone");
            
            modelBuilder.Entity<ContestationEntity>()
                .Property(c => c.ExamDate)
                .HasColumnType("timestamp without time zone");
            
            modelBuilder.Entity<ContestationEntity>()
                .Property(c => c.FiledAt)
                .HasColumnType("timestamp without time zone");

            modelBuilder.Entity<RoomEntity>().HasKey(r => r.Number);
            
            modelBuilder.Entity<RoomReservation>().HasKey(r => r.Id);
            modelBuilder.Entity<RoomReservation>()
                .HasIndex(r => new { r.RoomNumber, r.Date });

            modelBuilder.Entity<StudentRegistrationEntity>().HasKey(r => r.Id);
            modelBuilder.Entity<StudentRegistrationEntity>()
                .HasIndex(r => new { r.StudentRegNumber, r.CourseCode, r.ExamDate })
                .IsUnique();

            modelBuilder.Entity<ExamGradeEntity>().HasKey(g => g.Id);
            modelBuilder.Entity<ExamGradeEntity>()
                .HasIndex(g => new { g.StudentRegNumber, g.CourseCode, g.ExamDate })
                .IsUnique();

            modelBuilder.Entity<ContestationEntity>().HasKey(c => c.Id);
        }

        public void EnsureSeeded()
        {
            Database.EnsureCreated();
            
            if (!Rooms.Any())
            {
                Rooms.AddRange(new[]
                {
                    new RoomEntity { Number = "A101", Capacity = 30 },
                    new RoomEntity { Number = "A201", Capacity = 25 },
                    new RoomEntity { Number = "B105", Capacity = 20 },
                    new RoomEntity { Number = "B205", Capacity = 35 },
                    new RoomEntity { Number = "C301", Capacity = 40 },
                    new RoomEntity { Number = "C401", Capacity = 50 },
                });
                SaveChanges();
            }
        }
    }

    // Entity models for persistence
    public class RoomEntity
    {
        public string Number { get; set; } = string.Empty;
        public int Capacity { get; set; }
    }

    public class RoomReservation
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int DurationMinutes { get; set; }
        public string CourseCode { get; set; } = string.Empty;
    }

    public class StudentRegistrationEntity
    {
        public int Id { get; set; }
        public string StudentRegNumber { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public DateTime ExamDate { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public DateTime RegisteredAt { get; set; }
    }

    public class ExamGradeEntity
    {
        public int Id { get; set; }
        public string StudentRegNumber { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public DateTime ExamDate { get; set; }
        public decimal Grade { get; set; }
        public DateTime? PublishedAt { get; set; }
    }

    public class ContestationEntity
    {
        public int Id { get; set; }
        public string StudentRegNumber { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public DateTime ExamDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime FiledAt { get; set; }
    }

    /// <summary>
    /// Repository for exam scheduling persistence operations
    /// </summary>
    public static class ExamSchedulingRepository
    {
        public static IEnumerable<RoomNumber> FindAvailableRooms(
            SchedulingDbContext db,
            ExamDate date,
            Duration duration,
            Capacity capacity)
        {
            var d = date.Date;
            var reservedRoomNumbers = db.Reservations
                .Where(r => r.Date == d)
                .Select(r => r.RoomNumber)
                .ToHashSet();

            var rooms = db.Rooms
                .Where(r => r.Capacity >= capacity.Value && !reservedRoomNumbers.Contains(r.Number))
                .ToList();

            foreach (var r in rooms)
            {
                if (RoomNumber.TryCreate(r.Number, out var rn, out var _))
                    yield return rn!;
            }
        }

        public static bool ReserveRoom(
            SchedulingDbContext db,
            RoomNumber room,
            ExamDate date,
            Duration duration,
            CourseCode course)
        {
            var d = date.Date;
            
            // Check if already reserved
            var exists = db.Reservations.Any(r => r.RoomNumber == room.Value && r.Date == d);
            if (exists) return false;

            var reservation = new RoomReservation
            {
                RoomNumber = room.Value,
                Date = d,
                DurationMinutes = (int)duration.Value.TotalMinutes,
                CourseCode = course.Value
            };

            db.Reservations.Add(reservation);

            try
            {
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static RoomNumber? GetExamRoom(
            SchedulingDbContext db,
            CourseCode course,
            ExamDate date)
        {
            var reservation = db.Reservations
                .FirstOrDefault(r => r.CourseCode == course.Value && r.Date == date.Date);

            if (reservation == null) return null;

            RoomNumber.TryCreate(reservation.RoomNumber, out var room, out var _);
            return room;
        }

        public static bool ExamExists(
            SchedulingDbContext db,
            CourseCode course,
            ExamDate date)
        {
            return db.Reservations.Any(r => r.CourseCode == course.Value && r.Date == date.Date);
        }
    }

    /// <summary>
    /// Repository for student registration persistence operations
    /// </summary>
    public static class StudentRegistrationRepository
    {
        public static bool PersistRegistration(
            SchedulingDbContext db,
            StudentRegistrationNumber student,
            CourseCode course,
            ExamDate date,
            RoomNumber room)
        {
            // Check if already registered
            var exists = db.StudentRegistrations.Any(r =>
                r.StudentRegNumber == student.Value &&
                r.CourseCode == course.Value &&
                r.ExamDate == date.Date);

            if (exists) return false;

            var registration = new StudentRegistrationEntity
            {
                StudentRegNumber = student.Value,
                CourseCode = course.Value,
                ExamDate = date.Date,
                RoomNumber = room.Value,
                RegisteredAt = DateTime.Now
            };

            db.StudentRegistrations.Add(registration);

            try
            {
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static int GetStudentExamsOnDate(
            SchedulingDbContext db,
            StudentRegistrationNumber student,
            ExamDate date)
        {
            return db.StudentRegistrations
                .Count(r => r.StudentRegNumber == student.Value && r.ExamDate.Date == date.Date);
        }

        public static bool IsStudentRegistered(
            SchedulingDbContext db,
            StudentRegistrationNumber student,
            CourseCode course,
            ExamDate date)
        {
            return db.StudentRegistrations.Any(r =>
                r.StudentRegNumber == student.Value &&
                r.CourseCode == course.Value &&
                r.ExamDate == date.Date);
        }
    }

    /// <summary>
    /// Repository for exam grading persistence operations
    /// </summary>
    public static class ExamGradingRepository
    {
        public static bool PersistGrades(
            SchedulingDbContext db,
            ValidatedExamGrading grading)
        {
            try
            {
                foreach (var sg in grading.StudentGrades)
                {
                    // Check if grade already exists
                    var existing = db.ExamGrades.FirstOrDefault(g =>
                        g.StudentRegNumber == sg.Student.Value &&
                        g.CourseCode == grading.Course.Value &&
                        g.ExamDate == grading.Date.Date);

                    if (existing != null)
                    {
                        existing.Grade = sg.Grade.Value;
                        existing.PublishedAt = DateTime.Now;
                    }
                    else
                    {
                        db.ExamGrades.Add(new ExamGradeEntity
                        {
                            StudentRegNumber = sg.Student.Value,
                            CourseCode = grading.Course.Value,
                            ExamDate = grading.Date.Date,
                            Grade = sg.Grade.Value,
                            PublishedAt = DateTime.Now
                        });
                    }
                }

                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static DateTime? GetGradesPublishedDate(
            SchedulingDbContext db,
            CourseCode course,
            ExamDate date)
        {
            return db.ExamGrades
                .Where(g => g.CourseCode == course.Value && g.ExamDate == date.Date)
                .Select(g => g.PublishedAt)
                .FirstOrDefault();
        }
    }

    /// <summary>
    /// Repository for contestation persistence operations
    /// </summary>
    public static class ContestationRepository
    {
        public static bool PersistContestation(
            SchedulingDbContext db,
            StudentRegistrationNumber student,
            CourseCode course,
            ExamDate date,
            string reason)
        {
            try
            {
                db.Contestations.Add(new ContestationEntity
                {
                    StudentRegNumber = student.Value,
                    CourseCode = course.Value,
                    ExamDate = date.Date,
                    Reason = reason,
                    FiledAt = DateTime.Now
                });

                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
