# Exam Session Management System

A distributed system for managing university exam sessions with resilient HTTP communication using Polly retry policies.

## Architecture

The system consists of two ASP.NET Core Web APIs:

### 1. Main API - Laborator4-AI (Port 5001)
- **Domain-Driven Design** implementation
- Manages exam scheduling, student registrations, grades, and rooms
- PostgreSQL database (`exam_scheduling`)
- Implements workflows for:
  - Scheduling exams
  - Registering students
  - Publishing grades
  - Filing contestations

### 2. Notifications API (Port 5002)
- Receives room assignment notifications from the Main API
- In-memory storage for demonstration purposes
- Provides endpoints to view and manage notifications

## Key Features

### Resilient HTTP Communication with Polly
- **Typed HttpClient** pattern with dependency injection
- **Exponential backoff retry policy**: 3 attempts at 2s, 4s, 8s intervals
- Handles transient HTTP errors (5xx, 408, HttpRequestException)
- Configured in `Laborator4-AI/Program.cs`

### Domain Model
- **Value Objects**: CourseCode, ExamDate, Duration, Grade, RoomNumber, Capacity, StudentRegistrationNumber
- **Entities**: Room, RoomReservation, StudentRegistration, ExamGrade, Contestation
- **Operations**: Validation, allocation, scheduling, grading
- **Workflows**: Complete business processes with event sourcing

## Technology Stack

- **.NET 9.0** - Latest framework
- **ASP.NET Core Web API** - RESTful APIs
- **Entity Framework Core 9.0.4** - ORM
- **Npgsql** - PostgreSQL provider
- **Polly 7.2.4** - Resilience and transient-fault-handling
- **Microsoft.Extensions.Http.Polly 10.0.0** - HttpClient integration
- **Swashbuckle/Swagger** - API documentation

## Database Schema

PostgreSQL database: `exam_scheduling`

Tables:
- `Rooms` - Available exam rooms with capacity
- `Reservations` - Room bookings for exams
- `StudentRegistrations` - Student enrollments
- `ExamGrades` - Grades and contestations

## Running the Application

### Prerequisites
- .NET 9.0 SDK
- PostgreSQL server
- Database: `exam_scheduling` (connection string in `Laborator4-AI/Program.cs`)

### Start Both APIs

**Terminal 1 - Main API:**
```bash
cd Laborator4-AI
dotnet run --urls "http://localhost:5001"
```

**Terminal 2 - Notifications API:**
```bash
cd NotificationsAPI
dotnet run --urls "http://localhost:5002"
```

### Access Swagger UI

- **Main API**: http://localhost:5001
- **Notifications API**: http://localhost:5002

## API Endpoints

### Main API (5001)

**Scheduling:**
- `POST /api/Scheduling/schedule-exam` - Schedule an exam with notification

**Students:**
- `GET /api/Students` - List all students
- `POST /api/Students` - Register a new student

**Exams:**
- `GET /api/Exams` - List all scheduled exams

**Grades:**
- `GET /api/Grades` - List all grades
- `POST /api/Grades` - Publish grades

### Notifications API (5002)

- `POST /api/notifications` - Receive notification (called by Main API)
- `GET /api/notifications` - List all received notifications
- `GET /api/notifications/health` - Health check
- `DELETE /api/notifications` - Clear all notifications

## Testing Polly Retry

1. Start both APIs
2. Use Swagger UI on Main API (5001)
3. POST to `/api/Scheduling/schedule-exam` with:
```json
{
  "courseCode": "PSSC",
  "proposedDates": ["2025-01-15", "2025-01-20", "2025-01-25"],
  "durationMinutes": 120,
  "expectedStudents": 30
}
```
4. Check the terminal logs to see Polly retry mechanism in action
5. View received notifications at Notifications API (5002)

To test retry behavior:
- Stop Notifications API (5002)
- Schedule an exam from Main API (5001)
- Observe 3 retry attempts in Main API logs
- Restart Notifications API during retries to see recovery

## Project Structure

```
Gestionarea_Sesiunii_examen/
├── Laborator4-AI/                    # Main API
│   ├── Api/
│   │   ├── Controllers/              # API Controllers
│   │   └── Models/                   # DTOs
│   ├── Domain/
│   │   ├── Models/                   # Entities, Value Objects, Commands, Events
│   │   ├── Operations/               # Business operations
│   │   └── Workflows/                # Complete workflows
│   ├── Infrastructure/
│   │   ├── Persistence.cs            # EF Core DbContext
│   │   └── RoomAssignmentNotificationClient.cs  # Typed HttpClient
│   └── Program.cs                    # Startup configuration
├── NotificationsAPI/                 # Notifications API
│   ├── Controllers/
│   │   └── NotificationsController.cs
│   └── Program.cs
└── README.md                         # This file
```

## Implementation Highlights

### Typed HttpClient Configuration
```csharp
builder.Services.AddHttpClient<RoomAssignmentNotificationClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5002/");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddPolicyHandler(HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(3, retryAttempt => 
        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));
```

### Notification Flow
1. User schedules exam via Main API
2. `ScheduleExamWorkflow` executes
3. Room is allocated
4. `RoomAssignmentNotificationClient` sends notification to port 5002
5. Polly handles retries if Notifications API is unavailable
6. Notification stored and can be viewed via GET endpoint

## Author

University Project - Software Project Specification and Construction (PSSC)
