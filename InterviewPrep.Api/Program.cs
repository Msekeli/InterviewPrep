using InterviewPrep.Application.Interfaces;
using InterviewPrep.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using InterviewPrep.Infrastructure.Services;
using InterviewPrep.Application.Features.Questions;
using InterviewPrep.Application.Features.Results;
using InterviewPrep.Application.Features.Sessions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddScoped<CreateSessionHandler>();
builder.Services.AddScoped<GetSessionsHandler>();
builder.Services.AddScoped<GetSessionByIdHandler>();

builder.Services.AddScoped<GenerateQuestionsHandler>();
builder.Services.AddScoped<GetQuestionsHandler>();

builder.Services.AddScoped<SubmitAnswerHandler>();
builder.Services.AddScoped<GetAnswersHandler>();

var home = Environment.GetEnvironmentVariable("HOME");
var dataFolder = Path.Combine(home ?? builder.Environment.ContentRootPath, "Data");
Directory.CreateDirectory(dataFolder);

var dbPath = Path.Combine(dataFolder, "interviewprep.db");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddScoped<IInterviewSessionRepository, InterviewSessionRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IQuestionService, OpenAiQuestionService>();

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

app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

app.Run();