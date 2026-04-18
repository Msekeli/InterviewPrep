using InterviewPrep.Application.Features.Questions;
using InterviewPrep.Application.Features.Results;
using InterviewPrep.Application.Features.Sessions;
using InterviewPrep.Application.Interfaces;
using InterviewPrep.Infrastructure.Persistence;
using InterviewPrep.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

// ---------------------------------------------
// Application bootstrap and service registration
// ---------------------------------------------
var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------
// Core framework services (controllers + Swagger)
// ---------------------------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ---------------------------------------------
// Application handlers (use cases / business logic)
// ---------------------------------------------
builder.Services.AddScoped<CreateSessionHandler>();
builder.Services.AddScoped<GetSessionsHandler>();
builder.Services.AddScoped<GetSessionByIdHandler>();
builder.Services.AddScoped<GenerateQuestionsHandler>();
builder.Services.AddScoped<GetQuestionsHandler>();
builder.Services.AddScoped<SubmitAnswerHandler>();
builder.Services.AddScoped<GetAnswersHandler>();
builder.Services.AddScoped<CompleteSessionHandler>();

// ---------------------------------------------
// SQLite database path resolution (local or Azure)
// ---------------------------------------------
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

// ---------------------------------------------
// Database configuration (EF Core with SQLite)
// ---------------------------------------------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// ---------------------------------------------
// Infrastructure repositories (data access layer)
// ---------------------------------------------
builder.Services.AddScoped<IInterviewSessionRepository, InterviewSessionRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// ---------------------------------------------
// External AI service (Gemini replaces OpenAI)
// ---------------------------------------------
builder.Services.AddHttpClient<GeminiQuestionService>();
builder.Services.AddScoped<IQuestionService, GeminiQuestionService>();

// ---------------------------------------------
// Evaluation service (mock scoring for now)
// ---------------------------------------------
builder.Services.AddScoped<IInterviewEvaluatorService, MockInterviewEvaluatorService>();

// ---------------------------------------------
// Generic HttpClient (fallback / shared usage)
// ---------------------------------------------
builder.Services.AddHttpClient();

// ---------------------------------------------
// Build the application
// ---------------------------------------------
var app = builder.Build();

// ---------------------------------------------
// Swagger middleware (API documentation UI)
// ---------------------------------------------
app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "InterviewPrep API v1");
    options.RoutePrefix = "swagger";
});

// ---------------------------------------------
// Middleware pipeline (authorization + routing)
// ---------------------------------------------
app.UseAuthorization();
app.MapControllers();

// ---------------------------------------------
// Apply database migrations on startup
// ---------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

// ---------------------------------------------
// Run the application
// ---------------------------------------------
app.Run();