using InterviewPrep.Application.Features.Questions;
using InterviewPrep.Application.Features.Results;
using InterviewPrep.Application.Features.Sessions;
using InterviewPrep.Application.Interfaces;
using InterviewPrep.Infrastructure.Persistence;
using InterviewPrep.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddScoped<IInterviewSessionRepository, InterviewSessionRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IQuestionService, OpenAiQuestionService>();
builder.Services.AddScoped<IInterviewEvaluatorService, MockInterviewEvaluatorService>();

var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("./v1/swagger.json", "InterviewPrep API v1");
    options.RoutePrefix = "swagger";
});

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

app.Run();