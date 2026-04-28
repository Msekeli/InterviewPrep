using InterviewPrep.Application.Features.Questions;
using InterviewPrep.Application.Features.Results;
using InterviewPrep.Application.Features.Sessions;
using InterviewPrep.Application.Interfaces;
using InterviewPrep.Infrastructure.Persistence;
using InterviewPrep.Infrastructure.Services;
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
            .WithOrigins("http://localhost:3000", "http://127.0.0.1:3000")
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
var useMock = builder.Configuration.GetValue<bool>("UseMockServices");

if (useMock)
{
    builder.Services.AddScoped<IQuestionService, MockQuestionService>();
    builder.Services.AddScoped<IInterviewEvaluatorService, MockInterviewEvaluatorService>();
}
else
{
    builder.Services.AddScoped<IQuestionService, GeminiQuestionService>();
    builder.Services.AddScoped<IInterviewEvaluatorService, GeminiInterviewEvaluatorService>();
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddScoped<IInterviewSessionRepository, InterviewSessionRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "InterviewPrep API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors(FrontendCorsPolicy);
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

app.Run();