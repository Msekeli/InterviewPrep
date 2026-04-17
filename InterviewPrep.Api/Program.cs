using InterviewPrep.Application.Interfaces;
using InterviewPrep.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var home = Environment.GetEnvironmentVariable("HOME");
var dataFolder = Path.Combine(home ?? builder.Environment.ContentRootPath, "Data");
Directory.CreateDirectory(dataFolder);
var dbPath = Path.Combine(dataFolder, "interviewprep.db");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddScoped<IInterviewSessionRepository, InterviewSessionRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "InterviewPrep API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();