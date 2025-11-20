# 🎓 Sistem de Gestionare a Sesiunii de Examene

## 🚀 Quick Start - Web API

```bash
cd Laborator4-AI
dotnet run --urls="http://localhost:5001"
```

**Swagger UI:** http://localhost:5001

---

## 📋 Descriere

Sistem **DDD (Domain-Driven Design)** pentru gestionarea sesiunii de examene, implementat în **.NET 9.0** cu:
- ✅ **ASP.NET Core Web API** - REST API cu Swagger
- ✅ **PostgreSQL** - Bază de date persistentă
- ✅ **Entity Framework Core** - ORM
- ✅ **10 Endpoint-uri funcționale** - CRUD complet

---

## 🎯 Funcționalități Principale

### 📡 Web API (Swagger UI)
1. **Vizualizare examene** - GET /api/exams
2. **Vizualizare săli** - GET /api/exams/rooms
3. **Înregistrare studenți** - POST /api/students/register
4. **Publicare note** - POST /api/grades
5. **Statistici și rapoarte** - Pass rates, grade distribution

### 🎭 Actori
- **Secretariat**: Planifică examene, alocă săli
- **Profesori**: Publică note, gestionează examinări
- **Studenți**: Se înscriu, vizualizează rezultate, contestă
- **Administrator**: Gestionează capacități, conflicte

---

## 🏗️ Arhitectură DDD + Web API

```
Laborator4-AI/
├── Domain/
│   ├── Models/
│   │   ├── Commands/          # Input Commands (VerbNounCommand)
│   │   ├── Events/            # Domain Events (NounVerbedEvent)
│   │   ├── ValueObjects/      # Value Objects imutabile
│   │   └── Entities/          # Entity States (StateEntity)
│   ├── Operations/            # Domain Operations (VerbEntityOperation)
│   ├── Workflows/             # Workflow Composition
│   └── Exceptions/            # Domain Exceptions
│
├── Infrastructure/            # Persistence Layer
└── Program.cs                # Console Application Demo
```

### 📁 Structură Detaliată

#### Domain/Models/Commands/ (4 files)
- `ScheduleExamCommand.cs`
- `RegisterStudentCommand.cs`
- `PublishGradesCommand.cs`
- `FileContestationCommand.cs`

#### Domain/Models/Events/ (8 events in 4 files)
- `ExamScheduledEvent` / `ExamSchedulingFailedEvent`
- `StudentRegisteredEvent` / `StudentRegistrationFailedEvent`
- `GradesPublishedEvent` / `ExamGradingFailedEvent`
- `ContestationFiledEvent` / `ContestationFailedEvent`

#### Domain/Models/ValueObjects/ (7 files)
- `CourseCode`, `ExamDate`, `RoomNumber`, `Duration`
- `Capacity`, `StudentRegistrationNumber`, `Grade`

#### Domain/Models/Entities/ (16 states in 4 files)
- ExamScheduling: Unvalidated → Validated → RoomAllocated → Published / Invalid
- StudentRegistration: Unvalidated → Validated → Checked → Registered / Invalid
- ExamGrading: Unvalidated → Validated → Published / Invalid
- Contestation: Unvalidated → Validated → Checked → Filed / Invalid

#### Domain/Operations/ (20 operations)
- 4 base classes + 16 concrete operations implementing Transform pattern

#### Domain/Workflows/ (4 workflows)
- `ScheduleExamWorkflow`, `RegisterStudentWorkflow`
- `PublishGradesWorkflow`, `FileContestationWorkflow`

#### Domain/Exceptions/ (8 exceptions)
- `DomainException` (base)
- `InvalidCourseCodeException`, `InvalidExamDateException`
- `InvalidRoomNumberException`, `InvalidDurationException`
- `InvalidCapacityException`, `InvalidStudentRegistrationNumberException`
- `InvalidGradeException`


## 🔑 Pattern-uri Implementate

### 1. Value Objects Imutabile
- Constructor privat
- Metodă `TryCreate` pentru validare
- Imutabilitate completă
- Validare în constructor

### 2. Entity States (State Pattern)
- Interfață de bază
- Fiecare stare = record separat
- Constructor internal
- IReadOnlyCollection pentru liste

### 3. Operations (Transform Pattern)
- Pattern matching pentru stări
- Metode virtuale pentru extensibilitate
- Dependencies prin constructor
- Default behavior = identity

### 4. Workflows (Composition Pattern)
- Pipeline de transformări
- Zero business logic
- Doar compoziție de operații
- Dependency injection

## 🚀 Cum să Rulezi

```bash
cd Laborator4-AI
dotnet build
dotnet run
```

## ✅ Validări

- **CourseCode**: 2-4 litere uppercase + digit opțional
- **ExamDate**: Dată viitoare, în sesiuni, nu weekend
- **StudentRegistrationNumber**: "LM" + 5 cifre
- **Grade**: 1.00 - 10.00
- **Business Rules**: Max 2 examene/zi, contestație în 48h

## 📚 Referințe

- Domain-Driven Design - Eric Evans
- Clean Architecture - Robert C. Martin

---

Proiect dezvoltat pentru cursul PSSC, Universitatea Politehnica Timișoara
