using Google.GenAI;
using InterviewPrep.Application.Interfaces;
using InterviewPrep.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InterviewPrep.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddGeminiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var apiKey = configuration["Gemini:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                "Gemini API key is not configured.");
        }

        services.AddSingleton(new Client(apiKey: apiKey));

        services.AddScoped<IQuestionService, GeminiQuestionService>();
        services.AddScoped<IInterviewEvaluatorService, GeminiInterviewEvaluatorService>();

        return services;
    }
}