var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Room Assignment Notifications API",
        Version = "v1",
        Description = "API separat pentru primirea notificărilor despre asignările la săli"
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

// Enable Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Notifications API v1");
    c.RoutePrefix = string.Empty;
});

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

Console.WriteLine();
Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");
Console.WriteLine("║     NOTIFICATIONS API - Primește notificări de la Main API      ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
Console.WriteLine();
Console.WriteLine("🌐 Swagger UI:  http://localhost:5002");
Console.WriteLine("📡 API Base:    http://localhost:5002/api");
Console.WriteLine("📬 Endpoint:    POST http://localhost:5002/api/notifications");
Console.WriteLine();

app.Run("http://localhost:5002");
