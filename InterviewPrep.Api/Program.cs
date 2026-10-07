using InterviewPrep.Api.Middleware;
using InterviewPrep.Application.Features.Questions;
using InterviewPrep.Application.Features.Results;
using InterviewPrep.Application.Features.Sessions;
using InterviewPrep.Application.Interfaces;
using InterviewPrep.Infrastructure;
using InterviewPrep.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

const string FrontendCorsPolicy = "FrontendCorsPolicy";

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",
                "http://127.0.0.1:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddScoped<CreateSessionHandler>();
builder.Services.AddScoped<GetSessionsHandler>();
builder.Services.AddScoped<GetSessionByIdHandler>();
builder.Services.AddScoped<GenerateQuestionsHandler>();
builder.Services.AddScoped<GetQuestionsHandler>();
builder.Services.AddScoped<SubmitAnswerHandler>();
builder.Services.AddScoped<GetAnswersHandler>();
builder.Services.AddScoped<CompleteSessionHandler>();

var dbPath = builder.Configuration["SQLITE_DB_PATH"];

if (string.IsNullOrWhiteSpace(dbPath))
{
    var home = Environment.GetEnvironmentVariable("HOME");

    var dataFolder = Path.Combine(
        home ?? builder.Environment.ContentRootPath,
        "Data");

    Directory.CreateDirectory(dataFolder);

    dbPath = Path.Combine(
        dataFolder,
        "interviewprep.db");
}
else
{
    var directory = Path.GetDirectoryName(dbPath);

    if (!string.IsNullOrWhiteSpace(directory))
    {
        Directory.CreateDirectory(directory);
    }
}

/*
 * AI PROVIDER
 *
 * Change this value in appsettings.Development.json
 * to switch providers:
 *
 * "Gemini" → Google Gemini
 * "Groq"   → Groq Cloud
 * "Ollama" → local/cloud Ollama
 *
 * All providers implement the same application interfaces:
 *
 * IQuestionService
 * IInterviewEvaluatorService
 */

var aiProvider =
    builder.Configuration["AI:Provider"] ?? "Gemini";

if (aiProvider.Equals(
        "Gemini",
        StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddGeminiServices(
        builder.Configuration);
}
else if (aiProvider.Equals(
             "Groq",
             StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddGroqServices(
        builder.Configuration);
}
else if (aiProvider.Equals(
             "Ollama",
             StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddOllamaServices(
        builder.Configuration);
}
else
{
    throw new InvalidOperationException(
        $"Unsupported AI provider: '{aiProvider}'. " +
        "Supported providers are 'Gemini', 'Groq', and 'Ollama'.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(
        $"Data Source={dbPath}",
        b => b.MigrationsAssembly(
            "InterviewPrep.Infrastructure")));

builder.Services.AddScoped<
    IInterviewSessionRepository,
    InterviewSessionRepository>();

builder.Services.AddScoped<
    IUserRepository,
    UserRepository>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "InterviewPrep API v1");

        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseCors(FrontendCorsPolicy);

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var logger =
        scope.ServiceProvider
            .GetRequiredService<ILogger<Program>>();

    try
    {
        var dbContext =
            scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        logger.LogError(
            ex,
            "Database migration failed");

        throw;
    }
}

app.Run();