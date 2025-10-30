# Sistem de Gestionare a Sesiunii de Examene# Gestionarea_Sesiunii_Examen


## 📋 Descriere

Sistem DDD (Domain-Driven Design) pentru gestionarea sesiunii de examene la nivel de facultate, implementat în C# .NET 8 cu pattern-uri clean architecture.

## 🎯 Cerințe Funcționale

### Actori Principali
- **Secretariat**: Planifică examene, alocă săli
- **Profesori**: Propun date, corectează, publică note
- **Studenți**: Se înscriu, vizualizează rezultate, contestă
- **Administrator sistem**: Gestionează capacități, conflicte

### Scenarii Cheie
1. Profesorul propune 3 date posibile pentru examen
2. Secretariatul validează disponibilitatea sălilor și alocă
3. Studenții se înscriu la examene (max 2 examene/zi)
4. Profesorul introduce notele și le publică
5. Studentul contestă nota în termen de 48h
6. Sistemul generează rapoarte pentru promovabilitate

## 🏗️ Arhitectură DDD

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
