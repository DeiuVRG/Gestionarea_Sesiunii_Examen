using Laborator4_AI.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;

// Fix PostgreSQL DateTime timezone issues
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString = "Host=localhost;Database=exam_scheduling;Username=deiuvrg";
builder.Services.AddDbContext<SchedulingDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

// Configurare Typed HttpClient cu Polly Retry Policy
builder.Services.AddHttpClient<RoomAssignmentNotificationClient>(client =>
{
    // Configurare base address - trimite notificări către API-ul separat pe port 5002
    client.BaseAddress = new Uri("http://localhost:5002/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddPolicyHandler(HttpPolicyExtensions
    .HandleTransientHttpError() // Gestionează HttpRequestException, 5xx și 408
    .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.InternalServerError)
    .WaitAndRetryAsync(
        retryCount: 3,
        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
        onRetry: (outcome, timespan, retryAttempt, context) =>
        {
            Console.WriteLine($"🔄 Retry {retryAttempt} for room assignment notification after {timespan.TotalSeconds}s");
        }
    ));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Exam Scheduling API",
        Version = "v1",
        Description = "API pentru gestionarea sesiunii de examene - PSSC Lab 4"
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SchedulingDbContext>();
    db.Database.EnsureCreated();
    
    if (!db.Rooms.Any())
    {
        db.EnsureSeeded();
        Console.WriteLine("✅ Database seeded with initial data");
    }
    
    Console.WriteLine($"📂 Connected to PostgreSQL @ localhost/exam_scheduling");
    Console.WriteLine($"🏫 Rooms available: {db.Rooms.Count()}");
    Console.WriteLine($"📋 Exams scheduled: {db.Reservations.Count()}");
}

// Enable Swagger in all environments for testing
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Exam Scheduling API v1");
    c.RoutePrefix = string.Empty; // Swagger UI la root
});

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

Console.WriteLine();
Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");
Console.WriteLine("║        EXAM SCHEDULING API - Web API cu Swagger UI              ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
Console.WriteLine();
Console.WriteLine("🌐 Swagger UI:  http://localhost:5000");
Console.WriteLine("📡 API Base:    http://localhost:5000/api");
Console.WriteLine();

app.Run();
