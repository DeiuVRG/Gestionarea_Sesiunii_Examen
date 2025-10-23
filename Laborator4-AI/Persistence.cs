using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Laborator4_AI
{
    public class SchedulingDbContext : DbContext
    {
        public DbSet<RoomEntity> Rooms { get; set; }
        public DbSet<RoomReservation> Reservations { get; set; }

        private readonly string _dbPath;

        public SchedulingDbContext(string dbPath)
        {
            _dbPath = dbPath;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={_dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoomEntity>().HasKey(r => r.Number);
            modelBuilder.Entity<RoomReservation>().HasKey(r => r.Id);
            modelBuilder.Entity<RoomReservation>().HasIndex(r => new { r.RoomNumber, r.Date }).IsUnique(false);
        }

        public void EnsureSeeded()
        {
            Database.EnsureCreated();
            if (!Rooms.Any())
            {
                Rooms.AddRange(new[] {
                    new RoomEntity { Number = "A101", Capacity = 30 },
                    new RoomEntity { Number = "A201", Capacity = 25 },
                    new RoomEntity { Number = "B105", Capacity = 20 },
                    new RoomEntity { Number = "C301", Capacity = 40 },
                });
                SaveChanges();
            }
        }
    }

    public class RoomEntity
    {
        public string Number { get; set; }
        public int Capacity { get; set; }
    }

    public class RoomReservation
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; }
        public DateTime Date { get; set; } // date only
        public int DurationMinutes { get; set; }
    }

    public static class EfPersistence
    {
        // Find available rooms that have sufficient capacity and no reservation on same date
        public static IEnumerable<RoomNumber> FindAvailableRooms(SchedulingDbContext db, ExamDate date, Duration duration, Capacity capacity)
        {
            var d = date.Date;
            var reservedRoomNumbers = db.Reservations.Where(r => r.Date == d).Select(r => r.RoomNumber).ToHashSet();
            var rooms = db.Rooms.Where(r => r.Capacity >= capacity.Value && !reservedRoomNumbers.Contains(r.Number)).ToList();
            foreach (var r in rooms)
            {
                if (RoomNumber.TryCreate(r.Number, out var rn, out var _))
                    yield return rn!;
            }
        }

        // Reserve room (atomic-ish): check again and add reservation
        public static bool ReserveRoom(SchedulingDbContext db, RoomNumber room, ExamDate date, Duration duration)
        {
            var d = date.Date;
            // check if already reserved
            var exists = db.Reservations.Any(r => r.RoomNumber == room.Value && r.Date == d);
            if (exists) return false;
            var res = new RoomReservation { RoomNumber = room.Value, Date = d, DurationMinutes = (int)duration.Value.TotalMinutes };
            db.Reservations.Add(res);
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
    }
}

