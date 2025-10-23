using System;
using System.Collections.Generic;
using Xunit;
using Laborator4_AI;

namespace Laborator4_AI.Tests
{
    public class WorkflowTests
    {
        [Fact]
        public void ScheduleExamWorkflow_HappyPath_Publishes()
        {
            var workflow = new ScheduleExamWorkflow();
            var cmd = new ScheduleExamCommand
            {
                CourseCode = "PSSC",
                ProposedDate1 = "2026-01-20",
                Duration = "120",
                ExpectedStudents = "20"
            };

            bool CheckCourseExists(CourseCode c) => true;
            DateTime GetCourseEndDate(CourseCode c) => DateTime.Today.AddDays(-1);
            IEnumerable<RoomNumber> FindAvailableRooms(ExamDate d, Duration dur, Capacity cap)
            {
                RoomNumber.TryCreate("A101", out var r, out var _);
                return new[] { r! };
            }
            bool ReserveRoom(RoomNumber r, ExamDate d, Duration dur) => true;

            var result = workflow.Execute(cmd, CheckCourseExists, GetCourseEndDate, FindAvailableRooms, ReserveRoom);
            Assert.IsType<PublishedExamScheduling>(result);
            var pub = result as PublishedExamScheduling;
            Assert.NotNull(pub);
            Assert.Equal("PSSC", pub!.Course.Value);
        }

        [Fact]
        public void ScheduleExamWorkflow_NoRooms_ReturnsInvalid()
        {
            var workflow = new ScheduleExamWorkflow();
            var cmd = new ScheduleExamCommand
            {
                CourseCode = "PSSC",
                ProposedDate1 = "2026-01-20",
                Duration = "120",
                ExpectedStudents = "20"
            };

            bool CheckCourseExists(CourseCode c) => true;
            DateTime GetCourseEndDate(CourseCode c) => DateTime.Today.AddDays(-1);
            IEnumerable<RoomNumber> FindAvailableRooms(ExamDate d, Duration dur, Capacity cap) => Array.Empty<RoomNumber>();
            bool ReserveRoom(RoomNumber r, ExamDate d, Duration dur) => false;

            var result = workflow.Execute(cmd, CheckCourseExists, GetCourseEndDate, FindAvailableRooms, ReserveRoom);
            Assert.IsType<InvalidExamScheduling>(result);
            var invalid = result as InvalidExamScheduling;
            Assert.NotNull(invalid);
            Assert.Contains("No rooms available", string.Join(";", invalid!.Reasons));
        }
    }
}

