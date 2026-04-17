using InterviewPrep.Application.Features.Questions;
using InterviewPrep.Application.Features.Results;
using InterviewPrep.Application.Features.Sessions;
using InterviewPrep.Application.Interfaces;
using InterviewPrep.Infrastructure.Persistence;
using InterviewPrep.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//
// Add API services
//
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

//
// Register application handlers
// These keep controller actions thin by moving business logic into the Application layer.
//
builder.Services.AddScoped<CreateSessionHandler>();
builder.Services.AddScoped<GetSessionsHandler>();
builder.Services.AddScoped<GetSessionByIdHandler>();

builder.Services.AddScoped<GenerateQuestionsHandler>();
builder.Services.AddScoped<GetQuestionsHandler>();

builder.Services.AddScoped<SubmitAnswerHandler>();
builder.Services.AddScoped<GetAnswersHandler>();
builder.Services.AddScoped<CompleteSessionHandler>();

//
// Prepare SQLite database path
// On Azure, HOME is available, so this keeps the database in a writable location.
// Locally, it falls back to the app content root.
//
var dbPath = builder.Configuration["SQLITE_DB_PATH"];

if (string.IsNullOrWhiteSpace(dbPath))
{
    var home = Environment.GetEnvironmentVariable("HOME");
    var dataFolder = Path.Combine(home ?? builder.Environment.ContentRootPath, "Data");
    Directory.CreateDirectory(dataFolder);
    dbPath = Path.Combine(dataFolder, "interviewprep.db");
}
else
{
    var directory = Path.GetDirectoryName(dbPath);
    if (!string.IsNullOrWhiteSpace(directory))
    {
        Directory.CreateDirectory(directory);
    }
}

//
// Register EF Core with SQLite
//
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

//
// Register repositories
//
builder.Services.AddScoped<IInterviewSessionRepository, InterviewSessionRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

//
// Register services
// For MVP, question generation and evaluation can stay mocked or simplified.
//
builder.Services.AddScoped<IQuestionService, OpenAiQuestionService>();
builder.Services.AddScoped<IInterviewEvaluatorService, MockInterviewEvaluatorService>();

var app = builder.Build();

//
// Enable OpenAPI / Swagger in development only
//
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "InterviewPrep API v1");
        options.RoutePrefix = "swagger";
    });
}
//
// Middleware pipeline
//
app.UseAuthorization();

//
// Map controller endpoints
//
app.MapControllers();

//
// Apply pending EF Core migrations automatically at startup
// This is convenient for development and early MVP work.
//
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

app.Run();