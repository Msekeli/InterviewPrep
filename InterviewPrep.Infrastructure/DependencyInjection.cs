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

    public static IServiceCollection AddOllamaServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpClient<OllamaQuestionService>();
        services.AddHttpClient<OllamaInterviewEvaluatorService>();

        services.AddScoped<IQuestionService, OllamaQuestionService>();
        services.AddScoped<IInterviewEvaluatorService, OllamaInterviewEvaluatorService>();

        return services;
    }

    public static IServiceCollection AddGroqServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var apiKey = configuration["Groq:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                "Groq API key is not configured.");
        }

        services.AddHttpClient<GroqQuestionService>();
        services.AddHttpClient<GroqInterviewEvaluatorService>();

        services.AddScoped<IQuestionService, GroqQuestionService>();
        services.AddScoped<IInterviewEvaluatorService, GroqInterviewEvaluatorService>();

        return services;
    }
}